// File: Models/KhachHang.cs
// -------------------------
// Chuyển đổi tương đương với Java class `KhachHang.java`
//
// Ghi chú mapping giả định (chỉnh lại MaxLength/Required cho khớp schema thực tế nếu cần):
//   idKH          NVARCHAR(10)   PK  NOT NULL
//   hoTen         NVARCHAR(255)  NOT NULL
//   sdt           NVARCHAR(20)   NOT NULL
//   gioiTinh      NVARCHAR(10)   NOT NULL
//   ngayThamGia   DATE/DATETIME  NOT NULL
//   diemTichLuy   INT            NOT NULL
//   isDeleted     BIT            NULL   (xóa mềm)
//
// ✅ Ghi chú chuyển đổi:
//   - String (Java)  → string (C#)                           // THAY ĐỔI CÚ PHÁP
//   - Date (Java)    → DateTime (C#)                         // THAY ĐỔI KIỂU THỜI GIAN
//   - getter/setter  → property { get; set; }                // THAY ĐỔI CÚ PHÁP
//   - Thêm [Table], [Key], [Column], [MaxLength], [Required] // THÊM MỚI: cho EF Core
//   - isDeleted Boolean → bool? (nullable)                   // KHÁC VỚI JAVA: khớp BIT NULL
//   - Giữ constructor mặc định + constructor có tham số      // GIỮ NGUYÊN Ý NGHĨA

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Entities
{
    [Table("KhachHang")] // ← THÊM MỚI: tên bảng trong DB
    public class KhachHang
    {
        [Key]                 // ← THÊM MỚI: khóa chính (Java không cần annotation)
        [Column("idKH")]      // ← THÊM MỚI: map tên cột
        [MaxLength(10)]       // ← THÊM MỚI: giả định NVARCHAR(10) (chỉnh nếu schema khác)
        [Required]            // ← THÊM MỚI: NOT NULL
        public string IdKH { get; set; } = string.Empty;   // (Java: private String idKH)

        [Column("hoTen")]
        [MaxLength(255)]      // ← THÊM MỚI: giả định NVARCHAR(255)
        [Required]
        public string HoTen { get; set; } = string.Empty;  // (Java: private String hoTen)

        [Column("sdt")]
        [MaxLength(20)]       // ← THÊM MỚI: giả định NVARCHAR(20)
        [Required]
        public string Sdt { get; set; } = string.Empty;    // (Java: private String sdt)

        [Column("gioiTinh")]
        [MaxLength(10)]       // ← THÊM MỚI: giả định NVARCHAR(10) ("Nam"/"Nu"/"Khac"...)
        [Required]
        public string GioiTinh { get; set; } = string.Empty; // (Java: private String gioiTinh)

        [Column("ngayThamGia")]
        [Required]
        public DateTime NgayThamGia { get; set; }          // (Java: private Date ngayThamGia)
        // KHÁC VỚI JAVA: dùng DateTime cho phù hợp SQL DATETIME/DATE

        [Column("diemTichLuy")]
        [Required]
        public int DiemTichLuy { get; set; }               // (Java: private int diemTichLuy)

        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }               // (Java: private Boolean isDeleted)
        // KHÁC VỚI JAVA: dùng bool? để khớp cột BIT cho phép NULL

        // Constructor mặc định (GIỮ NGUYÊN)
        public KhachHang() { }

        // Constructor có tham số (GIỮ Ý NGHĨA như Java)
        public KhachHang(string idKH, string hoTen, string sdt, string gioiTinh, DateTime ngayThamGia)
        {
            IdKH = idKH;
            HoTen = hoTen;
            Sdt = sdt;
            GioiTinh = gioiTinh;
            NgayThamGia = ngayThamGia;
        }
    }
}
