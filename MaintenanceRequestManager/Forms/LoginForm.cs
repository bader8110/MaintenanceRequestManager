using MaintenanceRequestManager.Models;
using MaintenanceRequestManager.Services;

namespace MaintenanceRequestManager.Forms;

public sealed class LoginForm : Form
{
    private readonly Panel _card = new() { Tag = "surface" };
    private readonly Label _logo = new() { Text = "MR", Font = new Font("Segoe UI", 26, FontStyle.Bold), AutoSize = true, Tag = "accent" };
    private readonly Label _title = new() { Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true };
    private readonly Label _subtitle = new() { AutoSize = true, Tag = "muted" };
    private readonly TextBox _username = new();
    private readonly TextBox _password = new() { UseSystemPasswordChar = true };
    private readonly Label _error = new() { ForeColor = Color.Firebrick, AutoSize = true };
    private readonly Label _demo = new() { AutoSize = true, Tag = "muted" };
    private readonly Button _login = new() { Tag = "primary" };
    private readonly Button _language = new() { Tag = "secondary" };
    private readonly Button _theme = new() { Tag = "secondary" };

    public LoginForm()
    {
        Size = new Size(540, 580);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        _card.SetBounds(65, 55, 390, 440);
        _logo.Location = new Point(157, 27);
        _title.Location = new Point(48, 92);
        _subtitle.Location = new Point(135, 132);
        _username.SetBounds(55, 180, 280, 36);
        _password.SetBounds(55, 230, 280, 36);
        _login.SetBounds(55, 285, 280, 42);
        _login.Click += (_, _) => AttemptLogin();
        _error.Location = new Point(55, 340);
        _demo.Location = new Point(55, 370);
        _language.SetBounds(55, 400, 130, 32);
        _theme.SetBounds(205, 400, 130, 32);
        _language.Click += (_, _) =>
        {
            SettingsService.Current.Language = SettingsService.IsArabic ? AppLanguage.English : AppLanguage.Arabic;
            SettingsService.Save();
            ApplyLanguageAndTheme();
        };
        _theme.Click += (_, _) =>
        {
            SettingsService.Current.Theme = SettingsService.IsDark ? AppTheme.Light : AppTheme.Dark;
            SettingsService.Save();
            ApplyLanguageAndTheme();
        };

        _card.Controls.AddRange([_logo, _title, _subtitle, _username, _password, _login, _error, _demo, _language, _theme]);
        Controls.Add(_card);
        AcceptButton = _login;
        Activated += (_, _) => ApplyLanguageAndTheme();
        ApplyLanguageAndTheme();
    }

    private void ApplyLanguageAndTheme()
    {
        Text = SettingsService.T("Maintenance Request Manager - Login", "نظام بلاغات الصيانة - تسجيل الدخول");
        _title.Text = SettingsService.T("Maintenance Request Manager", "نظام إدارة بلاغات الصيانة");
        _subtitle.Text = SettingsService.T("Sign in to continue", "سجل الدخول للمتابعة");
        _username.PlaceholderText = SettingsService.T("Username", "اسم المستخدم");
        _password.PlaceholderText = SettingsService.T("Password", "كلمة المرور");
        _login.Text = SettingsService.T("Sign In", "تسجيل الدخول");
        _demo.Text = SettingsService.T("Demo: admin / admin123   or   user / user123", "تجريبي: admin / admin123 أو user / user123");
        _language.Text = SettingsService.IsArabic ? "English" : "العربية";
        _theme.Text = SettingsService.IsDark ? SettingsService.T("Light Mode", "الوضع الفاتح") : SettingsService.T("Dark Mode", "الوضع الداكن");
        _error.Text = string.Empty;
        CenterTextControls();
        ThemeService.Apply(this);
    }

    private void CenterTextControls()
    {
        _logo.Left = (_card.ClientSize.Width - _logo.Width) / 2;
        _title.Left = (_card.ClientSize.Width - _title.Width) / 2;
        _subtitle.Left = (_card.ClientSize.Width - _subtitle.Width) / 2;
    }

    private void AttemptLogin()
    {
        if (!AuthService.Login(_username.Text, _password.Text))
        {
            _error.Text = SettingsService.T("Invalid username or password.", "اسم المستخدم أو كلمة المرور غير صحيحة.");
            return;
        }

        Hide();
        while (AuthService.CurrentUser is not null)
        {
            using DashboardForm dashboard = new();
            DialogResult result = dashboard.ShowDialog();
            if (result != DialogResult.Retry) break;
        }
        AuthService.Logout();
        _password.Clear();
        Show();
        ApplyLanguageAndTheme();
    }
}
