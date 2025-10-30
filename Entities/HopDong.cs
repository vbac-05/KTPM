// File: Models/HopDong.cs
// ------------------------
// Entity tương đương với Java class `HopDong.java`
//
//  Cấu trúc bảng hiện tại (theo mô tả Java):
//    idHDong      NVARCHAR(10)   PK   NOT NULL
//    ngayBatDau   DATE           NOT NULL
//    ngayKetThuc  DATE           NOT NULL
//    noiDung      NVARCHAR(MAX)  NULL
//    idNV         NVARCHAR(10)   NULL   (FK → NhanVien)
//    idNCC        NVARCHAR(10)   NULL   (FK → NhaCungCap)
//    trangThai    NVARCHAR(50)   NOT NULL
//    isDeleted    BIT            NULL  (bổ sung thêm cho tính năng xóa mềm)
//
// ✅ Ghi chú chuyển đổi:
//   - `String`  → `string` (C# convention)
//   - `Date`    → `DateTime` (hoặc `DateTime?` nếu cho phép null)
//   - getter/setter → property `{ get; set; }`
//   - thêm `[Table]`, `[Key]`, `[Column]`, `[MaxLength]`, `[Required]`
//   - giữ nguyên `isDeleted` (đổi sang `bool?` để tương ứng BIT NULL)
//   - giữ constructor mặc định.
//

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    [Table("HopDong")] // ← THÊM MỚI: chỉ định tên bảng tương ứng trong DB
    public class HopDong
    {
        [Key] // ← THÊM MỚI: EF Core cần khóa chính (Java không cần annotation)
        [Column("idHDong")]
        [MaxLength(10)]
        public string IdHDong { get; set; } = string.Empty;  // (Java: private String idHDong)

        [Column("ngayBatDau")]
        [Required] // ← THÊM MỚI: NOT NULL trong SQL
        public DateTime NgayBatDau { get; set; } // (Java: private Date ngayBatDau)

        [Column("ngayKetThuc")]
        [Required]
        public DateTime NgayKetThuc { get; set; } // (Java: private Date ngayKetThuc)

        [Column("noiDung")]
        public string? NoiDung { get; set; } // (Java: private String noiDung)
        // ← CHỖ NÀY KHÁC VỚI JAVA: dùng string? vì cột này cho phép NULL

        [Column("idNV")]
        [MaxLength(10)]
        public string? IdNV { get; set; } // (Java: private String idNV)
        // ← CHỖ NÀY KHÁC VỚI JAVA: dùng string? vì có thể NULL nếu là HĐ nhà cung cấp

        [Column("idNCC")]
        [MaxLength(10)]
        public string? IdNCC { get; set; } // (Java: private String idNCC)
        // ← CHỖ NÀY KHÁC VỚI JAVA: dùng string? vì có thể NULL nếu là HĐ nhân viên

        [Column("trangThai")]
        [Required]
        [MaxLength(50)]
        public string TrangThai { get; set; } = string.Empty; // (Java: private String trangThai)

        [Column("isDeleted")]
        public bool? IsDeleted { get; set; } // (Java: private Boolean isDeleted)
        // ← CHỖ NÀY KHÁC VỚI JAVA: đổi Boolean → bool? cho tương thích BIT NULL trong SQL

        // Constructor mặc định (Java có → giữ nguyên)
        public HopDong() { }
    }
}
