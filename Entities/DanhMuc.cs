// File: Models/DanhMuc.cs
// -----------------------
// Entity tương đương với Java class `DanhMuc.java`
//
//  Cấu trúc bảng giả định (theo Java):
//    idDM  NVARCHAR(10)  NOT NULL   (khóa chính)
//    ten   NVARCHAR(100) NOT NULL
//
// ✅ Ghi chú chuyển đổi:
//   - `String` (Java) → `string` (C#)
//   - getter/setter → property { get; set; }
//   - thêm [Table], [Key], [Column], [MaxLength] để mapping với EF Core
//   - giữ nguyên constructor mặc định
//

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    [Table("DanhMuc")] // ← THÊM MỚI: chỉ định tên bảng tương ứng trong DB
    public class DanhMuc
    {
        [Key] // ← THÊM MỚI: đánh dấu khóa chính (Java không cần, EF Core cần)
        [Column("idDM")] // ← THÊM MỚI: map tên cột gốc trong SQL
        [MaxLength(10)] // ← THÊM MỚI: tương đương NVARCHAR(10)
        public string IdDM { get; set; } = string.Empty;  // (Java: private String idDM)

        [Column("ten")] // ← THÊM MỚI: map đúng cột SQL
        [MaxLength(100)] // ← THÊM MỚI: đặt giới hạn độ dài để match với NVARCHAR(100)
        public string Ten { get; set; } = string.Empty;   // (Java: private String ten)

        // Constructor mặc định (Java có → giữ nguyên)
        public DanhMuc() { }
    }
}
