using System;
using System.Data.SqlClient; // tương đương java.sql.Connection + DriverManager

namespace connectDB
{
    public static class DBConnection
    {
        // URL kết nối tới SQL Server
        // TODO: đổi server, database, user, password cho phù hợp môi trường của bạn
        private static readonly string CONNECTION_STRING =
            "Server=localhost,1433;" +                  // TODO: đổi host/port nếu khác
            "Database=QLTHUOC;" +                       // TODO: đổi tên DB nếu khác
            "User Id=sa;" +                             // TODO: đổi user
            "Password=123123;" +                        // TODO: đổi password
            "Encrypt=False;";                           // tương đương encrypt=false trong Java

        /// <summary>
        /// Trả về một SqlConnection đã mở tới database QLTHUOC.
        /// Giống như public static Connection getConnection() trong Java.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            try
            {
                var conn = new SqlConnection(CONNECTION_STRING);
                conn.Open(); // giống DriverManager.getConnection(...)
                return conn;
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine("Không thể kết nối SQL Server:");
                Console.Error.WriteLine(ex);
                throw; // ném ra cho DAO xử lý giống throws SQLException bên Java
            }
        }
    }
}
