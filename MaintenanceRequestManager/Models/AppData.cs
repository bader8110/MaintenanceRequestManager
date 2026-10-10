namespace MaintenanceRequestManager.Models;

public sealed class AppData
{
    public List<User> Users { get; set; } = [];
    public List<MaintenanceTicket> Tickets { get; set; } = [];
}
