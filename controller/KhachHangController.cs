using System;
using System.Collections.Generic;

// TODO: chỉnh namespace dưới đây cho khớp với cấu trúc thật của project
using QLThuocApp.DAO;       // Chứa KhachHangDAO
using QLThuocApp.Entities;  // Chứa entity KhachHang

namespace QLThuocApp.Controllers
{
    /// <summary>
    /// KhachHangController.cs
    /// Controller = cầu nối giữa GUI và DAO.
    /// Không viết SQL, chỉ gọi DAO và xử lý logic nhẹ như kiểm tra dữ liệu, cộng/trừ điểm, bắt lỗi để báo lại GUI.
    /// </summary>
    public class KhachHangController
    {
        private readonly KhachHangDAO khachHangDAO;

        public KhachHangController()
        {
            khachHangDAO = new KhachHangDAO(); // TODO: nếu DAO cần truyền connection hoặc context, thêm vào đây
        }

        /// <summary>
        /// Lấy toàn bộ danh sách khách hàng.
        /// </summary>
        public List<KhachHang> GetAllKhachHang()
        {
            return khachHangDAO.GetAll();
        }

        /// <summary>
        /// Thêm khách hàng mới.
        /// </summary>
        public bool AddKhachHang(KhachHang kh, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                if (kh == null)
                {
                    errorMsg = "Dữ liệu khách hàng không hợp lệ.";
                    return false;
                }

                // TODO: có thể thêm kiểm tra nghiệp vụ nhẹ ở đây, ví dụ:
                // if (string.IsNullOrWhiteSpace(kh.HoTen)) { errorMsg = "Họ tên không được để trống."; return false; }

                return khachHangDAO.Insert(kh);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng.
        /// </summary>
        public bool UpdateKhachHang(KhachHang kh, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                if (kh == null)
                {
                    errorMsg = "Khách hàng không hợp lệ.";
                    return false;
                }

                return khachHangDAO.Update(kh);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa khách hàng theo ID.
        /// </summary>
        public bool DeleteKhachHang(string idKH, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(idKH))
                {
                    errorMsg = "Mã khách hàng không hợp lệ.";
                    return false;
                }

                return khachHangDAO.Delete(idKH);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo họ tên hoặc số điện thoại.
        /// Nếu cả hai rỗng -> DAO sẽ trả về toàn bộ danh sách.
        /// </summary>
        public List<KhachHang> SearchKhachHang(string hoTen, string sdt)
        {
            return khachHangDAO.Search(hoTen, sdt);
        }

        /// <summary>
        /// Lấy khách hàng theo số điện thoại.
        /// </summary>
        public KhachHang GetBySDT(string sdt)
        {
            return khachHangDAO.GetBySDT(sdt);
        }

        /// <summary>
        /// Cộng điểm tích lũy cho khách hàng.
        /// </summary>
        public bool CongDiem(string idKH, int soDiemCong)
        {
            return khachHangDAO.CongDiem(idKH, soDiemCong);
        }

        /// <summary>
        /// Trừ điểm tích lũy (chỉ trừ khi đủ điểm).
        /// Đây là logic nghiệp vụ nhẹ — hợp lệ theo flow Controller.
        /// </summary>
        public bool TruDiem(string idKH, int soDiemTru)
        {
            var kh = khachHangDAO.GetById(idKH);
            if (kh == null) return false;
            if (kh.DiemTichLuy < soDiemTru) return false;

            return khachHangDAO.TruDiem(idKH, soDiemTru);
        }

        /// <summary>
        /// Cập nhật điểm tích lũy về giá trị cụ thể.
        /// </summary>
        public bool UpdateDiemTichLuy(string idKH, int diemMoi)
        {
            return khachHangDAO.UpdateDiemTichLuy(idKH, diemMoi);
        }

        /// <summary>
        /// Lấy điểm hiện tại của khách hàng.
        /// </summary>
        public int GetDiemHienTai(string idKH)
        {
            var kh = khachHangDAO.GetById(idKH);
            if (kh != null)
                return kh.DiemTichLuy;

            return 0;
        }
    }
}
