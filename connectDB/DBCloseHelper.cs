using System;
using Microsoft.Data.SqlClient; // TODO: Nếu đổi DBMS, đổi sang MySql.Data.MySqlClient hoặc tương ứng.

namespace connectDB
{
    public static class DBCloseHelper
    {
        /// <summary>
        /// Đóng SqlDataReader nếu không null và bắt lỗi.
        /// </summary>
        public static void Close(SqlDataReader reader) // TODO: Nếu đổi DBMS, đổi sang MySqlDataReader hoặc tương ứng.
        {
            if (reader != null)
            {
                try
                {
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Lỗi khi đóng SqlDataReader:");
                    Console.Error.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Đóng SqlCommand nếu không null.
        /// </summary>
        public static void Close(SqlCommand cmd) // TODO: Nếu đổi DBMS, đổi sang MySqlCommand hoặc tương ứng.
        {
            if (cmd != null)
            {
                try
                {
                    cmd.Dispose(); // TODO: Giữ nguyên; MySqlCommand cũng hỗ trợ Dispose().
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
        /// </summary>
        public static void Close(SqlConnection conn) // TODO: Nếu đổi DBMS, đổi sang MySqlConnection hoặc tương ứng.
        {
            if (conn != null)
            {
                try
                {
                    if (conn.State != System.Data.ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Dispose();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Lỗi khi đóng SqlConnection:");
                    Console.Error.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Đóng đồng thời Reader → Command → Connection.
        /// </summary>
        public static void CloseAll(SqlDataReader reader, SqlCommand cmd, SqlConnection conn) // TODO: Nếu đổi DBMS, sửa tất cả kiểu tương ứng.
        {
            Close(reader);
            Close(cmd);
            Close(conn);
        }

        /// <summary>
        /// Đóng Command → Connection.
        /// </summary>
        public static void CloseAll(SqlCommand cmd, SqlConnection conn) // TODO: Nếu đổi DBMS, sửa tất cả kiểu tương ứng.
        {
            Close(cmd);
            Close(conn);
        }
    }
}

