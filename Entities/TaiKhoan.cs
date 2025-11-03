// File: Models/TaiKhoan.cs
// Nguồn gốc: chuyển từ Java entities/TaiKhoan.java
// Mapping & quy ước đặt tên (QUAN TRỌNG):
// - Java String      -> C# string
// - camelCase (Java) -> PascalCase (chuẩn .NET)          // CHANGED
// - Thuộc tính ràng buộc dữ liệu: dùng get;set; tự động
// - Có thể để default = "" để tránh null trong binding

namespace QLThuocApp.Entities
{
    /// <summary>
    /// Bản đồ 1-1 với bảng TaiKhoan:
    ///   IdTK, Username, Password, IdNV, IdVT
    /// </summary>
    public class TaiKhoan
    {
        // idTK
        public string IdTK { get; set; } = string.Empty;

        // username
        public string Username { get; set; } = string.Empty;

        // password
        public string Password { get; set; } = string.Empty;

        // idNV
        public string IdNV { get; set; } = string.Empty;

        // idVT
        public string IdVT { get; set; } = string.Empty;

        // --- Constructors tương đương ---
        public TaiKhoan() { }

        public TaiKhoan(string idTK, string username, string password, string idNV, string idVT)
        {
            IdTK = idTK;
            Username = username;
            Password = password;
            IdNV = idNV;
            IdVT = idVT;
        }

        public override string ToString()
            => $"{IdTK} | {Username} | NV:{IdNV} | VT:{IdVT}";
    }
}
