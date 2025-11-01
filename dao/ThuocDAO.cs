using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using Entities; // Giả định namespace chứa lớp Thuoc

namespace DAO
{
    public class ThuocDAO
    {
        /// <summary>
        /// Lấy toàn bộ danh sách thuốc còn hiệu lực (chưa bị xóa mềm)
        /// </summary>
        public List<Thuoc> GetAllThuoc()
        {
            List<Thuoc> list = new List<Thuoc>();
            string sql = @"
                SELECT idThuoc, tenThuoc, hinhAnh, thanhPhan, donViTinh, danhMuc, xuatXu, 
                       soLuongTon, giaNhap, donGia, hanSuDung 
                FROM Thuoc WHERE isDeleted = 0";

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
                            Thuoc t = MapThuocFromReader(reader);
                            list.Add(t);
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
        /// Tìm kiếm thuốc theo ID hoặc tên (lọc ra thuốc chưa bị xóa)
        /// </summary>
        public List<Thuoc> SearchThuoc(string idThuoc, string tenThuoc)
        {
            List<Thuoc> list = new List<Thuoc>();
            StringBuilder sql = new StringBuilder(
                @"SELECT idThuoc, tenThuoc, hinhAnh, thanhPhan, donViTinh, danhMuc, xuatXu, 
                         soLuongTon, giaNhap, donGia, hanSuDung 
                  FROM Thuoc WHERE isDeleted = 0"
            );

            if (!string.IsNullOrWhiteSpace(idThuoc))
            {
                sql.Append(" AND idThuoc LIKE @idThuoc");
            }
            if (!string.IsNullOrWhiteSpace(tenThuoc))
            {
                sql.Append(" AND tenThuoc LIKE @tenThuoc");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn))
                    {
                        if (!string.IsNullOrWhiteSpace(idThuoc))
                        {
                            cmd.Parameters.AddWithValue("@idThuoc", $"%{idThuoc.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(tenThuoc))
                        {
                            cmd.Parameters.AddWithValue("@tenThuoc", $"%{tenThuoc.Trim()}%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Thuoc t = MapThuocFromReader(reader);
                                list.Add(t);
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
        /// Thêm thuốc mới
        /// </summary>
        public bool InsertThuoc(Thuoc t)
        {
            string sql = @"
                INSERT INTO Thuoc 
                (idThuoc, tenThuoc, hinhAnh, thanhPhan, donViTinh, danhMuc, xuatXu, 
                 soLuongTon, giaNhap, donGia, hanSuDung, isDeleted) 
                VALUES (@idThuoc, @tenThuoc, @hinhAnh, @thanhPhan, @donViTinh, @danhMuc, @xuatXu, 
                        @soLuongTon, @giaNhap, @donGia, @hanSuDung, 0)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = t.IdThuoc;
                        cmd.Parameters.Add("@tenThuoc", MySqlDbType.VarChar).Value = t.TenThuoc;
                        cmd.Parameters.Add("@hinhAnh", MySqlDbType.Blob).Value = (object)t.HinhAnh ?? DBNull.Value;
                        cmd.Parameters.Add("@thanhPhan", MySqlDbType.VarChar).Value = (object)t.ThanhPhan ?? DBNull.Value;
                        cmd.Parameters.Add("@donViTinh", MySqlDbType.VarChar).Value = t.DonViTinh;
                        cmd.Parameters.Add("@danhMuc", MySqlDbType.VarChar).Value = t.DanhMuc;
                        cmd.Parameters.Add("@xuatXu", MySqlDbType.VarChar).Value = t.XuatXu;
                        cmd.Parameters.Add("@soLuongTon", MySqlDbType.Int32).Value = t.SoLuongTon;
                        cmd.Parameters.Add("@giaNhap", MySqlDbType.Double).Value = t.GiaNhap;
                        cmd.Parameters.Add("@donGia", MySqlDbType.Double).Value = t.DonGia;
                        cmd.Parameters.Add("@hanSuDung", MySqlDbType.Date).Value = t.HanSuDung;

                        return cmd.ExecuteNonQuery() > 0;
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
        /// Cập nhật thuốc (phiên bản C# của hàm update thứ 2, có cả isDeleted)
        /// </summary>
        public bool UpdateThuoc(Thuoc t)
        {
            string sql = @"
                UPDATE Thuoc SET 
                tenThuoc = @tenThuoc, hinhAnh = @hinhAnh, thanhPhan = @thanhPhan, 
                donViTinh = @donViTinh, danhMuc = @danhMuc, xuatXu = @xuatXu, 
                soLuongTon = @soLuongTon, giaNhap = @giaNhap, donGia = @donGia, 
                hanSuDung = @hanSuDung, isDeleted = @isDeleted 
                WHERE idThuoc = @idThuoc";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@tenThuoc", MySqlDbType.VarChar).Value = t.TenThuoc;
                        cmd.Parameters.Add("@hinhAnh", MySqlDbType.Blob).Value = (object)t.HinhAnh ?? DBNull.Value;
                        cmd.Parameters.Add("@thanhPhan", MySqlDbType.VarChar).Value = (object)t.ThanhPhan ?? DBNull.Value;
                        cmd.Parameters.Add("@donViTinh", MySqlDbType.VarChar).Value = t.DonViTinh;
                        cmd.Parameters.Add("@danhMuc", MySqlDbType.VarChar).Value = t.DanhMuc;
                        cmd.Parameters.Add("@xuatXu", MySqlDbType.VarChar).Value = t.XuatXu;
                        cmd.Parameters.Add("@soLuongTon", MySqlDbType.Int32).Value = t.SoLuongTon;
                        cmd.Parameters.Add("@giaNhap", MySqlDbType.Double).Value = t.GiaNhap;
                        cmd.Parameters.Add("@donGia", MySqlDbType.Double).Value = t.DonGia;
                        cmd.Parameters.Add("@hanSuDung", MySqlDbType.Date).Value = t.HanSuDung;
                        
                        // Giả định IsDeleted là kiểu bool? (nullable bool) trong C#
                        cmd.Parameters.Add("@isDeleted", MySqlDbType.Bit).Value = (object)t.IsDeleted ?? DBNull.Value; 
                        
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = t.IdThuoc;

                        return cmd.ExecuteNonQuery() > 0;
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
        /// Xóa mềm thuốc (isDeleted = 1)
        /// </summary>
        public bool DeleteThuoc(string idThuoc)
        {
            string sql = "UPDATE Thuoc SET isDeleted = 1 WHERE idThuoc = @idThuoc";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = idThuoc;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Lỗi khi xóa mềm thuốc: " + ex.Message);
            }
        }

        /// <summary>
        /// Phục hồi thuốc đã bị xóa mềm (isDeleted = 0)
        /// </summary>
        public bool RestoreThuoc(string idThuoc)
        {
            string sql = "UPDATE Thuoc SET isDeleted = 0 WHERE idThuoc = @idThuoc";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = idThuoc;
                        return cmd.ExecuteNonQuery() > 0;
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
        /// Lấy thuốc theo id (chỉ thuốc chưa bị xóa)
        /// </summary>
        public Thuoc GetById(string idThuoc)
        {
            string sql = @"
                SELECT idThuoc, tenThuoc, hinhAnh, thanhPhan, donViTinh, danhMuc, xuatXu, 
                       soLuongTon, giaNhap, donGia, hanSuDung 
                FROM Thuoc WHERE idThuoc = @idThuoc AND isDeleted = 0";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = idThuoc;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapThuocFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        // --- Chức năng thùng rác (Trash) ---

        /// <summary>
        /// Lấy danh sách thuốc đã xóa (trong thùng rác)
        /// </summary>
        public List<Thuoc> GetDeleted()
        {
            List<Thuoc> list = new List<Thuoc>();
            string sql = "SELECT * FROM Thuoc WHERE isDeleted = 1";

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
                            // *Lưu ý: hàm MapThuocFromReader giả định isDeleted=0, 
                            // nên chúng ta map thủ công ở đây nếu cần lấy cả trường isDeleted
                            Thuoc t = new Thuoc
                            {
                                IdThuoc = reader["idThuoc"].ToString(),
                                TenThuoc = reader["tenThuoc"].ToString(),
                                HinhAnh = reader["hinhAnh"] == DBNull.Value ? null : (byte[])reader["hinhAnh"],
                                ThanhPhan = reader["thanhPhan"] == DBNull.Value ? null : reader["thanhPhan"].ToString(),
                                DonViTinh = reader["donViTinh"].ToString(),
                                DanhMuc = reader["danhMuc"].ToString(),
                                XuatXu = reader["xuatXu"].ToString(),
                                SoLuongTon = (int)reader["soLuongTon"],
                                GiaNhap = (double)reader["giaNhap"],
                                DonGia = (double)reader["donGia"],
                                HanSuDung = (DateTime)reader["hanSuDung"],
                                IsDeleted = true // Đánh dấu là đã xóa
                            };
                            list.Add(t);
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
        /// Khôi phục thuốc (trùng lặp với restoreThuoc)
        /// </summary>
        public bool Restore(string id)
        {
            string sql = "UPDATE Thuoc SET isDeleted = 0 WHERE idThuoc = @idThuoc";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = id;
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
        /// Xóa vĩnh viễn thuốc. (ĐÃ SỬA LỖI: Java code xóa từ PhieuNhap)
        /// </summary>
        public bool DeleteForever(string id)
        {
            // SỬA LỖI: Tệp Java gốc xóa từ 'PhieuNhap'. 
            // Đã sửa lại để xóa từ 'Thuoc' cho đúng ngữ cảnh.
            string sql = "DELETE FROM Thuoc WHERE idThuoc = @idThuoc"; 
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = id;
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
        /// Giảm số lượng tồn kho (atomic update)
        /// </summary>
        public bool GiamSoLuong(string idThuoc, int soLuongGiam)
        {
            string sql = "UPDATE Thuoc SET SoLuongTon = SoLuongTon - @soLuongGiam WHERE idThuoc = @idThuoc AND SoLuongTon >= @soLuongGiam";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@soLuongGiam", MySqlDbType.Int32).Value = soLuongGiam;
                        cmd.Parameters.Add("@idThuoc", MySqlDbType.VarChar).Value = idThuoc;
                        // Tham số @soLuongGiam thứ 2 sẽ được MySqlConnector tự động sử dụng lại
                        
                        int affected = cmd.ExecuteNonQuery();
                        
                        // Debugging (tương đương System.out.println)
                        Console.WriteLine($"SQL: {cmd.CommandText}");
                        Console.WriteLine($"idThuoc: {idThuoc}, soLuongGiam: {soLuongGiam}, affected: {affected}");
                        
                        return affected > 0;
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
        /// Phương thức trợ giúp (helper) để map dữ liệu từ Reader sang đối tượng Thuoc
        /// </summary>
        private Thuoc MapThuocFromReader(MySqlDataReader reader)
        {
            return new Thuoc
            {
                IdThuoc = reader["idThuoc"].ToString(),
                TenThuoc = reader["tenThuoc"].ToString(),
                // Xử lý cẩn thận các trường có thể NULL
                HinhAnh = reader["hinhAnh"] == DBNull.Value ? null : (byte[])reader["hinhAnh"],
                ThanhPhan = reader["thanhPhan"] == DBNull.Value ? null : reader["thanhPhan"].ToString(),
                DonViTinh = reader["donViTinh"].ToString(),
                DanhMuc = reader["danhMuc"].ToString(),
                XuatXu = reader["xuatXu"].ToString(),
                SoLuongTon = (int)reader["soLuongTon"],
                GiaNhap = (double)reader["giaNhap"],
                DonGia = (double)reader["donGia"],
                HanSuDung = (DateTime)reader["hanSuDung"],
                IsDeleted = false // Vì các hàm select chính đều có "WHERE isDeleted = 0"
            };
        }
    }
}