// File: Models/ChiTietPhieuNhap.cs
// Nguồn gốc: chuyển từ Java entities/ChiTietPhieuNhap.java
// Bảng gợi ý:
//   idPN (nvarchar(10)), idThuoc (nvarchar(10)), soLuong (int), giaNhap (float/double),
//   + tenThuoc (join từ Thuoc)
//
// LƯU Ý mapping & UI:
// - Java double -> C# decimal cho tiền tệ (tránh sai số float/double).
// - UI ViewPhieuNhapDialog hiện bind: "MaThuoc", "TenThuoc", "SoLuong", "DonGia", "ThanhTien".
//   => Thêm alias MaThuoc (trỏ IdThuoc), DonGia (trỏ GiaNhap), và computed ThanhTien.
// - TODO: Nếu bạn sửa DataGridView để bind "IdThuoc" & "GiaNhap" thay vì alias,
//   hãy đổi DataPropertyName trong UI và có thể xóa hai alias bên dưới.

namespace QLThuocWin.Models
{
    public class ChiTietPhieuNhap
    {
        /// <summary>Mã phiếu nhập (FK)</summary>
        public string IdPN { get; set; } = string.Empty;

        /// <summary>Mã thuốc (FK)</summary>
        public string IdThuoc { get; set; } = string.Empty;

        // NOTE: Alias để không phải đổi UI (DataPropertyName="MaThuoc")
        // TODO: Nếu đổi UI sang "IdThuoc", có thể xóa property này.
        public string MaThuoc => IdThuoc;

        /// <summary>Tên thuốc (join từ Thuoc)</summary>
        public string TenThuoc { get; set; } = string.Empty;

        /// <summary>Số lượng nhập</summary>
        public int SoLuong { get; set; }

        /// <summary>Giá nhập (tiền tệ) — dùng decimal</summary>
        // NOTE: Java dùng double; sang C# ưu tiên decimal cho số tiền.
        public decimal GiaNhap { get; set; }

        // NOTE: Alias để khớp UI (DataPropertyName="DonGia")
        // TODO: Nếu đổi UI sang "GiaNhap", có thể xóa property này.
        public decimal DonGia => GiaNhap;

        /// <summary>Thành tiền = SoLuong * GiaNhap (hỗ trợ cột "Thành tiền" trong grid)</summary>
        public decimal ThanhTien => SoLuong * GiaNhap;
    }
}
