using System.Text;
using System.Text.Json;

namespace PSXDownloadHelper.Modern;

/// <summary>
/// Serves exact URL-to-file mappings for GET/HEAD requests. Mapping files are
/// explicitly configured by the user and are never derived from URL path input.
/// </summary>
internal sealed class LocalFileMap
{
    private readonly string _configPath;
    private readonly Action<string> _log;

    public LocalFileMap(Action<string> log, string? configPath = null)
    {
        _log = log;
        _configPath = configPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PSXDownloadHelper", "local-mappings.json");
        _ = LoadMappings();
    }

    public string ConfigPath => _configPath;

    public async Task<bool> TryServeAsync(
        Uri target, string method, string requestHeader, Stream client,
        CancellationToken cancellationToken)
    {
        if (!method.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
            !method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
            return false;

        var mappings = LoadMappings();
        var mapping = mappings.FirstOrDefault(item => Matches(item.Url, target));
        if (mapping is null)
            return false;

        string filePath;
        try
        {
            filePath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(mapping.FilePath));
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            _log($"Local map has an invalid file path for {target.Host}.");
            await WriteTextResponseAsync(client, 500, "Invalid local mapping path", cancellationToken);
            return true;
        }

        if (!File.Exists(filePath))
        {
            _log($"Local map target does not exist: {filePath}");
            await WriteTextResponseAsync(client, 404, "Mapped local file not found", cancellationToken);
            return true;
        }

        await using var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var length = file.Length;
        var rangeHeader = GetHeader(requestHeader, "Range");
        long offset = 0;
        long count = length;
        var partial = false;

        if (!string.IsNullOrWhiteSpace(rangeHeader) &&
            string.IsNullOrWhiteSpace(GetHeader(requestHeader, "If-Range")) &&
            TryParseRange(rangeHeader, length, out var rangeOffset, out var rangeCount, out var satisfiable))
        {
            if (!satisfiable)
            {
                var invalid = Encoding.ASCII.GetBytes(
                    $"HTTP/1.1 416 Range Not Satisfiable\\r\\nAccept-Ranges: bytes\\r\\nContent-Range: bytes */{length}\\r\\nContent-Length: 0\\r\\nConnection: close\\r\\n\\r\\n");
                await client.WriteAsync(invalid, cancellationToken);
                _log($"LOCAL 416 {target.PathAndQuery}");
                return true;
            }

            offset = rangeOffset;
            count = rangeCount;
            partial = true;
        }

        var status = partial ? 206 : 200;
        var contentType = GetContentType(filePath);
        var responseHeader = new StringBuilder()
            .Append($"HTTP/1.1 {status} {(partial ? "Partial Content" : "OK")}\r\n")
            .Append($"Content-Type: {contentType}\r\n")
            .Append($"Content-Length: {count}\r\n")
            .Append("Accept-Ranges: bytes\r\n")
            .Append("Connection: close\r\n");
        if (partial)
            responseHeader.Append($"Content-Range: bytes {offset}-{offset + count - 1}/{length}\r\n");
        responseHeader.Append("\r\n");

        await client.WriteAsync(Encoding.ASCII.GetBytes(responseHeader.ToString()), cancellationToken);
        if (method.Equals("GET", StringComparison.OrdinalIgnoreCase) && count > 0)
        {
            file.Position = offset;
            var buffer = new byte[64 * 1024];
            var remaining = count;
            while (remaining > 0)
            {
                var read = await file.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, remaining)), cancellationToken);
                if (read == 0)
                    break;
                await client.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                remaining -= read;
            }
        }

        _log($"LOCAL {status} {target.PathAndQuery} ({count} bytes)");
        return true;
    }

    private List<MappingEntry> LoadMappings()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
            if (!File.Exists(_configPath))
            {
                var example = new[]
                {
                    new MappingEntry
                    {
                        Url = "http://example.com/game.pkg",
                        FilePath = @"C:\PSXOffline\game.pkg"
                    }
                };
                File.WriteAllText(_configPath,
                    JsonSerializer.Serialize(example, new JsonSerializerOptions { WriteIndented = true }));
                _log($"Created local mapping template: {_configPath}");
                return [];
            }

            return JsonSerializer.Deserialize<List<MappingEntry>>(File.ReadAllText(_configPath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            _log($"Could not read local mappings: {ex.Message}");
            return [];
        }
    }

    private static bool Matches(string configuredUrl, Uri target)
    {
        if (!Uri.TryCreate(configuredUrl, UriKind.Absolute, out var configured) ||
            (configured.Scheme != Uri.UriSchemeHttp && configured.Scheme != Uri.UriSchemeHttps))
            return false;

        return configured.Scheme.Equals(target.Scheme, StringComparison.OrdinalIgnoreCase) &&
               configured.Host.Equals(target.Host, StringComparison.OrdinalIgnoreCase) &&
               configured.Port == target.Port &&
               string.Equals(configured.PathAndQuery, target.PathAndQuery, StringComparison.Ordinal);
    }

    private static string? GetHeader(string rawHeader, string name)
    {
        foreach (var line in rawHeader.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Skip(1))
        {
            var separator = line.IndexOf(':');
            if (separator > 0 && line[..separator].Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                return line[(separator + 1)..].Trim();
        }
        return null;
    }

    private static bool TryParseRange(string header, long length, out long offset, out long count, out bool satisfiable)
    {
        offset = 0;
        count = 0;
        satisfiable = false;
        if (!header.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase) || header.Contains(','))
            return false;

        var spec = header[6..].Trim();
        var dash = spec.IndexOf('-');
        if (dash < 0 || spec.IndexOf('-', dash + 1) >= 0)
            return false;

        var first = spec[..dash].Trim();
        var last = spec[(dash + 1)..].Trim();
        if (first.Length == 0)
        {
            if (!long.TryParse(last, out var suffixLength))
                return false;
            if (suffixLength <= 0 || length <= 0)
                return true;
            count = Math.Min(suffixLength, length);
            offset = length - count;
            satisfiable = true;
            return true;
        }

        if (!long.TryParse(first, out offset) || offset < 0)
            return false;
        var end = length - 1;
        if (last.Length > 0 && (!long.TryParse(last, out end) || end < offset))
            return false;
        if (length <= 0 || offset >= length)
            return true;

        end = Math.Min(end, length - 1);
        count = end - offset + 1;
        satisfiable = true;
        return true;
    }

    private static string GetContentType(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".pkg" => "application/octet-stream",
        ".zip" => "application/zip",
        ".json" => "application/json",
        ".txt" => "text/plain; charset=utf-8",
        ".html" or ".htm" => "text/html; charset=utf-8",
        ".xml" => "application/xml",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        _ => "application/octet-stream"
    };

    private static async Task WriteTextResponseAsync(Stream client, int status, string message, CancellationToken token)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var header = Encoding.ASCII.GetBytes(
            $"HTTP/1.1 {status} {message}\r\nContent-Type: text/plain; charset=utf-8\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n");
        await client.WriteAsync(header, token);
        await client.WriteAsync(body, token);
    }

    private sealed class MappingEntry
    {
        public string Url { get; set; } = "";
        public string FilePath { get; set; } = "";
    }
}
