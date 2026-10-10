using MaintenanceRequestManager.Models;
using MaintenanceRequestManager.Services;

namespace MaintenanceRequestManager.Forms;

public sealed class TicketDetailsForm : Form
{
    private readonly MaintenanceTicket _ticket;
    private readonly ComboBox _status = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _priority = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _assignedTo = new();
    private readonly TextBox _notes = new() { Multiline = true, ScrollBars = ScrollBars.Vertical };

    public TicketDetailsForm(MaintenanceTicket ticket)
    {
        _ticket = ticket;
        bool isAdmin = AuthService.CurrentUser!.Role == UserRole.Admin;
        Text = SettingsService.T($"Request #{ticket.Id}", $"البلاغ رقم {ticket.Id}");
        Size = new Size(680, 700);
        StartPosition = FormStartPosition.CenterParent;

        Label heading = new() { Text = ticket.Title, Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, MaximumSize = new Size(590, 0), Location = new Point(35, 25) };
        Label meta = new() { Text = SettingsService.T($"#{ticket.Id}  |  Created by {ticket.CreatedByName} on {ticket.CreatedAt:yyyy-MM-dd HH:mm}", $"#{ticket.Id}  |  أنشأه {ticket.CreatedByName} بتاريخ {ticket.CreatedAt:yyyy-MM-dd HH:mm}"), AutoSize = true, Location = new Point(38, 70), Tag = "muted" };
        Label details = new() { Text = SettingsService.T($"Category: {SettingsService.CategoryText(ticket.Category)}\nLocation: {ticket.Location}\n\nDescription:\n{ticket.Description}", $"التصنيف: {SettingsService.CategoryText(ticket.Category)}\nالموقع: {ticket.Location}\n\nوصف المشكلة:\n{ticket.Description}"), AutoSize = true, MaximumSize = new Size(590, 170), Location = new Point(38, 110) };
        Controls.AddRange([heading, meta, details]);

        int y = 290;
        _status.Items.AddRange(Enum.GetValues<TicketStatus>().Select(s => new UiChoice<TicketStatus>(s, SettingsService.StatusText(s))).ToArray());
        _status.SelectedIndex = Array.IndexOf(Enum.GetValues<TicketStatus>(), ticket.Status);
        AddField(SettingsService.T("Status", "الحالة"), _status, ref y, isAdmin);
        _priority.Items.AddRange(Enum.GetValues<TicketPriority>().Select(p => new UiChoice<TicketPriority>(p, SettingsService.PriorityText(p))).ToArray());
        _priority.SelectedIndex = Array.IndexOf(Enum.GetValues<TicketPriority>(), ticket.Priority);
        AddField(SettingsService.T("Priority", "الأولوية"), _priority, ref y, isAdmin);
        _assignedTo.Text = ticket.AssignedTo == "Not assigned" ? SettingsService.T("Not assigned", "غير مسند") : ticket.AssignedTo;
        AddField(SettingsService.T("Assigned technician", "الفني المسؤول"), _assignedTo, ref y, isAdmin);
        _notes.Text = ticket.AdminNotes;
        Label notesLabel = new() { Text = SettingsService.T("Admin notes", "ملاحظات المسؤول"), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(38, y) };
        _notes.SetBounds(38, y + 25, 590, 100);
        _notes.ReadOnly = !isAdmin;
        Controls.AddRange([notesLabel, _notes]);

        Button close = new() { Text = SettingsService.T("Close", "إغلاق"), DialogResult = DialogResult.Cancel, Tag = "secondary" };
        close.SetBounds(405, 590, 105, 40);
        Controls.Add(close);
        if (isAdmin)
        {
            Button save = new() { Text = SettingsService.T("Save Changes", "حفظ التعديلات"), Tag = "primary" };
            save.SetBounds(520, 590, 108, 40);
            save.Click += (_, _) => SaveChanges();
            Controls.Add(save);
            AcceptButton = save;
        }
        CancelButton = close;
        ThemeService.Apply(this);
    }

    private void AddField(string label, Control control, ref int y, bool enabled)
    {
        Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(38, y + 6) });
        control.SetBounds(220, y, 408, 34);
        control.Enabled = enabled;
        Controls.Add(control);
        y += 55;
    }

    private void SaveChanges()
    {
        _ticket.Status = ((UiChoice<TicketStatus>)_status.SelectedItem!).Value;
        _ticket.Priority = ((UiChoice<TicketPriority>)_priority.SelectedItem!).Value;
        string assigned = _assignedTo.Text.Trim();
        _ticket.AssignedTo = string.IsNullOrWhiteSpace(assigned) || assigned == "غير مسند" ? "Not assigned" : assigned;
        _ticket.AdminNotes = _notes.Text.Trim();
        DataService.UpdateTicket(_ticket);
        MessageBox.Show(SettingsService.T("Changes saved successfully.", "تم حفظ التعديلات بنجاح."), SettingsService.T("Saved", "تم الحفظ"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
