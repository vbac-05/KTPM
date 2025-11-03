using System;
using System.Collections.Generic;

// TODO: chỉnh namespace này cho đúng với cấu trúc project thật
using QLThuocApp.DAO;       // Chứa ThuocDAO
using QLThuocApp.Entities;  // Chứa entity Thuoc

namespace QLThuocApp.Controllers
{
    /// <summary>
    /// ThuocController.cs
    /// Controller là cầu nối giữa GUI và DAO cho đối tượng Thuoc.
    /// Không viết SQL ở đây. Chỉ xử lý nghiệp vụ nhẹ (validate, bắt lỗi, tính toán) và gọi DAO.
    /// </summary>
    public class ThuocController
    {
        private readonly ThuocDAO thuocDAO;

        public ThuocController()
        {
            thuocDAO = new ThuocDAO(); // TODO: nếu DAO yêu cầu DBConnection, thêm vào constructor
        }

        /// <summary>
        /// Lấy toàn bộ danh sách thuốc.
        /// </summary>
        public List<Thuoc> GetAllThuoc()
        {
            return thuocDAO.GetAllThuoc();
        }

        /// <summary>
        /// Lấy thông tin thuốc theo ID.
        /// </summary>
        public Thuoc GetById(string idThuoc)
        {
            return thuocDAO.GetById(idThuoc);
        }

        /// <summary>
        /// Thêm thuốc mới.
        /// </summary>
        public bool AddThuoc(Thuoc t, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (t == null)
                {
                    errorMsg = "Thông tin thuốc không hợp lệ.";
                    return false;
                }

                // TODO: có thể thêm kiểm tra nghiệp vụ nhẹ, ví dụ:
                // if (string.IsNullOrWhiteSpace(t.TenThuoc)) { errorMsg = "Tên thuốc không được để trống."; return false; }

                return thuocDAO.InsertThuoc(t);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin thuốc.
        /// </summary>
        public bool UpdateThuoc(Thuoc t, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (t == null)
                {
                    errorMsg = "Dữ liệu thuốc không hợp lệ.";
                    return false;
                }

                return thuocDAO.UpdateThuoc(t);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa thuốc theo ID.
        /// </summary>
        public bool DeleteThuoc(string idThuoc, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idThuoc))
                {
                    errorMsg = "Mã thuốc không hợp lệ.";
                    return false;
                }

                return thuocDAO.DeleteThuoc(idThuoc);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Giảm số lượng thuốc trong kho.
        /// </summary>
        public bool GiamSoLuongThuoc(string idThuoc, int soLuong, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idThuoc))
                {
                    errorMsg = "Mã thuốc không hợp lệ.";
                    return false;
                }

                if (soLuong <= 0)
                {
                    errorMsg = "Số lượng giảm phải lớn hơn 0.";
                    return false;
                }

                // TODO: kiểm tra tồn kho (nếu cần) trước khi trừ

                // Gọi DAO để giảm số lượng thuốc
                return new ThuocDAO().GiamSoLuong(idThuoc, soLuong);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm thuốc theo mã hoặc tên.
        /// Nếu cả hai tham số rỗng thì trả về toàn bộ danh sách.
        /// </summary>
        public List<Thuoc> SearchThuoc(string idThuoc, string tenThuoc)
        {
            return thuocDAO.SearchThuoc(idThuoc, tenThuoc);
        }
    }
}
