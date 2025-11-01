using System;
using MySql.Data.MySqlClient; // Sử dụng thư viện MySQL
using Entities; // Giả định namespace chứa lớp TaiKhoan

namespace DAO
{
    /// <summary>
    /// Chứa các phương thức truy vấn bảng TaiKhoan (Phiên bản C# cho MySQL).
    /// </summary>
    public class TaiKhoanDAO
    {
        /// <summary>
        /// Kiểm tra username/password (Tối ưu bằng ExecuteScalar).
        /// </summary>
        public bool CheckLogin(string username, string password)
        {
            string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE username = @username AND password = @password";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
                        cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;

                        // ExecuteScalar trả về giá trị đầu tiên (kiểu long cho COUNT)
                        long count = (long)cmd.ExecuteScalar(); 
                        return count > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message); // Tương đương e.printStackTrace()
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin đầy đủ của một tài khoản (bao gồm idVT) theo username.
        /// Trả về TaiKhoan nếu tìm thấy, ngược lại trả về null.
        /// </summary>
        public TaiKhoan GetByUsername(string username)
        {
            string sql = "SELECT idTK, username, password, idNV, idVT FROM TaiKhoan WHERE username = @username";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TaiKhoan tk = new TaiKhoan
                                {
                                    IdTK = reader["idTK"].ToString(),
                                    Username = reader["username"].ToString(),
                                    Password = reader["password"].ToString(),
                                    IdNV = reader["idNV"].ToString(),
                                    IdVT = reader["idVT"].ToString()
                                };
                                return tk;
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
        /// Chèn mới tài khoản (nếu cần dùng từ C#).
        /// </summary>
        public bool Insert(TaiKhoan tk)
        {
            string sql = "INSERT INTO TaiKhoan (idTK, username, password, idNV, idVT) VALUES (@idTK, @username, @password, @idNV, @idVT)";
            
            try
            {
                using (MySqlConnection conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@idTK", MySqlDbType.VarChar).Value = tk.IdTK;
                        cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = tk.Username;
                        cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = tk.Password;
                        cmd.Parameters.Add("@idNV", MySqlDbType.VarChar).Value = tk.IdNV;
                        cmd.Parameters.Add("@idVT", MySqlDbType.VarChar).Value = tk.IdVT;
                        
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
    }
}