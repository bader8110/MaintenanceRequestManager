using MaintenanceRequestManager.Models;
using MaintenanceRequestManager.Services;

namespace MaintenanceRequestManager.Forms;

public sealed class DashboardForm : Form
{
    private readonly DataGridView _grid = new();
    private readonly TextBox _search = new();
    private readonly ComboBox _statusFilter = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _priorityFilter = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _categoryFilter = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _stats = new() { Tag = "muted" };
    private List<MaintenanceTicket> _visibleTickets = [];

    public DashboardForm()
    {
        User user = AuthService.CurrentUser!;
        Text = SettingsService.T("Maintenance Request Manager", "نظام إدارة بلاغات الصيانة");
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1100, 700);

        Panel header = new() { Dock = DockStyle.Top, Height = 78, Tag = "header" };
        Label appTitle = new() { Text = SettingsService.T("Maintenance Request Manager", "نظام إدارة بلاغات الصيانة"), Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(25, 22), Tag = "header" };
        Label userInfo = new() { Text = $"{user.FullName}  |  {SettingsService.T(user.Role.ToString(), user.Role == UserRole.Admin ? "مسؤول" : "مستخدم")}", AutoSize = true, Tag = "header" };
        Button settings = new() { Text = SettingsService.T("Settings", "الإعدادات"), Tag = "secondary" };
        settings.SetBounds(0, 20, 100, 38);
        settings.Click += (_, _) => OpenSettings();
        Button logout = new() { Text = SettingsService.T("Logout", "تسجيل خروج"), Tag = "danger" };
        logout.SetBounds(0, 20, 105, 38);
        logout.Click += (_, _) => Logout();
        void PositionHeaderItems()
        {
            logout.Left = header.ClientSize.Width - logout.Width - 25;
            settings.Left = logout.Left - settings.Width - 10;
            userInfo.Left = settings.Left - userInfo.Width - 20;
            userInfo.Top = 30;
        }
        header.Controls.AddRange([appTitle, userInfo, settings, logout]);
        header.Resize += (_, _) => PositionHeaderItems();
        PositionHeaderItems();
        Controls.Add(header);

        Panel toolbar = new() { Dock = DockStyle.Top, Height = 152, Padding = new Padding(25, 18, 25, 12), Tag = "surface" };
        _search.PlaceholderText = SettingsService.T("Search by ID, title or location...", "ابحث بالرقم أو العنوان أو الموقع...");
        _search.SetBounds(25, 18, 270, 36);
        _search.TextChanged += (_, _) => RefreshGrid();
        _statusFilter.Items.Add(new UiChoice<TicketStatus?>(null, SettingsService.T("All statuses", "كل الحالات")));
        _statusFilter.Items.AddRange(Enum.GetValues<TicketStatus>().Select(s => new UiChoice<TicketStatus?>(s, SettingsService.StatusText(s))).ToArray());
        _statusFilter.SelectedIndex = 0;
        _statusFilter.SetBounds(305, 18, 155, 36);
        _statusFilter.SelectedIndexChanged += (_, _) => RefreshGrid();
        _priorityFilter.Items.Add(new UiChoice<TicketPriority?>(null, SettingsService.T("All priorities", "كل الأولويات")));
        _priorityFilter.Items.AddRange(Enum.GetValues<TicketPriority>().Select(p => new UiChoice<TicketPriority?>(p, SettingsService.PriorityText(p))).ToArray());
        _priorityFilter.SelectedIndex = 0;
        _priorityFilter.SetBounds(470, 18, 145, 36);
        _priorityFilter.SelectedIndexChanged += (_, _) => RefreshGrid();
        string[] categories = ["Electrical", "Plumbing", "Air Conditioning", "Furniture", "Network / IT", "Other"];
        _categoryFilter.Items.Add(new UiChoice<string?>(null, SettingsService.T("All categories", "كل التصنيفات")));
        _categoryFilter.Items.AddRange(categories.Select(c => new UiChoice<string?>(c, SettingsService.CategoryText(c))).ToArray());
        _categoryFilter.SelectedIndex = 0;
        _categoryFilter.SetBounds(625, 18, 165, 36);
        _categoryFilter.SelectedIndexChanged += (_, _) => RefreshGrid();
        Button create = MakeButton(SettingsService.T("+ New Request", "+ بلاغ جديد"), 800, 18, 130, "primary");
        create.Click += (_, _) => { using NewTicketForm form = new(); if (form.ShowDialog(this) == DialogResult.OK) RefreshGrid(); };
        Button export = MakeButton(SettingsService.T("Export CSV", "تصدير CSV"), 940, 18, 110, "success");
        export.Click += (_, _) => ExportCsv();
        Button refresh = MakeButton(SettingsService.T("Refresh", "تحديث"), 25, 72, 95, "secondary");
        refresh.Click += (_, _) => RefreshGrid();
        Button clear = MakeButton(SettingsService.T("Clear", "مسح"), 130, 72, 80, "secondary");
        clear.Click += (_, _) => { _search.Clear(); _statusFilter.SelectedIndex = 0; _priorityFilter.SelectedIndex = 0; _categoryFilter.SelectedIndex = 0; RefreshGrid(); };
        Button open = MakeButton(SettingsService.T("View Details", "عرض التفاصيل"), 220, 72, 130, "primary");
        open.Click += (_, _) => OpenSelectedTicket();
        Button edit = MakeButton(SettingsService.T("Edit Request", "تعديل البلاغ"), 360, 72, 125, "success");
        edit.Click += (_, _) => EditSelectedTicket();
        Button delete = MakeButton(SettingsService.T("Delete Request", "حذف البلاغ"), 495, 72, 130, "danger");
        delete.Click += (_, _) => DeleteSelectedTicket();
        _stats.AutoSize = true;
        _stats.Location = new Point(650, 82);
        toolbar.Controls.AddRange([_search, _statusFilter, _priorityFilter, _categoryFilter, create, export, refresh, clear, open, edit, delete, _stats]);
        Controls.Add(toolbar);

