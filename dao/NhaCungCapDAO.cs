using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa lớp NhaCungCap

namespace QLThuocApp.DAO
{
    public class NhaCungCapDAO
    {
        /// <summary>
        /// Lấy tất cả nhà cung cấp chưa xóa
        /// </summary>
        public List<NhaCungCap> GetAll()
        {
            List<NhaCungCap> list = new List<NhaCungCap>();
            string sql = "SELECT idNCC, tenNCC, sdt, diaChi FROM NhaCungCap WHERE (isDeleted IS NULL OR isDeleted = 0)";

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
                            NhaCungCap ncc = new NhaCungCap
                            {
                                IdNCC = reader["idNCC"].ToString(),
                                TenNCC = reader["tenNCC"].ToString(),
                                Sdt = reader["sdt"].ToString(),
                                DiaChi = reader["diaChi"].ToString()
                            };
                            list.Add(ncc);
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
        /// Thêm mới NCC (isDeleted = 0)
        /// </summary>
        public bool Insert(NhaCungCap ncc)
        {
            string sql = "INSERT INTO NhaCungCap (idNCC, tenNCC, sdt, diaChi, isDeleted) VALUES (@idNCC, @tenNCC, @sdt, @diaChi, 0)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idNCC", MySqlDbType.VarChar).Value = ncc.IdNCC;
                        cmd.Parameters.Add("@tenNCC", MySqlDbType.VarChar).Value = ncc.TenNCC;
                        cmd.Parameters.Add("@sdt", MySqlDbType.VarChar).Value = ncc.Sdt;
                        cmd.Parameters.Add("@diaChi", MySqlDbType.VarChar).Value = ncc.DiaChi;
                        
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
        /// Cập nhật thông tin (trừ isDeleted)
        /// </summary>
        public bool Update(NhaCungCap ncc)
        {
            string sql = "UPDATE NhaCungCap SET tenNCC = @tenNCC, sdt = @sdt, diaChi = @diaChi WHERE idNCC = @idNCC";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@tenNCC", MySqlDbType.VarChar).Value = ncc.TenNCC;
                        cmd.Parameters.Add("@sdt", MySqlDbType.VarChar).Value = ncc.Sdt;
                        cmd.Parameters.Add("@diaChi", MySqlDbType.VarChar).Value = ncc.DiaChi;
                        cmd.Parameters.Add("@idNCC", MySqlDbType.VarChar).Value = ncc.IdNCC;
                        
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
        public bool Delete(string idNCC)
        {
            string sql = "UPDATE NhaCungCap SET isDeleted = 1 WHERE idNCC = @idNCC";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idNCC", MySqlDbType.VarChar).Value = idNCC;
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
        /// Tìm kiếm NCC theo id hoặc tên (chỉ hiện chưa xóa)
        /// </summary>
        public List<NhaCungCap> Search(string idNCC, string tenNCC)
        {
            List<NhaCungCap> list = new List<NhaCungCap>();
            StringBuilder sql = new StringBuilder(
                "SELECT idNCC, tenNCC, sdt, diaChi FROM NhaCungCap WHERE (isDeleted IS NULL OR isDeleted = 0)"
            );

            // Xây dựng SQL động
            if (!string.IsNullOrWhiteSpace(idNCC))
            {
                sql.Append(" AND idNCC LIKE @idNCC");
            }
            if (!string.IsNullOrWhiteSpace(tenNCC))
            {
                sql.Append(" AND tenNCC LIKE @tenNCC");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn))
                    {
                        // Gán tham số
                        if (!string.IsNullOrWhiteSpace(idNCC))
                        {
                            cmd.Parameters.AddWithValue("@idNCC", $"%{idNCC.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(tenNCC))
                        {
                            cmd.Parameters.AddWithValue("@tenNCC", $"%{tenNCC.Trim()}%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NhaCungCap ncc = new NhaCungCap
                                {
                                    IdNCC = reader["idNCC"].ToString(),
                                    TenNCC = reader["tenNCC"].ToString(),
                                    Sdt = reader["sdt"].ToString(),
                                    DiaChi = reader["diaChi"].ToString()
                                };
                                list.Add(ncc);
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
        /// Lấy danh sách NCC đã xóa
        /// </summary>