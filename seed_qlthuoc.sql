-- Tạo DB + chọn UTF-8 đầy đủ
CREATE DATABASE IF NOT EXISTS QLTHUOC
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
USE QLTHUOC;

-- Bảng vai trò
DROP TABLE IF EXISTS vaitro;
CREATE TABLE vaitro (
  idVT  VARCHAR(10) PRIMARY KEY,
  tenVT VARCHAR(50) NOT NULL
) ENGINE=InnoDB;

INSERT INTO vaitro(idVT, tenVT) VALUES
('VT01', 'Admin'),
('VT02', 'NhanVien');

-- Nhân viên
DROP TABLE IF EXISTS nhanvien;
CREATE TABLE nhanvien (
  id INT AUTO_INCREMENT PRIMARY KEY,
  hoTen VARCHAR(100) NOT NULL,
  sdt VARCHAR(20),
  vaiTro VARCHAR(50),
  is_deleted TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB;

INSERT INTO nhanvien(hoTen, sdt, vaiTro) VALUES
('Nguyễn Văn A', '0901111222', 'Quản trị'),
('Trần Thị B', '0903333444', 'Bán hàng');

-- Tài khoản (đơn giản: mật khẩu dạng plain để test; khi làm thật hãy băm)
DROP TABLE IF EXISTS taikhoan;
CREATE TABLE taikhoan (
  id INT AUTO_INCREMENT PRIMARY KEY,
  tenTK VARCHAR(50) NOT NULL UNIQUE,
  matKhau VARCHAR(255) NOT NULL,
  idVT VARCHAR(10) NOT NULL,
  idNV INT,
  FOREIGN KEY (idVT) REFERENCES vaitro(idVT),
  FOREIGN KEY (idNV) REFERENCES nhanvien(id)
) ENGINE=InnoDB;

INSERT INTO taikhoan(tenTK, matKhau, idVT, idNV) VALUES
('admin', 'admin', 'VT01', 1),
('nv',    'nv',    'VT02', 2);

-- Khách hàng
DROP TABLE IF EXISTS khachhang;
CREATE TABLE khachhang (
  id INT AUTO_INCREMENT PRIMARY KEY,
  hoTen VARCHAR(100) NOT NULL,
  sdt VARCHAR(20),
  diem INT DEFAULT 0,
  is_deleted TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB;

INSERT INTO khachhang(hoTen, sdt, diem) VALUES
('Phạm Minh C', '0912345678', 10),
('Lê Thị D',    '0987654321',  0),
('Hoàng Văn E', '0909090909',  5);

-- Nhà cung cấp
DROP TABLE IF EXISTS nhacungcap;
CREATE TABLE nhacungcap (
  id INT AUTO_INCREMENT PRIMARY KEY,
  ten VARCHAR(150) NOT NULL,
  sdt VARCHAR(20),
  diaChi VARCHAR(255),
  is_deleted TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB;

INSERT INTO nhacungcap(ten, sdt, diaChi) VALUES
('Cty Dược An Khang', '0281234567', 'Q.1, TP.HCM'),
('Cty Dược Bình Minh', '0247654321', 'Cầu Giấy, Hà Nội');

-- Thuốc
DROP TABLE IF EXISTS thuoc;
CREATE TABLE thuoc (
  id INT AUTO_INCREMENT PRIMARY KEY,
  ten VARCHAR(200) NOT NULL,
  donGia DECIMAL(18,2) NOT NULL,
  soLuong INT NOT NULL DEFAULT 0,
  donVi VARCHAR(50),
  hanSuDung DATE,
  is_deleted TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB;

INSERT INTO thuoc(ten, donGia, soLuong, donVi, hanSuDung) VALUES
('Paracetamol 500mg', 3500,  200, 'viên', '2027-12-31'),
('Vitamin C 1000mg',  8000,  150, 'viên', '2026-06-30'),
('Amoxicillin 500mg', 6500,  120, 'viên', '2026-10-31'),
('Natri Clorid 0.9%', 12000,  80, 'chai', '2028-01-31'),
('Acetylcystein 200', 7000,   90, 'gói',  '2027-03-31');

-- Hóa đơn
DROP TABLE IF EXISTS hoadon;
CREATE TABLE hoadon (
  id INT AUTO_INCREMENT PRIMARY KEY,
  idNV INT NOT NULL,
  idKH INT,
  thoiGian DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  tongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
  pttt VARCHAR(50),
  trangThai VARCHAR(50),
  is_deleted TINYINT(1) NOT NULL DEFAULT 0,
  FOREIGN KEY (idNV) REFERENCES nhanvien(id),
  FOREIGN KEY (idKH) REFERENCES khachhang(id)
) ENGINE=InnoDB;

-- Chi tiết hóa đơn
DROP TABLE IF EXISTS chitiethoadon;
CREATE TABLE chitiethoadon (
  id INT AUTO_INCREMENT PRIMARY KEY,
  idHD INT NOT NULL,
  idThuoc INT NOT NULL,
  soLuong INT NOT NULL,
  donGia DECIMAL(18,2) NOT NULL,
  FOREIGN KEY (idHD) REFERENCES hoadon(id) ON DELETE CASCADE,
  FOREIGN KEY (idThuoc) REFERENCES thuoc(id)
) ENGINE=InnoDB;

-- Phiếu nhập
DROP TABLE IF EXISTS phieunhap;
CREATE TABLE phieunhap (
  id INT AUTO_INCREMENT PRIMARY KEY,
  idNV INT NOT NULL,
  idNCC INT NOT NULL,
  thoiGian DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  tongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
  ghiChu VARCHAR(255),
  is_deleted TINYINT(1) NOT NULL DEFAULT 0,
  FOREIGN KEY (idNV) REFERENCES nhanvien(id),
  FOREIGN KEY (idNCC) REFERENCES nhacungcap(id)
) ENGINE=InnoDB;

-- Chi tiết phiếu nhập
DROP TABLE IF EXISTS chitietphieunhap;
CREATE TABLE chitietphieunhap (
  id INT AUTO_INCREMENT PRIMARY KEY,
  idPN INT NOT NULL,
  idThuoc INT NOT NULL,
  soLuong INT NOT NULL,
  donGia DECIMAL(18,2) NOT NULL,
  FOREIGN KEY (idPN) REFERENCES phieunhap(id) ON DELETE CASCADE,
  FOREIGN KEY (idThuoc) REFERENCES thuoc(id)
) ENGINE=InnoDB;

-- Phản hồi (nếu panel của bạn dùng)
DROP TABLE IF EXISTS phanhoi;
CREATE TABLE phanhoi (
  id INT AUTO_INCREMENT PRIMARY KEY,
  hoTen VARCHAR(100),
  sdt VARCHAR(20),
  noiDung TEXT,
  thoiGian DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  is_deleted TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB;

-- ==================== DỮ LIỆU MẪU HÓA ĐƠN ====================
-- Hóa đơn 1 của KH 1, NV 2
INSERT INTO hoadon(idNV, idKH, thoiGian, tongTien, pttt, trangThai)
VALUES (2, 1, NOW(), 0, 'Tiền mặt', 'Đã thanh toán');
SET @idHD1 = LAST_INSERT_ID();

INSERT INTO chitiethoadon(idHD, idThuoc, soLuong, donGia)
VALUES
(@idHD1, 1, 10, 3500),   -- Paracetamol
(@idHD1, 2,  5, 8000);   -- Vitamin C

-- Cập nhật tổng tiền HD1
UPDATE hoadon
SET tongTien = (
  SELECT SUM(soLuong * donGia) FROM chitiethoadon WHERE idHD = @idHD1
)
WHERE id = @idHD1;

-- Trừ tồn kho theo chi tiết hóa đơn
UPDATE thuoc t
JOIN chitiethoadon c ON t.id = c.idThuoc
SET t.soLuong = t.soLuong - c.soLuong
WHERE c.idHD = @idHD1;

-- ==================== DỮ LIỆU MẪU PHIẾU NHẬP ====================
INSERT INTO phieunhap(idNV, idNCC, thoiGian, tongTien, ghiChu)
VALUES (1, 1, NOW(), 0, 'Nhập đầu kỳ');
SET @idPN1 = LAST_INSERT_ID();

INSERT INTO chitietphieunhap(idPN, idThuoc, soLuong, donGia)
VALUES
(@idPN1, 3, 50, 6000),  -- Amoxicillin
(@idPN1, 5, 40, 6500);  -- Acetylcystein

-- Cập nhật tổng tiền PN1
UPDATE phieunhap
SET tongTien = (
  SELECT SUM(soLuong * donGia) FROM chitietphieunhap WHERE idPN = @idPN1
)
WHERE id = @idPN1;

-- Tăng tồn kho theo phiếu nhập
UPDATE thuoc t
JOIN chitietphieunhap c ON t.id = c.idThuoc
SET t.soLuong = t.soLuong + c.soLuong
WHERE c.idPN = @idPN1;
