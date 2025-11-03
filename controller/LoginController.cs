using System;

// TODO: đổi namespace theo project thật của bạn
using QLThuocApp.DAO;       // Chứa TaiKhoanDAO
using QLThuocApp.Entities;  // Chứa entity TaiKhoan

namespace QLThuocApp.Controllers
{
    /// <summary>
    /// LoginController.cs
    /// Controller xử lý nghiệp vụ đăng nhập.
    /// Không viết SQL, chỉ kiểm tra dữ liệu đầu vào và gọi DAO để lấy thông tin tài khoản.
    /// </summary>
    public class LoginController
    {
        private readonly TaiKhoanDAO taiKhoanDAO;

        public LoginController()
        {
            taiKhoanDAO = new TaiKhoanDAO(); // TODO: nếu DAO yêu cầu DBConnection hoặc context, truyền thêm ở đây
        }

        /// <summary>
        /// Xác thực username/password.
        /// Nếu hợp lệ, trả về đối tượng TaiKhoan (chứa idVT, v.v...).
        /// Nếu sai, trả về null.
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Tài khoản hợp lệ hoặc null nếu sai</returns>
        public TaiKhoan AuthenticateAndGetAccount(string username, string password)
        {
            // Kiểm tra dữ liệu đầu vào — nghiệp vụ nhẹ, hợp lệ trong Controller
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            // Gọi DAO để lấy tài khoản theo username
            TaiKhoan tk = taiKhoanDAO.GetByUsername(username);

            // Nếu tồn tại và password khớp (so sánh chuỗi tạm thời)
            if (tk != null && tk.Password == password)
            {
                return tk;
            }

            return null;
        }
    }
}
