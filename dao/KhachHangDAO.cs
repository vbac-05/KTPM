using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa lớp KhachHang

namespace QLThuocApp.DAO
{
    /// <summary>
    /// CRUD cơ bản cho bảng KhachHang (Phiên bản C# cho MySQL).
    /// </summary>
    public class KhachHangDAO
    {
        /// <summary>
        /// Lấy toàn bộ danh sách KhachHang (chưa bị xóa).
        /// </summary>
        public List<KhachHang> GetAll()
        {
            List<KhachHang> list = new List<KhachHang>();
            string sql = "SELECT idKH, hoTen, sdt, gioiTinh, ngayThamGia FROM KhachHang WHERE (isDeleted IS NULL OR isDeleted = 0)";

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
                            KhachHang kh = new KhachHang
                            {
                                IdKH = reader["idKH"].ToString(),
                                HoTen = reader["hoTen"].ToString(),
                                Sdt = reader["sdt"].ToString(),
                                GioiTinh = reader["gioiTinh"].ToString(),
                                NgayThamGia = (DateTime)reader["ngayThamGia"]
                            };
                            list.Add(kh);
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
        /// Thêm mới KhachHang (isDeleted = 0).
        /// </summary>
        public bool Insert(KhachHang kh)
        {
            string sql = "INSERT INTO KhachHang (idKH, hoTen, sdt, gioiTinh, ngayThamGia, isDeleted) VALUES (@idKH, @hoTen, @sdt, @gioiTinh, @ngayThamGia, 0)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = kh.IdKH;
                        cmd.Parameters.Add("@hoTen", MySqlDbType.VarChar).Value = kh.HoTen;
                        cmd.Parameters.Add("@sdt", MySqlDbType.VarChar).Value = kh.Sdt;
                        cmd.Parameters.Add("@gioiTinh", MySqlDbType.VarChar).Value = kh.GioiTinh;
                        cmd.Parameters.Add("@ngayThamGia", MySqlDbType.Date).Value = kh.NgayThamGia;
                        
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Trùng PK (tương đương 2627 của SQL Server)
                {
                    throw new Exception("ID khách hàng đã tồn tại!");
                }
                Console.WriteLine(ex.Message);
                throw new Exception("Lỗi SQL khi thêm khách hàng: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật KhachHang.
        /// </summary>
        public bool Update(KhachHang kh)
        {
            string sql = "UPDATE KhachHang SET hoTen = @hoTen, sdt = @sdt, gioiTinh = @gioiTinh, ngayThamGia = @ngayThamGia WHERE idKH = @idKH";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@hoTen", MySqlDbType.VarChar).Value = kh.HoTen;
                        cmd.Parameters.Add("@sdt", MySqlDbType.VarChar).Value = kh.Sdt;
                        cmd.Parameters.Add("@gioiTinh", MySqlDbType.VarChar).Value = kh.GioiTinh;
                        cmd.Parameters.Add("@ngayThamGia", MySqlDbType.Date).Value = kh.NgayThamGia;
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = kh.IdKH;
                        
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Lỗi SQL khi cập nhật khách hàng: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa mềm KhachHang theo idKH.
        /// </summary>
        public bool Delete(string idKH)
        {
            string sql = "UPDATE KhachHang SET isDeleted = 1 WHERE idKH = @idKH";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = idKH;
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451) // Lỗi FK (tương đương 547 của SQL Server)
                {
                    throw new Exception("Không thể xóa vì khách hàng đã có hóa đơn liên quan!");
                }
                Console.WriteLine(ex.Message);
                throw new Exception("Lỗi SQL khi xóa khách hàng: " + ex.Message);
            }
        }

        /// <summary>
        /// Tìm kiếm Khách hàng theo hoTen hoặc sdt (hoặc cả hai).
        /// </summary>
        public List<KhachHang> Search(string hoTen, string sdt)
        {
            List<KhachHang> list = new List<KhachHang>();
            StringBuilder sql = new StringBuilder(
                "SELECT idKH, hoTen, sdt, gioiTinh, ngayThamGia FROM KhachHang WHERE (isDeleted IS NULL OR isDeleted = 0)"
            );

            // Xây dựng SQL động
            if (!string.IsNullOrWhiteSpace(hoTen))
            {
                sql.Append(" AND hoTen LIKE @hoTen");
            }
            if (!string.IsNullOrWhiteSpace(sdt))
            {
                sql.Append(" AND sdt LIKE @sdt");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn))
                    {
                        // Gán tham số
                        if (!string.IsNullOrWhiteSpace(hoTen))
                        {
                            cmd.Parameters.AddWithValue("@hoTen", $"%{hoTen.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(sdt))
                        {
                            cmd.Parameters.AddWithValue("@sdt", $"%{sdt.Trim()}%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                KhachHang kh = new KhachHang
                                {
                                    IdKH = reader["idKH"].ToString(),
                                    HoTen = reader["hoTen"].ToString(),
                                    Sdt = reader["sdt"].ToString(),
                                    GioiTinh = reader["gioiTinh"].ToString(),
                                    NgayThamGia = (DateTime)reader["ngayThamGia"]
                                };
                                list.Add(kh);
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
        /// Lấy KhachHang theo SĐT.
        /// </summary>
        public KhachHang GetBySDT(string sdt)
        {
            string sql = "SELECT idKH, hoTen, sdt, gioiTinh, ngayThamGia FROM KhachHang WHERE sdt = @sdt AND (isDeleted IS NULL OR isDeleted = 0)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@sdt", MySqlDbType.VarChar).Value = sdt;
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                KhachHang kh = new KhachHang
                                {
                                    IdKH = reader["idKH"].ToString(),
                                    HoTen = reader["hoTen"].ToString(),
                                    Sdt = reader["sdt"].ToString(),
                                    GioiTinh = reader["gioiTinh"].ToString(),
                                    NgayThamGia = (DateTime)reader["ngayThamGia"]
                                };
                                return kh;
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

        /// <summary>
        /// Cập nhật điểm tích lũy (ghi đè).
        /// </summary>
        public bool UpdateDiemTichLuy(string idKH, int diemMoi)
        {
            string sql = "UPDATE KhachHang SET diemTichLuy = @diemMoi WHERE idKH = @idKH";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@diemMoi", MySqlDbType.Int32).Value = diemMoi;
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = idKH;
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
        /// Cộng dồn điểm tích lũy.
        /// </summary>
        public bool CongDiem(string idKH, int soDiemCong)
        {
            string sql = "UPDATE KhachHang SET diemTichLuy = diemTichLuy + @soDiemCong WHERE idKH = @idKH";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@soDiemCong", MySqlDbType.Int32).Value = soDiemCong;
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = idKH;
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
        /// Trừ điểm tích lũy (nếu đủ điểm).
        /// </summary>
        public bool TruDiem(string idKH, int soDiemTru)
        {
            // Đảm bảo trừ an toàn (không bị âm)
            string sql = "UPDATE KhachHang SET diemTichLuy = diemTichLuy - @soDiemTru WHERE idKH = @idKH AND diemTichLuy >= @soDiemTru";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@soDiemTru", MySqlDbType.Int32).Value = soDiemTru;
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = idKH;
                        // Tham số @soDiemTru được dùng ở 2 nơi, provider sẽ xử lý
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
        /// Lấy KhachHang theo ID (kèm điểm tích lũy).
        /// </summary>
        public KhachHang GetById(string idKH)
        {
            string sql = "SELECT * FROM KhachHang WHERE idKH = @idKH";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = idKH;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                KhachHang kh = new KhachHang
                                {
                                    IdKH = reader["idKH"].ToString(),
                                    HoTen = reader["hoTen"].ToString(),
                                    Sdt = reader["sdt"].ToString(),
                                    GioiTinh = reader["gioiTinh"].ToString(),
                                    NgayThamGia = (DateTime)reader["ngayThamGia"],
                                    DiemTichLuy = (int)reader["diemTichLuy"]
                                };
                                return kh;
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
        /// Lấy danh sách khách hàng đã xóa
        /// </summary>
        public List<KhachHang> GetDeleted()
        {
            List<KhachHang> list = new List<KhachHang>();
            string sql = "SELECT * FROM KhachHang WHERE isDeleted = 1";

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
                            KhachHang kh = new KhachHang
                            {
                                IdKH = reader["idKH"].ToString(),
                                HoTen = reader["hoTen"].ToString(),
                                Sdt = reader["sdt"].ToString(),
                                GioiTinh = reader["gioiTinh"].ToString(),
                                NgayThamGia = (DateTime)reader["ngayThamGia"]
                            };
                            list.Add(kh);
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
        /// Khôi phục khách hàng từ thùng rác
        /// </summary>
        public bool Restore(string idKH)
        {
            string sql = "UPDATE KhachHang SET isDeleted = 0 WHERE idKH = @idKH";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = idKH;
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
        /// Xóa vĩnh viễn khách hàng (khỏi thùng rác)
        /// </summary>
        public bool DeleteForever(string idKH)
        {
            string sql = "DELETE FROM KhachHang WHERE idKH = @idKH";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idKH", MySqlDbType.VarChar).Value = idKH;
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