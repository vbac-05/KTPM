using System;using System;using System;

using MySql.Data.MySqlClient;

using MySql.Data.MySqlClient;using Microsoft.Data.SqlClient; // TODO: Nếu đổi DBMS, đổi sang MySql.Data.MySqlClient hoặc tương ứng.

namespace QLThuocApp.ConnectDB

{

    /// <summary>

    /// Helper class để đóng các resource của MySQLnamespace QLThuocApp.ConnectDBnamespace QLThuocApp.ConnectDB

    /// LƯU Ý: Class này không thực sự cần thiết khi dùng 'using' statement

    /// Giữ lại để tương thích với code cũ, nhưng khuyến nghị dùng 'using' thay thế{{

    /// </summary>

    public static class DBCloseHelper    /// <summary>    public static class DBCloseHelper

    {

        /// <summary>    /// Helper class để đóng các resource của MySQL    {

        /// Đóng MySqlDataReader nếu không null

        /// </summary>    /// LƯU Ý: Class này không thực sự cần thiết khi dùng 'using' statement        /// <summary>

        public static void Close(MySqlDataReader reader)

        {    /// Giữ lại để tương thích với code cũ, nhưng khuyến nghị dùng 'using' thay thế        /// Đóng SqlDataReader nếu không null và bắt lỗi.

            if (reader != null)

            {    /// </summary>        /// </summary>

                try

                {    public static class DBCloseHelper        public static void Close(SqlDataReader reader) // TODO: Nếu đổi DBMS, đổi sang MySqlDataReader hoặc tương ứng.

                    reader.Close();

                }    {        {

                catch (Exception e)

                {        /// <summary>            if (reader != null)

                    Console.Error.WriteLine("Lỗi khi đóng MySqlDataReader:");

                    Console.Error.WriteLine(e);        /// Đóng MySqlDataReader nếu không null và bắt lỗi.            {

                }

            }        /// </summary>                try

        }

        public static void Close(MySqlDataReader reader)                {

        /// <summary>

        /// Đóng MySqlCommand nếu không null        {                    reader.Close();

        /// </summary>

        public static void Close(MySqlCommand cmd)            if (reader != null)                }

        {

            if (cmd != null)            {                catch (Exception e)

            {

                try                try                {

                {

                    cmd.Dispose();                {                    Console.Error.WriteLine("Lỗi khi đóng SqlDataReader:");

                }

                catch (Exception e)                    reader.Close();                    Console.Error.WriteLine(e);

                {

                    Console.Error.WriteLine("Lỗi khi đóng MySqlCommand:");                }                }

                    Console.Error.WriteLine(e);

                }                catch (Exception e)            }

            }

        }                {        }



        /// <summary>                    Console.Error.WriteLine("Lỗi khi đóng MySqlDataReader:");

        /// Đóng MySqlConnection nếu không null và chưa đóng

        /// </summary>                    Console.Error.WriteLine(e);        /// <summary>

        public static void Close(MySqlConnection conn)

        {                }        /// Đóng SqlCommand nếu không null.

            if (conn != null)

            {            }        /// </summary>

                try

                {        }        public static void Close(SqlCommand cmd) // TODO: Nếu đổi DBMS, đổi sang MySqlCommand hoặc tương ứng.

                    if (conn.State != System.Data.ConnectionState.Closed)

                    {        {

                        conn.Close();

                    }        /// <summary>            if (cmd != null)

                }

                catch (Exception e)        /// Đóng MySqlCommand nếu không null.            {

                {

                    Console.Error.WriteLine("Lỗi khi đóng MySqlConnection:");        /// </summary>                try

                    Console.Error.WriteLine(e);

                }        public static void Close(MySqlCommand cmd)                {

            }

        }        {                    cmd.Dispose(); // TODO: Giữ nguyên; MySqlCommand cũng hỗ trợ Dispose().



        /// <summary>            if (cmd != null)                }

        /// Đóng tất cả resources (reader, command, connection)

        /// </summary>            {                catch (Exception e)

        public static void CloseAll(MySqlDataReader reader, MySqlCommand cmd, MySqlConnection conn)

        {                try                {

            Close(reader);

            Close(cmd);                {                    Console.Error.WriteLine("Lỗi khi đóng SqlCommand:");

            Close(conn);

        }                    cmd.Dispose();                    Console.Error.WriteLine(e);



        /// <summary>                }                }

        /// Đóng command và connection

        /// </summary>                catch (Exception e)            }

        public static void CloseAll(MySqlCommand cmd, MySqlConnection conn)

        {                {        }

            Close(cmd);

            Close(conn);                    Console.Error.WriteLine("Lỗi khi đóng MySqlCommand:");

        }

    }                    Console.Error.WriteLine(e);        /// <summary>

}

                }        /// Đóng SqlConnection nếu không null và chưa đóng.

            }        /// </summary>

        }        public static void Close(SqlConnection conn) // TODO: Nếu đổi DBMS, đổi sang MySqlConnection hoặc tương ứng.

        {

        /// <summary>            if (conn != null)

        /// Đóng MySqlConnection nếu không null và chưa đóng.            {

        /// </summary>                try

        public static void Close(MySqlConnection conn)                {

        {                    if (conn.State != System.Data.ConnectionState.Closed)

            if (conn != null)                    {

            {                        conn.Close();

                try                    }

                {                    conn.Dispose();

                    if (conn.State != System.Data.ConnectionState.Closed)                }

                    {                catch (Exception e)

                        conn.Close();                {

                    }                    Console.Error.WriteLine("Lỗi khi đóng SqlConnection:");

                }                    Console.Error.WriteLine(e);

                catch (Exception e)                }

                {            }

                    Console.Error.WriteLine("Lỗi khi đóng MySqlConnection:");        }

                    Console.Error.WriteLine(e);

                }        /// <summary>

            }        /// Đóng đồng thời Reader → Command → Connection.

        }        /// </summary>

    }        public static void CloseAll(SqlDataReader reader, SqlCommand cmd, SqlConnection conn) // TODO: Nếu đổi DBMS, sửa tất cả kiểu tương ứng.

}        {

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

