using MaintenanceRequestManager.Models;
using MaintenanceRequestManager.Services;

namespace MaintenanceRequestManager.Forms;

public sealed class SettingsForm : Form
{
    private readonly ComboBox _language = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _theme = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly CheckBox _confirmLogout = new();

    public SettingsForm()
    {
        Text = SettingsService.T("Settings", "الإعدادات");
        Size = new Size(500, 390);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Label heading = new() { Text = SettingsService.T("Application Settings", "إعدادات التطبيق"), Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(35, 30) };
        Label note = new() { Text = SettingsService.T("Changes are applied immediately after saving.", "يتم تطبيق التغييرات مباشرة بعد الحفظ."), AutoSize = true, Location = new Point(38, 72), Tag = "muted" };
        Controls.AddRange([heading, note]);

        _language.Items.AddRange([
            new UiChoice<AppLanguage>(AppLanguage.English, "English"),
            new UiChoice<AppLanguage>(AppLanguage.Arabic, "العربية")
        ]);
        _language.SelectedIndex = SettingsService.Current.Language == AppLanguage.English ? 0 : 1;
        int y = 120;
        AddField(SettingsService.T("Language", "اللغة"), _language, ref y);

        _theme.Items.AddRange([
            new UiChoice<AppTheme>(AppTheme.Light, SettingsService.T("Light", "فاتح")),
            new UiChoice<AppTheme>(AppTheme.Dark, SettingsService.T("Dark", "داكن"))
        ]);
        _theme.SelectedIndex = SettingsService.Current.Theme == AppTheme.Light ? 0 : 1;
        AddField(SettingsService.T("Appearance", "المظهر"), _theme, ref y);

        _confirmLogout.Text = SettingsService.T("Ask for confirmation before logout", "طلب تأكيد قبل تسجيل الخروج");
        _confirmLogout.Checked = SettingsService.Current.ConfirmBeforeLogout;
        _confirmLogout.SetBounds(38, y, 370, 32);
        Controls.Add(_confirmLogout);

        Button cancel = new() { Text = SettingsService.T("Cancel", "إلغاء"), DialogResult = DialogResult.Cancel, Tag = "secondary" };
        cancel.SetBounds(250, 295, 95, 40);
        Button save = new() { Text = SettingsService.T("Save", "حفظ"), Tag = "primary" };
        save.SetBounds(355, 295, 95, 40);
        save.Click += (_, _) => SaveSettings();
        Controls.AddRange([cancel, save]);
        AcceptButton = save;
        CancelButton = cancel;
        ThemeService.Apply(this);
    }

    private void AddField(string label, Control control, ref int y)
    {
        Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(38, y + 7) });
        control.SetBounds(190, y, 260, 34);
        Controls.Add(control);
        y += 65;
    }

    private void SaveSettings()
    {
        SettingsService.Current.Language = ((UiChoice<AppLanguage>)_language.SelectedItem!).Value;
        SettingsService.Current.Theme = ((UiChoice<AppTheme>)_theme.SelectedItem!).Value;
        SettingsService.Current.ConfirmBeforeLogout = _confirmLogout.Checked;
        SettingsService.Save();
        DialogResult = DialogResult.OK;
        Close();
    }
}
