// File: Models/VaiTro.cs
// -----------------------
// Bản C# tương đương 100% với Java class `VaiTro.java`
//
// ✅ Giữ nguyên:
//   - constructor mặc định
//   - constructor có tham số
//   - getter/setter y hệt Java
//   - comment gốc từ Java
// ✅ Chỉ thêm annotation EF Core: [Table], [Key], [Column], [MaxLength], [Required]
// ✅ Có comment đánh dấu từng chỗ đã thay đổi so với Java

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    /**
     * VaiTro.cs
     * Lớp entity ánh xạ bảng VaiTro trong CSDL.
     */
    [Table("VaiTro")] // ← THÊM MỚI: chỉ định tên bảng trong CSDL
    public class VaiTro
    {
        [Key] // ← THÊM MỚI: EF Core yêu cầu khóa chính
        [Column("idVT")] // ← THÊM MỚI: map tên cột trong DB
        [MaxLength(10)] // ← THÊM MỚI: giới hạn NVARCHAR(10) (giả định)
        [Required]
        private string idVT;  // khóa chính (Java: private String idVT)

        [Column("ten")] // ← THÊM MỚI: map tên cột
        [MaxLength(100)] // ← THÊM MỚI: giả định NVARCHAR(100)
        [Required]
        private string ten;   // tên vai trò (ví dụ: "Admin", "Nhân viên", ...)

        // Constructor mặc định (Java có → giữ nguyên)
        public VaiTro()
        {
        }

        // Constructor có tham số (Java có → giữ nguyên)
        public VaiTro(string idVT, string ten)
        {
            this.idVT = idVT;
            this.ten = ten;
        }

        // Getter/Setter giữ nguyên tên như Java
        public string GetIdVT()
        {
            return idVT;
        }

        public void SetIdVT(string idVT)
        {
            this.idVT = idVT;
        }

        public string GetTen()
        {
            return ten;
        }

        public void SetTen(string ten)
        {
            this.ten = ten;
        }
    }
}
