using System;
using System.Collections.Generic;

// TODO: chỉnh lại namespace cho đúng với project thực tế của bạn
using QLThuocApp.dao;       // chứa NhaCungCapDAO
using QLThuocApp.entities;  // chứa entity NhaCungCap

namespace QLThuocApp.controller
{
    /// <summary>
    /// NhaCungCapController.cs
    /// Controller = cầu nối giữa GUI và DAO.
    /// Nhận yêu cầu từ GUI (thêm, sửa, xóa, tìm kiếm, lấy danh sách),
    /// có thể làm thêm nghiệp vụ nhẹ như kiểm tra dữ liệu, rồi gọi DAO để thao tác DB.
    /// Không viết SQL ở đây (SQL nằm ở DAO).
    /// </summary>
    public class NhaCungCapController
    {
        private readonly NhaCungCapDAO nccDAO;

        public NhaCungCapController()
        {
            nccDAO = new NhaCungCapDAO(); // TODO: nếu DAO cần DBConnection hoặc context, thêm vào constructor
        }

        /// <summary>
        /// Lấy toàn bộ danh sách nhà cung cấp.
        /// </summary>
        public List<NhaCungCap> GetAllNhaCungCap()
        {
            return nccDAO.GetAll();
        }

        /// <summary>
        /// Thêm mới nhà cung cấp.
        /// </summary>
        /// <param name="ncc">Đối tượng nhà cung cấp cần thêm.</param>
        /// <param name="errorMsg">Chuỗi lỗi để GUI hiển thị (nếu có).</param>
        /// <returns>True nếu thành công, false nếu lỗi.</returns>
        public bool AddNhaCungCap(NhaCungCap ncc, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                if (ncc == null)
                {
                    errorMsg = "Dữ liệu nhà cung cấp không hợp lệ.";
                    return false;
                }

                // TODO: Có thể thêm logic nghiệp vụ nhẹ ở đây, ví dụ:
                // if (string.IsNullOrWhiteSpace(ncc.TenNCC)) { errorMsg = "Tên nhà cung cấp không được để trống."; return false; }

                return nccDAO.Insert(ncc);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp.
        /// </summary>
        public bool UpdateNhaCungCap(NhaCungCap ncc, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                if (ncc == null)
                {
                    errorMsg = "Thông tin nhà cung cấp không hợp lệ.";
                    return false;
                }

                return nccDAO.Update(ncc);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa nhà cung cấp theo mã ID.
        /// </summary>
        public bool DeleteNhaCungCap(string idNCC, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(idNCC))
                {
                    errorMsg = "Mã nhà cung cấp không hợp lệ.";
                    return false;
                }

                return nccDAO.Delete(idNCC);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm nhà cung cấp theo mã hoặc tên.
        /// Nếu cả hai rỗng, DAO sẽ trả toàn bộ danh sách.
        /// </summary>
        public List<NhaCungCap> SearchNhaCungCap(string idNCC, string tenNCC)
        {
            return nccDAO.Search(idNCC, tenNCC);
        }
    }
}
