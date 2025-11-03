using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa lớp PhanHoi

namespace QLThuocApp.DAO
{
    public class PhanHoiDAO
    {
        /// <summary>
        /// Lấy tất cả phản hồi chưa bị xóa mềm
        /// </summary>
        public List<PhanHoi> GetAll()
        {
            List<PhanHoi> list = new List<PhanHoi>();
            string sql = "SELECT idPH, idKH, idHD, noiDung, thoiGian, danhGia FROM PhanHoi WHERE (isDeleted IS NULL OR isDeleted = 0)";

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
                            PhanHoi ph = new PhanHoi
                            {
                                IdPH = reader["idPH"].ToString(),
                                IdKH = reader["idKH"].ToString(),
                                IdHD = reader["idHD"].ToString(),
                                NoiDung = reader["noiDung"].ToString(),
                                ThoiGian = (DateTime)reader["thoiGian"],
                                DanhGia = (int)reader["danhGia"]
                            };
                            list.Add(ph);
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
        /// Thêm phản hồi mới (set mặc định isDeleted = 0)
        /// </summary>
        public bool Insert(PhanHoi ph)
        {
            string sql = "INSERT INTO PhanHoi (idPH, idKH, idHD, noiDung, thoiGian, danhGia, isDeleted) VALUES (@idPH, @idKH, @idHD, @noiDung, @thoiGian, @danhGia, 0)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPH", MySqlDbType.VarChar).Value = ph.IdPH;
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = ph.IdKH;
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = ph.IdHD;
                        cmd.Parameters.Add("@noiDung", MySqlDbType.VarChar).Value = ph.NoiDung;
                        cmd.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = ph.ThoiGian;
                        cmd.Parameters.Add("@danhGia", MySqlDbType.Int32).Value = ph.DanhGia;
                        
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
        /// Cập nhật phản hồi (không cho sửa isDeleted)
        /// </summary>
        public bool Update(PhanHoi ph)
        {
            string sql = "UPDATE PhanHoi SET idKH = @idKH, idHD = @idHD, noiDung = @noiDung, thoiGian = @thoiGian, danhGia = @danhGia WHERE idPH = @idPH";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = ph.IdKH;
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = ph.IdHD;
                        cmd.Parameters.Add("@noiDung", MySqlDbType.VarChar).Value = ph.NoiDung;
                        cmd.Parameters.Add("@thoiGian", MySqlDbType.DateTime).Value = ph.ThoiGian;
                        cmd.Parameters.Add("@danhGia", MySqlDbType.Int32).Value = ph.DanhGia;
                        cmd.Parameters.Add("@idPH", MySqlDbType.VarChar).Value = ph.IdPH;
                        
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
        /// XÓA MỀM: cập nhật isDeleted = 1
        /// </summary>
        public bool Delete(string idPH)
        {
            string sql = "UPDATE PhanHoi SET isDeleted = 1 WHERE idPH = @idPH";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPH", MySqlDbType.VarChar).Value = idPH;
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
        /// Tìm kiếm phản hồi (lọc chưa xóa)
        /// </summary>
        public List<PhanHoi> Search(string idPH, string idKH)
        {
            List<PhanHoi> list = new List<PhanHoi>();
            StringBuilder sql = new StringBuilder(
                "SELECT idPH, idKH, idHD, noiDung, thoiGian, danhGia FROM PhanHoi WHERE (isDeleted IS NULL OR isDeleted = 0)"
            );

            if (!string.IsNullOrWhiteSpace(idPH))
            {
                sql.Append(" AND idPH LIKE @idPH");
            }
            if (!string.IsNullOrWhiteSpace(idKH))
            {
                sql.Append(" AND idKH LIKE @idKH");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn))
                    {
                        if (!string.IsNullOrWhiteSpace(idPH))
                        {
                            cmd.Parameters.AddWithValue("@idPH", $"%{idPH.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(idKH))
                        {
                            cmd.Parameters.AddWithValue("@idKH", $"%{idKH.Trim()}%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PhanHoi ph = new PhanHoi
                                {
                                    IdPH = reader["idPH"].ToString(),
                                    IdKH = reader["idKH"].ToString(),
                                    IdHD = reader["idHD"].ToString(),
                                    NoiDung = reader["noiDung"].ToString(),
                                    ThoiGian = (DateTime)reader["thoiGian"],
                                    DanhGia = (int)reader["danhGia"]
                                };
                                list.Add(ph);
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

        // --- Chức năng thùng rác (Trash) ---

        /// <summary>
        /// Lấy danh sách phản hồi đã xóa
        /// </summary>
        public List<PhanHoi> GetDeleted()
        {
            List<PhanHoi> list = new List<PhanHoi>();
            string sql = "SELECT * FROM PhanHoi WHERE isDeleted = 1";

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
                            PhanHoi p = new PhanHoi
                            {
                                IdPH = reader["idPH"].ToString(),
                                IdKH = reader["idKH"].ToString(),
                                IdHD = reader["idHD"].ToString(),
                                NoiDung = reader["noiDung"].ToString(),
                                ThoiGian = (DateTime)reader["thoiGian"],
                                DanhGia = (int)reader["danhGia"]
                            };
                            list.Add(p);
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
        /// Khôi phục phản hồi từ thùng rác
        /// </summary>
        public bool Restore(string idPH)
        {
            string sql = "UPDATE PhanHoi SET isDeleted = 0 WHERE idPH = @idPH";
            
            try
            {
                // Thay thế hoàn toàn khối try-catch-finally thủ công bằng 'using'
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPH", MySqlDbType.VarChar).Value = idPH;
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
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
        /// Xóa vĩnh viễn phản hồi (khỏi thùng rác)
        /// </summary>
        public bool DeleteForever(string idPH)
        {
            string sql = "DELETE FROM PhanHoi WHERE idPH = @idPH";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPH", MySqlDbType.VarChar).Value = idPH;
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