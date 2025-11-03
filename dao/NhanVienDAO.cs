using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using QLThuocApp.Entities;
using QLThuocApp.ConnectDB; // Giả định namespace chứa lớp NhanVien

namespace QLThuocApp.DAO
{
    /// <summary>
    /// CRUD cho NhanVien và TaiKhoan (Phiên bản C# cho MySQL).
    /// </summary>
    public class NhanVienDAO
    {
        /// <summary>
        /// Lấy tất cả NhanVien (kèm TaiKhoan) chưa bị xóa.
        /// </summary>
        public List<NhanVien> GetAll()
        {
            List<NhanVien> list = new List<NhanVien>();
            string sql = @"
                SELECT n.idNV, n.hoTen, n.sdt, n.gioiTinh, n.namSinh, n.ngayVaoLam, 
                       n.luong, n.trangThai, n.isDeleted, t.username, t.password 
                FROM NhanVien n 
                LEFT JOIN TaiKhoan t ON n.idNV = t.idNV 
                WHERE (n.isDeleted IS NULL OR n.isDeleted = 0)";

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
                            NhanVien nv = new NhanVien
                            {
                                IdNV = reader["idNV"].ToString(),
                                HoTen = reader["hoTen"].ToString(),
                                Sdt = reader["sdt"].ToString(),
                                GioiTinh = reader["gioiTinh"].ToString(),
                                NamSinh = (int)reader["namSinh"],
                                NgayVaoLam = (DateTime)reader["ngayVaoLam"],
                                Luong = reader["luong"].ToString(),
                                TrangThai = reader["trangThai"].ToString(),
                                // Xử lý DBNull cho LEFT JOIN
                                Username = reader["username"] == DBNull.Value ? null : reader["username"].ToString(),
                                Password = reader["password"] == DBNull.Value ? null : reader["password"].ToString()
                            };
                            list.Add(nv);
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
        /// Tìm kiếm NhanVien theo idNV hoặc hoTen.
        /// </summary>
        public List<NhanVien> Search(string idNV, string hoTen)
        {
            List<NhanVien> list = new List<NhanVien>();
            StringBuilder sql = new StringBuilder(
                @"SELECT n.idNV, n.hoTen, n.sdt, n.gioiTinh, n.namSinh, n.ngayVaoLam, 
                         n.luong, n.trangThai, n.isDeleted, t.username, t.password 
                  FROM NhanVien n 
                  LEFT JOIN TaiKhoan t ON n.idNV = t.idNV 
                  WHERE (n.isDeleted IS NULL OR n.isDeleted = 0)"
            );

            if (!string.IsNullOrWhiteSpace(idNV))
            {
                sql.Append(" AND n.idNV LIKE @idNV");
            }
            if (!string.IsNullOrWhiteSpace(hoTen))
            {
                sql.Append(" AND n.hoTen LIKE @hoTen");
            }

            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn))
                    {
                        if (!string.IsNullOrWhiteSpace(idNV))
                        {
                            cmd.Parameters.AddWithValue("@idNV", $"%{idNV.Trim()}%");
                        }
                        if (!string.IsNullOrWhiteSpace(hoTen))
                        {
                            cmd.Parameters.AddWithValue("@hoTen", $"%{hoTen.Trim()}%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NhanVien nv = new NhanVien
                                {
                                    IdNV = reader["idNV"].ToString(),
                                    HoTen = reader["hoTen"].ToString(),
                                    Sdt = reader["sdt"].ToString(),
                                    GioiTinh = reader["gioiTinh"].ToString(),
                                    NamSinh = (int)reader["namSinh"],
                                    NgayVaoLam = (DateTime)reader["ngayVaoLam"],
                                    Luong = reader["luong"].ToString(),
                                    TrangThai = reader["trangThai"].ToString(),
                                    Username = reader["username"] == DBNull.Value ? null : reader["username"].ToString(),
                                    Password = reader["password"] == DBNull.Value ? null : reader["password"].ToString()
                                };
                                list.Add(nv);
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
        /// Thêm mới NhanVien và TaiKhoan (nếu có) trong một Transaction.
        /// </summary>
        public bool Insert(NhanVien nv)
        {
            string sqlNV = @"
                INSERT INTO NhanVien (idNV, hoTen, sdt, gioiTinh, namSinh, ngayVaoLam, luong, trangThai) 
                VALUES (@idNV, @hoTen, @sdt, @gioiTinh, @namSinh, @ngayVaoLam, @luong, @trangThai)";
            string sqlTK = @"
                INSERT INTO TaiKhoan (idTK, username, password, idNV, idVT) 
                VALUES (@idTK, @username, @password, @idNV, @idVT)";

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Thêm NhanVien
                        using (MySqlCommand cmdNV = new MySqlCommand(sqlNV, conn, trans))
                        {
                            cmdNV.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = nv.IdNV;
                            cmdNV.Parameters.Add("@hoTen", MySqlDbType.VarChar).Value = nv.HoTen;
                            cmdNV.Parameters.Add("@sdt", MySqlDbType.VarChar).Value = nv.Sdt;
                            cmdNV.Parameters.Add("@gioiTinh", MySqlDbType.VarChar).Value = nv.GioiTinh;
                            cmdNV.Parameters.Add("@namSinh", MySqlDbType.Int32).Value = nv.NamSinh;
                            cmdNV.Parameters.Add("@ngayVaoLam", MySqlDbType.Date).Value = nv.NgayVaoLam;
                            cmdNV.Parameters.Add("@luong", MySqlDbType.VarChar).Value = nv.Luong;
                            cmdNV.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = nv.TrangThai;
                            cmdNV.ExecuteNonQuery();
                        }

                        // 2. Thêm TaiKhoan (nếu có username)
                        if (!string.IsNullOrWhiteSpace(nv.Username))
                        {
                            using (MySqlCommand cmdTK = new MySqlCommand(sqlTK, conn, trans))
                            {
                                string idTK = "TK" + nv.IdNV;
                                cmdTK.Parameters.Add("@idTK", MySqlDbType.VarChar).Value = idTK;
                                cmdTK.Parameters.Add("@username", MySqlDbType.VarChar).Value = nv.Username;
                                cmdTK.Parameters.Add("@password", MySqlDbType.VarChar).Value = nv.Password;
                                cmdTK.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = nv.IdNV;
                                cmdTK.Parameters.Add("@idVT", MySqlDbType.VarChar).Value = string.IsNullOrWhiteSpace(nv.RoleId) ? "VT02" : nv.RoleId;
                                cmdTK.ExecuteNonQuery();
                            }
                        }

                        trans.Commit(); // Hoàn tất giao dịch
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        trans.Rollback(); // Hoàn tác
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Cập nhật NhanVien và đồng bộ TaiKhoan (Insert/Update/Delete) trong một Transaction.
        /// </summary>
        public bool Update(NhanVien nv)
        {
            string sqlNV = @"
                UPDATE NhanVien SET hoTen = @hoTen, sdt = @sdt, gioiTinh = @gioiTinh, 
                                    namSinh = @namSinh, ngayVaoLam = @ngayVaoLam, 
                                    luong = @luong, trangThai = @trangThai 
                WHERE idNV = @idNV";
            
            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Cập nhật NhanVien
                        using (MySqlCommand cmdNV = new MySqlCommand(sqlNV, conn, trans))
                        {
                            cmdNV.Parameters.Add("@hoTen", MySqlDbType.VarChar).Value = nv.HoTen;
                            cmdNV.Parameters.Add("@sdt", MySqlDbType.VarChar).Value = nv.Sdt;
                            cmdNV.Parameters.Add("@gioiTinh", MySqlDbType.VarChar).Value = nv.GioiTinh;
                            cmdNV.Parameters.Add("@namSinh", MySqlDbType.Int32).Value = nv.NamSinh;
                            cmdNV.Parameters.Add("@ngayVaoLam", MySqlDbType.Date).Value = nv.NgayVaoLam;
                            cmdNV.Parameters.Add("@luong", MySqlDbType.VarChar).Value = nv.Luong;
                            cmdNV.Parameters.Add("@trangThai", MySqlDbType.VarChar).Value = nv.TrangThai;
                            cmdNV.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = nv.IdNV;
                            cmdNV.ExecuteNonQuery();
                        }

                        // 2. Kiểm tra TaiKhoan có tồn tại không
                        string sqlCheck = "SELECT COUNT(*) FROM TaiKhoan WHERE idNV = @idNV";
                        bool existsTK;
                        using (MySqlCommand cmdCheck = new MySqlCommand(sqlCheck, conn, trans))
                        {
                            cmdCheck.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = nv.IdNV;
                            long count = (long)cmdCheck.ExecuteScalar();
                            existsTK = (count > 0);
                        }

                        // 3. Đồng bộ TaiKhoan
                        if (!string.IsNullOrWhiteSpace(nv.Username))
                        {
                            // Có username -> Phải INSERT hoặc UPDATE
                            if (existsTK)
                            {
                                // UPDATE
                                string sqlTKup = "UPDATE TaiKhoan SET username = @username, password = @password, idVT = @idVT WHERE idNV = @idNV";
                                using (MySqlCommand cmdTK = new MySqlCommand(sqlTKup, conn, trans))
                                {
                                    cmdTK.Parameters.Add("@username", MySqlDbType.VarChar).Value = nv.Username;
                                    cmdTK.Parameters.Add("@password", MySqlDbType.VarChar).Value = nv.Password;
                                    cmdTK.Parameters.Add("@idVT", MySqlDbType.VarChar).Value = string.IsNullOrWhiteSpace(nv.RoleId) ? "VT02" : nv.RoleId;
                                    cmdTK.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = nv.IdNV;
                                    cmdTK.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // INSERT
                                string sqlTKin = "INSERT INTO TaiKhoan (idTK, username, password, idNV, idVT) VALUES (@idTK, @username, @password, @idNV, @idVT)";
                                using (MySqlCommand cmdTK = new MySqlCommand(sqlTKin, conn, trans))
                                {
                                    string idTK = "TK" + nv.IdNV;
                                    cmdTK.Parameters.Add("@idTK", MySqlDbType.VarChar).Value = idTK;
                                    cmdTK.Parameters.Add("@username", MySqlDbType.VarChar).Value = nv.Username;
                                    cmdTK.Parameters.Add("@password", MySqlDbType.VarChar).Value = nv.Password;
                                    cmdTK.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = nv.IdNV;
                                    cmdTK.Parameters.Add("@idVT", MySqlDbType.VarChar).Value = string.IsNullOrWhiteSpace(nv.RoleId) ? "VT02" : nv.RoleId;
                                    cmdTK.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            // Không có username -> Phải DELETE (nếu tồn tại)
                            if (existsTK)
                            {
                                string sqlTKdel = "DELETE FROM TaiKhoan WHERE idNV = @idNV";
                                using (MySqlCommand cmdTK = new MySqlCommand(sqlTKdel, conn, trans))
                                {
                                    cmdTK.Parameters.Add("@idNV", MySqlDbType.VarCode).Value = nv.IdNV;
                                    cmdTK.ExecuteNonQuery();
                                }
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        trans.Rollback();
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Xóa mềm NhanVien (không xóa TaiKhoan).
        /// </summary>
        public bool Delete(string idNV)
        {
            string sql = "UPDATE NhanVien SET isDeleted = 1 WHERE idNV = @idNV";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = idNV;
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
        /// Lấy danh sách nhân viên đã bị xóa mềm.
        /// </summary>
        public List<NhanVien> GetDeleted()
        {
            List<NhanVien> list = new List<NhanVien>();
            // Lưu ý: Java code không join với TaiKhoan ở đây
            string sql = "SELECT * FROM NhanVien WHERE isDeleted = 1";

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
                            NhanVien nv = new NhanVien
                            {
                                IdNV = reader["idNV"].ToString(),
                                HoTen = reader["hoTen"].ToString(),
                                Sdt = reader["sdt"].ToString(),
                                GioiTinh = reader["gioiTinh"].ToString(),
                                NamSinh = (int)reader["namSinh"],
                                NgayVaoLam = (DateTime)reader["ngayVaoLam"],
                                Luong = reader["luong"].ToString(),
                                TrangThai = reader["trangThai"].ToString(),
                                IsDeleted = true
                            };
                            list.Add(nv);
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
        /// Khôi phục nhân viên từ thùng rác.
        /// </summary>
        public bool Restore(string id)
        {
            string sql = "UPDATE NhanVien SET isDeleted = 0 WHERE idNV = @idNV";
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = id;
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
        /// Xóa vĩnh viễn NhanVien và TaiKhoan liên quan.
        /// </summary>
        public bool DeleteForever(string id)
        {
            string sqlTK = "DELETE FROM TaiKhoan WHERE idNV = @idNV";
            string sqlNV = "DELETE FROM NhanVien WHERE idNV = @idNV";

            using (MySqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Xóa TaiKhoan (con) trước
                        using (MySqlCommand cmdTK = new MySqlCommand(sqlTK, conn, trans))
                        {
                            cmdTK.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = id;
                            cmdTK.ExecuteNonQuery();
                        }

                        // 2. Xóa NhanVien (cha) sau
                        using (MySqlCommand cmdNV = new MySqlCommand(sqlNV, conn, trans))
                        {
                            cmdNV.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = id;
                            int rows = cmdNV.ExecuteNonQuery();
                            
                            trans.Commit();
                            return rows > 0;
                        }
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