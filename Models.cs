
using System;
using System.Collections.Generic;

namespace QLThuocWin.Models
{
    public class Thuoc
    {
        public string IdThuoc { get; set; } = "";
        public string TenThuoc { get; set; } = "";
        public string DonVi { get; set; } = "Viên";
        public string DanhMuc { get; set; } = "";
        public string XuatXu { get; set; } = "";
        public DateTime HSD { get; set; } = DateTime.Today.AddYears(1);
        public decimal DonGia { get; set; }
        public int SoLuongTon { get; set; }
    }

    public class KhachHang
    {
        public string IdKH { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string Sdt { get; set; } = "";
        public string Email { get; set; } = "";
        public int DiemHienTai { get; set; }
    }

    public class NhanVien
    {
        public string IdNV { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string VaiTro { get; set; } = "";
        public string Sdt { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class NhaCungCap
    {
        public string IdNCC { get; set; } = "";
        public string TenNCC { get; set; } = "";
        public string Sdt { get; set; } = "";
        public string Email { get; set; } = "";
        public string DiaChi { get; set; } = "";
    }

    public class HoaDon
    {
        public string IdHD { get; set; } = "";
        public DateTime ThoiGian { get; set; }
        public string IdNV { get; set; } = "";
        public string IdKH { get; set; } = "";
        public decimal TongTienThucTra { get; set; }
        public string PhuongThucThanhToan { get; set; } = "";
        public string TrangThaiDonHang { get; set; } = "";
    }

    public class ChiTietHoaDon
    {
        public string IdHD { get; set; } = "";
        public string IdThuoc { get; set; } = "";
        public string TenThuoc { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class PhieuNhap
    {
        public string IdPN { get; set; } = "";
        public DateTime NgayNhap { get; set; }
        public string IdNCC { get; set; } = "";
        public string GhiChu { get; set; } = "";
        public decimal TongTien { get; set; }
    }

    public class ChiTietPhieuNhap
    {
        public string IdPN { get; set; } = "";
        public string IdThuoc { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class PhanHoi
    {
        public string IdPH { get; set; } = "";
        public string IdKH { get; set; } = "";
        public DateTime NgayGui { get; set; }
        public string TieuDe { get; set; } = "";
        public string NoiDung { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }

    public class HopDong
    {
        public string IdHDG { get; set; } = "";
        public string IdNCC { get; set; } = "";
        public DateTime NgayKy { get; set; }
        public string HieuLuc { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }

    public class DeletedItem
    {
        public string Id { get; set; } = "";
        public string Ten { get; set; } = "";
        public DateTime NgayXoa { get; set; }
        public string Loai { get; set; } = ""; // Thuoc/HoaDon/KhachHang/...
    }
}
