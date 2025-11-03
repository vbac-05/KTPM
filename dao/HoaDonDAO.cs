using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa các lớp HoaDon, ChiTietHoaDon
// DBCloseHelper và các khối try-finally được thay thế bằng cấu trúc using(..)
namespace QLThuocApp.DAO
{
    /// <summary>
    /// CRUD cho bảng HoaDon (Phiên bản C# cho MySQL).
    /// </summary>
    public class HoaDonDAO
    {
        /// <summary>
        /// Lấy toàn bộ danh sách Hóa đơn (chưa bị xóa mềm).
        /// </summary>
        public List<HoaDon> GetAllHoaDon()
        {
            List<HoaDon> list = new List<HoaDon>();
            string sql = "SELECT idHD, thoiGian, idNV, idKH, tongTien, phuongThucThanhToan, trangThaiDonHang FROM HoaDon WHERE isDeleted IS NULL OR isDeleted = 0";

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HoaDon hd = new HoaDon
                            {
                                IdHD = reader["idHD"].ToString(),
                                ThoiGian = (DateTime)reader["thoiGian"],
                                IdNV = reader["idNV"].ToString(),
                                IdKH = reader["idKH"].ToString(),
                                TongTien = (double)reader["tongTien"],
                                // Xử lý DBNull cho trường có thể null
                                PhuongThucThanhToan = reader["phuongThucThanhToan"] == DBNull.Value ? null : reader["phuongThucThanhToan"].ToString(),
                                TrangThaiDonHang = reader["trangThaiDonHang"].ToString()
                            };
                            list.Add(hd);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }

