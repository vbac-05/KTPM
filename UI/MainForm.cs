using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLThuocWin.UI
{
    public class MainForm : Form
    {
        private readonly string _roleId;
        private readonly string _username;
        private TabControl tabbed;

        // TODO: Nếu bạn không muốn truyền username, có thể tạo overload MainForm(string roleId)
        public MainForm(string roleId, string username)
        {
            _roleId = roleId?.Trim() ?? string.Empty;
            _username = string.IsNullOrWhiteSpace(username) ? "User" : username.Trim();

            Text = "Hệ thống Quản lý Nhà thuốc - " + _username; // TODO: đổi title nếu cần
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            // (Tuỳ chọn) Icon cho form:
            // TODO: this.Icon = Properties.Resources.app; 

            BuildTopBar();                // Thanh trên cùng + nút Đăng xuất
            BuildTabsByRole(_roleId);     // Thêm tab theo role (VT01 / VT02 / fallback)
        }

        private void BuildTopBar()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.WhiteSmoke };
            Controls.Add(top);

            // Nhãn chào
            var lblHello = new Label
            {
                AutoSize = true,
                Text = "Xin chào, " + _username,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                Location = new Point(10, 14)
            };
            top.Controls.Add(lblHello);

            // Nút Đăng xuất (góc phải)
            var btnLogout = new Button
            {
                Text = "Đăng xuất",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            // TODO: gắn icon nếu có resource: btnLogout.Image = Properties.Resources.logout; btnLogout.ImageAlign = ContentAlignment.MiddleLeft;
            btnLogout.Click += (s, e) =>
            {
                // Đóng MainForm và mở lại LoginForm — giống Java
                Close();
                // TODO: nếu bạn có DI, có thể cần LoginForm(...) khác
                new LoginForm().Show();
            };
            // canh phải sau khi Add vào top
            top.Controls.Add(btnLogout);
            top.Resize += (s, e) =>
            {
                btnLogout.Location = new Point(top.Width - btnLogout.Width - 12, 10);
            };
            // đặt vị trí lần đầu
            btnLogout.Location = new Point(top.Width - btnLogout.Width - 12, 10);
        }

        private void BuildTabsByRole(string roleId)
        {
            tabbed = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabbed);

            // ===== Khởi tạo tất cả panel trước như Java =====
            // Lưu ý: Trong phần C# trước đây bạn có class tên "ThuocControl".
            // Để “match” Java, ta dùng ThuocPanel. 
            // TODO: Nếu project của bạn vẫn là ThuocControl, đổi dòng dưới về new ThuocControl().
            var thuocPanel       = new ThuocPanel();        // TODO: đổi sang class thật nếu tên khác
            var nhanVienPanel    = new NhanVienPanel();     // TODO: đổi sang class thật nếu tên khác
            var khachHangPanel   = new KhachHangPanel();    // TODO: đổi sang class thật nếu tên khác
            var nhaCungCapPanel  = new NhaCungCapPanel();   // TODO: đổi sang class thật nếu tên khác
            var hoaDonPanel      = new HoaDonPanel();       // TODO: đổi sang class thật nếu tên khác
            var phieuNhapPanel   = new PhieuNhapPanel();    // TODO: đổi sang class thật nếu tên khác
            var phanHoiPanel     = new PhanHoiPanel();      // TODO: đổi sang class thật nếu tên khác
            var hopDongPanel     = new HopDongPanel();      // TODO: đổi sang class thật nếu tên khác
            var trashPanel       = new TrashPanel();        // TODO: đổi sang class thật nếu tên khác

            // ===== Map role giống Java =====
            if (string.Equals(roleId, "VT01", StringComparison.OrdinalIgnoreCase))
            {
                // Admin: đủ 9 tab
                tabbed.TabPages.Add("Thuốc").Controls.Add(thuocPanel);
                tabbed.TabPages.Add("Nhân viên").Controls.Add(nhanVienPanel);
                tabbed.TabPages.Add("Khách hàng").Controls.Add(khachHangPanel);
                tabbed.TabPages.Add("Nhà cung cấp").Controls.Add(nhaCungCapPanel);
                tabbed.TabPages.Add("Hóa đơn").Controls.Add(hoaDonPanel);
                tabbed.TabPages.Add("Phiếu nhập").Controls.Add(phieuNhapPanel);
                tabbed.TabPages.Add("Phản hồi").Controls.Add(phanHoiPanel);
                tabbed.TabPages.Add("Hợp đồng").Controls.Add(hopDongPanel);
                tabbed.TabPages.Add("Thùng rác").Controls.Add(trashPanel);
            }
            else if (string.Equals(roleId, "VT02", StringComparison.OrdinalIgnoreCase))
            {
                // Nhân viên: 7 tab (không “Nhân viên”, “Hợp đồng”? — theo Java bạn đã ẩn HĐ, có “Thùng rác”)
                tabbed.TabPages.Add("Thuốc").Controls.Add(thuocPanel);
                tabbed.TabPages.Add("Khách hàng").Controls.Add(khachHangPanel);
                tabbed.TabPages.Add("Nhà cung cấp").Controls.Add(nhaCungCapPanel);
                tabbed.TabPages.Add("Hóa đơn").Controls.Add(hoaDonPanel);
                tabbed.TabPages.Add("Phiếu nhập").Controls.Add(phieuNhapPanel);
                tabbed.TabPages.Add("Phản hồi").Controls.Add(phanHoiPanel);
                tabbed.TabPages.Add("Thùng rác").Controls.Add(trashPanel);
            }
            else
            {
                // Fallback: tránh UI trắng — giống Java: ít nhất “Thuốc”, “Khách hàng”
                tabbed.TabPages.Add("Thuốc").Controls.Add(thuocPanel);
                tabbed.TabPages.Add("Khách hàng").Controls.Add(khachHangPanel);
            }
        }
    }
}
