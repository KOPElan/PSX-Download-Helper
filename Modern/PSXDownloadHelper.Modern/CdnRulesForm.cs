using System.ComponentModel;
using System.Text.Json;

namespace PSXDownloadHelper.Modern;

internal sealed class CdnRulesForm : Form
{
    private readonly CdnRuleStore _store;
    private readonly BindingList<CdnRule> _rules;
    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill,
        AutoGenerateColumns = true,
        AllowUserToAddRows = true,
        AllowUserToDeleteRows = true,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };

    public CdnRulesForm(CdnRuleStore store)
    {
        _store = store;
        _rules = new BindingList<CdnRule>(_store.Load());
        Text = "CDN Rules";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(900, 460);
        Size = new Size(1120, 600);
        Font = new Font("Segoe UI", 9F);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 2 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        var add = new Button { Text = "Add rule", AutoSize = true };
        var remove = new Button { Text = "Remove selected", AutoSize = true };
        var save = new Button { Text = "Save rules", AutoSize = true };
        var import = new Button { Text = "Import JSON", AutoSize = true };
        var export = new Button { Text = "Export JSON", AutoSize = true };
        bar.Controls.AddRange([add, remove, save, import, export]);
        bar.Controls.Add(new Label
        {
            Text = "Higher priority runs first. Use *.example.com for subdomains.",
            AutoSize = true,
            Margin = new Padding(12, 8, 0, 0)
        });
        root.Controls.Add(bar, 0, 0);
        root.Controls.Add(_grid, 0, 1);
        _grid.DataSource = _rules;

        add.Click += (_, _) => _rules.Add(new CdnRule());
        remove.Click += (_, _) =>
        {
            if (_grid.CurrentRow?.DataBoundItem is CdnRule rule)
                _rules.Remove(rule);
        };
        save.Click += (_, _) => SaveRules();
        import.Click += (_, _) => ImportRules();
        export.Click += (_, _) => ExportRules();
        FormClosing += (_, _) => _grid.EndEdit();
    }

    private void SaveRules()
    {
        try
        {
            _grid.EndEdit();
            _store.Save(_rules);
            MessageBox.Show(this, $"Saved {_rules.Count} CDN rules.", "CDN rules",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Invalid CDN rules",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ImportRules()
    {
        using var dialog = new OpenFileDialog { Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*" };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            var imported = JsonSerializer.Deserialize<List<CdnRule>>(File.ReadAllText(dialog.FileName),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
            foreach (var rule in imported)
                rule.NormalizeAndValidate();
            _rules.Clear();
            foreach (var rule in imported)
                _rules.Add(rule);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportRules()
    {
        using var dialog = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", FileName = "cdn-rules.json" };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            _grid.EndEdit();
            File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(_rules.ToList(),
                new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
