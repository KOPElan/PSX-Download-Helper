using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace PSXDownloadHelper.Modern;

/// <summary>
/// Imports non-secret settings from the legacy XML configuration into the
/// modern per-user JSON settings file. The source file is never modified.
/// </summary>
internal static class LegacySettingsMigration
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PSXDownloadHelper");

    public static string ConfigPath => Path.Combine(SettingsDirectory, "settings.json");

    public sealed record ImportedSettings(
        string ListenAddress,
        int Port,
        string Language,
        string Theme);

    public static ImportedSettings Import(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new ArgumentException("Select a legacy configuration file.", nameof(sourcePath));
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("The selected legacy settings file was not found.", sourcePath);

        var values = ReadSafeValues(sourcePath);
        var address = Find(values, "ListenAddress", "ProxyAddress", "ProxyListenAddress", "Host", "IPAddress");
        if (!IPAddress.TryParse(address, out _))
            address = "127.0.0.1";

        var portText = Find(values, "ListenPort", "ProxyPort", "Port", "ProxyListenPort");
        if (!int.TryParse(portText, out var port) || port is < 1 or > 65535)
            port = 8080;

        var language = Limit(Find(values, "Language", "SelectedLanguage", "Culture"), 32, "en");
        var theme = Limit(Find(values, "Theme", "SelectedTheme", "ColorTheme"), 32, "System");

        var settings = new Dictionary<string, object?>
        {
            ["listenAddress"] = address,
            ["port"] = port,
            ["language"] = language,
            ["theme"] = theme,
            ["importedAtUtc"] = DateTimeOffset.UtcNow
        };

        Directory.CreateDirectory(SettingsDirectory);
        var tempPath = ConfigPath + ".tmp";
        File.WriteAllText(tempPath, JsonSerializer.Serialize(settings,
            new JsonSerializerOptions { WriteIndented = true }));
        File.Move(tempPath, ConfigPath, true);

        return new ImportedSettings(address, port, language, theme);
    }

    private static Dictionary<string, string> ReadSafeValues(string path)
    {
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreWhitespace = true,
            MaxCharactersInDocument = 2 * 1024 * 1024
        };

        using var reader = XmlReader.Create(path, settings);
        var document = XDocument.Load(reader);
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var element in document.Descendants())
        {
            var key = (string?)element.Attribute("key")
                      ?? (string?)element.Attribute("name")
                      ?? element.Name.LocalName;
            var value = (string?)element.Attribute("value")
                        ?? (string?)element.Attribute("serializeAs") is not null
                            ? element.Value
                            : element.Value;
            if (string.IsNullOrWhiteSpace(key) || IsSensitiveKey(key))
                continue;

            var normalized = NormalizeKey(key);
            if (normalized.Length > 0 && !string.IsNullOrWhiteSpace(value))
                values[normalized] = value.Trim();
        }

        return values;
    }

    private static bool IsSensitiveKey(string key)
    {
        var normalized = NormalizeKey(key);
        return normalized.Contains("password", StringComparison.Ordinal) ||
               normalized.Contains("passwd", StringComparison.Ordinal) ||
               normalized.Contains("cookie", StringComparison.Ordinal) ||
               normalized.Contains("token", StringComparison.Ordinal) ||
               normalized.Contains("secret", StringComparison.Ordinal) ||
               normalized.Contains("credential", StringComparison.Ordinal) ||
               normalized.Contains("apikey", StringComparison.Ordinal);
    }

    private static string Find(Dictionary<string, string> values, params string[] candidates)
    {
        foreach (var candidate in candidates)
            if (values.TryGetValue(NormalizeKey(candidate), out var value))
                return value;
        return "";
    }

    private static string NormalizeKey(string value) =>
        new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    private static string Limit(string value, int maxLength, string fallback)
    {
        value = (value ?? "").Trim();
        if (value.Length == 0)
            return fallback;
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
