// File: Models/ChiTietHoaDon.cs
// ------------------------------
// Entity tương đương với Java class `ChiTietHoaDon.java`
// Dùng cho bảng dbo.ChiTietHoaDon
//
//  Các cột gốc:
//    idHD     NVARCHAR(10)  NOT NULL   (FK → HoaDon)
//    idThuoc  NVARCHAR(10)  NOT NULL   (FK → Thuoc)
//    soLuong  INT           NOT NULL
//    donGia   FLOAT         NOT NULL
//
//  Bổ sung trường `tenThuoc` (NVARCHAR(255)) – lấy khi JOIN với Thuoc
//
//  ✅ Ghi chú chuyển đổi:
//   - Dùng kiểu `string` (thay cho Java String)
//   - Dùng `double` như Java (hoặc có thể đổi sang `decimal` nếu muốn chính xác tiền tệ)
//   - Thêm [Key] + [Column] để EF Core hiểu khóa chính/phụ.
//   - `tenThuoc` đánh dấu [NotMapped] vì không có cột thật trong bảng gốc.
//

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        // EF Core yêu cầu khóa chính; ở đây idHD + idThuoc là composite key.
        // Ta sẽ cấu hình thêm trong DbContext.OnModelCreating().
        [Column("idHD")]
        [Required]
        [MaxLength(10)]
        public string IdHD { get; set; } = string.Empty;   // (Java: private String idHD)

        [Column("idThuoc")]
        [Required]
        [MaxLength(10)]
        public string IdThuoc { get; set; } = string.Empty; // (Java: private String idThuoc)

        // Thuộc tính mở rộng, không có cột thật trong DB.
        // Sử dụng khi JOIN với bảng Thuoc.
        [NotMapped] // ← CHỖ NÀY KHÁC VỚI JAVA: để tránh EF map cột này xuống DB
        [MaxLength(255)]
        public string? TenThuoc { get; set; } // (Java: private String tenThuoc)

        [Column("soLuong")]
        [Required]
        public int SoLuong { get; set; } // (Java: private int soLuong)

        [Column("donGia")]
        [Required]
        public double DonGia { get; set; } // (Java: private double donGia)

        // Constructor mặc định (Java có → giữ nguyên)
        public ChiTietHoaDon() { }

        // Gợi ý: có thể thêm constructor tiện dụng nếu cần
        // public ChiTietHoaDon(string idHD, string idThuoc, int soLuong, double donGia) { ... }
    }
}
