using System;
using System.Drawing;
using System.Windows.Forms;
// using QLThuocApp.Services; // TODO: bật khi bạn có tầng Service thực

namespace QLThuocApp.UI
{
    public class LoginForm : Form
    {
        private TextBox txtUsername = new TextBox();
        private TextBox txtPassword = new TextBox();
        private Button btnLogin = new Button();
        private Button btnGuest = new Button();

        // TODO: Inject service thật nếu có (DI)
        // private readonly ILoginService _loginService;
        // public LoginForm(ILoginService loginService) { _loginService = loginService; ... }

        public LoginForm()
        {
            Text = "Đăng nhập hệ thống";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(420, 230);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;

            // ====== Title ======
            var lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP",
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold)
                // TODO: Icon tiêu đề nếu muốn: this.Icon = new Icon("..."); 
            };
            // căn giữa nhẹ nhàng
            lblTitle.Location = new Point((ClientSize.Width - lblTitle.PreferredWidth) / 2, 15);
            Controls.Add(lblTitle);

            // ====== Username ======
            var lblUser = new Label { Text = "Username:", AutoSize = true, Location = new Point(30, 70) };
            Controls.Add(lblUser);

            txtUsername.Location = new Point(120, 68);
            txtUsername.Width = 240;
            Controls.Add(txtUsername);

            // ====== Password ======
            var lblPass = new Label { Text = "Password:", AutoSize = true, Location = new Point(30, 105) };
            Controls.Add(lblPass);

            txtPassword.Location = new Point(120, 103);
            txtPassword.Width = 240;
            txtPassword.PasswordChar = '•';
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) PerformLogin(); }; // Enter để login
            Controls.Add(txtPassword);

            // ====== Buttons ======
            btnLogin.Text = "  Login";
            btnLogin.Width = 110;
            btnLogin.Location = new Point(250, 145);
            btnLogin.Click += (s, e) => PerformLogin();
            // TODO: Thêm icon nếu có resource: btnLogin.Image = Properties.Resources.login; btnLogin.ImageAlign = ContentAlignment.MiddleLeft;
            Controls.Add(btnLogin);

            btnGuest.Text = "  Chế độ khách";
            btnGuest.Width = 140;
            btnGuest.Location = new Point(20, 145);
            btnGuest.Click += (s, e) => OpenGuestMode();
            // TODO: Thêm icon nếu có resource: btnGuest.Image = Properties.Resources.guest; btnGuest.ImageAlign = ContentAlignment.MiddleLeft;
            Controls.Add(btnGuest);
        }

        private void PerformLogin()
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username/Password", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ====== LOGIN THẬT ======
            // TODO: THAY bằng gọi Service/Repo thực tế tương đương Java: loginController.authenticateAndGetAccount(username, password)
            // Ví dụ:
            // var account = _loginService.Authenticate(username, password);
            // if (account == null) { ... } else { var roleId = account.RoleId; ... }

            // DEMO: giả lập tài khoản
            // "admin/admin" -> VT01 (Admin); "nv/nv" -> VT02 (Nhân viên)
            bool ok = (username == "admin" && password == "admin")
                   || (username == "nv" && password == "nv");

            if (!ok)
            {
                MessageBox.Show("Sai username hoặc password", "Lỗi đăng nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string roleId = username == "admin" ? "VT01" : "VT02"; // TODO: Lấy từ account.IdVT (giống Java: tk.getIdVT())

            // ====== MỞ MAIN FORM ======
            // TODO: THAY MainForm bằng form chính của bạn, mapping tham số giống Java MainFrame(roleId)
            // var main = new MainForm(roleId, username); // nếu MainForm có nhận roleId/username
            var main = new MainForm(roleId); // TODO: nếu constructor khác, đổi ở đây
            main.Show();

            // Đóng form login
            Hide();
            // Nếu muốn đóng hẳn tiến trình khi MainForm đóng:
            main.FormClosed += (s, e) => Close();
        }

        private void OpenGuestMode()
        {
            // Mở chế độ khách: phiếu phản hồi
            // TODO: Nếu GuestFeedbackForm có phụ thuộc Service, chỉnh constructor cho phù hợp
            var guest = new GuestFeedbackForm();
            guest.StartPosition = FormStartPosition.CenterScreen;
            guest.Show();

            // Đóng form login
            Hide();
            guest.FormClosed += (s, e) => Show(); // quay lại login khi đóng guest (nếu muốn)
        }
    }
}
