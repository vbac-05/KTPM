using System;
using System.Threading;
using System.Windows.Forms;

namespace QLThuocWin
{
    internal static class Program
    {
        /// <summary>
        /// Điểm vào ứng dụng WinForms.
        /// Flow:
        ///   1) Hiển thị LoginForm dưới dạng dialog
        ///   2) Nếu đăng nhập OK -> chạy MainForm(roleId, username)
        ///   3) Nếu Hủy/Sai -> thoát ứng dụng
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // (Tuỳ chọn) Đảm bảo chỉ một instance chạy
            // using var mtx = new Mutex(initiallyOwned: true, name: "QLThuocWin_SingleInstance", out bool isNew);
            // if (!isNew) return;

            // Cấu hình WinForms .NET (DPI, font, visual styles…)
            ApplicationConfiguration.Initialize();

            // Bắt lỗi chưa xử lý toàn cục -> hiện MessageBox thay vì crash câm
            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show("Đã xảy ra lỗi không mong muốn:\n\n" + e.Exception.Message,
                    "Lỗi ứng dụng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show("Lỗi nghiêm trọng:\n\n" + (ex?.Message ?? e.ExceptionObject.ToString()),
                    "Lỗi ứng dụng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // ======== FLOW KHỞI ĐỘNG ========
            using (var login = new UI.LoginForm())
            {
                var result = login.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // LoginForm đã set RoleId/Username trước khi Close()
                    var roleId = string.IsNullOrWhiteSpace(login.RoleId) ? "VT02" : login.RoleId.Trim();
                    var username = string.IsNullOrWhiteSpace(login.Username) ? "User" : login.Username.Trim();

                    // Chạy MainForm trong message loop chính
                    Application.Run(new UI.MainForm(roleId, username));
                }
                else
                {
                    // Người dùng hủy/thoát ở màn login
                    Application.Exit();
                }
            }
        }
    }
}
