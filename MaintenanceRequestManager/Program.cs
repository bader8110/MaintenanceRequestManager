using MaintenanceRequestManager.Forms;
using MaintenanceRequestManager.Services;

namespace MaintenanceRequestManager;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        SettingsService.Initialize();
        DataService.Initialize();
        Application.Run(new LoginForm());
    }
}
