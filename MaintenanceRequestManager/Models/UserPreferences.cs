namespace MaintenanceRequestManager.Models;

public enum AppLanguage
{
    English,
    Arabic
}

public enum AppTheme
{
    Light,
    Dark
}

public sealed class UserPreferences
{
    public AppLanguage Language { get; set; } = AppLanguage.English;
    public AppTheme Theme { get; set; } = AppTheme.Light;
    public bool ConfirmBeforeLogout { get; set; } = true;
}
