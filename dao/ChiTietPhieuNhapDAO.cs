using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa lớp ChiTietPhieuNhap

namespace QLThuocApp.DAO
{
    public class ChiTietPhieuNhapDAO
    {
        /// <summary>
        /// Lấy danh sách chi tiết phiếu nhập theo idPN (dùng cho chức năng xem chi tiết)
        /// </summary>
        public List<ChiTietPhieuNhap> GetByIdPN(string idPN)
        {
            List<ChiTietPhieuNhap> list = new List<ChiTietPhieuNhap>();
            string sql = @"
                SELECT ct.idPN, ct.idThuoc, t.tenThuoc, ct.soLuong, ct.giaNhap 
                FROM ChiTietPhieuNhap ct 
                JOIN Thuoc t ON ct.idThuoc = t.idThuoc 
                WHERE ct.idPN = @idPN";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = idPN;
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ChiTietPhieuNhap ct = new ChiTietPhieuNhap();
                                ct.IdPN = reader["idPN"].ToString();
                                ct.IdThuoc = reader["idThuoc"].ToString();
                                ct.TenThuoc = reader["tenThuoc"].ToString();
                                ct.SoLuong = (int)reader["soLuong"];
                                ct.GiaNhap = (double)reader["giaNhap"];
                                list.Add(ct);
                            }
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
        /// Thêm một chi tiết phiếu nhập mới
        /// </summary>
        public bool Insert(ChiTietPhieuNhap ct)
        {
            string sql = "INSERT INTO ChiTietPhieuNhap (idPN, idThuoc, soLuong, giaNhap) VALUES (@idPN, @idThuoc, @soLuong, @giaNhap)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Giả định ct có các thuộc tính (Properties)
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = ct.IdPN;
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = ct.IdThuoc;
                        cmd.Parameters.Add("@soLuong", MySqlDbType.Int32).Value = ct.SoLuong;
                        cmd.Parameters.Add("@giaNhap", MySqlDbType.Double).Value = ct.GiaNhap;
                        
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
        /// Xóa tất cả chi tiết theo idPN (ít dùng)
        /// </summary>
        public bool DeleteByPhieuNhap(string idPN)
        {
            string sql = "DELETE FROM ChiTietPhieuNhap WHERE idPN = @idPN";
            
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
        /// Cập nhật số lượng, giá nhập chi tiết phiếu nhập
        /// </summary>
        public bool Update(ChiTietPhieuNhap ct)
        {
            string sql = "UPDATE ChiTietPhieuNhap SET soLuong = @soLuong, giaNhap = @giaNhap WHERE idPN = @idPN AND idThuoc = @idThuoc";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@soLuong", MySqlDbType.Int32).Value = ct.SoLuong;
                        cmd.Parameters.Add("@giaNhap", MySqlDbType.Double).Value = ct.GiaNhap;
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = ct.IdPN;
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = ct.IdThuoc;
                        
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
        /// Xóa một chi tiết cụ thể theo idPN và idThuoc
        /// </summary>
        public void DeleteByPhieuNhapAndThuoc(string idPN, string idThuoc)
        {
            string sql = "DELETE FROM ChiTietPhieuNhap WHERE idPN = @idPN AND idThuoc = @idThuoc";
            
            // Cấu trúc này tương đương với try-with-resources của Java
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idPN", MySqlDbType.VarChar).Value = idPN;
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = idThuoc;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}