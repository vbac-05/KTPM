// File: Models/Thuoc.cs
// ----------------------
// Bản C# tương đương 100% với Java class `Thuoc.java`
// Giữ nguyên: constructor, getter/setter, biến, comment, logic
// Chỉ thêm annotation EF Core để map DB

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuocApp.Core.Models
{
    [Table("Thuoc")] // ← THÊM MỚI: tên bảng trong DB
    public class Thuoc
    {
        [Key] // ← THÊM MỚI: EF Core yêu cầu khóa chính
        [Column("idThuoc")]
        [MaxLength(10)]
        [Required]
        private string idThuoc;

        [Column("tenThuoc")]
        [MaxLength(255)]
        [Required]
        private string tenThuoc;

        [Column("hinhAnh")]
        private byte[]? hinhAnh;

        [Column("thanhPhan")]
        [MaxLength(255)]
        private string? thanhPhan;

        [Column("donViTinh")]
        [MaxLength(255)]
        [Required]
        private string donViTinh;  // mới

        [Column("danhMuc")]
        [MaxLength(255)]
        [Required]
        private string danhMuc;    // mới

        [Column("xuatXu")]
        [MaxLength(10)]
        [Required]
        private string xuatXu;     // mới

        [Column("soLuongTon")]
        [Required]
        private int soLuongTon;

        [Column("giaNhap")]
        [Required]
        private double giaNhap;

        [Column("donGia")]
        [Required]
        private double donGia;

        [Column("hanSuDung")]
        [Required]
        private DateTime hanSuDung;

        [Column("isDeleted")]
        private bool? isDeleted; // (Java: private Boolean isDeleted)
        // ← KHÁC VỚI JAVA: dùng bool? vì BIT NULL trong SQL

        // Constructor mặc định (Java có → giữ nguyên)
        public Thuoc()
        {
        }

        // Getter & Setter giống hệt Java
        public string GetIdThuoc()
        {
            return idThuoc;
        }

        public void SetIdThuoc(string idThuoc)
        {
            this.idThuoc = idThuoc;
        }

        public string GetTenThuoc()
        {
            return tenThuoc;
        }

        public void SetTenThuoc(string tenThuoc)
        {
            this.tenThuoc = tenThuoc;
        }

        public byte[]? GetHinhAnh()
        {
            return hinhAnh;
        }

        public void SetHinhAnh(byte[]? hinhAnh)
        {
            this.hinhAnh = hinhAnh;
        }

        public string? GetThanhPhan()
        {
            return thanhPhan;
        }

        public void SetThanhPhan(string? thanhPhan)
        {
            this.thanhPhan = thanhPhan;
        }

        public string GetDonViTinh()
        {
            return donViTinh;
        }

        public void SetDonViTinh(string donViTinh)
        {
            this.donViTinh = donViTinh;
        }

        public string GetDanhMuc()
        {
            return danhMuc;
        }

        public void SetDanhMuc(string danhMuc)
        {
            this.danhMuc = danhMuc;
        }

        public string GetXuatXu()
        {
            return xuatXu;
        }

        public void SetXuatXu(string xuatXu)
        {
            this.xuatXu = xuatXu;
        }

        public int GetSoLuongTon()
        {
            return soLuongTon;
        }

        public void SetSoLuongTon(int soLuongTon)
        {
            this.soLuongTon = soLuongTon;
        }

        public double GetGiaNhap()
        {
            return giaNhap;
        }

        public void SetGiaNhap(double giaNhap)
        {
            this.giaNhap = giaNhap;
        }

        public double GetDonGia()
        {
            return donGia;
        }

        public void SetDonGia(double donGia)
        {
            this.donGia = donGia;
        }

        public DateTime GetHanSuDung()
        {
            return hanSuDung;
        }

        public void SetHanSuDung(DateTime hanSuDung)
        {
            this.hanSuDung = hanSuDung;
        }

        // DA them 2 ham nay
        public bool? GetIsDeleted()
        {
            return isDeleted;
        }

        public void SetIsDeleted(bool? isDeleted)
        {
            this.isDeleted = isDeleted;
        }
    }
}
