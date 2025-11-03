using System;
using System.Drawing;
using System.Windows.Forms;
// using QLThuocWin.Services;        // nếu có tầng Service
using QLThuocWin.Controller;          // LoginController của bạn
using QLThuocWin.Entities;            // TaiKhoan (nếu cần)

namespace QLThuocWin.UI
{
    public class LoginForm : Form
    {
        private readonly TextBox txtUsername = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly Button btnLogin = new Button();
        private readonly Button btnGuest = new Button();

        // KẾT QUẢ SAU KHI LOGIN: Program.cs sẽ đọc 2 thuộc tính này
        public string RoleId { get; private set; } = "";
        public string Username { get; private set; } = "";

        private readonly LoginController _loginController;

        public LoginForm()
        {
            // nếu bạn chưa có controller, có thể comment dòng dưới và dùng demo fallback ở PerformLogin()
            _loginController = new LoginController();

            Text = "Đăng nhập hệ thống";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(420, 230);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;

            // ===== Title =====
            var lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP",
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold)
            };
            Controls.Add(lblTitle);

            // ===== Username =====
            var lblUser = new Label { Text = "Username:", AutoSize = true };
            Controls.Add(lblUser);
            Controls.Add(txtUsername);

            // ===== Password =====
            var lblPass = new Label { Text = "Password:", AutoSize = true };
            Controls.Add(lblPass);
            txtPassword.PasswordChar = '•';
            Controls.Add(txtPassword);

            // ===== Buttons =====
            btnLogin.Text = "  Login";
            btnLogin.Click += (s, e) => PerformLogin();
            Controls.Add(btnLogin);

            btnGuest.Text = "  Chế độ khách";
            btnGuest.Click += (s, e) => OpenGuestMode();
            Controls.Add(btnGuest);

            // ===== Layout simple =====
            Layout += (_, __) =>
            {
                lblTitle.Location = new Point((ClientSize.Width - lblTitle.PreferredWidth) / 2, 15);

                lblUser.Location = new Point(30, 70);
                txtUsername.SetBounds(120, 68, 240, txtUsername.Height);

                lblPass.Location = new Point(30, 105);
                txtPassword.SetBounds(120, 103, 240, txtPassword.Height);

                btnLogin.SetBounds(250, 145, 110, 28);
                btnGuest.SetBounds(20, 145, 140, 28);
            };

            // ===== Shortcuts =====
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) PerformLogin(); };
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
        }

        private void PerformLogin()
        {
            var user = txtUsername.Text.Trim();
            var pass = txtPassword.Text; // TODO: hash nếu DB lưu băm

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username/Password", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ======= LOGIN THẬT: qua Controller/DAO (MySQL) =======
                // Giả sử LoginController có hàm Authenticate trả về TaiKhoan hoặc null
                // TaiKhoan: { TenTK, IdVT, ... }
                TaiKhoan? tk = null;
                if (_loginController != null)
                {
                    tk = _loginController.Authenticate(user, pass);
                }

                // ======= Fallback DEMO nếu controller chưa xong =======
                if (tk == null)
                {
                    if ((user == "admin" && pass == "admin") || (user == "nv" && pass == "nv"))
                    {
                        tk = new TaiKhoan
                        {
                            TenTK = user,
                            IdVT = (user == "admin") ? "VT01" : "VT02"
                        };
                    }
                }

                if (tk == null)
                {
                    MessageBox.Show("Sai username hoặc password", "Lỗi đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Thành công → gán thuộc tính để Program.cs mở MainForm
                Username = tk.TenTK ?? user;
                RoleId   = tk.IdVT ?? "VT02";

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng nhập: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenGuestMode()
        {
            // Gợi ý: chế độ khách nên mở form riêng rồi quay về Login
            using var guest = new GuestFeedbackForm();
            Hide();
            guest.StartPosition = FormStartPosition.CenterScreen;
            guest.ShowDialog();
            Show(); // quay lại login sau khi đóng guest
        }
    }
}
