// File: Models/HoaDon.cs
// ----------------------
// Entity tương đương với Java class `HoaDon.java`
//
//  Cấu trúc bảng hiện tại (theo Java comment):
//    idHD                   NVARCHAR(10)   PK  NOT NULL
//    thoiGian               DATETIME       NOT NULL
//    idNV                   NVARCHAR(10)   NOT NULL   (FK -> NhanVien)
//    idKH                   NVARCHAR(10)   NOT NULL   (FK -> KhachHang)
//    tongTien               FLOAT          NOT NULL
//    phuongThucThanhToan    NVARCHAR(50)   NULL
//    trangThaiDonHang       NVARCHAR(50)   NOT NULL
//
// ✅ Ghi chú chuyển đổi:
//   - `String` → `string` (C# convention)
//   - `Date` → `DateTime` (tương ứng kiểu DATETIME SQL)
//   - getter/setter → property { get; set; }
//   - Thêm [Table], [Key], [Column], [MaxLength], [Required]/nullable
//   - `phuongThucThanhToan` cho phép null nên khai báo `string?`
//   - Giữ nguyên constructor mặc định
//

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    [Table("HoaDon")] // ← THÊM MỚI: chỉ định tên bảng tương ứng trong DB
    public class HoaDon
    {
        [Key] // ← THÊM MỚI: đánh dấu khóa chính (Java không cần, EF Core cần)
        [Column("idHD")] // ← THÊM MỚI: map tên cột SQL
        [MaxLength(10)] // ← THÊM MỚI: tương đương NVARCHAR(10)
        public string IdHD { get; set; } = string.Empty;  // (Java: private String idHD)

        [Column("thoiGian")]
        [Required] // ← THÊM MỚI: NOT NULL trong SQL
        public DateTime ThoiGian { get; set; } // (Java: private Date thoiGian)

        [Column("idNV")]
        [Required]
        [MaxLength(10)]
        public string IdNV { get; set; } = string.Empty; // (Java: private String idNV)

        [Column("idKH")]
        [Required]
        [MaxLength(10)]
        public string IdKH { get; set; } = string.Empty; // (Java: private String idKH)

        [Column("tongTien")]
        [Required]
        public double TongTien { get; set; } // (Java: private double tongTien)
        // Gợi ý: có thể đổi sang decimal nếu cần độ chính xác tiền tệ cao hơn

        [Column("phuongThucThanhToan")]
        [MaxLength(50)]
        public string? PhuongThucThanhToan { get; set; } // (Java: private String phuongThucThanhToan)
        // ← CHỖ NÀY KHÁC VỚI JAVA: dùng `string?` vì cột này có thể NULL trong DB

        [Column("trangThaiDonHang")]
        [Required]
        [MaxLength(50)]
        public string TrangThaiDonHang { get; set; } = string.Empty; // (Java: private String trangThaiDonHang)

        // Constructor mặc định (Java có → giữ nguyên)
        public HoaDon() { }
    }
}
