using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PSXDownloadHelper.Modern;

/// <summary>
/// Small asynchronous HTTP forward proxy. HTTPS is passed through as an opaque
/// CONNECT tunnel; TLS is never intercepted or downgraded.
/// </summary>
internal sealed class ProxyServer
{
    private const int MaxHeaderBytes = 64 * 1024;
    private readonly ConcurrentDictionary<int, TcpClient> _clients = new();
    private readonly LocalFileMap _localFileMap = new(message => Log?.Invoke(message));
    private TcpListener? _listener;
    private CancellationTokenSource? _shutdown;
    private Task? _acceptLoop;
    private int _nextClientId;

    public event Action<string>? Log;

    public bool IsRunning => _listener is not null;

    public Task StartAsync(IPAddress address, int port)
    {
        if (IsRunning)
            throw new InvalidOperationException("Proxy is already running.");

        _shutdown = new CancellationTokenSource();
        _listener = new TcpListener(address, port);
        try
        {
            _listener.Start();
            Log?.Invoke($"Listening on {address}:{port}");
            _acceptLoop = AcceptLoopAsync(_shutdown.Token);
            return Task.CompletedTask;
        }
        catch
        {
            _listener.Stop();
            _listener = null;
            _shutdown.Dispose();
            _shutdown = null;
            throw;
        }
    }

