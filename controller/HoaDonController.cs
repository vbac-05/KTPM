using System;
using System.Collections.Generic;

// TODO: chỉnh namespace dưới đây cho khớp solution của bạn
using QLThuocApp.connectDB;      // DBConnection, DBCloseHelper nếu bạn cần dùng chỗ khác
using QLThuocApp.DAO;            // HoaDonDAO
using QLThuocApp.Entities;       // HoaDon, ChiTietHoaDon
using QLThuocApp.controller;     // KhachHangController (có thể đổi namespace nếu tách riêng)

namespace QLThuocApp.Controllers
{
    /// <summary>
    /// Controller = cầu nối giữa GUI và DAO.
    /// - Nhận yêu cầu từ GUI
    /// - Làm nghiệp vụ nhẹ (validate, gom lỗi, tính toán đơn giản)
    /// - Gọi DAO để thao tác DB
    /// - Không viết SQL ở đây
    /// </summary>
    public class HoaDonController
    {
        private readonly HoaDonDAO hoaDonDAO;
        private readonly KhachHangController khachHangController; 
        // TODO: Nếu KhachHangController của bạn yêu cầu tham số (ví dụ DAO inject vào),
        // hãy sửa constructor tương ứng.

        public HoaDonController()
        {
            hoaDonDAO = new HoaDonDAO();
            khachHangController = new KhachHangController();
        }

        /// <summary>
        /// Lấy tất cả hóa đơn để hiển thị lên GUI.
        /// </summary>
        public List<HoaDon> GetAllHoaDon()
        {
            return hoaDonDAO.GetAllHoaDon();
        }

        /// <summary>
        /// Thêm hóa đơn mới cùng danh sách chi tiết (master-detail).
        /// GUI đưa vào: đối tượng HoaDon + list ChiTietHoaDon
        /// Out errorMsg để GUI show MessageBox nếu lỗi.
        /// </summary>
        public bool AddHoaDon(HoaDon hd, List<ChiTietHoaDon> chiTietList, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                // Validate nhẹ trước khi đẩy xuống DAO (ví dụ: tổng tiền không âm)
                if (hd == null)
                {
                    errorMsg = "Hóa đơn không hợp lệ.";
                    return false;
                }

                if (chiTietList == null || chiTietList.Count == 0)
                {
                    errorMsg = "Hóa đơn phải có ít nhất một dòng chi tiết.";
                    return false;
                }

                // Gọi DAO để insert trong transaction
                return hoaDonDAO.InsertHoaDonWithDetails(hd, chiTietList);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin cơ bản của hóa đơn (không sửa chi tiết).
        /// </summary>
        public bool UpdateHoaDon(HoaDon hd, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (hd == null)
                {
                    errorMsg = "Dữ liệu hóa đơn không hợp lệ.";
                    return false;
                }

                return hoaDonDAO.UpdateHoaDon(hd);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa (hoặc đánh dấu xóa) hóa đơn theo idHD.
        /// </summary>
        public bool DeleteHoaDon(string idHD, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idHD))
                {
                    errorMsg = "Mã hóa đơn không hợp lệ.";
                    return false;
                }

                return hoaDonDAO.DeleteHoaDon(idHD);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm hóa đơn theo idHD / idNV / idKH.
        /// Tham số rỗng => DAO sẽ xử lý để trả về phù hợp.
        /// </summary>
        public List<HoaDon> SearchHoaDon(string idHD, string idNV, string idKH)
        {
            return hoaDonDAO.SearchHoaDon(idHD, idNV, idKH);
        }

        // ================== PHẦN ĐIỂM KHÁCH HÀNG ==================
        // Controller hóa đơn có thể “hỏi hộ GUI” về điểm khách hàng
        // và yêu cầu trừ/cộng điểm thông qua KhachHangController.
        // Đây vẫn là nghiệp vụ nhẹ (business logic giao dịch với khách hàng).

        public int GetDiemHienTai(string idKH)
        {
            return khachHangController.GetDiemHienTai(idKH);
        }

        public bool TruDiem(string idKH, int soDiemTru)
        {
            return khachHangController.TruDiem(idKH, soDiemTru);
        }

        public bool CongDiem(string idKH, int soDiemCong)
        {
            return khachHangController.CongDiem(idKH, soDiemCong);
        }

        // ================== SINH MÃ HÓA ĐƠN TIẾP THEO ==================
        // Logic nhẹ, không truy cập DB trực tiếp ngoài DAO.
        // Cách làm: lấy danh sách hóa đơn từ DAO, tự tính mã kế tiếp theo format HD001, HD002,...

        public string GetNextHoaDonId()
        {
            var ds = GetAllHoaDon();
            int max = 0;

            foreach (var hd in ds)
            {
                if (!string.IsNullOrEmpty(hd.IdHD) && hd.IdHD.StartsWith("HD"))
                {
                    string so = hd.IdHD.Substring(2); // bỏ "HD"
                    if (int.TryParse(so, out int num))
                    {
                        if (num > max) max = num;
                    }
                }
            }

            int nextNum = max + 1;
            return $"HD{nextNum:000}"; // HD001, HD002, ...
        }

        // ================== THỐNG KÊ ==================
        // Quan trọng: KHÔNG có SQL ở controller.
        // Chúng ta gọi DAO, DAO mới query DB.
        //
        // => Bạn cần implement các hàm tương ứng trong HoaDonDAO:
        //    - Dictionary<string,int> TinhDoanhThuTheoNgay(string fromDate, string toDate)
        //    - Dictionary<string,int> TinhDoanhThuTheoThang(int year)
        //
        // Lý do return Dictionary<string,int>: để GUI lấy ra vẽ chart (ngày -> doanh thu).

        public Dictionary<string, int> TinhDoanhThuTheoNgay(string fromDate, string toDate)
        {
            // TODO: đảm bảo HoaDonDAO.TinhDoanhThuTheoNgay(...) đã được viết trong DAO.
            return hoaDonDAO.TinhDoanhThuTheoNgay(fromDate, toDate);
        }

        public Dictionary<string, int> TinhDoanhThuTheoThang(int year)
        {
            // TODO: đảm bảo HoaDonDAO.TinhDoanhThuTheoThang(...) đã được viết trong DAO.
            return hoaDonDAO.TinhDoanhThuTheoThang(year);
        }

        // ================== HÀM BỔ SUNG ==================

        /// <summary>
        /// Lấy 1 hóa đơn theo ID (dùng cho màn hình chi tiết hóa đơn).
        /// </summary>
        public HoaDon GetHoaDonById(string idHD)
        {
            return hoaDonDAO.GetById(idHD);
        }

        /// <summary>
        /// Cập nhật hóa đơn và toàn bộ chi tiết (xóa chi tiết cũ, chèn chi tiết mới) trong 1 transaction.
        /// Toàn bộ SQL / transaction sẽ do DAO xử lý.
        /// </summary>
        public bool UpdateHoaDonWithDetails(HoaDon hd, List<ChiTietHoaDon> chiTietList, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (hd == null)
                {
                    errorMsg = "Hóa đơn không hợp lệ.";
                    return false;
                }

                if (chiTietList == null || chiTietList.Count == 0)
                {
                    errorMsg = "Danh sách chi tiết không hợp lệ.";
                    return false;
                }

                return hoaDonDAO.UpdateWithDetails(hd, chiTietList);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }
    }
}
