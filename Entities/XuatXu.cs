// File: Models/XuatXu.cs
// ----------------------
// Bản C# tương đương 100% với Java class `XuatXu.java`
//
// ✅ Giữ nguyên:
//   - constructor mặc định
//   - getter/setter y hệt Java
//   - comment gốc
// ✅ Chỉ thêm annotation EF Core: [Table], [Key], [Column], [MaxLength], [Required]
// ✅ Có comment chi tiết cho tất cả chỗ thay đổi

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    // XuatXu.cs
    [Table("XuatXu")] // ← THÊM MỚI: xác định tên bảng tương ứng trong DB
    public class XuatXu
    {
        [Key] // ← THÊM MỚI: EF Core yêu cầu chỉ định khóa chính
        [Column("idXX")] // ← THÊM MỚI: map đúng tên cột trong DB
        [MaxLength(10)] // ← THÊM MỚI: giả định NVARCHAR(10)
        [Required]
        private string idXX; // (Java: private String idXX)

        [Column("ten")] // ← THÊM MỚI: map đúng tên cột trong DB
        [MaxLength(100)] // ← THÊM MỚI: giả định NVARCHAR(100)
        [Required]
        private string ten; // (Java: private String ten)

        // Constructor mặc định (Java có → giữ nguyên)
        public XuatXu()
        {
        }

        // Getter/Setter giữ nguyên tên và cấu trúc như Java
        public string GetIdXX()
        {
            return idXX;
        }

        public void SetIdXX(string idXX)
        {
            this.idXX = idXX;
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
