using MaintenanceRequestManager.Models;
using MaintenanceRequestManager.Services;

namespace MaintenanceRequestManager.Forms;

public sealed class NewTicketForm : Form
{
    private readonly MaintenanceTicket? _ticketToEdit;
    private readonly TextBox _title = new();
    private readonly ComboBox _category = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _location = new();
    private readonly ComboBox _priority = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _description = new() { Multiline = true, ScrollBars = ScrollBars.Vertical };
    private readonly Button _cancel = new() { Tag = "secondary" };
    private readonly Button _save = new() { Tag = "primary" };

    public NewTicketForm(MaintenanceTicket? ticketToEdit = null)
    {
        _ticketToEdit = ticketToEdit;
        bool editing = ticketToEdit is not null;
        Text = SettingsService.T(editing ? "Edit Maintenance Request" : "Create Maintenance Request", editing ? "تعديل بلاغ الصيانة" : "إنشاء بلاغ صيانة");
        Size = new Size(600, 650);
        StartPosition = FormStartPosition.CenterParent;

        int y = 30;
        AddField(SettingsService.T("Request title", "عنوان البلاغ"), _title, ref y);
        string[] categories = ["Electrical", "Plumbing", "Air Conditioning", "Furniture", "Network / IT", "Other"];
        _category.Items.AddRange(categories.Select(c => new UiChoice<string>(c, SettingsService.CategoryText(c))).ToArray());
        _category.SelectedIndex = 0;
        AddField(SettingsService.T("Category", "التصنيف"), _category, ref y);
        AddField(SettingsService.T("Location", "الموقع"), _location, ref y);
        _priority.Items.AddRange(Enum.GetValues<TicketPriority>().Select(p => new UiChoice<TicketPriority>(p, SettingsService.PriorityText(p))).ToArray());
        _priority.SelectedIndex = 1;
        AddField(SettingsService.T("Priority", "الأولوية"), _priority, ref y);

        Label descLabel = new() { Text = SettingsService.T("Description", "وصف المشكلة"), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(35, y) };
        _description.SetBounds(35, y + 25, 510, 150);
        Controls.AddRange([descLabel, _description]);
        y += 200;

        _cancel.Text = SettingsService.T("Cancel", "إلغاء");
        _cancel.DialogResult = DialogResult.Cancel;
        _cancel.SetBounds(325, y, 105, 40);
        _save.Text = SettingsService.T(editing ? "Save Changes" : "Submit Request", editing ? "حفظ التعديلات" : "إرسال البلاغ");
        _save.SetBounds(440, y, 105, 40);
        _save.Click += (_, _) => SaveTicket();
        Controls.AddRange([_cancel, _save]);
        AcceptButton = _save;
        CancelButton = _cancel;
        if (editing)
        {
            _title.Text = ticketToEdit!.Title;
            _location.Text = ticketToEdit.Location;
            _description.Text = ticketToEdit.Description;
            _category.SelectedIndex = Array.FindIndex(categories, c => c == ticketToEdit.Category);
            _priority.SelectedIndex = Array.IndexOf(Enum.GetValues<TicketPriority>(), ticketToEdit.Priority);
        }
        ThemeService.Apply(this);
    }

    private void AddField(string label, Control control, ref int y)
    {
        Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(35, y) });
        control.SetBounds(35, y + 25, 510, 34);
        Controls.Add(control);
        y += 85;
    }

    private void SaveTicket()
    {
        if (string.IsNullOrWhiteSpace(_title.Text) || string.IsNullOrWhiteSpace(_location.Text) || string.IsNullOrWhiteSpace(_description.Text))
        {
            MessageBox.Show(SettingsService.T("Please complete the title, location, and description.", "أكمل عنوان البلاغ والموقع ووصف المشكلة."), SettingsService.T("Missing information", "بيانات ناقصة"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_ticketToEdit is not null)
        {
            _ticketToEdit.Title = _title.Text.Trim();
            _ticketToEdit.Category = ((UiChoice<string>)_category.SelectedItem!).Value;
            _ticketToEdit.Location = _location.Text.Trim();
            _ticketToEdit.Priority = ((UiChoice<TicketPriority>)_priority.SelectedItem!).Value;
            _ticketToEdit.Description = _description.Text.Trim();
            DataService.UpdateTicket(_ticketToEdit);
            MessageBox.Show(SettingsService.T("Request updated successfully.", "تم تعديل البلاغ بنجاح."), SettingsService.T("Saved", "تم الحفظ"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        User current = AuthService.CurrentUser!;
        DataService.AddTicket(new MaintenanceTicket
        {
            Title = _title.Text.Trim(),
            Category = ((UiChoice<string>)_category.SelectedItem!).Value,
            Location = _location.Text.Trim(),
            Priority = ((UiChoice<TicketPriority>)_priority.SelectedItem!).Value,
            Description = _description.Text.Trim(),
            CreatedByUserId = current.Id,
            CreatedByName = current.FullName
        });
        MessageBox.Show(SettingsService.T("Request submitted successfully.", "تم إرسال البلاغ بنجاح."), SettingsService.T("Success", "تم"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
