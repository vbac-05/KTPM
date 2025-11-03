// File: Models/PhanHoi.cs
// -----------------------
// Bản C# tương đương với Java class `PhanHoi.java`
//
// Cấu trúc bảng (theo mô tả Java):
//   idPH      NVARCHAR(10)   PK   NOT NULL
//   idKH      NVARCHAR(10)   FK → KhachHang.idKH
//   idHD      NVARCHAR(10)   FK → HoaDon.idHD
//   noiDung   NVARCHAR(MAX)  NULL
//   thoiGian  DATETIME       NOT NULL
//   danhGia   INT            NOT NULL
//   isDeleted BIT            NULL
//
// ✅ Ghi chú chuyển đổi:
//   - `String` → `string`                         // đổi cú pháp
//   - `Date` → `DateTime`                         // đổi kiểu thời gian
//   - getter/setter → property { get; set; }      // đổi cú pháp property
//   - thêm `[Table]`, `[Key]`, `[Column]`, `[MaxLength]`, `[Required]`
//   - giữ nguyên cả hai constructor (rỗng & đầy đủ tham số)
//   - `isDeleted` dùng `bool?` để khớp kiểu BIT NULL
//

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Entities
{
    [Table("PhanHoi")] // ← THÊM MỚI: tên bảng trong DB
    public class PhanHoi
    {
        [Key] // ← THÊM MỚI: EF Core cần chỉ định khóa chính
        [Column("idPH")]
        [MaxLength(10)] // ← GIẢ ĐỊNH NVARCHAR(10), chỉnh nếu schema khác
        [Required]
        public string IdPH { get; set; } = string.Empty; // (Java: private String idPH)

        [Column("idKH")]
        [MaxLength(10)]
        public string? IdKH { get; set; } // (Java: private String idKH)
        // ← KHÁC VỚI JAVA: dùng string? vì FK có thể NULL trong một số trường hợp

        [Column("idHD")]
        [MaxLength(10)]
        public string? IdHD { get; set; } // (Java: private String idHD)
        // ← KHÁC VỚI JAVA: dùng string? cho an toàn khi dữ liệu thiếu FK

        [Column("noiDung")]
        public string? NoiDung { get; set; } // (Java: private String noiDung)
        // ← KHÁC VỚI JAVA: dùng string? vì cột này cho phép NULL

        [Column("thoiGian")]
        [Required]
        public DateTime ThoiGian { get; set; } // (Java: private Date thoiGian)
        // ← KHÁC VỚI JAVA: dùng DateTime (chuẩn .NET)

        [Column("danhGia")]
        [Required]
        public int DanhGia { get; set; } // (Java: private int danhGia)

        [Column("isDeleted")]
        public bool? IsDeleted { get; set; } // (Java: private Boolean isDeleted)
        // ← KHÁC VỚI JAVA: bool? cho phép null để khớp BIT NULL trong SQL

        // Constructor mặc định (Java có → giữ nguyên)
        public PhanHoi() { }

        // Constructor đầy đủ (Java có → giữ nguyên)
        public PhanHoi(string idPH, string idKH, string idHD, string noiDung, DateTime thoiGian, int danhGia)
        {
            IdPH = idPH;
            IdKH = idKH;
            IdHD = idHD;
            NoiDung = noiDung;
            ThoiGian = thoiGian;
            DanhGia = danhGia;
        }
    }
}
