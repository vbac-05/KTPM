using System;
using System.Globalization; // Để dùng CultureInfo
using System.Text.RegularExpressions; // Để dùng Regex

namespace QLThuocApp.Utils
{
    /**
     * Validator.cs (Tương đương Validator.java)
     *
     * Các phương thức tiện ích để kiểm tra tính hợp lệ của dữ liệu đầu vào.
     */
    public static class Validator
    {
        /// <summary>
        /// Kiểm tra xem chuỗi text có thể được chuyển sang DateTime với định dạng format không.
        /// Chế độ kiểm tra nghiêm ngặt (tương đương setLenient(false)).
        /// Tương đương: isDateTime(String dateStr, String pattern)
        /// </summary>
        /// <param name="dateStr">Chuỗi cần kiểm tra.</param>
        /// <param name="pattern">Mẫu định dạng (ví dụ: "dd/MM/yyyy HH:mm").</param>
        /// <returns>true nếu text đúng định dạng, false nếu không.</returns>
        public static bool IsDateTime(string dateStr, string pattern)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
            {
                return false;
            }

            // DateTime.TryParseExact là phương thức C# tương đương với
            // SimpleDateFormat.setLenient(false).parse()
            // Nó trả về true nếu parse thành công, false nếu thất bại.
            return DateTime.TryParseExact(dateStr,
                                          pattern,
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out _); // _ (discard) nghĩa là chúng ta không cần giá trị trả về
        }

        /// <summary>
        /// Tương đương: isDate(String text, String format)
        /// (Trong C#, phương thức này giống hệt IsDateTime)
        /// </summary>
        public static bool IsDate(string text, string format)
        {
            return IsDateTime(text, format);
        }

        /// <summary>
        /// Kiểm tra xem chuỗi text có thể parse thành kiểu double không.
        /// Tương đương: isDouble(String text)
        /// </summary>
        public static bool IsDouble(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            // double.TryParse là phương thức an toàn (tương đương try-catch)
            return double.TryParse(text.Trim(), out _);
        }

        /// <summary>
        /// Kiểm tra xem chuỗi text có thể parse thành kiểu int không.
        /// Tương đương: isInteger(String text)
        /// </summary>
        public static bool IsInteger(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            // int.TryParse là phương thức an toàn (tương đương try-catch)
            return int.TryParse(text.Trim(), out _);
        }

        /// <summary>
        /// Kiểm tra xem chuỗi text có phải là số điện thoại hợp lệ (10 hoặc 11 chữ số) hay không.
        /// Tương đương: isPhone(String text)
        /// </summary>
        public static bool IsPhone(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            // Regex.IsMatch là C# tương đương với string.matches(regex) của Java
            return Regex.IsMatch(text.Trim(), @"^\d{10,11}$");
        }

        /// <summary>
        /// Kiểm tra xem chuỗi text có phải là email hợp lệ hay không.
        /// Tương đương: isEmail(String text)
        /// </summary>
        public static bool IsEmail(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            // Sử dụng regex tương tự của Java
            return Regex.IsMatch(text.Trim(), @"^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+$");
        }
    }
}