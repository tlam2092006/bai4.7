using System.Drawing;

namespace bai4._7;

public class FormProduct : Form
{
    private readonly DataGridView productGrid = new();
    private int nextProductId = 4;

    public event Action<string>? StatusChanged;

    public FormProduct()
    {
        Text = "Quản lý Sản phẩm";
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

        var addButton = new Button { Text = "Thêm sản phẩm", AutoSize = true };
        addButton.Click += (_, _) => AddProduct();
        var deleteButton = new Button { Text = "Xóa sản phẩm", AutoSize = true };
        deleteButton.Click += (_, _) => DeleteSelectedProduct();
        var refreshButton = new Button { Text = "Làm mới", AutoSize = true };
        refreshButton.Click += (_, _) => RefreshProducts();
        commandPanel.Controls.AddRange([addButton, deleteButton, refreshButton]);

        productGrid.Dock = DockStyle.Fill;
        productGrid.AllowUserToAddRows = false;
        productGrid.AllowUserToDeleteRows = false;
        productGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        productGrid.MultiSelect = false;
        productGrid.ReadOnly = true;
        productGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        productGrid.CellMouseDown += SelectRowFromContextMenu;
        productGrid.ContextMenuStrip = CreateContextMenu();
        productGrid.Columns.Add("Id", "Mã SP");
        productGrid.Columns.Add("Name", "Tên sản phẩm");
        productGrid.Columns.Add("Category", "Danh mục");
        productGrid.Columns.Add("Price", "Đơn giá");

        Controls.Add(productGrid);
        Controls.Add(commandPanel);
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Xóa dòng", null, (_, _) => DeleteSelectedProduct());
        menu.Items.Add("Làm mới dữ liệu", null, (_, _) => RefreshProducts());
        menu.Items.Add("Xem chi tiết", null, (_, _) => ShowSelectedProduct());
        return menu;
    }

    private void SelectRowFromContextMenu(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
        {
            productGrid.ClearSelection();
            productGrid.Rows[e.RowIndex].Selected = true;
            productGrid.CurrentCell = productGrid.Rows[e.RowIndex].Cells[0];
        }
    }

    private void LoadSampleData()
    {
        productGrid.Rows.Clear();
        productGrid.Rows.Add("SP001", "Bàn phím cơ", "Thiết bị", "850.000 VNĐ");
        productGrid.Rows.Add("SP002", "Chuột không dây", "Thiết bị", "450.000 VNĐ");
        productGrid.Rows.Add("SP003", "Màn hình 24 inch", "Thiết bị", "3.200.000 VNĐ");
        nextProductId = 4;
    }

    private void AddProduct()
    {
        var id = $"SP{nextProductId++:000}";
        productGrid.Rows.Add(id, $"Sản phẩm mới {nextProductId - 1}", "Khác", "0 VNĐ");
        StatusChanged?.Invoke($"Đã thêm sản phẩm {id}.");
    }

    private void DeleteSelectedProduct()
    {
        if (productGrid.CurrentRow is null || productGrid.CurrentRow.IsNewRow)
        {
            StatusChanged?.Invoke("Chưa chọn sản phẩm để xóa.");
            return;
        }

        var id = productGrid.CurrentRow.Cells[0].Value?.ToString() ?? string.Empty;
        productGrid.Rows.Remove(productGrid.CurrentRow);
        StatusChanged?.Invoke($"Đã xóa sản phẩm {id}.");
    }

    private void RefreshProducts()
    {
        LoadSampleData();
        StatusChanged?.Invoke("Đã làm mới danh sách sản phẩm.");
    }

    private void ShowSelectedProduct()
    {
        if (productGrid.CurrentRow is null || productGrid.CurrentRow.IsNewRow)
        {
            StatusChanged?.Invoke("Chưa chọn sản phẩm để xem chi tiết.");
            return;
        }

        var values = productGrid.CurrentRow.Cells.Cast<DataGridViewCell>()
            .Select(cell => cell.Value?.ToString() ?? string.Empty);
        MessageBox.Show(string.Join(Environment.NewLine, values), "Chi tiết sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        StatusChanged?.Invoke("Đã xem chi tiết sản phẩm.");
    }
}