        /// <summary>
        /// Thêm mới một Hóa đơn.
        /// Báo lỗi nếu trùng khóa hoặc thiếu khóa ngoại.
        /// </summary>
        public bool InsertHoaDon(HoaDon hd)
        {
            string sql = "INSERT INTO HoaDon (idHD, thoiGian, idNV, idKH, tongTien, phuongThucThanhToan, trangThaiDonHang) VALUES (@idHD, @thoiGian, @idNV, @idKH, @tongTien, @phuongThuc, @trangThai)";

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = hd.IdHD;
                        cmd.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = hd.ThoiGian;
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = hd.IdNV;
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = hd.IdKH;
                        cmd.Parameters.Add("@tongTien", MySqlDbType.Double).Value = hd.TongTien;
                        // Xử lý giá trị null
                        cmd.Parameters.Add("@phuongThuc", MySqlDbType.VarChar).Value = (object)hd.PhuongThucThanhToan ?? DBNull.Value;
                        cmd.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = hd.TrangThaiDonHang;
                        
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Trùng PK (tương đương 2627 của SQL Server)
                {
                    throw new Exception("ID hóa đơn đã tồn tại!");
                }
                if (ex.Number == 1452) // FK constraint failed (tương đương 547 của SQL Server)
                {
                    throw new Exception("ID nhân viên hoặc khách hàng không tồn tại!");
                }
                Console.WriteLine(ex.Message);
                throw new Exception("Lỗi SQL khi thêm hóa đơn: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật Hóa đơn (theo idHD).
        /// </summary>
        public bool UpdateHoaDon(HoaDon hd)
        {
            string sql = "UPDATE HoaDon SET thoiGian=@thoiGian, idNV=@idNV, idKH=@idKH, tongTien=@tongTien, phuongThucThanhToan=@phuongThuc, trangThaiDonHang=@trangThai WHERE idHD=@idHD";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = hd.ThoiGian;
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = hd.IdNV;
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = hd.IdKH;
                        cmd.Parameters.Add("@tongTien", MySqlDbType.Double).Value = hd.TongTien;
                        cmd.Parameters.Add("@phuongThuc", MySqlDbType.VarChar).Value = (object)hd.PhuongThucThanhToan ?? DBNull.Value;
                        cmd.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = hd.TrangThaiDonHang;
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = hd.IdHD;
                        
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1452) 
                {
                    throw new Exception("ID nhân viên hoặc khách hàng không tồn tại!");
                }
                Console.WriteLine(ex.Message);
                throw new Exception("Lỗi SQL khi cập nhật hóa đơn: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa mềm Hóa đơn theo idHD.
        /// </summary>
        public bool DeleteHoaDon(string idHD)
        {
            string sql = "UPDATE HoaDon SET isDeleted = 1 WHERE idHD = @idHD";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm Hóa đơn theo idHD, idNV hoặc idKH.
        /// </summary>
        public List<HoaDon> SearchHoaDon(string idHD, string idNV, string idKH)
        {
            List<HoaDon> list = new List<HoaDon>();
            var sqlBuilder = new StringBuilder(
                "SELECT idHD, thoiGian, idNV, idKH, tongTien, phuongThucThanhToan, trangThaiDonHang FROM HoaDon WHERE 1=1"
            );

            // Xây dựng câu lệnh SQL động một cách an toàn
            if (!string.IsNullOrWhiteSpace(idHD))
            {
                sqlBuilder.Append(" AND idHD LIKE @idHD");
            }
            if (!string.IsNullOrWhiteSpace(idNV))
            {
                sqlBuilder.Append(" AND idNV LIKE @idNV");
            }
            if (!string.IsNullOrWhiteSpace(idKH))
            {
                sqlBuilder.Append(" AND idKH LIKE @idKH");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sqlBuilder.ToString(), conn))
                    {
                        // Thêm tham số
                        if (!string.IsNullOrWhiteSpace(idHD))
                        {
                            cmd.Parameters.AddWithValue("@idHD", $"%{idHD.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(idNV))
                        {
                            cmd.Parameters.AddWithValue("@idNV", $"%{idNV.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(idKH))
                        {
                            cmd.Parameters.AddWithValue("@idKH", $"%{idKH.Trim()}%");
                        }
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // ... map data to HoaDon object ...
                                HoaDon hd = new HoaDon
                                {
                                    IdHD = reader["idHD"].ToString(),
                                    ThoiGian = (DateTime)reader["thoiGian"],
                                    IdNV = reader["idNV"].ToString(),
                                    IdKH = reader["idKH"].ToString(),
                                    TongTien = (double)reader["tongTien"],
                                    PhuongThucThanhToan = reader["phuongThucThanhToan"] == DBNull.Value ? null : reader["phuongThucThanhToan"].ToString(),
                                    TrangThaiDonHang = reader["trangThaiDonHang"].ToString()
                                };
                                list.Add(hd);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }

        /// <summary>
        /// Thêm Hóa đơn và các chi tiết (sử dụng Transaction).
        /// </summary>
        public bool InsertHoaDonWithDetails(HoaDon hd, List<ChiTietHoaDon> chiTietList)
        {
            string sqlHD = "INSERT INTO HoaDon (idHD, thoiGian, idNV, idKH, tongTien, phuongThucThanhToan, trangThaiDonHang) VALUES (@idHD, @thoiGian, @idNV, @idKH, @tongTien, @phuongThuc, @trangThai)";
            string sqlCT = "INSERT INTO ChiTietHoaDon (idHD, idThuoc, soLuong, donGia) VALUES (@idHD, @idThuoc, @soLuong, @donGia)";

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction()) // Bắt đầu Transaction
                {
                    try
                    {
                        // 1. Thêm hóa đơn
                        using (MySqlCommand cmdHD = new MySqlCommand(sqlHD, conn, trans))
                        {
                            cmdHD.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = hd.IdHD;
                            cmdHD.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = hd.ThoiGian;
                            cmdHD.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = hd.IdNV;
                            cmdHD.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = hd.IdKH;
                            cmdHD.Parameters.Add("@tongTien", MySqlDbType.Double).Value = hd.TongTien;
                            cmdHD.Parameters.Add("@phuongThuc", MySqlDbType.VarChar).Value = (object)hd.PhuongThucThanhToan ?? DBNull.Value;
                            cmdHD.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = hd.TrangThaiDonHang;
                            cmdHD.ExecuteNonQuery();
                        }

                        // 2. Thêm từng chi tiết hóa đơn (tương đương executeBatch)
                        using (MySqlCommand cmdCT = new MySqlCommand(sqlCT, conn, trans))
                        {
                            // Chuẩn bị các tham số
                            cmdCT.Parameters.Add("@idHD", MySqlDbType.VarChar);
                            cmdCT.Parameters.Add("@idThuoc", MySqlDbType.VarChar);
                            cmdCT.Parameters.Add("@soLuong", MySqlDbType.Int32);
                            cmdCT.Parameters.Add("@donGia", MySqlDbType.Double);
                            
                            foreach (ChiTietHoaDon ct in chiTietList)
                            {
                                cmdCT.Parameters["@idHD"].Value = hd.IdHD; // Dùng idHD của Hóa đơn cha
                                cmdCT.Parameters["@idThuoc"].Value = ct.IdThuoc;
                                cmdCT.Parameters["@soLuong"].Value = ct.SoLuong;
                                cmdCT.Parameters["@donGia"].Value = ct.DonGia;
                                cmdCT.ExecuteNonQuery();
                            }
                        }

                        trans.Commit(); // Hoàn tất giao dịch
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); // Hoàn tác nếu có lỗi
                        Console.WriteLine(ex.Message);
                        throw new Exception("Lỗi khi thêm hóa đơn và chi tiết: " + ex.Message);
                    }
                } // Transaction được giải phóng
            } // Connection được đóng
        }

        /// <summary>
        /// Lấy idKH theo idHD (Đã sửa để dùng DBConnection).
        /// </summary>
        public string GetKhachHangIdByHoaDonId(string idHD)
        {
            string sql = "SELECT idKH FROM HoaDon WHERE idHD = @idHD";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection()) // Sửa lại: Dùng DBConnection
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;
                        
                        // ExecuteScalar hiệu quả hơn khi chỉ lấy 1 giá trị
                        object result = cmd.ExecuteScalar(); 
                        
                        return result?.ToString(); // Trả về string hoặc null
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null; // không tìm thấy
        }

        /// <summary>
        /// Kiểm tra Hóa đơn có tồn tại không.
        /// </summary>
        public bool Exists(string idHD)
        {
            string sql = "SELECT COUNT(*) FROM HoaDon WHERE idHD = @idHD";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;
                        // COUNT(*) trả về kiểu long
                        long count = (long)cmd.ExecuteScalar(); 
                        return count > 0;
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Lấy các hóa đơn đã bị xóa (trong thùng rác).
        /// </summary>
        public List<HoaDon> GetDeleted()
        {
            List<HoaDon> list = new List<HoaDon>();
            string sql = "SELECT * FROM HoaDon WHERE isDeleted = 1";

            try
            {
                using (MySqlConnection con = DBConnection.GetConnection())
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HoaDon hd = new HoaDon
                            {
                                IdHD = reader["idHD"].ToString(),
                                ThoiGian = (DateTime)reader["thoiGian"],
                                IdNV = reader["idNV"].ToString(),
                                IdKH = reader["idKH"].ToString(),
                                TongTien = (double)reader["tongTien"],
                                PhuongThucThanhToan = reader["phuongThucThanhToan"] == DBNull.Value ? null : reader["phuongThucThanhToan"].ToString(),
                                TrangThaiDonHang = reader["trangThaiDonHang"].ToString()
                            };
                            list.Add(hd);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        /// <summary>
        /// Khôi phục hóa đơn từ thùng rác.
        /// </summary>
        public bool Restore(string idHD)
        {
            string sql = "UPDATE HoaDon SET isDeleted = 0 WHERE idHD = @idHD";
            try
            {
                using (MySqlConnection con = DBConnection.GetConnection())
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Xóa vĩnh viễn hóa đơn (khỏi thùng rác).
        /// </summary>
        public bool DeleteForever(string idHD)
        {
            string sql = "DELETE FROM HoaDon WHERE idHD = @idHD";
            try
            {
                using (MySqlConnection con = DBConnection.GetConnection())
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Lấy một Hóa đơn theo ID.
        /// </summary>
        public HoaDon GetById(string idHD)
        {
            HoaDon result = null;
            string sql = "SELECT * FROM HoaDon WHERE idHD = @idHD";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = new HoaDon
                                {
                                    IdHD = reader["idHD"].ToString(),
                                    ThoiGian = (DateTime)reader["thoiGian"],
                                    IdNV = reader["idNV"].ToString(),
                                    IdKH = reader["idKH"].ToString(),
                                    TongTien = (double)reader["tongTien"],
                                    PhuongThucThanhToan = reader["phuongThucThanhToan"] == DBNull.Value ? null : reader["phuongThucThanhToan"].ToString(),
                                    TrangThaiDonHang = reader["trangThaiDonHang"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật Hóa đơn và chi tiết (Xóa cũ, thêm mới chi tiết).
        /// </summary>
        public bool UpdateWithDetails(HoaDon hd, List<ChiTietHoaDon> chiTietList)
        {
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Cập nhật hóa đơn
                        string sqlUpdateHD = "UPDATE HoaDon SET thoiGian=@thoiGian, idNV=@idNV, idKH=@idKH, tongTien=@tongTien, phuongThucThanhToan=@phuongThuc, trangThaiDonHang=@trangThai WHERE idHD=@idHD";
                        using (MySqlCommand cmdUpdate = new MySqlCommand(sqlUpdateHD, conn, trans))
                        {
                            cmdUpdate.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = hd.ThoiGian;
                            cmdUpdate.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = hd.IdNV;
                            cmdUpdate.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = hd.IdKH;
                            cmdUpdate.Parameters.Add("@tongTien", MySqlDbType.Double).Value = hd.TongTien;
                            cmdUpdate.Parameters.Add("@phuongThuc", MySqlDbType.VarChar).Value = (object)hd.PhuongThucThanhToan ?? DBNull.Value;
                            cmdUpdate.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = hd.TrangThaiDonHang;
                            cmdUpdate.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = hd.IdHD;
                            cmdUpdate.ExecuteNonQuery();
                        }

                        // 2. Xóa chi tiết hóa đơn cũ
                        string sqlDeleteCT = "DELETE FROM ChiTietHoaDon WHERE idHD = @idHD";
                        using (MySqlCommand cmdDelete = new MySqlCommand(sqlDeleteCT, conn, trans))
                        {
                            cmdDelete.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = hd.IdHD;
                            cmdDelete.ExecuteNonQuery();
                        }

                        // 3. Thêm mới chi tiết hóa đơn
                        string sqlInsertCT = "INSERT INTO ChiTietHoaDon (idHD, idThuoc, soLuong, donGia) VALUES (@idHD, @idThuoc, @soLuong, @donGia)";
                        using (MySqlCommand cmdInsert = new MySqlCommand(sqlInsertCT, conn, trans))
                        {
                            // Chuẩn bị tham số
                            cmdInsert.Parameters.Add("@idHD", MySqlDbType.VarChar);
                            cmdInsert.Parameters.Add("@idThuoc", MySqlDbType.VarChar);
                            cmdInsert.Parameters.Add("@soLuong", MySqlDbType.Int32);
                            cmdInsert.Parameters.Add("@donGia", MySqlDbType.Double);

                            foreach (ChiTietHoaDon ct in chiTietList)
                            {
                                // Lưu ý: Mã Java gốc dùng ct.getIdHD(). 
                                // Đảm bảo rằng đối tượng ChiTietHoaDon trong C# cũng có IdHD chính xác.
                                cmdInsert.Parameters["@idHD"].Value = ct.IdHD; 
                                cmdInsert.Parameters["@idThuoc"].Value = ct.IdThuoc;
                                cmdInsert.Parameters["@soLuong"].Value = ct.SoLuong;
                                cmdInsert.Parameters["@donGia"].Value = ct.DonGia;
                                cmdInsert.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
        }
    }
}