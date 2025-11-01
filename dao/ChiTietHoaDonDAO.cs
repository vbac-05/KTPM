using System;
using System.Collections.Generic;
using System.Data;
// 1. Thay đổi thư viện: từ System.Data.SqlClient sang MySql.Data.MySqlClient
using MySql.Data.MySqlClient; 
using Entities; // Giả định bạn có namespace này cho lớp ChiTietHoaDon

namespace DAO
{
    /// <summary>
    /// DAO cho bảng dbo.ChiTietHoaDon (Phiên bản C# cho MySQL)
    /// </summary>
    public class ChiTietHoaDonDAO
    {
        /// <summary>
        /// Lấy danh sách ChiTietHoaDon (kèm tên thuốc) theo idHD.
        /// 
        /// SQL:
        ///   SELECT ct.idHD, ct.idThuoc, t.tenThuoc, ct.soLuong, ct.donGia
        ///     FROM ChiTietHoaDon ct
        ///     JOIN Thuoc t ON ct.idThuoc = t.idThuoc
        ///    WHERE ct.idHD = @idHD
        /// </summary>
        /// <param name="idHD">Mã hóa đơn cần lấy chi tiết.</param>
        /// <returns>Danh sách ChiTietHoaDon có tenThuoc.</returns>
        public List<ChiTietHoaDon> GetByIdHD(string idHD)
        {
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();
            // Câu lệnh SELECT này chuẩn SQL và chạy tốt trên MySQL
            string sql = @"
                SELECT ct.idHD, ct.idThuoc, t.tenThuoc, ct.soLuong, ct.donGia 
                FROM ChiTietHoaDon ct 
                JOIN Thuoc t ON ct.idThuoc = t.idThuoc 
                WHERE ct.idHD = @idHD";

            try
            {
                // 2. Thay đổi lớp Connection
                using (MySqlConnection conn = DBConnection.GetConnection()) // Giả định DBConnection.GetConnection() trả về MySqlConnection
                {
                    conn.Open();
                    // 3. Thay đổi lớp Command
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // 4. Thay đổi kiểu tham số (hoặc dùng AddWithValue)
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;

                        // 5. Thay đổi lớp DataReader
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ChiTietHoaDon ct = new ChiTietHoaDon();
                                ct.IdHD = reader["idHD"].ToString();
                                ct.IdThuoc = reader["idThuoc"].ToString();
                                ct.TenThuoc = reader["tenThuoc"].ToString();
                                ct.SoLuong = (int)reader["soLuong"];
                                ct.DonGia = (double)reader["donGia"];
                                
                                list.Add(ct);
                            }
                        }
                    }
                } 
            }
            catch (MySqlException ex) // 6. Thay đổi lớp Exception
            {
                Console.WriteLine(ex.Message); 
            }
            return list;
        }

        /// <summary>
        /// Lấy idThuoc đầu tiên tìm thấy theo idHD.
        /// Sử dụng ExecuteScalar để tối ưu.
        /// </summary>
        /// <param name="idHD"></param>
        /// <returns>idThuoc hoặc null nếu không tìm thấy.</returns>
        public string GetFirstIdThuocByHD(string idHD)
        {
            // 7. Thay đổi cú pháp SQL: SELECT TOP 1 (SQL Server) -> LIMIT 1 (MySQL)
            string sql = "SELECT idThuoc FROM ChiTietHoaDon WHERE idHD = @idHD LIMIT 1";
            string idThuoc = null;

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idHD", MySqlDbType.VarChar).Value = idHD;
                        
                        object result = cmd.ExecuteScalar(); 

                        if (result != null && result != DBNull.Value)
                        {
                            idThuoc = result.ToString();
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return idThuoc;
        }
    }
}