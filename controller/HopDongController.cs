using System;
using System.Collections.Generic;

// TODO: chỉnh namespace dưới đây cho khớp với cấu trúc project của bạn
using QLThuocApp.DAO;       // Chứa HopDongDAO
using QLThuocApp.Entities;  // Chứa entity HopDong

namespace QLThuocApp.Controllers
{
    /// <summary>
    /// HopDongController.cs
    /// Controller = cầu nối giữa GUI và DAO.
    /// Nhận yêu cầu từ GUI (thêm, sửa, xóa, tìm kiếm, lấy danh sách),
    /// thực hiện kiểm tra nhẹ nếu cần, rồi gọi DAO để thao tác CSDL.
    /// Không viết SQL ở đây (SQL nằm ở DAO).
    /// </summary>
    public class HopDongController
    {
        private readonly HopDongDAO hopDongDAO;

        public HopDongController()
        {
            hopDongDAO = new HopDongDAO(); // Khởi tạo DAO
        }

        /// <summary>
        /// Lấy toàn bộ danh sách hợp đồng từ DAO.
        /// </summary>
        public List<HopDong> GetAllHopDong()
        {
            return hopDongDAO.GetAllHopDong();
        }

        /// <summary>
        /// Thêm hợp đồng mới.
        /// </summary>
        /// <param name="hd">Đối tượng HopDong cần thêm.</param>
        /// <param name="errorMsg">Chuỗi lỗi (nếu có) để GUI hiển thị.</param>
        /// <returns>true nếu thành công, false nếu lỗi.</returns>
        public bool AddHopDong(HopDong hd, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (hd == null)
                {
                    errorMsg = "Dữ liệu hợp đồng không hợp lệ.";
                    return false;
                }

                // TODO: bạn có thể thêm kiểm tra nghiệp vụ nhẹ, ví dụ:
                // if (string.IsNullOrEmpty(hd.IdNCC)) { errorMsg = "Thiếu nhà cung cấp."; return false; }

                return hopDongDAO.InsertHopDong(hd);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin hợp đồng.
        /// </summary>
        public bool UpdateHopDong(HopDong hd, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (hd == null)
                {
                    errorMsg = "Hợp đồng cần cập nhật không hợp lệ.";
                    return false;
                }

                return hopDongDAO.UpdateHopDong(hd);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa hợp đồng theo mã idHDong.
        /// </summary>
        public bool DeleteHopDong(string idHDong, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idHDong))
                {
                    errorMsg = "Mã hợp đồng không hợp lệ.";
                    return false;
                }

                return hopDongDAO.DeleteHopDong(idHDong);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm hợp đồng theo idHDong, idNV, idNCC.
        /// Nếu tham số rỗng, DAO sẽ xử lý để trả về danh sách phù hợp.
        /// </summary>
        public List<HopDong> SearchHopDong(string idHDong, string idNV, string idNCC)
        {
            return hopDongDAO.SearchHopDong(idHDong, idNV, idNCC);
        }
    }
}
