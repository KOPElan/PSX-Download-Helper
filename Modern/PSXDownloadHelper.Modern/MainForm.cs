using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

namespace PSXDownloadHelper.Modern;

internal sealed class MainForm : Form
{
    private readonly ProxyServer _proxy = new();
    private readonly TextBox _address = new() { Text = "127.0.0.1", Width = 150 };
    private readonly NumericUpDown _port = new() { Minimum = 1, Maximum = 65535, Value = 8080, Width = 90 };
    private readonly Button _start = new() { Text = "Start proxy", AutoSize = true };
    private readonly Button _stop = new() { Text = "Stop", AutoSize = true, Enabled = false };
    private readonly Button _cdnRulesButton = new() { Text = "CDN rules", AutoSize = true };
    private readonly CdnRuleStore _cdnRuleStore = new();
    private readonly Button _offlineUrlsButton = new() { Text = "Offline URLs", AutoSize = true };
    private readonly OfflineUrlStore _offlineUrlStore = new();
    private readonly Label _status = new() { Text = "Stopped", AutoSize = true, ForeColor = Color.Firebrick };
    private readonly ListBox _events = new() { Dock = DockStyle.Fill, HorizontalScrollbar = true };

    public MainForm()
    {
        Text = "PSX Download Helper — Modern";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(760, 480);
        Size = new Size(980, 620);
        Font = new Font("Segoe UI", 9F);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(14)
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var heading = new Label
        {
            Text = "PSX Download Helper",
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 17F),
            Margin = new Padding(0, 0, 0, 12)
        };
        root.Controls.Add(heading, 0, 0);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 0, 0, 12)
        };
        toolbar.Controls.Add(new Label { Text = "Listen address", AutoSize = true, Margin = new Padding(0, 8, 5, 0) });
        toolbar.Controls.Add(_address);
        toolbar.Controls.Add(new Label { Text = "Port", AutoSize = true, Margin = new Padding(10, 8, 5, 0) });
        toolbar.Controls.Add(_port);
        toolbar.Controls.Add(_start);
        toolbar.Controls.Add(_stop);
        toolbar.Controls.Add(_cdnRulesButton);
        toolbar.Controls.Add(_offlineUrlsButton);
        toolbar.Controls.Add(_status);
        _status.Margin = new Padding(12, 8, 0, 0);
        root.Controls.Add(toolbar, 0, 1);

        var logPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        logPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        logPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        logPanel.Controls.Add(new Label { Text = "Activity log", AutoSize = true, Font = new Font("Segoe UI Semibold", 10F) }, 0, 0);
        logPanel.Controls.Add(_events, 0, 1);
        root.Controls.Add(logPanel, 0, 2);

        _start.Click += async (_, _) => await StartProxyAsync();
        _stop.Click += async (_, _) => await StopProxyAsync();
        _cdnRulesButton.Click += (_, _) =>
        {
            using var dialog = new CdnRulesForm(_cdnRuleStore);
            dialog.ShowDialog(this);
        };
        _offlineUrlsButton.Click += (_, _) =>
        {
            using var dialog = new OfflineUrlsForm(_offlineUrlStore);
            dialog.ShowDialog(this);
        };
        _proxy.Log += AddLog;
        FormClosed += (_, _) => _ = _proxy.StopAsync();

        AddLog("Ready. The default address is loopback-only.");
        AddLog($"Local URL mappings: {_proxy.LocalMappingsPath}");
        AddLog($"CDN rules: {_cdnRuleStore.ConfigPath}");
        AddLog($"Offline URL mappings: {_offlineUrlStore.ConfigPath}");
        AddLog("HTTPS uses CONNECT tunnelling; encrypted payloads are not inspected.");
    }

    private async Task StartProxyAsync()
    {
        if (!IPAddress.TryParse(_address.Text.Trim(), out var address))
        {
            MessageBox.Show(this, "Enter a valid IPv4 or IPv6 address.", "Invalid address",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            await _proxy.StartAsync(address, (int)_port.Value);
            _start.Enabled = false;
            _stop.Enabled = true;
            _address.Enabled = false;
            _port.Enabled = false;
            _status.Text = "Running";
            _status.ForeColor = Color.ForestGreen;
            if (!IPAddress.IsLoopback(address))
                AddLog("WARNING: listening beyond localhost. Restrict Windows Firewall access to trusted networks.");
        }
        catch (SocketException ex)
        {
            MessageBox.Show(this, ex.Message, "Could not start proxy",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not start proxy",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task StopProxyAsync()
    {
        await _proxy.StopAsync();
        _start.Enabled = true;
        _stop.Enabled = false;
        _address.Enabled = true;
        _port.Enabled = true;
        _status.Text = "Stopped";
        _status.ForeColor = Color.Firebrick;
    }

    private void AddLog(string message)
    {
        if (IsDisposed || !IsHandleCreated)
            return;
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action(() => AddLog(message))); }
            catch (InvalidOperationException) { }
            return;
        }

        _events.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (_events.Items.Count > 1000)
            _events.Items.RemoveAt(0);
        _events.TopIndex = _events.Items.Count - 1;
    }
}