    public async Task StopAsync()
    {
        var listener = _listener;
        var shutdown = _shutdown;
        if (listener is null || shutdown is null)
            return;

        _listener = null;
        _shutdown = null;
        shutdown.Cancel();
        listener.Stop();

        foreach (var client in _clients.Values)
            client.Dispose();

        if (_acceptLoop is not null)
        {
            try { await _acceptLoop.ConfigureAwait(false); }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
        }

        shutdown.Dispose();
        Log?.Invoke("Proxy stopped.");
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        var listener = _listener!;
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { break; }
            catch (ObjectDisposedException) { break; }
            catch (SocketException) when (cancellationToken.IsCancellationRequested) { break; }

            var id = Interlocked.Increment(ref _nextClientId);
            _clients[id] = client;
            _ = HandleClientAsync(id, client, cancellationToken);
        }
    }

    private async Task HandleClientAsync(int id, TcpClient client, CancellationToken serverToken)
    {
        using (client)
        {
            try
            {
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(serverToken);
                var clientStream = client.GetStream();
                var headerBytes = await ReadHeaderAsync(clientStream, linked.Token).ConfigureAwait(false);
                if (headerBytes.Length == 0)
                    return;

                var headerText = Encoding.ASCII.GetString(headerBytes);
                var firstLine = headerText.Split("\r\n", 2)[0];
                var parts = firstLine.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                {
                    await WriteErrorAsync(clientStream, 400, "Bad Request", linked.Token).ConfigureAwait(false);
                    return;
                }

                if (parts[0].Equals("CONNECT", StringComparison.OrdinalIgnoreCase))
                {
                    await HandleConnectAsync(parts[1], clientStream, linked.Token).ConfigureAwait(false);
                    return;
                }

                if (!Uri.TryCreate(parts[1], UriKind.Absolute, out var target) ||
                    !target.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
                {
                    await WriteErrorAsync(clientStream, 400, "Expected an absolute http:// URL or CONNECT", linked.Token).ConfigureAwait(false);
                    return;
                }

                if (await _localFileMap.TryServeAsync(target, parts[0], headerText, clientStream, linked.Token).ConfigureAwait(false))
                    return;

                var transferEncoding = GetHeaderValue(headerText, "Transfer-Encoding");
                if (!string.IsNullOrWhiteSpace(transferEncoding) &&
                    transferEncoding.Contains("chunked", StringComparison.OrdinalIgnoreCase))
                {
                    await WriteErrorAsync(clientStream, 501, "Chunked request bodies are not supported", linked.Token).ConfigureAwait(false);
                    return;
                }

                if (!TryGetContentLength(headerText, out var requestBodyLength))
                {
                    await WriteErrorAsync(clientStream, 400, "Invalid Content-Length", linked.Token).ConfigureAwait(false);
                    return;
                }

                Log?.Invoke($"HTTP {target.Host}:{target.Port} {target.PathAndQuery}");
                using var upstream = new TcpClient();
                await upstream.ConnectAsync(target.Host, target.Port, linked.Token).ConfigureAwait(false);
                var upstreamStream = upstream.GetStream();

                var rewrittenHeader = RewriteRequestHeader(headerText, target, parts[0], parts[2]);
                var bytes = Encoding.ASCII.GetBytes(rewrittenHeader);
                await upstreamStream.WriteAsync(bytes, linked.Token).ConfigureAwait(false);

                if (GetHeaderValue(headerText, "Expect")?.Equals("100-continue", StringComparison.OrdinalIgnoreCase) == true &&
                    requestBodyLength > 0)
                {
                    await clientStream.WriteAsync(Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\\r\\n\\r\\n"), linked.Token).ConfigureAwait(false);
                }

                if (requestBodyLength > 0)
                    await CopyExactlyAsync(clientStream, upstreamStream, requestBodyLength, linked.Token).ConfigureAwait(false);

                await upstreamStream.CopyToAsync(clientStream, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (IOException ex) { Log?.Invoke($"Client disconnected: {ex.Message}"); }
            catch (SocketException ex) { Log?.Invoke($"Network error: {ex.Message}"); }
            catch (Exception ex) { Log?.Invoke($"Request error: {ex.Message}"); }
            finally
            {
                _clients.TryRemove(id, out _);
            }
        }
    }

    private async Task HandleConnectAsync(string authority, NetworkStream clientStream, CancellationToken token)
    {
        if (!Uri.TryCreate($"tcp://{authority}", UriKind.Absolute, out var destination) ||
            string.IsNullOrWhiteSpace(destination.Host) || destination.Port is < 1 or > 65535)
        {
            await WriteErrorAsync(clientStream, 400, "Invalid CONNECT destination", token).ConfigureAwait(false);
            return;
        }

        Log?.Invoke($"CONNECT {destination.Host}:{destination.Port}");
        using var upstream = new TcpClient();
        await upstream.ConnectAsync(destination.Host, destination.Port, token).ConfigureAwait(false);
        var upstreamStream = upstream.GetStream();
        var established = Encoding.ASCII.GetBytes("HTTP/1.1 200 Connection Established\r\nProxy-Agent: PSX-Download-Helper\r\n\r\n");
        await clientStream.WriteAsync(established, token).ConfigureAwait(false);

        using var tunnelCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        var upload = clientStream.CopyToAsync(upstreamStream, tunnelCancellation.Token);
        var download = upstreamStream.CopyToAsync(clientStream, tunnelCancellation.Token);
        await Task.WhenAny(upload, download).ConfigureAwait(false);
        tunnelCancellation.Cancel();
        try { await Task.WhenAll(upload, download).ConfigureAwait(false); }
        catch (OperationCanceledException) { }
        catch (IOException) { }
    }

    private static string? GetHeaderValue(string rawHeader, string name)
    {
        foreach (var line in rawHeader.Split("\\r\\n", StringSplitOptions.RemoveEmptyEntries).Skip(1))
        {
            var separator = line.IndexOf(':');
            if (separator > 0 && line[..separator].Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                return line[(separator + 1)..].Trim();
        }
        return null;
    }

    private static bool TryGetContentLength(string rawHeader, out long length)
    {
        var value = GetHeaderValue(rawHeader, "Content-Length");
        if (value is null)
        {
            length = 0;
            return true;
        }
        return long.TryParse(value, out length) && length >= 0;
    }

    private static async Task CopyExactlyAsync(Stream source, Stream destination, long bytesToCopy, CancellationToken token)
    {
        var buffer = new byte[64 * 1024];
        var remaining = bytesToCopy;
        while (remaining > 0)
        {
            var read = await source.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, remaining)), token).ConfigureAwait(false);
            if (read == 0)
                throw new EndOfStreamException("Client closed before the complete request body was received.");
            await destination.WriteAsync(buffer.AsMemory(0, read), token).ConfigureAwait(false);
            remaining -= read;
        }
    }

    private static string RewriteRequestHeader(string rawHeader, Uri target, string method, string version)
    {
        var lines = rawHeader.Split("\r\n", StringSplitOptions.None);
        var builder = new StringBuilder();
        builder.Append(method).Append(' ')
            .Append(string.IsNullOrEmpty(target.PathAndQuery) ? "/" : target.PathAndQuery)
            .Append(' ').Append(version).Append("\r\n");

        foreach (var line in lines.Skip(1))
        {
            if (line.Length == 0)
                continue;
            var separator = line.IndexOf(':');
            if (separator <= 0)
                continue;
            var name = line[..separator].Trim();
            if (name.Equals("Proxy-Connection", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Proxy-Authorization", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Connection", StringComparison.OrdinalIgnoreCase))
                continue;
            builder.Append(line).Append("\r\n");
        }

        builder.Append("Connection: close\r\n\r\n");
        return builder.ToString();
    }

    private static async Task<byte[]> ReadHeaderAsync(NetworkStream stream, CancellationToken token)
    {
        using var buffer = new MemoryStream();
        var oneByte = new byte[1];
        while (buffer.Length < MaxHeaderBytes)
        {
            var read = await stream.ReadAsync(oneByte, token).ConfigureAwait(false);
            if (read == 0)
                break;
            buffer.WriteByte(oneByte[0]);
            var length = buffer.Length;
            if (length >= 4)
            {
                var data = buffer.GetBuffer();
                if (data[length - 4] == 13 && data[length - 3] == 10 &&
                    data[length - 2] == 13 && data[length - 1] == 10)
                    return buffer.ToArray();
            }
        }

        if (buffer.Length >= MaxHeaderBytes)
            throw new InvalidDataException("HTTP header exceeds 64 KiB.");
        return buffer.ToArray();
    }

    private static async Task WriteErrorAsync(NetworkStream stream, int code, string message, CancellationToken token)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var header = Encoding.ASCII.GetBytes(
            $"HTTP/1.1 {code} {message}\r\nConnection: close\r\nContent-Type: text/plain; charset=utf-8\r\nContent-Length: {body.Length}\r\n\r\n");
        await stream.WriteAsync(header, token).ConfigureAwait(false);
        await stream.WriteAsync(body, token).ConfigureAwait(false);
    }
}
