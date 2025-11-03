using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa lớp PhieuNhap

namespace QLThuocApp.DAO
{
    /// <summary>
    /// CRUD cơ bản cho bảng PhieuNhap (Phiên bản C# cho MySQL).
    /// </summary>
    public class PhieuNhapDAO
    {
        /// <summary>
        /// Lấy toàn bộ danh sách PhieuNhap (chưa bị xóa).
        /// </summary>
        public List<PhieuNhap> GetAll()
        {
            List<PhieuNhap> list = new List<PhieuNhap>();
            string sql = "SELECT idPN, thoiGian, idNV, idNCC, tongTien FROM PhieuNhap WHERE (isDeleted IS NULL OR isDeleted = 0)";

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
                            PhieuNhap pn = new PhieuNhap
                            {
                                IdPN = reader["idPN"].ToString(),
                                ThoiGian = (DateTime)reader["thoiGian"],
                                IdNV = reader["idNV"].ToString(),
                                IdNCC = reader["idNCC"].ToString(),
                                TongTien = (double)reader["tongTien"]
                            };
                            list.Add(pn);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message); // Tương đương e.printStackTrace()
            }
            return list;
        }

        /// <summary>
        /// Thêm mới PhieuNhap.
        /// </summary>
        public bool Insert(PhieuNhap pn)
        {
            string sql = "INSERT INTO PhieuNhap (idPN, thoiGian, idNV, idNCC, tongTien) VALUES (@idPN, @thoiGian, @idNV, @idNCC, @tongTien)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = pn.IdPN;
                        cmd.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = pn.ThoiGian;
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = pn.IdNV;
                        cmd.Parameters.Add("@idNCC", MySqlDbType.VarChar).Value = pn.IdNCC;
                        cmd.Parameters.Add("@tongTien", MySqlDbType.Double).Value = pn.TongTien;
                        
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
        /// Cập nhật PhieuNhap.
        /// </summary>
        public bool Update(PhieuNhap pn)
        {
            string sql = "UPDATE PhieuNhap SET thoiGian = @thoiGian, idNV = @idNV, idNCC = @idNCC, tongTien = @tongTien WHERE idPN = @idPN";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = pn.ThoiGian;
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = pn.IdNV;
                        cmd.Parameters.Add("@idNCC", MySqlDbType.VarChar).Value = pn.IdNCC;
                        cmd.Parameters.Add("@tongTien", MySqlDbType.Double).Value = pn.TongTien;
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = pn.IdPN;
                        
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
        /// Xóa PhieuNhap theo idPN (XÓA CỨNG - Không khuyến khích dùng nếu đã có Xóa Mềm).
        /// </summary>
        public bool Delete(string idPN)
        {
            string sql = "DELETE FROM PhieuNhap WHERE idPN = @idPN";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = idPN;
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
        /// Tìm kiếm PhieuNhap (đã sửa để lọc các phiếu chưa xóa).
        /// </summary>
        public List<PhieuNhap> Search(string idPN, string idNV, string idNCC)
        {
            List<PhieuNhap> list = new List<PhieuNhap>();
            // SỬA LỖI LOGIC: Thêm (isDeleted IS NULL OR isDeleted = 0) để đồng bộ với GetAll()
            StringBuilder sql = new StringBuilder(
                "SELECT idPN, thoiGian, idNV, idNCC, tongTien FROM PhieuNhap WHERE (isDeleted IS NULL OR isDeleted = 0)"
            );

            if (!string.IsNullOrWhiteSpace(idPN))
            {
                sql.Append(" AND idPN LIKE @idPN");
            }
            if (!string.IsNullOrWhiteSpace(idNV))
            {
                sql.Append(" AND idNV LIKE @idNV");
            }
            if (!string.IsNullOrWhiteSpace(idNCC))
            {
                sql.Append(" AND idNCC LIKE @idNCC");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn))
                    {
                        if (!string.IsNullOrWhiteSpace(idPN))
                        {
                            cmd.Parameters.AddWithValue("@idPN", $"%{idPN.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(idNV))
                        {
                            cmd.Parameters.AddWithValue("@idNV", $"%{idNV.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(idNCC))
                        {
                            cmd.Parameters.AddWithValue("@idNCC", $"%{idNCC.Trim()}%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PhieuNhap pn = new PhieuNhap
                                {
                                    IdPN = reader["idPN"].ToString(),
                                    ThoiGian = (DateTime)reader["thoiGian"],
                                    IdNV = reader["idNV"].ToString(),
                                    IdNCC = reader["idNCC"].ToString(),
                                    TongTien = (double)reader["tongTien"]
                                };
                                list.Add(pn);
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
        /// Xóa mềm PhieuNhap.
        /// </summary>
        public bool DeletePhieuNhap(string idPN)
        {
            string sql = "UPDATE PhieuNhap SET isDeleted = 1 WHERE idPN = @idPN";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = idPN;
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

        // --- Chức năng thùng rác (Trash) ---

        /// <summary>
        /// Lấy danh sách phiếu nhập đã xóa (trong thùng rác)
        /// </summary>
        public List<PhieuNhap> GetDeleted()
        {
            List<PhieuNhap> list = new List<PhieuNhap>();
            string sql = "SELECT * FROM PhieuNhap WHERE isDeleted = 1";

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
                            PhieuNhap pn = new PhieuNhap
                            {
                                IdPN = reader["idPN"].ToString(),
                                ThoiGian = (DateTime)reader["thoiGian"],
                                IdNV = reader["idNV"].ToString(),
                                IdNCC = reader["idNCC"].ToString(),
                                TongTien = (double)reader["tongTien"]
                            };
                            list.Add(pn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }

        /// <summary>
        /// Khôi phục phiếu nhập từ thùng rác
        /// </summary>
        public bool Restore(string id)
        {
            string sql = "UPDATE PhieuNhap SET isDeleted = 0 WHERE idPN = @idPN";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = id;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Xóa vĩnh viễn phiếu nhập (khỏi thùng rác)
        /// </summary>
        public bool DeleteForever(string id)
        {
            string sql = "DELETE FROM PhieuNhap WHERE idPN = @idPN";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = id;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}