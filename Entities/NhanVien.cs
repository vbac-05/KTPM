// File: Models/NhanVien.cs
// ------------------------
// Bản C# tương đương với Java class `NhanVien.java`
//
// Ghi chú theo mô tả:
//   - Bảng dbo.NhanVien đã bổ sung: luong NVARCHAR(50) NOT NULL, trangThai NVARCHAR(50) NOT NULL
//   - Các cột cũ: idNV, hoTen, sdt, gioiTinh, namSinh, ngayVaoLam
//   - Thông tin tài khoản: username, password, roleId
//   - Xóa mềm: isDeleted (BIT NULL)
//
// ✅ Thay đổi khi chuyển sang C# / EF Core:
//   - String → string; Date → DateTime                      // THAY CÚ PHÁP/KIỂU
//   - getter/setter → property { get; set; }               // THAY CÚ PHÁP
//   - Thêm [Table], [Key], [Column], [MaxLength], [Required] // THÊM MỚI để map DB
//   - isDeleted: Boolean → bool? (nullable)                // KHỚP BIT NULL
//   - Giới hạn độ dài (MaxLength) đặt theo giả định thông dụng; cần chỉnh theo schema thực tế

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Entities
{
    [Table("NhanVien")] // ← THÊM MỚI: tên bảng trong CSDL
    public class NhanVien
    {
        [Key]                 // ← THÊM MỚI: khóa chính (Java không cần annotation)
        [Column("idNV")]      // ← THÊM MỚI: map tên cột
        [MaxLength(10)]       // ← GIẢ ĐỊNH: NVARCHAR(10) (chỉnh nếu schema khác)
        [Required]
        public string IdNV { get; set; } = string.Empty;  // (Java: private String idNV)

        [Column("hoTen")]
        [MaxLength(255)]      // ← GIẢ ĐỊNH: NVARCHAR(255)
        [Required]
        public string HoTen { get; set; } = string.Empty; // (Java: private String hoTen)

        [Column("sdt")]
        [MaxLength(20)]       // ← GIẢ ĐỊNH: NVARCHAR(20)
        [Required]
        public string Sdt { get; set; } = string.Empty;   // (Java: private String sdt)

        [Column("gioiTinh")]
        [MaxLength(10)]       // ← GIẢ ĐỊNH: NVARCHAR(10)
        [Required]
        public string GioiTinh { get; set; } = string.Empty; // (Java: private String gioiTinh)

        [Column("namSinh")]
        [Required]
        public int NamSinh { get; set; }                  // (Java: private int namSinh)

        [Column("ngayVaoLam")]
        [Required]
        public DateTime NgayVaoLam { get; set; }          // (Java: private Date ngayVaoLam)
        // ← KHÁC VỚI JAVA: dùng DateTime để khớp SQL DATETIME/DATE

        // Các trường mới:
        [Column("luong")]
        [MaxLength(50)]
        [Required]                                        // ← THEO MÔ TẢ: NOT NULL
        public string Luong { get; set; } = string.Empty; // (Java: private String luong)

        [Column("trangThai")]
        [MaxLength(50)]
        [Required]                                        // ← THEO MÔ TẢ: NOT NULL
        public string TrangThai { get; set; } = string.Empty; // (Java: private String trangThai)

        // Thông tin tài khoản
        [Column("username")]
        [MaxLength(50)]                                   // ← GIẢ ĐỊNH: NVARCHAR(50)
        [Required]                                        // ← THƯỜNG LÀ NOT NULL (chỉnh nếu cần)
        public string Username { get; set; } = string.Empty; // (Java: private String username)

        [Column("password")]
        [MaxLength(255)]                                  // ← GIẢ ĐỊNH: NVARCHAR(255) (đủ chứa hash)
        [Required]
        public string Password { get; set; } = string.Empty; // (Java: private String password)

        [Column("roleId")]
        [MaxLength(10)]                                   // ← GIẢ ĐỊNH: NVARCHAR(10)
        [Required]
        public string RoleId { get; set; } = string.Empty;   // (Java: private String roleId)

        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }              // (Java: private Boolean isDeleted)
        // ← KHÁC VỚI JAVA: dùng bool? để khớp cột BIT NULL

        // Constructor mặc định (GIỮ NGUYÊN như Java)
        public NhanVien() { }
    }
}
