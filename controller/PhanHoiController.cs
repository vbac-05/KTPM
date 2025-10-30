using System;
using System.Collections.Generic;

// TODO: chỉnh namespace này cho đúng với project thật của bạn
using QLThuocApp.dao;       // Chứa PhanHoiDAO, HoaDonDAO, KhachHangDAO
using QLThuocApp.entities;  // Chứa entity PhanHoi, KhachHang, HoaDon

namespace QLThuocApp.controller
{
    /// <summary>
    /// PhanHoiController.cs
    /// Controller = cầu nối giữa GUI và DAO.
    /// Gọi DAO để thực hiện CRUD và thêm các nghiệp vụ nhẹ (validate, sinh mã, xử lý logic phản hồi).
    /// Không viết SQL tại đây.
    /// </summary>
    public class PhanHoiController
    {
        private readonly PhanHoiDAO phanHoiDAO;
        private readonly HoaDonDAO hoaDonDAO; // để kiểm tra hóa đơn
        private readonly KhachHangDAO khachHangDAO; // dùng để sinh idKH khách mới

        public PhanHoiController()
        {
            phanHoiDAO = new PhanHoiDAO();
            hoaDonDAO = new HoaDonDAO();
            khachHangDAO = new KhachHangDAO();
        }

        /// <summary>
        /// Lấy tất cả phản hồi.
        /// </summary>
        public List<PhanHoi> GetAllPhanHoi()
        {
            return phanHoiDAO.GetAll();
        }

        /// <summary>
        /// Thêm phản hồi mới.
        /// </summary>
        public bool AddPhanHoi(PhanHoi ph, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (ph == null)
                {
                    errorMsg = "Phản hồi không hợp lệ.";
                    return false;
                }

                return phanHoiDAO.Insert(ph);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật phản hồi.
        /// </summary>
        public bool UpdatePhanHoi(PhanHoi ph, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (ph == null)
                {
                    errorMsg = "Dữ liệu phản hồi không hợp lệ.";
                    return false;
                }

                return phanHoiDAO.Update(ph);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa phản hồi theo mã idPH.
        /// </summary>
        public bool DeletePhanHoi(string idPH, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idPH))
                {
                    errorMsg = "Mã phản hồi không hợp lệ.";
                    return false;
                }

                return phanHoiDAO.Delete(idPH);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm phản hồi theo idPH hoặc idKH.
        /// </summary>
        public List<PhanHoi> SearchPhanHoi(string idPH, string idKH)
        {
            return phanHoiDAO.Search(idPH, idKH);
        }

        // ============================================================
        // BỔ SUNG CHỨC NĂNG GỬI PHẢN HỒI CHẾ ĐỘ KHÁCH
        // ============================================================

        /// <summary>
        /// Gửi phản hồi ở chế độ khách (guest).
        /// - Kiểm tra hóa đơn có tồn tại không.
        /// - Lấy idKH từ hóa đơn.
        /// - Nếu khách mới => sinh mã KH0xxx.
        /// - Sinh idPH = PHxxx tự tăng.
        /// </summary>
        public bool AddPhanHoiGuest(string idHD, string sdt, string noiDung, int danhGia, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idHD))
                {
                    errorMsg = "Mã hóa đơn không hợp lệ.";
                    return false;
                }

                if (!hoaDonDAO.Exists(idHD))
                {
                    errorMsg = "Hóa đơn không tồn tại!";
                    return false;
                }

                // Lấy idKH từ hóa đơn
                string idKH = hoaDonDAO.GetKhachHangIdByHoaDonId(idHD); // TODO: đảm bảo DAO đã có hàm này

                if (idKH == null)
                {
                    errorMsg = "Không tìm thấy khách hàng cho hóa đơn này!";
                    return false;
                }

                // Nếu cần thêm khách hàng mới (trường hợp KH không tồn tại)
                if (string.IsNullOrWhiteSpace(idKH))
                {
                    idKH = GenerateNextGuestIdKH();
                    // TODO: thêm khách hàng mới vào DB (nếu bạn muốn)
                    // khachHangDAO.Insert(new KhachHang(idKH, "Khách mới", sdt, ...));
                }

                string idPH = GenerateNextPhanHoiId();
                DateTime thoiGian = DateTime.Now;

                var ph = new PhanHoi(idPH, idKH, idHD, noiDung, thoiGian, danhGia);
                return phanHoiDAO.Insert(ph);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        // ============================================================
        // HÀM SINH MÃ TỰ ĐỘNG
        // ============================================================

        /// <summary>
        /// Sinh mã khách hàng mới dạng KH0xxx.
        /// </summary>
        private string GenerateNextGuestIdKH()
        {
            var list = khachHangDAO.GetAll();
            int max = 0;

            foreach (var kh in list)
            {
                if (kh.IdKH != null && kh.IdKH.StartsWith("KH0"))
                {
                    if (int.TryParse(kh.IdKH.Substring(3), out int num))
                    {
                        if (num > max) max = num;
                    }
                }
            }

            return $"KH0{(max + 1):000}";
        }

        /// <summary>
        /// Sinh mã phản hồi mới dạng PHxxx.
        /// </summary>
        private string GenerateNextPhanHoiId()
        {
            var list = phanHoiDAO.GetAll();
            int max = 0;

            foreach (var ph in list)
            {
                if (ph.IdPH != null && ph.IdPH.StartsWith("PH"))
                {
                    if (int.TryParse(ph.IdPH.Substring(2), out int num))
                    {
                        if (num > max) max = num;
                    }
                }
            }

            return $"PH{(max + 1):000}";
        }
    }
}
