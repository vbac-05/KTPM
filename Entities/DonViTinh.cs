// File: Models/DonViTinh.cs
// --------------------------
// Entity tương đương với Java class `DonViTinh.java`
//
//  Cấu trúc bảng giả định (theo Java):
//    idDVT  NVARCHAR(10)  NOT NULL   (khóa chính)
//    ten    NVARCHAR(100) NOT NULL
//
// ✅ Ghi chú chuyển đổi:
//   - `String` (Java) → `string` (C#)
//   - getter/setter → property { get; set; }
//   - thêm [Table], [Key], [Column], [MaxLength] để EF Core mapping đúng
//   - giữ nguyên constructor mặc định
//

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Entities
{
    [Table("DonViTinh")] // ← THÊM MỚI: xác định tên bảng trong CSDL
    public class DonViTinh
    {
        [Key] // ← THÊM MỚI: đánh dấu khóa chính (Java không cần, EF Core cần)
        [Column("idDVT")] // ← THÊM MỚI: map chính xác tên cột trong SQL
        [MaxLength(10)] // ← THÊM MỚI: tương ứng NVARCHAR(10)
        public string IdDVT { get; set; } = string.Empty;  // (Java: private String idDVT)

        [Column("ten")] // ← THÊM MỚI: map đúng tên cột SQL
        [MaxLength(100)] // ← THÊM MỚI: tương ứng NVARCHAR(100)
        public string Ten { get; set; } = string.Empty;    // (Java: private String ten)

        // Constructor mặc định (Java có → giữ nguyên)
        public DonViTinh() { }
    }
}
