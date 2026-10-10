using MaintenanceRequestManager.Models;

namespace MaintenanceRequestManager.Services;

public static class AuthService
{
    public static User? CurrentUser { get; private set; }

    public static bool Login(string username, string password)
    {
        CurrentUser = DataService.Data.Users.FirstOrDefault(u =>
            u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
        return CurrentUser is not null;
    }

    public static void Logout() => CurrentUser = null;
}
