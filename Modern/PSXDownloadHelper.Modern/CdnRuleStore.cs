using System.Text.Json;

namespace PSXDownloadHelper.Modern;

internal sealed class CdnRuleStore
{
    private readonly string _path;

    public string ConfigPath => _path;

    public CdnRuleStore(string? configPath = null)
    {
        _path = configPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PSXDownloadHelper", "cdn-rules.json");
        EnsureConfig();
    }

    public List<CdnRule> Load()
    {
        try
        {
            EnsureConfig();
            var loaded = JsonSerializer.Deserialize<List<CdnRule>>(File.ReadAllText(_path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
            var valid = new List<CdnRule>();
            foreach (var rule in loaded)
            {
                try { valid.Add(rule.NormalizeAndValidate()); }
                catch (InvalidDataException) { }
            }
            return valid;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return [];
        }
    }

    public void Save(IEnumerable<CdnRule> rules)
    {
        var validated = rules.Select(rule => rule.NormalizeAndValidate()).ToList();
        var duplicate = validated.GroupBy(rule =>
            $"{rule.MatchHost}|{rule.MatchPathPrefix}", StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
            throw new InvalidDataException($"Duplicate CDN rule: {duplicate.Key}");

        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temp = _path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(validated,
            new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temp, _path, true);
    }

    public Uri Rewrite(Uri source, out string? ruleName)
    {
        ruleName = null;
        foreach (var rule in Load().Where(rule => rule.Enabled).OrderByDescending(rule => rule.Priority))
        {
            if (!rule.IsMatch(source))
                continue;
            var builder = new UriBuilder(source)
            {
                Host = rule.TargetHost,
                Path = rule.RewritePath(source.AbsolutePath)
            };
            ruleName = rule.Name;
            return builder.Uri;
        }
        return source;
    }

    private void EnsureConfig()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        if (!File.Exists(_path))
            File.WriteAllText(_path, "[]");
    }
}

internal sealed class CdnRule
{
    public bool Enabled { get; set; } = true;
    public string Name { get; set; } = "New CDN rule";
    public string MatchHost { get; set; } = "";
    public string TargetHost { get; set; } = "";
    public string MatchPathPrefix { get; set; } = "/";
    public string TargetPathPrefix { get; set; } = "/";
    public int Priority { get; set; }

    public CdnRule NormalizeAndValidate()
    {
        Name = (Name ?? "").Trim();
        MatchHost = NormalizeHost(MatchHost);
        TargetHost = NormalizeHost(TargetHost);
        MatchPathPrefix = NormalizePrefix(MatchPathPrefix);
        TargetPathPrefix = NormalizePrefix(TargetPathPrefix);
        if (Name.Length == 0)
            throw new InvalidDataException("Rule name is required.");
        if (Priority is < -100000 or > 100000)
            throw new InvalidDataException($"Priority out of range in rule '{Name}'.");
        return this;
    }

    public bool IsMatch(Uri uri)
    {
        var hostMatches = MatchHost.StartsWith("*.", StringComparison.Ordinal)
            ? uri.Host.EndsWith(MatchHost[1..], StringComparison.OrdinalIgnoreCase) &&
              uri.Host.Length > MatchHost.Length - 1
            : uri.Host.Equals(MatchHost, StringComparison.OrdinalIgnoreCase);
        if (!hostMatches)
            return false;

        if (MatchPathPrefix == "/")
            return true;
        var prefix = MatchPathPrefix.TrimEnd('/');
        return uri.AbsolutePath.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
               uri.AbsolutePath.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase);
    }

    public string RewritePath(string path)
    {
        if (MatchPathPrefix == "/")
            return JoinPath(TargetPathPrefix, path.TrimStart('/'));
        var consumed = MatchPathPrefix.TrimEnd('/').Length;
        return JoinPath(TargetPathPrefix, path[consumed..].TrimStart('/'));
    }

    private static string JoinPath(string prefix, string remainder)
    {
        // Prefix is already normalized with a leading slash; trim both sides
        // before joining so we never produce a double-leading slash.
        var left = prefix.Trim('/');
        var right = remainder.TrimStart('/');
        if (left.Length == 0)
            return "/" + right;
        return "/" + left + (right.Length == 0 ? "" : "/" + right);
    }

    private static string NormalizePrefix(string? value)
    {
        var result = string.IsNullOrWhiteSpace(value) ? "/" : value.Trim();
        if (!result.StartsWith('/'))
            result = "/" + result;
        if (result.Contains('\\') || result.Split('/').Any(segment => segment is "." or ".."))
            throw new InvalidDataException("Path prefixes cannot contain traversal segments or backslashes.");
        return result;
    }

    private static string NormalizeHost(string? value)
    {
        var host = (value ?? "").Trim().ToLowerInvariant();
        var wildcard = host.StartsWith("*.", StringComparison.Ordinal);
        var candidate = wildcard ? host[2..] : host;
        if (candidate.Length == 0 ||
            !Uri.TryCreate("https://" + candidate, UriKind.Absolute, out var parsed) ||
            parsed.Host != candidate || candidate.Contains('/') || candidate.Contains(':'))
            throw new InvalidDataException($"Invalid host name: '{value}'. Use a hostname without scheme or port.");
        return wildcard ? "*." + candidate : candidate;
    }
}
