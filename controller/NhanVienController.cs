using System;
using System.Collections.Generic;

// TODO: chỉnh lại namespace bên dưới cho đúng với project thật của bạn
using QLThuocApp.dao;       // Chứa NhanVienDAO
using QLThuocApp.entities;  // Chứa entity NhanVien

namespace QLThuocApp.controller
{
    /// <summary>
    /// NhanVienController.cs
    /// Controller là cầu nối giữa GUI và DAO cho NhanVien.
    /// Không viết SQL tại đây; chỉ gọi DAO và có thể làm nghiệp vụ nhẹ (validate, xử lý lỗi).
    /// </summary>
    public class NhanVienController
    {
        private readonly NhanVienDAO nhanVienDAO;

        public NhanVienController()
        {
            nhanVienDAO = new NhanVienDAO(); // TODO: nếu DAO yêu cầu truyền DBConnection, thêm vào constructor
        }

        /// <summary>
        /// Lấy toàn bộ danh sách nhân viên.
        /// </summary>
        public List<NhanVien> GetAllNhanVien()
        {
            return nhanVienDAO.GetAll();
        }

        /// <summary>
        /// Thêm nhân viên mới.
        /// </summary>
        /// <param name="nv">Đối tượng nhân viên cần thêm.</param>
        /// <param name="errorMsg">Biến nhận thông báo lỗi (nếu có).</param>
        /// <returns>True nếu thêm thành công, false nếu lỗi.</returns>
        public bool AddNhanVien(NhanVien nv, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (nv == null)
                {
                    errorMsg = "Dữ liệu nhân viên không hợp lệ.";
                    return false;
                }

                // TODO: bạn có thể thêm kiểm tra nghiệp vụ nhẹ ở đây
                // if (string.IsNullOrWhiteSpace(nv.HoTen)) { errorMsg = "Tên nhân viên không được để trống."; return false; }

                return nhanVienDAO.Insert(nv);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên.
        /// </summary>
        public bool UpdateNhanVien(NhanVien nv, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (nv == null)
                {
                    errorMsg = "Thông tin nhân viên không hợp lệ.";
                    return false;
                }

                return nhanVienDAO.Update(nv);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa nhân viên theo mã ID.
        /// </summary>
        public bool DeleteNhanVien(string idNV, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(idNV))
                {
                    errorMsg = "Mã nhân viên không hợp lệ.";
                    return false;
                }

                return nhanVienDAO.Delete(idNV);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm nhân viên theo mã hoặc họ tên.
        /// Nếu cả hai tham số r
