
using System;
using System.Collections.Generic;
using System.Linq;
using QLThuocWin.Models;

namespace QLThuocWin.Services
{
    public static class DemoData
    {
        private static List<Thuoc> _thuoc = new List<Thuoc> {
            new Thuoc{ IdThuoc="T001", TenThuoc="Paracetamol", DonGia=12000, SoLuongTon=200 },
            new Thuoc{ IdThuoc="T002", TenThuoc="Cefalexin", DonGia=35000, SoLuongTon=150 },
            new Thuoc{ IdThuoc="T003", TenThuoc="Vitamin C", DonGia=10000, SoLuongTon=500 },
            new Thuoc{ IdThuoc="T004", TenThuoc="Amoxicillin", DonGia=28000, SoLuongTon=100 },
        };

        private static List<KhachHang> _kh = new List<KhachHang> {
            new KhachHang{ IdKH="KH001", HoTen="Nguyễn Văn A", Sdt="0901000111", DiemHienTai=12 },
            new KhachHang{ IdKH="KH002", HoTen="Trần Thị B", Sdt="0902222333", DiemHienTai=3 },
            new KhachHang{ IdKH="KH003", HoTen="Lê Văn C", Sdt="0919888777", DiemHienTai=30 },
        };

        private static int _counter = 1;

        public static List<Thuoc> GetAllThuoc() => _thuoc.Select(t => new Thuoc{
            IdThuoc = t.IdThuoc, TenThuoc = t.TenThuoc, DonGia = t.DonGia, SoLuongTon = t.SoLuongTon
        }).ToList();

        public static List<KhachHang> GetAllKhach() => _kh.Select(k => new KhachHang{
            IdKH = k.IdKH, HoTen = k.HoTen, Sdt = k.Sdt, DiemHienTai = k.DiemHienTai
        }).ToList();

        public static string GetNextHoaDonId() => $"HD{DateTime.Now:yyMMdd}-{_counter++:000}";

        public static int GetDiemHienTai(string idKH) =>
            _kh.FirstOrDefault(k => k.IdKH == idKH)?.DiemHienTai ?? 0;

        public static bool GiamSoLuongThuoc(string idThuoc, int qty)
        {
            var t = _thuoc.FirstOrDefault(x => x.IdThuoc == idThuoc);
            if (t == null || qty <= 0 || t.SoLuongTon < qty) return false;
            t.SoLuongTon -= qty;
            return true;
        }

        public static Thuoc? FindByTen(string ten) =>
            _thuoc.FirstOrDefault(t => string.Equals(t.TenThuoc, ten, StringComparison.OrdinalIgnoreCase));
    }
}
