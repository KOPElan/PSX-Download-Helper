using System.Text.Json;

namespace PSXDownloadHelper.Modern;

internal sealed class OfflineUrlStore
{
    private readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PSXDownloadHelper", "offline-urls.json");

    public string ConfigPath => _path;

    public OfflineUrlStore() => EnsureConfig();

    public List<OfflineUrlRule> Load()
    {
        try
        {
            EnsureConfig();
            var loaded = JsonSerializer.Deserialize<List<OfflineUrlRule>>(File.ReadAllText(_path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
            var valid = new List<OfflineUrlRule>();
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

    public void Save(IEnumerable<OfflineUrlRule> rules)
    {
        var validated = rules.Select(rule => rule.NormalizeAndValidate()).ToList();
        var duplicates = validated
            .GroupBy(rule => OfflineUrlRule.UrlKey(new Uri(rule.SourceUrl)), StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicates is not null)
            throw new InvalidDataException($"Duplicate source URL: {duplicates.Key}");

        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temp = _path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(validated,
            new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temp, _path, true);
    }

    public bool TryRewrite(Uri source, out OfflineUrlRule? matchedRule, out Uri target)
    {
        matchedRule = null;
        target = source;
        foreach (var rule in Load().Where(rule => rule.Enabled).OrderByDescending(rule => rule.Priority))
        {
            if (!rule.IsMatch(source))
                continue;

            matchedRule = rule;
            target = new Uri(rule.TargetUrl);
            return true;
        }
        return false;
    }

    private void EnsureConfig()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        if (!File.Exists(_path))
            File.WriteAllText(_path, "[]");
    }
}

internal sealed class OfflineUrlRule
{
    public bool Enabled { get; set; } = true;
    public string Name { get; set; } = "New offline URL rule";
    public string SourceUrl { get; set; } = "";
    public string TargetUrl { get; set; } = "";
    public string HostHeader { get; set; } = "";
    public string Cookie { get; set; } = "";
    public int Priority { get; set; }

    public OfflineUrlRule NormalizeAndValidate()
    {
        Name = (Name ?? "").Trim();
        SourceUrl = NormalizeHttpUrl(SourceUrl, "Source URL");
        TargetUrl = NormalizeHttpUrl(TargetUrl, "Target URL");
        HostHeader = (HostHeader ?? "").Trim();
        Cookie = (Cookie ?? "").Trim();

        if (Name.Length == 0)
            throw new InvalidDataException("Rule name is required.");
        if (Priority is < -100000 or > 100000)
            throw new InvalidDataException($"Priority out of range in rule '{Name}'.");
        if (HostHeader.Length > 0)
        {
            if (HostHeader.Contains('\r') || HostHeader.Contains('\n') ||
                !Uri.TryCreate("http://" + HostHeader, UriKind.Absolute, out var hostUri) ||
                hostUri.AbsolutePath != "/" || !string.IsNullOrEmpty(hostUri.Query) ||
                !string.IsNullOrEmpty(hostUri.Fragment) || !string.IsNullOrEmpty(hostUri.UserInfo))
                throw new InvalidDataException($"Invalid Host header in rule '{Name}'.");
        }
        if (Cookie.Length > 8192 || Cookie.Contains('\r') || Cookie.Contains('\n'))
            throw new InvalidDataException($"Invalid Cookie header in rule '{Name}'.");
        return this;
    }

    public bool IsMatch(Uri source)
    {
        var expected = new Uri(SourceUrl);
        return source.Scheme.Equals(expected.Scheme, StringComparison.OrdinalIgnoreCase) &&
               source.Host.Equals(expected.Host, StringComparison.OrdinalIgnoreCase) &&
               source.Port == expected.Port &&
               string.Equals(source.PathAndQuery, expected.PathAndQuery, StringComparison.Ordinal);
    }

    public static string UrlKey(Uri uri) =>
        $"{uri.Scheme.ToLowerInvariant()}://{uri.Authority.ToLowerInvariant()}{uri.PathAndQuery}";

    private static string NormalizeHttpUrl(string? value, string label)
    {
        if (!Uri.TryCreate((value ?? "").Trim(), UriKind.Absolute, out var uri) ||
            !uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Fragment))
            throw new InvalidDataException($"{label} must be an absolute HTTP URL without credentials or fragment. HTTPS proxy requests are encrypted CONNECT tunnels and cannot be rewritten.");

        return uri.GetLeftPart(UriPartial.Authority) + uri.PathAndQuery;
    }
}
