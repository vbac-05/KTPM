using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLThuocWin.UI
{
    /// <summary>
    /// Chuẩn hoá cách các Panel (UserControl) tải dữ liệu khi được hiển thị.
    /// Các Panel nên implement ILoadable và viết hàm LoadData().
    /// </summary>
    public interface ILoadable
    {
        void LoadData();
    }

    public class MainForm : Form
    {
        private readonly string _roleId;
        private readonly string _username;

        private TabControl _tabs = default!;
        private StatusStrip _status = default!;
        private ToolStripStatusLabel _statusUser = default!;
        private ToolStripStatusLabel _statusRole = default!;
        private ToolStripStatusLabel _statusTime = default!;
        private Timer _clockTimer = default!;

        public MainForm(string roleId, string username)
        {
            _roleId   = (roleId ?? "").Trim();
            _username = string.IsNullOrWhiteSpace(username) ? "User" : username.Trim();

            Text = "Hệ thống Quản lý Nhà thuốc";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            BuildTopBar();
            BuildTabsByRole(_roleId);
            BuildStatusBar();
            BuildClock();
        }

        private void BuildTopBar()
        {
            var top = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.WhiteSmoke
            };
            Controls.Add(top);

            var lblTitle = new Label
            {
                AutoSize = true,
                Text = $"Xin chào, {_username}",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                Location = new Point(12, 14)
            };
            top.Controls.Add(lblTitle);

            var btnRefresh = new Button
            {
                Text = "Làm mới tab",
                Height = 28,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnRefresh.Click += (s, e) => RefreshCurrentTab();
            top.Controls.Add(btnRefresh);

            var btnLogout = new Button
            {
                Text = "Đăng xuất",
                Height = 28,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogout.Click += OnLogoutClicked;
            top.Controls.Add(btnLogout);

            // canh phải
            top.Resize += (s, e) =>
            {
                btnLogout.Left  = top.Width - btnLogout.Width - 12;
                btnLogout.Top   = 10;
                btnRefresh.Left = btnLogout.Left - btnRefresh.Width - 8;
                btnRefresh.Top  = 10;
            };
        }

        private void BuildTabsByRole(string roleId)
        {
            _tabs = new TabControl { Dock = DockStyle.Fill };
            _tabs.SelectedIndexChanged += (s, e) => TryLoadSelectedTab();
            Controls.Add(_tabs);

            // ========== KHAI BÁO CÁC TAB THEO ROLE ==========
            // Lưu ý: các class dưới đây phải tồn tại trong UI/
            // Nếu bạn dùng ThuocControl thay vì ThuocPanel, chỉ cần đổi generic type tương ứng.

            void AddAllTabsForAdmin()
            {
                AddTab<ThuocPanel>("Thuốc");
                AddTab<NhanVienPanel>("Nhân viên");
                AddTab<KhachHangPanel>("Khách hàng");
                AddTab<NhaCungCapPanel>("Nhà cung cấp");
                AddTab<HoaDonPanel>("Hóa đơn");
                AddTab<PhieuNhapPanel>("Phiếu nhập");
                AddTab<PhanHoiPanel>("Phản hồi");
                AddTab<HopDongPanel>("Hợp đồng");
                AddTab<TrashPanel>("Thùng rác");
            }

            void AddTabsForStaff()
            {
                AddTab<ThuocPanel>("Thuốc");
                AddTab<KhachHangPanel>("Khách hàng");
                AddTab<NhaCungCapPanel>("Nhà cung cấp");
                AddTab<HoaDonPanel>("Hóa đơn");
                AddTab<PhieuNhapPanel>("Phiếu nhập");
                AddTab<PhanHoiPanel>("Phản hồi");
                AddTab<TrashPanel>("Thùng rác");
            }

            if (string.Equals(roleId, "VT01", StringComparison.OrdinalIgnoreCase))
                AddAllTabsForAdmin();
            else if (string.Equals(roleId, "VT02", StringComparison.OrdinalIgnoreCase))
                AddTabsForStaff();
            else
            {
                // Fallback (không rõ role): ít nhất cho hiện 2 tab cơ bản
                AddTab<ThuocPanel>("Thuốc");
                AddTab<KhachHangPanel>("Khách hàng");
            }
        }

        /// <summary>
        /// Tạo 1 TabPage chứa 1 UserControl kiểu T (Dock=Fill).
        /// Nếu type T không tồn tại, sẽ tạo tab báo lỗi để bạn dễ phát hiện.
        /// </summary>
        private void AddTab<T>(string title) where T : Control, new()
        {
            var page = new TabPage(title);
            try
            {
                var content = new T { Dock = DockStyle.Fill };
                page.Controls.Add(content);
            }
            catch (Exception ex)
            {
                page.Controls.Add(new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    Dock = DockStyle.Fill,
                    Text = $"Không thể khởi tạo {typeof(T).Name}.\r\n" +
                           $"Hãy kiểm tra class trong UI/ và namespace.\r\n\r\nChi tiết:\r\n{ex}"
                });
            }
            _tabs.TabPages.Add(page);
        }

        private void TryLoadSelectedTab()
        {
            if (_tabs.SelectedTab == null) return;
            if (_tabs.SelectedTab.Controls.Count == 0) return;

            var content = _tabs.SelectedTab.Controls[0];
            if (content is ILoadable loadable)
            {
                try { loadable.LoadData(); }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải dữ liệu tab \"{_tabs.SelectedTab.Text}\":\n{ex.Message}",
                        "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshCurrentTab() => TryLoadSelectedTab();

        private void BuildStatusBar()
        {
            _status = new StatusStrip();
            _statusUser = new ToolStripStatusLabel($"User: {_username}");
            _statusRole = new ToolStripStatusLabel($"Role: {_roleId}");
            _statusTime = new ToolStripStatusLabel(DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"));

            _status.Items.Add(_statusUser);
            _status.Items.Add(new ToolStripStatusLabel("|"));
            _status.Items.Add(_statusRole);
            _status.Items.Add(new ToolStripStatusLabel("|"));
            _status.Items.Add(_statusTime);

            Controls.Add(_status);
        }

        private void BuildClock()
        {
            _clockTimer = new Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) => _statusTime.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            _clockTimer.Start();
        }

        private void OnLogoutClicked(object? sender, EventArgs e)
        {
            // Quy trình an toàn:
            // 1) Ẩn MainForm hiện tại
            // 2) Mở LoginForm như dialog (nếu có)
            // 3) Nếu login thành công => mở MainForm mới; nếu huỷ => đóng app

            this.Hide();
            try
            {
                using (var login = new LoginForm()) // Nếu chưa có LoginForm, hãy tạo/đổi class name đúng
                {
                    var result = login.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var mf = new MainForm(login.RoleId, login.Username);
                        mf.FormClosed += (_, __) => this.Close(); // đóng main cũ khi main mới đóng
                        mf.Show();
                        return;
                    }
                }
            }
            catch
            {
                // Nếu chưa có LoginForm hoặc lỗi, fallback: thoát hẳn
            }
            this.Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Lazy load cho tab đầu tiên
            TryLoadSelectedTab();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit(); // đảm bảo thoát process khi form chính đóng
        }
    }
}
