using System;
using System.Collections.Generic;

// TODO: chỉnh lại namespace cho đúng với cấu trúc project của bạn
using QLThuocApp.dao;       // Chứa PhieuNhapDAO
using QLThuocApp.entities;  // Chứa entity PhieuNhap

namespace QLThuocApp.controller
{
    /// <summary>
    /// PhieuNhapController.cs
    /// Controller là cầu nối giữa GUI và DAO cho PhieuNhap.
    /// Không viết SQL ở đây. Chỉ nhận yêu cầu từ GUI, kiểm tra dữ liệu (nếu cần),
    /// rồi gọi DAO để thực hiện CRUD hoặc tìm kiếm.
    /// </summary>
    public class PhieuNhapController
    {
        private readonly PhieuNhapDAO phieuNhapDAO;

        public PhieuNhapController()
        {
            phieuNhapDAO = new PhieuNhapDAO(); // TODO: nếu DAO yêu cầu DBConnection, thêm tham số vào constructor
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phiếu nhập.
        /// </summary>
        public List<PhieuNhap> GetAllPhieuNhap()
        {
            return phieuNhapDAO.GetAll();
        }

        /// <summary>
        /// Thêm phiếu nhập mới.
        /// </summary>
        /// <param name="pn">Đối tượng phiếu nhập cần thêm.</param>
        /// <param name="errorMsg">Biến nhận thông báo lỗi (nếu có).</param>
        /// <returns>True nếu thêm thành công, false nếu lỗi.</returns>
        public bool AddPhieuNhap(PhieuNhap pn, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (pn == null)
                {
                    errorMsg = "Dữ liệu phiếu nhập không hợp lệ.";
                    return false;
                }

                // TODO: có thể thêm kiểm tra nghiệp vụ nhẹ ở đây, ví dụ:
                // if (string.IsNullOrWhiteSpace(pn.IdNCC)) { errorMsg = "Thiếu nhà cung cấp."; return false; }

                return phieuNhapDAO.Insert(pn);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin phiếu nhập.
        /// </summary>
        public bool UpdatePhieuNhap(PhieuNhap pn, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (pn == null)
                {
                    errorMsg = "Dữ liệu phiếu nhập không hợp lệ.";
                    return false;
                }

                return phieuNhapDAO.Update(pn);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa phiếu nhập theo mã idPN.
        /// </summary>
        public bool DeletePhieuNhap(string idPN, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idPN))
                {
                    errorMsg = "Mã phiếu nhập không hợp lệ.";
                    return false;
                }

                // ⚠️ Bản Java gọi hàm đặc biệt deletePhieuNhap() thay vì delete()
                // → vì vậy ở C# ta giữ nguyên phương thức tương ứng.
                return phieuNhapDAO.DeletePhieuNhap(idPN);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm phiếu nhập theo idPN, idNV hoặc idNCC.
        /// Nếu cả ba tham số đều rỗng, DAO sẽ trả về toàn bộ danh sách.
        /// </summary>
        public List<PhieuNhap> SearchPhieuNhap(string idPN, string idNV, string idNCC)
        {
            return phieuNhapDAO.Search(idPN, idNV, idNCC);
        }
    }
}
