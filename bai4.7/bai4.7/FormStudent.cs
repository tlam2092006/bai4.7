using System.Drawing;

namespace bai4._7;

public class FormStudent : Form
{
    private readonly DataGridView studentGrid = new();
    private int nextStudentId = 4;

    public event Action<string>? StatusChanged;

    public FormStudent()
    {
        Text = "Quản lý Sinh viên";
        Size = new Size(900, 550);
        StartPosition = FormStartPosition.CenterParent;
        InitializeControls();
        LoadSampleData();
    }

    private void InitializeControls()
    {
        var commandPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(8),
            BackColor = Color.Gainsboro
        };

        var addButton = new Button { Text = "Thêm sinh viên", AutoSize = true };
        addButton.Click += (_, _) => AddStudent();
        var deleteButton = new Button { Text = "Xóa sinh viên", AutoSize = true };
        deleteButton.Click += (_, _) => DeleteSelectedStudent();
        var refreshButton = new Button { Text = "Làm mới", AutoSize = true };
        refreshButton.Click += (_, _) => RefreshStudents();
        commandPanel.Controls.AddRange([addButton, deleteButton, refreshButton]);

        studentGrid.Dock = DockStyle.Fill;
        studentGrid.AllowUserToAddRows = false;
        studentGrid.AllowUserToDeleteRows = false;
        studentGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        studentGrid.MultiSelect = false;
        studentGrid.ReadOnly = true;
        studentGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        studentGrid.CellMouseDown += SelectRowFromContextMenu;
        studentGrid.ContextMenuStrip = CreateContextMenu();
        studentGrid.Columns.Add("Id", "Mã SV");
        studentGrid.Columns.Add("Name", "Họ và tên");
        studentGrid.Columns.Add("Class", "Lớp");
        studentGrid.Columns.Add("Email", "Email");

        Controls.Add(studentGrid);
        Controls.Add(commandPanel);
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Xóa dòng", null, (_, _) => DeleteSelectedStudent());
        menu.Items.Add("Làm mới dữ liệu", null, (_, _) => RefreshStudents());
        menu.Items.Add("Xem chi tiết", null, (_, _) => ShowSelectedStudent());
        return menu;
    }

    private void SelectRowFromContextMenu(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
        {
            studentGrid.ClearSelection();
            studentGrid.Rows[e.RowIndex].Selected = true;
            studentGrid.CurrentCell = studentGrid.Rows[e.RowIndex].Cells[0];
        }
    }

    private void LoadSampleData()
    {
        studentGrid.Rows.Clear();
        studentGrid.Rows.Add("SV001", "Nguyễn Văn An", "CNTT1", "an@example.com");
        studentGrid.Rows.Add("SV002", "Trần Thị Bình", "CNTT1", "binh@example.com");
        studentGrid.Rows.Add("SV003", "Lê Minh Châu", "CNTT2", "chau@example.com");
        nextStudentId = 4;
    }

    private void AddStudent()
    {
        var id = $"SV{nextStudentId++:000}";
        studentGrid.Rows.Add(id, $"Sinh viên mới {nextStudentId - 1}", "CNTT1", $"sv{nextStudentId - 1}@example.com");
        StatusChanged?.Invoke($"Đã thêm sinh viên {id}.");
    }

    private void DeleteSelectedStudent()
    {
        if (studentGrid.CurrentRow is null || studentGrid.CurrentRow.IsNewRow)
        {
            StatusChanged?.Invoke("Chưa chọn sinh viên để xóa.");
            return;
        }

        var id = studentGrid.CurrentRow.Cells[0].Value?.ToString() ?? string.Empty;
        studentGrid.Rows.Remove(studentGrid.CurrentRow);
        StatusChanged?.Invoke($"Đã xóa sinh viên {id}.");
    }

    private void RefreshStudents()
    {
        LoadSampleData();
        StatusChanged?.Invoke("Đã làm mới danh sách sinh viên.");
    }

    private void ShowSelectedStudent()
    {
        if (studentGrid.CurrentRow is null || studentGrid.CurrentRow.IsNewRow)
        {
            StatusChanged?.Invoke("Chưa chọn sinh viên để xem chi tiết.");
            return;
        }

        var values = studentGrid.CurrentRow.Cells.Cast<DataGridViewCell>()
            .Select(cell => cell.Value?.ToString() ?? string.Empty);
        MessageBox.Show(string.Join(Environment.NewLine, values), "Chi tiết sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
        StatusChanged?.Invoke("Đã xem chi tiết sinh viên.");
    }
}
