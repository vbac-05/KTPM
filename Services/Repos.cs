
using System;
using System.Collections.Generic;
using System.Linq;
using QLThuocWin.Models;

namespace QLThuocWin.Services
{
    public static class Repos
    {
        // Demo data
        public static List<Thuoc> ThuocRepo = new()
        {
            new Thuoc{ IdThuoc="T001", TenThuoc="Paracetamol", DonVi="Viên", DanhMuc="Giảm đau", XuatXu="VN", HSD=DateTime.Today.AddYears(2), DonGia=12000, SoLuongTon=200 },
            new Thuoc{ IdThuoc="T002", TenThuoc="Cefalexin", DonVi="Viên", DanhMuc="Kháng sinh", XuatXu="VN", HSD=DateTime.Today.AddYears(1), DonGia=35000, SoLuongTon=150 },
            new Thuoc{ IdThuoc="T003", TenThuoc="Vitamin C", DonVi="Viên", DanhMuc="Vitamin", XuatXu="VN", HSD=DateTime.Today.AddYears(1), DonGia=10000, SoLuongTon=500 },
        };

        public static List<KhachHang> KhachRepo = new()
        {
            new KhachHang{ IdKH="KH001", HoTen="Nguyễn Văn A", Sdt="0901234567", Email="a@example.com", DiemHienTai=12 },
            new KhachHang{ IdKH="KH002", HoTen="Trần Thị B", Sdt="0902222333", Email="b@example.com", DiemHienTai=3 },
        };

        public static List<NhanVien> NhanVienRepo = new()
        {
            new NhanVien{ IdNV="NV001", HoTen="Admin", VaiTro="VT01", Sdt="0909999999", Email="admin@shop.vn" },
            new NhanVien{ IdNV="NV002", HoTen="Nhân viên 1", VaiTro="VT02", Sdt="0908888888", Email="nv1@shop.vn" },
        };

        public static List<NhaCungCap> NCCRepo = new()
        {
            new NhaCungCap{ IdNCC="NCC001", TenNCC="Dược ABC", Sdt="0281111111", Email="abc@duoc.vn", DiaChi="HCM" },
            new NhaCungCap{ IdNCC="NCC002", TenNCC="Dược XYZ", Sdt="0242222222", Email="xyz@duoc.vn", DiaChi="HN" },
        };

        public static List<HoaDon> HoaDonRepo = new();
        public static List<ChiTietHoaDon> CTHoaDonRepo = new();

        public static List<PhieuNhap> PhieuNhapRepo = new();
        public static List<ChiTietPhieuNhap> CTPhieuNhapRepo = new();

        public static List<PhanHoi> PhanHoiRepo = new();
        public static List<HopDong> HopDongRepo = new();

        public static List<DeletedItem> TrashRepo = new();

        private static int _hdCounter = 1;
        private static int _pnCounter = 1;
        private static int _phCounter = 1;
        private static int _hdgCounter = 1;

        public static string NextHD() => $"HD{DateTime.Now:yyMMdd}-{_hdCounter++:000}";
        public static string NextPN() => $"PN{DateTime.Now:yyMMdd}-{_pnCounter++:000}";
        public static string NextPH() => $"PH{DateTime.Now:yyMMdd}-{_phCounter++:000}";
        public static string NextHDG() => $"HDG{DateTime.Now:yyMMdd}-{_hdgCounter++:000}";

        // Thuoc ops
        public static bool GiamTon(string idThuoc, int qty)
        {
            var t = ThuocRepo.FirstOrDefault(x => x.IdThuoc == idThuoc);
            if (t == null || qty <= 0 || t.SoLuongTon < qty) return false;
            t.SoLuongTon -= qty;
            return true;
        }
        public static Thuoc? FindThuocByTen(string ten) =>
            ThuocRepo.FirstOrDefault(x => string.Equals(x.TenThuoc, ten, StringComparison.OrdinalIgnoreCase));
        public static int GetDiemHienTai(string idKH) =>
            KhachRepo.FirstOrDefault(k => k.IdKH == idKH)?.DiemHienTai ?? 0;
    }
}
