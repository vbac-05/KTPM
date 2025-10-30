// File: Models/PhieuNhap.cs
// -------------------------
// Bản C# tương đương với Java class `PhieuNhap.java`
//
// Cấu trúc bảng (theo mô tả Java):
//   idPN      NVARCHAR(10)   PK   NOT NULL
//   thoiGian  DATETIME       NOT NULL
//   idNV      NVARCHAR(10)   FK → NhanVien.idNV
//   idNCC     NVARCHAR(10)   FK → NhaCungCap.idNCC
//   tongTien  FLOAT          NOT NULL
//   isDeleted BIT            NULL
//
// ✅ Ghi chú chuyển đổi:
//   - `String` → `string`                            // đổi cú pháp
//   - `Date` → `DateTime`                            // đổi kiểu thời gian
//   - getter/setter → property { get; set; }         // đổi cú pháp
//   - thêm `[Table]`, `[Key]`, `[Column]`, `[Required]`, `[MaxLength]`  // thêm mới cho EF Core
//   - `isDeleted` đổi `Boolean` → `bool?`            // khớp kiểu BIT NULL trong SQL
//   - giữ nguyên constructor mặc định
//

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    [Table("PhieuNhap")] // ← THÊM MỚI: tên bảng trong DB
    public class PhieuNhap
    {
        [Key] // ← THÊM MỚI: EF Core cần xác định khóa chính
        [Column("idPN")]
        [MaxLength(10)] // ← GIẢ ĐỊNH: NVARCHAR(10)
        [Required]
        public string IdPN { get; set; } = string.Empty;  // (Java: private String idPN)

        [Column("thoiGian")]
        [Required]
        public DateTime ThoiGian { get; set; }           // (Java: private Date thoiGian)
        // ← KHÁC VỚI JAVA: dùng DateTime thay cho java.util.Date

        [Column("idNV")]
        [MaxLength(10)]
        public string? IdNV { get; set; }                // (Java: private String idNV)
        // ← KHÁC VỚI JAVA: dùng string? để phòng trường hợp cột này có thể NULL

        [Column("idNCC")]
        [MaxLength(10)]
        public string? IdNCC { get; set; }               // (Java: private String idNCC)
        // ← KHÁC VỚI JAVA: dùng string? vì có thể NULL nếu không liên kết NCC

        [Column("tongTien")]
        [Required]
        public double TongTien { get; set; }             // (Java: private double tongTien)
        // ← GIỮ NGUYÊN: có thể đổi sang decimal nếu cần độ chính xác tiền tệ cao

        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }             // (Java: private Boolean isDeleted)
        // ← KHÁC VỚI JAVA: đổi Boolean → bool? để khớp BIT NULL trong SQL

        // Constructor mặc định (Java có → giữ nguyên)
        public PhieuNhap() { }
    }
}
