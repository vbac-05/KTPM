using System;
using MySql.Data.MySqlClient;

namespace QLThuocApp.ConnectDB
{
    public static class DBConnection
    {
        private static readonly string CONNECTION_STRING =
            "Server=localhost;" +
            "Port=3306;" +
            "Database=QLTHUOC;" +
            "User=root;" +
            "Password=123123;" +
            "SslMode=None;";

        public static MySqlConnection GetConnection()
        {
            try
            {
                var conn = new MySqlConnection(CONNECTION_STRING);
                conn.Open();
                return conn;
            }
            catch (MySqlException ex)
            {
                Console.Error.WriteLine("Không thể kết nối MySQL Server:");
                Console.Error.WriteLine($"Error Code: {ex.Number}");
                Console.Error.WriteLine($"Message: {ex.Message}");
                throw;
            }
        }
    }
}
