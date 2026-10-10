using System.Text.Json;
using MaintenanceRequestManager.Models;

namespace MaintenanceRequestManager.Services;

public static class DataService
{
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MaintenanceRequestManager");

    private static readonly string DataFile = Path.Combine(DataDirectory, "data.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static AppData Data { get; private set; } = new();

    public static void Initialize()
    {
        Directory.CreateDirectory(DataDirectory);
        if (File.Exists(DataFile))
        {
            try
            {
                Data = JsonSerializer.Deserialize<AppData>(File.ReadAllText(DataFile), JsonOptions) ?? new AppData();
            }
            catch
            {
                Data = CreateSeedData();
                Save();
            }
        }
        else
        {
            Data = CreateSeedData();
            Save();
        }
    }

    public static void AddTicket(MaintenanceTicket ticket)
    {
        ticket.Id = Data.Tickets.Count == 0 ? 1001 : Data.Tickets.Max(t => t.Id) + 1;
        ticket.CreatedAt = DateTime.Now;
        ticket.UpdatedAt = DateTime.Now;
        Data.Tickets.Add(ticket);
        Save();
    }

    public static void UpdateTicket(MaintenanceTicket ticket)
    {
        ticket.UpdatedAt = DateTime.Now;
        Save();
    }

    public static void DeleteTicket(MaintenanceTicket ticket)
    {
        Data.Tickets.Remove(ticket);
        Save();
    }

    public static void Save()
    {
        Directory.CreateDirectory(DataDirectory);
        File.WriteAllText(DataFile, JsonSerializer.Serialize(Data, JsonOptions));
    }

    public static string ExportCsv(IEnumerable<MaintenanceTicket> tickets)
    {
        string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            $"MaintenanceTickets_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
        List<string> lines = ["ID,Title,Category,Location,Priority,Status,Created By,Assigned To,Created At"];
        lines.AddRange(tickets.Select(t => string.Join(",", new[]
        {
            t.Id.ToString(), Csv(t.Title), Csv(t.Category), Csv(t.Location),
            t.Priority.ToString(), t.Status.ToString(), Csv(t.CreatedByName),
            Csv(t.AssignedTo), t.CreatedAt.ToString("yyyy-MM-dd HH:mm")
        })));
        File.WriteAllLines(file, lines);
        return file;
    }

    private static AppData CreateSeedData()
    {
        AppData data = new();
        data.Users.AddRange([
            new User { Id = 1, FullName = "System Administrator", Username = "admin", Password = "admin123", Role = UserRole.Admin },
            new User { Id = 2, FullName = "Demo User", Username = "user", Password = "user123", Role = UserRole.User }
        ]);
        data.Tickets.Add(new MaintenanceTicket
        {
            Id = 1001, Title = "Air conditioner is not cooling", Description = "The AC runs but does not cool the room.",
            Category = "Air Conditioning", Location = "Building A - Room 104", Priority = TicketPriority.High,
            Status = TicketStatus.New, CreatedByUserId = 2, CreatedByName = "Demo User"
        });
        return data;
    }
}