        ConfigureGrid();
        Controls.Add(_grid);
        _grid.BringToFront();
        ThemeService.Apply(this);
        RefreshGrid();
    }

    private Button MakeButton(string text, int x, int y, int width, string tag)
    {
        Button button = new() { Text = text, Tag = tag };
        button.SetBounds(x, y, width, 36);
        return button;
    }

    private void ConfigureGrid()
    {
        _grid.Dock = DockStyle.Fill;
        _grid.BorderStyle = BorderStyle.None;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.ColumnHeadersHeight = 42;
        _grid.RowTemplate.Height = 40;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdColumn", HeaderText = SettingsService.T("ID", "الرقم"), DataPropertyName = "Id", FillWeight = 40 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TitleColumn", HeaderText = SettingsService.T("Title", "العنوان"), DataPropertyName = "Title", FillWeight = 145 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoryColumn", HeaderText = SettingsService.T("Category", "التصنيف"), DataPropertyName = "Category", FillWeight = 80 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "LocationColumn", HeaderText = SettingsService.T("Location", "الموقع"), DataPropertyName = "Location", FillWeight = 100 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PriorityColumn", HeaderText = SettingsService.T("Priority", "الأولوية"), DataPropertyName = "Priority", FillWeight = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "StatusColumn", HeaderText = SettingsService.T("Status", "الحالة"), DataPropertyName = "Status", FillWeight = 70 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreatedByColumn", HeaderText = SettingsService.T("Created By", "مقدم البلاغ"), DataPropertyName = "CreatedByName", FillWeight = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "UpdatedColumn", HeaderText = SettingsService.T("Updated", "آخر تحديث"), DataPropertyName = "UpdatedAt", DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }, FillWeight = 90 });
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) OpenSelectedTicket(); };
        _grid.CellFormatting += FormatGridCell;
    }

    private void FormatGridCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.Value is null) return;
        string columnName = _grid.Columns[e.ColumnIndex].Name;
        if (columnName == "StatusColumn" && e.Value is TicketStatus status)
        {
            e.Value = SettingsService.StatusText(status);
            e.CellStyle.BackColor = ThemeService.StatusColor(status);
            e.CellStyle.ForeColor = Color.White;
            e.CellStyle.SelectionBackColor = ThemeService.StatusColor(status);
            e.CellStyle.SelectionForeColor = Color.White;
            e.FormattingApplied = true;
        }
        else if (columnName == "PriorityColumn" && e.Value is TicketPriority priority)
        {
            e.Value = SettingsService.PriorityText(priority);
            e.CellStyle.ForeColor = ThemeService.PriorityColor(priority);
            e.FormattingApplied = true;
        }
        else if (columnName == "CategoryColumn")
        {
            e.Value = SettingsService.CategoryText(e.Value.ToString()!);
            e.FormattingApplied = true;
        }
    }

    private void RefreshGrid()
    {
        User user = AuthService.CurrentUser!;
        IEnumerable<MaintenanceTicket> query = DataService.Data.Tickets;
        if (user.Role != UserRole.Admin) query = query.Where(t => t.CreatedByUserId == user.Id);
        if (_statusFilter.SelectedItem is UiChoice<TicketStatus?> choice && choice.Value.HasValue) query = query.Where(t => t.Status == choice.Value.Value);
        if (_priorityFilter.SelectedItem is UiChoice<TicketPriority?> priority && priority.Value.HasValue) query = query.Where(t => t.Priority == priority.Value.Value);
        if (_categoryFilter.SelectedItem is UiChoice<string?> category && !string.IsNullOrWhiteSpace(category.Value)) query = query.Where(t => t.Category == category.Value);
        string search = _search.Text.Trim();
        if (search.Length > 0) query = query.Where(t => t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || t.Location.Contains(search, StringComparison.OrdinalIgnoreCase) || t.Id.ToString().Contains(search));
        _visibleTickets = query.OrderByDescending(t => t.UpdatedAt).ToList();
        _grid.DataSource = null;
        _grid.DataSource = _visibleTickets;
        _stats.Text = SettingsService.T(
            $"Total: {_visibleTickets.Count}    New: {_visibleTickets.Count(t => t.Status == TicketStatus.New)}    In progress: {_visibleTickets.Count(t => t.Status == TicketStatus.InProgress)}    Resolved: {_visibleTickets.Count(t => t.Status == TicketStatus.Resolved)}",
            $"الإجمالي: {_visibleTickets.Count}    جديد: {_visibleTickets.Count(t => t.Status == TicketStatus.New)}    قيد التنفيذ: {_visibleTickets.Count(t => t.Status == TicketStatus.InProgress)}    تم الحل: {_visibleTickets.Count(t => t.Status == TicketStatus.Resolved)}");
    }

    private void OpenSelectedTicket()
    {
        if (_grid.CurrentRow?.DataBoundItem is not MaintenanceTicket ticket) return;
        using TicketDetailsForm form = new(ticket);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshGrid();
    }

    private void EditSelectedTicket()
    {
        if (_grid.CurrentRow?.DataBoundItem is not MaintenanceTicket ticket) return;
        User user = AuthService.CurrentUser!;
        if (user.Role != UserRole.Admin && (ticket.CreatedByUserId != user.Id || ticket.Status != TicketStatus.New))
        {
            MessageBox.Show(SettingsService.T("You can edit only your new requests.", "يمكنك تعديل بلاغاتك الجديدة فقط."), SettingsService.T("Access denied", "غير مسموح"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using NewTicketForm form = new(ticket);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshGrid();
    }

    private void DeleteSelectedTicket()
    {
        if (_grid.CurrentRow?.DataBoundItem is not MaintenanceTicket ticket) return;
        User user = AuthService.CurrentUser!;
        if (user.Role != UserRole.Admin && (ticket.CreatedByUserId != user.Id || ticket.Status != TicketStatus.New))
        {
            MessageBox.Show(SettingsService.T("You can delete only your new requests.", "يمكنك حذف بلاغاتك الجديدة فقط."), SettingsService.T("Access denied", "غير مسموح"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        DialogResult answer = MessageBox.Show(SettingsService.T($"Delete request #{ticket.Id}? This cannot be undone.", $"هل تريد حذف البلاغ رقم {ticket.Id}؟ لا يمكن التراجع."), SettingsService.T("Confirm deletion", "تأكيد الحذف"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answer != DialogResult.Yes) return;
        DataService.DeleteTicket(ticket);
        RefreshGrid();
    }

    private void OpenSettings()
    {
        using SettingsForm form = new();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        DialogResult = DialogResult.Retry;
        Close();
    }

    private void Logout()
    {
        if (SettingsService.Current.ConfirmBeforeLogout)
        {
            DialogResult answer = MessageBox.Show(SettingsService.T("Do you want to log out?", "هل تريد تسجيل الخروج؟"), SettingsService.T("Logout", "تسجيل الخروج"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;
        }
        AuthService.Logout();
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void ExportCsv()
    {
        if (_visibleTickets.Count == 0)
        {
            MessageBox.Show(SettingsService.T("There are no requests to export.", "لا توجد بلاغات لتصديرها."));
            return;
        }
        string path = DataService.ExportCsv(_visibleTickets);
        MessageBox.Show(SettingsService.T($"The CSV file was saved to:\n{path}", $"تم حفظ ملف CSV في:\n{path}"), SettingsService.T("Export completed", "اكتمل التصدير"), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
