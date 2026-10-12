using System.Drawing;

namespace bai4._7;

public class FormMain : Form
{
    private readonly ToolStripStatusLabel userStatusLabel = new("Người dùng: Admin");
    private readonly ToolStripStatusLabel messageStatusLabel = new("Sẵn sàng");
    private readonly ToolStripStatusLabel timeStatusLabel = new();
    private readonly System.Windows.Forms.Timer clockTimer = new();

    public FormMain()
    {
        InitializeMainForm();
        BuildMenus();
        BuildToolStrip();
        BuildStatusStrip();
        BuildContextMenu();

        clockTimer.Interval = 1000;
        clockTimer.Tick += (_, _) => timeStatusLabel.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        clockTimer.Start();
        FormClosed += (_, _) => clockTimer.Dispose();
    }

    private void InitializeMainForm()
    {
        IsMdiContainer = true;
        Text = "Quản lý hệ thống - MDI";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(900, 600);
        BackColor = Color.WhiteSmoke;
    }

    private void BuildMenus()
    {
        var menuStrip = new MenuStrip();

        var systemMenu = new ToolStripMenuItem("Hệ thống");
        systemMenu.DropDownItems.Add("Đăng xuất", null, (_, _) => Logout());
        systemMenu.DropDownItems.Add(new ToolStripSeparator());
        systemMenu.DropDownItems.Add("Thoát", null, (_, _) => Close());

        var catalogMenu = new ToolStripMenuItem("Danh mục");
        catalogMenu.DropDownItems.Add("Quản lý Sinh viên", null, (_, _) => OpenChild<FormStudent>());
        catalogMenu.DropDownItems.Add("Quản lý Sản phẩm", null, (_, _) => OpenChild<FormProduct>());

        var windowMenu = new ToolStripMenuItem("Cửa sổ");
        windowMenu.DropDownItems.Add("Xếp tầng (Cascade)", null, (_, _) => LayoutMdi(MdiLayout.Cascade));
        windowMenu.DropDownItems.Add("Xếp ngang (Tile Horizontal)", null, (_, _) => LayoutMdi(MdiLayout.TileHorizontal));
        windowMenu.DropDownItems.Add("Xếp dọc (Tile Vertical)", null, (_, _) => LayoutMdi(MdiLayout.TileVertical));

        menuStrip.Items.AddRange([systemMenu, catalogMenu, windowMenu]);
        MainMenuStrip = menuStrip;
        Controls.Add(menuStrip);
    }

    private void BuildToolStrip()
    {
        var toolStrip = new ToolStrip { Dock = DockStyle.Top, GripStyle = ToolStripGripStyle.Hidden };
        toolStrip.Items.Add(new ToolStripButton("Sinh viên", null, (_, _) => OpenChild<FormStudent>())
        {
            ToolTipText = "Mở quản lý sinh viên"
        });
        toolStrip.Items.Add(new ToolStripButton("Sản phẩm", null, (_, _) => OpenChild<FormProduct>())
        {
            ToolTipText = "Mở quản lý sản phẩm"
        });
        toolStrip.Items.Add(new ToolStripSeparator());
        toolStrip.Items.Add(new ToolStripButton("Xếp tầng", null, (_, _) => LayoutMdi(MdiLayout.Cascade)));
        toolStrip.Items.Add(new ToolStripButton("Đóng cửa sổ", null, (_, _) => ActiveMdiChild?.Close()));
        Controls.Add(toolStrip);
    }

    private void BuildStatusStrip()
    {
        var statusStrip = new StatusStrip();
        timeStatusLabel.Spring = true;
        timeStatusLabel.TextAlign = ContentAlignment.MiddleRight;
        statusStrip.Items.Add(userStatusLabel);
        statusStrip.Items.Add(new ToolStripSeparator());
        statusStrip.Items.Add(messageStatusLabel);
        statusStrip.Items.Add(timeStatusLabel);
        Controls.Add(statusStrip);
    }

    private void BuildContextMenu()
    {
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Đóng cửa sổ hiện tại", null, (_, _) => ActiveMdiChild?.Close());
        contextMenu.Items.Add("Xếp tầng các cửa sổ", null, (_, _) => LayoutMdi(MdiLayout.Cascade));
        ContextMenuStrip = contextMenu;
    }

    private void Logout()
    {
        userStatusLabel.Text = "Người dùng: Chưa đăng nhập";
        SetStatus("Đã đăng xuất khỏi hệ thống.");
    }

    private void SetStatus(string message) => messageStatusLabel.Text = message;

    private void OpenChild<T>() where T : Form, new()
    {
        var existingChild = MdiChildren.OfType<T>().FirstOrDefault();
        if (existingChild is not null)
        {
            existingChild.Activate();
            SetStatus($"Đã kích hoạt cửa sổ {existingChild.Text}.");
            return;
        }

        var child = new T { MdiParent = this };
        if (child is FormStudent student)
            student.StatusChanged += SetStatus;
        else if (child is FormProduct product)
            product.StatusChanged += SetStatus;

        child.FormClosed += (_, _) => SetStatus($"Đã đóng cửa sổ {child.Text}.");
        child.Show();
        SetStatus($"Đã mở cửa sổ {child.Text}.");
    }
}
