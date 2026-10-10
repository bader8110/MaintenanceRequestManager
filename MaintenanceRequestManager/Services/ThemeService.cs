using MaintenanceRequestManager.Models;

namespace MaintenanceRequestManager.Services;

public static class ThemeService
{
    public static Color Background => SettingsService.IsDark ? Color.FromArgb(18, 24, 30) : Color.FromArgb(245, 247, 250);
    public static Color Surface => SettingsService.IsDark ? Color.FromArgb(29, 37, 45) : Color.White;
    public static Color Field => SettingsService.IsDark ? Color.FromArgb(38, 48, 57) : Color.White;
    public static Color Text => SettingsService.IsDark ? Color.FromArgb(235, 241, 244) : Color.FromArgb(32, 42, 50);
    public static Color Muted => SettingsService.IsDark ? Color.FromArgb(169, 184, 194) : Color.DimGray;
    public static Color Border => SettingsService.IsDark ? Color.FromArgb(65, 78, 88) : Color.FromArgb(215, 224, 230);
    public static Color Header => SettingsService.IsDark ? Color.FromArgb(12, 32, 43) : Color.FromArgb(24, 63, 86);
    public static Color Primary => Color.FromArgb(33, 115, 160);
    public static Color Success => Color.FromArgb(65, 125, 88);
    public static Color Danger => Color.FromArgb(184, 67, 67);

    public static void Apply(Form form)
    {
        form.BackColor = Background;
        form.ForeColor = Text;
        form.RightToLeft = SettingsService.IsArabic ? RightToLeft.Yes : RightToLeft.No;
        ApplyChildren(form.Controls);
    }

    private static void ApplyChildren(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            string tag = control.Tag?.ToString() ?? string.Empty;
            switch (control)
            {
                case Panel panel:
                    panel.BackColor = tag == "header" ? Header : Surface;
                    panel.ForeColor = tag == "header" ? Color.White : Text;
                    break;
                case Label label:
                    label.ForeColor = tag switch
                    {
                        "header" => Color.White,
                        "muted" => Muted,
                        "accent" => Primary,
                        _ => Text
                    };
                    break;
                case TextBox textBox:
                    textBox.BackColor = Field;
                    textBox.ForeColor = Text;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ComboBox comboBox:
                    comboBox.BackColor = Field;
                    comboBox.ForeColor = Text;
                    break;
                case CheckBox checkBox:
                    checkBox.ForeColor = Text;
                    break;
                case Button button:
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.ForeColor = Color.White;
                    button.BackColor = tag switch
                    {
                        "danger" => Danger,
                        "success" => Success,
                        "secondary" => SettingsService.IsDark ? Color.FromArgb(73, 87, 98) : Color.FromArgb(94, 109, 119),
                        _ => Primary
                    };
                    break;
                case DataGridView grid:
                    ApplyGrid(grid);
                    break;
            }

            if (control.HasChildren) ApplyChildren(control.Controls);
        }
    }

    private static void ApplyGrid(DataGridView grid)
    {
        grid.BackgroundColor = Background;
        grid.GridColor = Border;
        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = SettingsService.IsDark ? Color.FromArgb(47, 91, 116) : Color.FromArgb(204, 231, 244);
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.AlternatingRowsDefaultCellStyle.BackColor = SettingsService.IsDark ? Color.FromArgb(34, 43, 51) : Color.FromArgb(248, 251, 252);
        grid.ColumnHeadersDefaultCellStyle.BackColor = Header;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Header;
        grid.EnableHeadersVisualStyles = false;
    }

    public static Color StatusColor(TicketStatus status) => status switch
    {
        TicketStatus.New => Color.FromArgb(48, 133, 192),
        TicketStatus.InProgress => Color.FromArgb(210, 139, 32),
        TicketStatus.Resolved => Color.FromArgb(46, 139, 87),
        TicketStatus.Closed => Color.FromArgb(112, 124, 132),
        _ => Primary
    };

    public static Color PriorityColor(TicketPriority priority) => priority switch
    {
        TicketPriority.Urgent => Color.FromArgb(205, 67, 67),
        TicketPriority.High => Color.FromArgb(224, 126, 45),
        TicketPriority.Medium => Color.FromArgb(58, 126, 169),
        _ => Color.FromArgb(86, 145, 101)
    };
}
