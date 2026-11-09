using System.ComponentModel;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace PSXDownloadHelper.Modern;

internal sealed class OfflineUrlsForm : Form
{
    private readonly OfflineUrlStore _store;
    private readonly BindingList<OfflineUrlRule> _rules;
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

    public OfflineUrlsForm(OfflineUrlStore store)
    {
        _store = store;
        _rules = new BindingList<OfflineUrlRule>(_store.Load());
        Text = "Offline URL Manager";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(1000, 500);
        Size = new Size(1260, 680);
        Font = new Font("Segoe UI", 9F);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 2 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        var add = new Button { Text = "Add mapping", AutoSize = true };
        var remove = new Button { Text = "Remove selected", AutoSize = true };
        var save = new Button { Text = "Save mappings", AutoSize = true };
        var import = new Button { Text = "Import JSON", AutoSize = true };
        var export = new Button { Text = "Export JSON", AutoSize = true };
        var legacyImport = new Button { Text = "Import legacy XML", AutoSize = true };
        bar.Controls.AddRange([add, remove, save, import, export, legacyImport]);
        bar.Controls.Add(new Label
        {
            Text = "Exact HTTP URL mapping. Optional Host and Cookie headers support legacy offline mirrors.",
            AutoSize = true,
            Margin = new Padding(12, 8, 0, 0)
        });
        root.Controls.Add(bar, 0, 0);
        root.Controls.Add(_grid, 0, 1);
        _grid.DataSource = _rules;

        add.Click += (_, _) => _rules.Add(new OfflineUrlRule());
        remove.Click += (_, _) =>
        {
            if (_grid.CurrentRow?.DataBoundItem is OfflineUrlRule rule)
                _rules.Remove(rule);
        };
        save.Click += (_, _) => SaveRules();
        import.Click += (_, _) => ImportRules();
        export.Click += (_, _) => ExportRules();
        legacyImport.Click += (_, _) => ImportLegacyXml();
        FormClosing += (_, _) => _grid.EndEdit();
    }

    private void SaveRules()
    {
        try
        {
            _grid.EndEdit();
            _store.Save(_rules);
            MessageBox.Show(this, $"Saved {_rules.Count} offline URL mappings.", "Offline URL Manager",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Invalid offline URL mapping",
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
            var imported = JsonSerializer.Deserialize<List<OfflineUrlRule>>(File.ReadAllText(dialog.FileName),
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


    private void ImportLegacyXml()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Legacy DataHistory.xml|DataHistory.xml;*.xml|XML files (*.xml)|*.xml|All files (*.*)|*.*"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };
            using var reader = XmlReader.Create(dialog.FileName, settings);
            var document = XDocument.Load(reader);
            var existing = new HashSet<string>(
                _rules.Where(rule => Uri.TryCreate(rule.SourceUrl, UriKind.Absolute, out _))
                    .Select(rule => OfflineUrlRule.UrlKey(new Uri(rule.SourceUrl))),
                StringComparer.Ordinal);
            var imported = 0;
            var skipped = 0;

            foreach (var record in document.Root?.Elements("PsnRecord") ?? Enumerable.Empty<XElement>())
            {
                var enabledText = record.Element("isLixian")?.Value;
                var source = record.Element("PsnUrl")?.Value;
                var target = record.Element("LixianUrl")?.Value;
                var name = record.Element("Names")?.Value;

                if (!bool.TryParse(enabledText, out var isOffline) || !isOffline ||
                    string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target))
                {
                    skipped++;
                    continue;
                }

                try
                {
                    var rule = new OfflineUrlRule
                    {
                        Name = string.IsNullOrWhiteSpace(name) ? "Imported legacy offline URL" : name.Trim(),
                        SourceUrl = source.Trim(),
                        TargetUrl = target.Trim(),
                        Enabled = true
                    }.NormalizeAndValidate();
                    var key = OfflineUrlRule.UrlKey(new Uri(rule.SourceUrl));
                    if (!existing.Add(key))
                    {
                        skipped++;
                        continue;
                    }
                    _rules.Add(rule);
                    imported++;
                }
                catch (InvalidDataException)
                {
                    // HTTPS and malformed legacy records cannot be rewritten by the HTTP-only proxy.
                    skipped++;
                }
            }

            MessageBox.Show(this,
                $"Imported {imported} HTTP offline URL mappings. Skipped {skipped} disabled, duplicate, HTTPS or invalid records. Save mappings to persist the import.",
                "Legacy import complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Legacy XML import failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportRules()
    {
        using var dialog = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", FileName = "offline-urls.json" };
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
