using System;
using System.Data.SqlClient;

namespace connectDB
{
    public static class DBCloseHelper
    {
        /// <summary>
        /// Đóng SqlDataReader nếu không null và bắt mọi Exception.
        /// Tương đương close(ResultSet rs) trong Java.
        /// </summary>
        public static void Close(SqlDataReader reader)
        {
            if (reader != null)
            {
                try
                {
                    reader.Close();
                }
                catch (Exception e) // SqlException kế thừa Exception
                {
                    Console.Error.WriteLine("Lỗi khi đóng SqlDataReader:");
                    Console.Error.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Đóng SqlCommand nếu không null.
        /// Tương đương close(Statement stmt) trong Java.
        /// </summary>
        public static void Close(SqlCommand cmd)
        {
            if (cmd != null)
            {
                try
                {
                    cmd.Dispose(); // SqlCommand không có .Close(), dùng Dispose()
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Lỗi khi đóng SqlCommand:");
                    Console.Error.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Đóng SqlConnection nếu không null và chưa đóng.
        /// Tương đương close(Connection conn) trong Java.
        /// </summary>
        public static void Close(SqlConnection conn)
        {
            if (conn != null)
            {
                try
                {
                    if (conn.State != System.Data.ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Dispose(); // giải phóng luôn
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Lỗi khi đóng SqlConnection:");
                    Console.Error.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Đóng đồng thời SqlDataReader, SqlCommand và SqlConnection.
        /// Thứ tự: Reader → Command → Connection.
        /// Tương đương closeAll(ResultSet, Statement, Connection) trong Java.
        /// </summary>
        public static void CloseAll(SqlDataReader reader, SqlCommand cmd, SqlConnection conn)
        {
            Close(reader);
            Close(cmd);
            Close(conn);
        }

        /// <summary>
        /// Đóng SqlCommand và SqlConnection.
        /// Tương đương closeAll(Statement, Connection) trong Java.
        /// </summary>
        public static void CloseAll(SqlCommand cmd, SqlConnection conn)
        {
            Close(cmd);
            Close(conn);
        }
    }
}
