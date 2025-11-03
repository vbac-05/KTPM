using System;
using System.Globalization;

namespace QLThuocApp.Utils
{
    /// <summary>
    /// Các phương thức tiện ích C# tương đương với DateHelper của Java.
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// Mẫu định dạng ngày-tháng-năm mặc định.
        /// </summary>
        public const string DEFAULT_DATE_PATTERN = "dd/MM/yyyy";

        /// <summary>
        /// Chuyển DateTime thành String với mẫu mặc định "dd/MM/yyyy".
        /// Tương đương: toString(Date date)
        /// </summary>
        /// <param name="date">Đối tượng DateTime (có thể null).</param>
        /// <returns>Chuỗi định dạng ngày hoặc rỗng nếu date == null.</returns>
        public static string ToString(DateTime? date)
        {
            // Sử dụng toán tử null-conditional (?) và null-coalescing (??)
            // Tương đương: if (date == null) return "";
            return date?.ToString(DEFAULT_DATE_PATTERN) ?? "";
        }

        /// <summary>
        /// Chuyển String thành DateTime theo mẫu mặc định "dd/MM/yyyy".
        /// Tương đương: toDate(String dateStr)
        /// </summary>
        /// <param name="dateStr">Chuỗi ngày (vd: "31/12/2024").</param>
        /// <returns>Đối tượng DateTime? (nullable) hoặc null nếu không thể parse.</returns>
        public static DateTime? ToDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
            {
                return null;
            }

            // DateTime.TryParseExact là phương thức an toàn (giống try-catch)
            // và nghiêm ngặt (tương đương setLenient(false))
            if (DateTime.TryParseExact(dateStr,
                                       DEFAULT_DATE_PATTERN,
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out DateTime result))
            {
                return result; // Trả về ngày nếu thành công
            }

            return null; // Trả về null nếu thất bại
        }

        /// <summary>
        /// Chuyển DateTime thành String theo mẫu tùy chỉnh.
        /// Tương đương: toString(Date date, String pattern)
        /// </summary>
        public static string ToString(DateTime? date, string pattern)
        {
            if (date == null)
            {
                return "";
            }
            // C# tích hợp sẵn hàm .ToString(pattern)
            return date.Value.ToString(pattern);
        }

        /// <summary>
        /// Chuyển String thành DateTime theo mẫu tùy chỉnh.
        /// Tương đương: toDate(String dateStr, String pattern)
        /// </summary>
        public static DateTime? ToDate(string dateStr, string pattern)
        {
            if (string.IsNullOrWhiteSpace(dateStr) || string.IsNullOrWhiteSpace(pattern))
            {
                return null;
            }

            if (DateTime.TryParseExact(dateStr,
                                       pattern,
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out DateTime result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Chuyển String thành DateTime (nghiêm ngặt).
        /// Tương đương: toDateTime(String dateStr, String pattern)
        /// </summary>
        /// <remarks>
        /// Trong C#, TryParseExact đã là "nghiêm ngặt" (non-lenient) mặc định,
        /// nên phương thức này hoạt động giống hệt ToDate(dateStr, pattern).
        /// </remarks>
        public static DateTime? ToDateTime(string dateStr, string pattern)
        {
            return ToDate(dateStr, pattern);
        }

        /// <summary>
        /// Lấy Date hiện tại (chỉ ngày, 00:00:00).
        /// Tương đương: now()
        /// </summary>
        /// <returns>Đối tượng DateTime của ngày hiện tại (00:00:00).</returns>
        public static DateTime Now()
        {
            // Cách làm của Java (toString -> toDate) tương đương 
            // với thuộc tính DateTime.Today trong C#.
            return DateTime.Today;
        }
    }
}