using System.Text.Json;
using MaintenanceRequestManager.Models;

namespace MaintenanceRequestManager.Services;

public static class SettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MaintenanceRequestManager");

    private static readonly string SettingsFile = Path.Combine(SettingsDirectory, "settings.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static UserPreferences Current { get; private set; } = new();
    public static bool IsArabic => Current.Language == AppLanguage.Arabic;
    public static bool IsDark => Current.Theme == AppTheme.Dark;

    public static void Initialize()
    {
        Directory.CreateDirectory(SettingsDirectory);
        if (!File.Exists(SettingsFile)) return;
        try
        {
            Current = JsonSerializer.Deserialize<UserPreferences>(File.ReadAllText(SettingsFile), JsonOptions) ?? new UserPreferences();
        }
        catch
        {
            Current = new UserPreferences();
        }
    }

    public static string T(string english, string arabic) => IsArabic ? arabic : english;

    public static void Save()
    {
        Directory.CreateDirectory(SettingsDirectory);
        File.WriteAllText(SettingsFile, JsonSerializer.Serialize(Current, JsonOptions));
    }

    public static string StatusText(TicketStatus status) => status switch
    {
        TicketStatus.New => T("New", "جديد"),
        TicketStatus.InProgress => T("In Progress", "قيد التنفيذ"),
        TicketStatus.Resolved => T("Resolved", "تم الحل"),
        TicketStatus.Closed => T("Closed", "مغلق"),
        _ => status.ToString()
    };

    public static string PriorityText(TicketPriority priority) => priority switch
    {
        TicketPriority.Low => T("Low", "منخفضة"),
        TicketPriority.Medium => T("Medium", "متوسطة"),
        TicketPriority.High => T("High", "مرتفعة"),
        TicketPriority.Urgent => T("Urgent", "عاجلة"),
        _ => priority.ToString()
    };

    public static string CategoryText(string category) => category switch
    {
        "Electrical" => T("Electrical", "كهرباء"),
        "Plumbing" => T("Plumbing", "سباكة"),
        "Air Conditioning" => T("Air Conditioning", "تكييف"),
        "Furniture" => T("Furniture", "أثاث"),
        "Network / IT" => T("Network / IT", "شبكات وتقنية معلومات"),
        "Other" => T("Other", "أخرى"),
        _ => category
    };
}
