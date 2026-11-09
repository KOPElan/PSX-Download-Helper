using System.Diagnostics;
using System.Security.Principal;

namespace PSXDownloadHelper.Modern;

/// <summary>
/// Compatibility helper for the legacy Windows Hosted Network feature.
/// Modern Windows/driver combinations may not support Hosted Network.
/// </summary>
internal static class WifiHostedNetwork
{
    public static bool IsAdministrator
    {
        get
        {
            using var identity = WindowsIdentity.GetCurrent();
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public static async Task<string> GetDriverStatusAsync(CancellationToken cancellationToken = default)
    {
        var result = await RunNetshAsync(new[] { "wlan", "show", "drivers" }, cancellationToken);
        return result;
    }

    public static Task<string> ConfigureAsync(string ssid, string password, CancellationToken cancellationToken = default)
    {
        ValidateCredentials(ssid, password);
        return RunNetshAsync(new[] { "wlan", "set", "hostednetwork", "mode=allow", $"ssid={ssid}", $"key={password}" }, cancellationToken);
    }

    public static Task<string> StartAsync(CancellationToken cancellationToken = default) =>
        RunNetshAsync(new[] { "wlan", "start", "hostednetwork" }, cancellationToken);

    public static Task<string> StopAsync(CancellationToken cancellationToken = default) =>
        RunNetshAsync(new[] { "wlan", "stop", "hostednetwork" }, cancellationToken);

    private static void ValidateCredentials(string ssid, string password)
    {
        if (string.IsNullOrWhiteSpace(ssid) || ssid.Length > 32 || ssid.Any(char.IsControl))
            throw new ArgumentException("SSID must contain 1–32 non-control characters.", nameof(ssid));
        if (password.Length is < 8 or > 63 || password.Any(char.IsControl))
            throw new ArgumentException("Wi-Fi password must contain 8–63 non-control characters.", nameof(password));
    }

    private static async Task<string> RunNetshAsync(IEnumerable<string> arguments, CancellationToken cancellationToken)
    {
        var start = new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.SystemDirectory, "netsh.exe"),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        foreach (var argument in arguments)
            start.ArgumentList.Add(argument);

        using var process = new Process { StartInfo = start };
        process.Start();
        var stdout = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderr = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var output = await stdout;
        var error = await stderr;
        var combined = string.Join(Environment.NewLine, new[] { output.Trim(), error.Trim() }.Where(x => x.Length > 0));
        if (process.ExitCode != 0)
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(combined)
                ? $"netsh exited with code {process.ExitCode}."
                : combined);
        return string.IsNullOrWhiteSpace(combined) ? "Command completed successfully." : combined;
    }
}
