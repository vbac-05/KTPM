using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa lớp HopDong

namespace QLThuocApp.DAO
{
    public class HopDongDAO
    {
        /// <summary>
        /// Lấy toàn bộ hợp đồng chưa bị xóa
        /// </summary>
        public List<HopDong> GetAllHopDong()
        {
            List<HopDong> list = new List<HopDong>();
            string sql = @"
                SELECT idHDong, ngayBatDau, ngayKetThuc, noiDung, idNV, idNCC, trangThai 
                FROM HopDong WHERE (isDeleted IS NULL OR isDeleted = 0)";

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
                            HopDong hd = new HopDong
                            {
                                IdHDong = reader["idHDong"].ToString(),
                                NgayBatDau = (DateTime)reader["ngayBatDau"],
                                NgayKetThuc = (DateTime)reader["ngayKetThuc"],
                                NoiDung = reader["noiDung"] == DBNull.Value ? null : reader["noiDung"].ToString(),
                                IdNV = reader["idNV"] == DBNull.Value ? null : reader["idNV"].ToString(),
                                IdNCC = reader["idNCC"] == DBNull.Value ? null : reader["idNCC"].ToString(),
                                TrangThai = reader["trangThai"].ToString()
                            };
                            list.Add(hd);
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
        /// Tìm kiếm hợp đồng (chỉ hiện chưa xóa)
        /// </summary>
        public List<HopDong> SearchHopDong(string idHDong, string idNV, string idNCC)
        {
            List<HopDong> list = new List<HopDong>();
            StringBuilder sql = new StringBuilder(
                @"SELECT idHDong, ngayBatDau, ngayKetThuc, noiDung, idNV, idNCC, trangThai 
                  FROM HopDong WHERE (isDeleted IS NULL OR isDeleted = 0)"
            );

            // Xây dựng SQL động với tham số an toàn
            if (!string.IsNullOrWhiteSpace(idHDong)) {
                sql.Append(" AND idHDong LIKE @idHDong");
            }
            if (!string.IsNullOrWhiteSpace(idNV)) {
                sql.Append(" AND idNV LIKE @idNV");
            }
            if (!string.IsNullOrWhiteSpace(idNCC)) {
                sql.Append(" AND idNCC LIKE @idNCC");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn))
                    {
                        // Gán giá trị cho tham số
                        if (!string.IsNullOrWhiteSpace(idHDong)) {
                            cmd.Parameters.AddWithValue("@idHDong", $"%{idHDong.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(idNV)) {
                            cmd.Parameters.AddWithValue("@idNV", $"%{idNV.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(idNCC)) {
                            cmd.Parameters.AddWithValue("@idNCC", $"%{idNCC.Trim()}%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HopDong hd = new HopDong
                                {
                                    IdHDong = reader["idHDong"].ToString(),
                                    NgayBatDau = (DateTime)reader["ngayBatDau"],
                                    NgayKetThuc = (DateTime)reader["ngayKetThuc"],
                                    NoiDung = reader["noiDung"] == DBNull.Value ? null : reader["noiDung"].ToString(),
                                    IdNV = reader["idNV"] == DBNull.Value ? null : reader["idNV"].ToString(),
                                    IdNCC = reader["idNCC"] == DBNull.Value ? null : reader["idNCC"].ToString(),
                                    TrangThai = reader["trangThai"].ToString()
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
        /// Thêm mới hợp đồng (isDeleted = 0)
        /// </summary>
        public bool InsertHopDong(HopDong hd)
        {
            string sql = @"
                INSERT INTO HopDong (idHDong, ngayBatDau, ngayKetThuc, noiDung, idNV, idNCC, trangThai, isDeleted) 
                VALUES (@idHDong, @ngayBatDau, @ngayKetThuc, @noiDung, @idNV, @idNCC, @trangThai, 0)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHDong", MySqlDbType.VarChar).Value = hd.IdHDong;
                        cmd.Parameters.Add("@ngayBatDau", MySqlDbType.Date).Value = hd.NgayBatDau;
                        cmd.Parameters.Add("@ngayKetThuc", MySqlDbType.Date).Value = hd.NgayKetThuc;
                        // Xử lý giá trị NULL an toàn
                        cmd.Parameters.Add("@noiDung", MySqlDbType.VarChar).Value = (object)hd.NoiDung ?? DBNull.Value;
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = (object)hd.IdNV ?? DBNull.Value;
                        cmd.Parameters.Add("@idNCC", MySqlDbType.VarChar).Value = (object)hd.IdNCC ?? DBNull.Value;
                        cmd.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = hd.TrangThai;

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
        /// Cập nhật hợp đồng (không đổi isDeleted)
        /// </summary>
        public bool UpdateHopDong(HopDong hd)
        {
            string sql = @"
                UPDATE HopDong SET ngayBatDau = @ngayBatDau, ngayKetThuc = @ngayKetThuc, noiDung = @noiDung, 
                                   idNV = @idNV, idNCC = @idNCC, trangThai = @trangThai 
                WHERE idHDong = @idHDong";

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@ngayBatDau", MySqlDbType.Date).Value = hd.NgayBatDau;
                        cmd.Parameters.Add("@ngayKetThuc", MySqlDbType.Date).Value = hd.NgayKetThuc;
                        cmd.Parameters.Add("@noiDung", MySqlDbType.VarChar).Value = (object)hd.NoiDung ?? DBNull.Value;
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = (object)hd.IdNV ?? DBNull.Value;
                        cmd.Parameters.Add("@idNCC", MySqlDbType.VarChar).Value = (object)hd.IdNCC ?? DBNull.Value;
                        cmd.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = hd.TrangThai;
                        cmd.Parameters.Add("@idHDong", MySqlDbType.VarChar).Value = hd.IdHDong;

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
        /// XÓA MỀM: chỉ cập nhật isDeleted = 1
        /// </summary>
        public bool DeleteHopDong(string idHDong)
        {
            string sql = "UPDATE HopDong SET isDeleted = 1 WHERE idHDong = @idHDong";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHDong", MySqlDbType.VarChar).Value = idHDong;
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
        /// Lấy danh sách hợp đồng đã xóa (trong thùng rác)
        /// </summary>
        public List<HopDong> GetDeleted()
        {
            List<HopDong> list = new List<HopDong>();
            string sql = "SELECT * FROM HopDong WHERE isDeleted = 1";

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
                            HopDong hd = new HopDong
                            {
                                IdHDong = reader["idHDong"].ToString(),
                                NgayBatDau = (DateTime)reader["ngayBatDau"],
                                NgayKetThuc = (DateTime)reader["ngayKetThuc"],
                                NoiDung = reader["noiDung"] == DBNull.Value ? null : reader["noiDung"].ToString(),
                                IdNV = reader["idNV"] == DBNull.Value ? null : reader["idNV"].ToString(),
                                IdNCC = reader["idNCC"] == DBNull.Value ? null : reader["idNCC"].ToString(),
                                TrangThai = reader["trangThai"].ToString()
                            };
                            list.Add(hd);
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
        /// Khôi phục hợp đồng từ thùng rác
        /// </summary>
        public bool Restore(string idHDong)
        {
            string sql = "UPDATE HopDong SET isDeleted = 0 WHERE idHDong = @idHDong";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHDong", MySqlDbType.VarChar).Value = idHDong;
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
        /// Xóa vĩnh viễn hợp đồng (khỏi thùng rác)
        /// </summary>
        public bool DeleteForever(string idHDong)
        {
            string sql = "DELETE FROM HopDong WHERE idHDong = @idHDong";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHDong", MySqlDbType.VarChar).Value = idHDong;
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