using System;
using Microsoft.Data.SqlClient; // TODO: Nếu đổi DBMS (MySQL / PostgreSQL), đổi namespace này cho phù hợp.

namespace connectDB
{
    public static class DBConnection
    {
        // TODO: Đổi thông tin kết nối (server, port, database, user, password) khi chuyển môi trường hoặc DB khác.
        private static readonly string CONNECTION_STRING =
            "Server=localhost,1433;" +       // TODO: đổi host và port nếu dùng máy chủ khác (VD: 192.168.1.5,3306 cho MySQL)
            "Database=QLTHUOC;" +            // TODO: đổi tên database nếu khác (VD: PharmacyDB)
            "User Id=sa;" +                  // TODO: đổi username cho phù hợp (VD: root, admin, ...)
            "Password=123123;" +             // TODO: đổi mật khẩu tương ứng
            "Encrypt=False;";                // TODO: bật Encrypt=True nếu deploy production (SSL)

        /// <summary>
        /// Trả về một SqlConnection đã mở tới database.
        /// ĐÂY là “đường ống” để DAO làm việc với DB.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            try
            {
                var conn = new SqlConnection(CONNECTION_STRING); // TODO: Nếu đổi sang MySQL, đổi sang MySqlConnection
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine("Không thể kết nối SQL Server:");
                Console.Error.WriteLine(ex);
                throw;
            }
        }
    }
}

