using System.Windows.Forms;

namespace PSXDownloadHelper.Modern;

internal sealed class WifiHelperForm : Form
{
    private readonly TextBox _ssid = new() { Width = 220, PlaceholderText = "Network name (SSID)" };
    private readonly TextBox _password = new() { Width = 220, UseSystemPasswordChar = true, PlaceholderText = "8–63 character password" };
    private readonly TextBox _output = new() { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
    private readonly Button _configure = new() { Text = "Configure hotspot", AutoSize = true };
    private readonly Button _start = new() { Text = "Start hotspot", AutoSize = true };
    private readonly Button _stop = new() { Text = "Stop hotspot", AutoSize = true };
    private readonly Button _check = new() { Text = "Check Wi-Fi support", AutoSize = true };

    public WifiHelperForm()
    {
        Text = "Wi-Fi hotspot helper";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(620, 430);
        Size = new Size(760, 520);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), RowCount = 4, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        root.Controls.Add(new Label
        {
            Text = "Legacy Hosted Network compatibility tool. Support depends on the Windows version and Wi-Fi driver; this is not the newer Windows Mobile Hotspot feature.",
            AutoSize = true,
            MaximumSize = new Size(700, 0)
        }, 0, 0);

        var credentials = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
        credentials.Controls.Add(_ssid);
        credentials.Controls.Add(_password);
        root.Controls.Add(credentials, 0, 1);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        actions.Controls.AddRange(new Control[] { _check, _configure, _start, _stop });
        root.Controls.Add(actions, 0, 2);
        root.Controls.Add(_output, 0, 3);

        _check.Click += async (_, _) => await ExecuteAsync(WifiHostedNetwork.GetDriverStatusAsync);
        _configure.Click += async (_, _) => await ExecuteAsync(() => WifiHostedNetwork.ConfigureAsync(_ssid.Text, _password.Text));
        _start.Click += async (_, _) => await ExecuteAsync(WifiHostedNetwork.StartAsync);
        _stop.Click += async (_, _) => await ExecuteAsync(WifiHostedNetwork.StopAsync);

        Shown += async (_, _) => await ExecuteAsync(WifiHostedNetwork.GetDriverStatusAsync);
    }

    private async Task ExecuteAsync(Func<Task<string>> operation)
    {
        try
        {
            if (!WifiHostedNetwork.IsAdministrator)
                Append("Note: Windows may require running PSX Download Helper as Administrator for this operation.");
            Append(await operation());
        }
        catch (Exception ex)
        {
            Append($"Operation failed: {ex.Message}");
        }
    }

    private void Append(string message)
    {
        _output.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}{Environment.NewLine}");
    }
}
