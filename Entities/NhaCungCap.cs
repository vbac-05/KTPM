// File: Models/NhaCungCap.cs
// ---------------------------
// Entity tương đương với Java class `NhaCungCap.java`
//
// Cấu trúc bảng (theo Java):
//   idNCC     NVARCHAR(10)   PK   NOT NULL
//   tenNCC    NVARCHAR(255)  NOT NULL
//   sdt       NVARCHAR(20)   NOT NULL
//   diaChi    NVARCHAR(255)  NULL
//   isDeleted BIT            NULL
//
// ✅ Ghi chú chuyển đổi:
//   - `String` (Java) → `string` (C#)
//   - getter/setter → property { get; set; }
//   - thêm [Table], [Key], [Column], [MaxLength], [Required] để mapping EF Core
//   - `isDeleted` đổi `Boolean` → `bool?` (nullable) để khớp cột BIT có thể NULL
//   - Giữ nguyên constructor mặc định
//

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Entities
{
    [Table("NhaCungCap")] // ← THÊM MỚI: tên bảng tương ứng trong CSDL
    public class NhaCungCap
    {
        [Key] // ← THÊM MỚI: đánh dấu khóa chính (Java không cần)
        [Column("idNCC")] // ← THÊM MỚI: map đúng tên cột
        [MaxLength(10)] // ← THÊM MỚI: tương đương NVARCHAR(10)
        [Required] // ← THÊM MỚI: NOT NULL
        public string IdNCC { get; set; } = string.Empty;  // (Java: private String idNCC)

        [Column("tenNCC")]
        [MaxLength(255)]
        [Required]
        public string TenNCC { get; set; } = string.Empty; // (Java: private String tenNCC)

        [Column("sdt")]
        [MaxLength(20)]
        [Required]
        public string Sdt { get; set; } = string.Empty;    // (Java: private String sdt)

        [Column("diaChi")]
        [MaxLength(255)]
        public string? DiaChi { get; set; }                // (Java: private String diaChi)
        // ← KHÁC VỚI JAVA: dùng string? vì có thể NULL trong DB

        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }               // (Java: private Boolean isDeleted)
        // ← KHÁC VỚI JAVA: đổi Boolean → bool? để khớp kiểu BIT NULL

        // Constructor mặc định (Java có → giữ nguyên)
        public NhaCungCap() { }
    }
}
