using System;
using System.Drawing;
using System.Drawing.Drawing2D; // Dùng cho InterpolationMode (scaling)
using System.Drawing.Imaging;  // Dùng cho ImageFormat
using System.IO;

namespace Utils
{
    /// <summary>
    /// Xử lý ảnh (đọc/ghi byte[] ↔ Image, tạo Image, scale ảnh...).
    /// Lớp này tương đương với ImageHelper.java, được chuyển sang C#.
    /// Yêu cầu package NuGet: System.Drawing.Common (cho .NET Core/5+)
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Chuyển byte[] (lấy từ CSDL) thành đối tượng Image.
        /// Trong C#, 'Image' hoặc 'Bitmap' là lớp cơ sở,
        /// tương đương với 'BufferedImage' hoặc 'Image' của Java.
        /// 'ImageIcon' của Swing tương đương với 'PictureBox' (WinForms)
        /// hoặc 'Image' (WPF) control.
        /// </summary>
        /// <param name="data">Mảng byte (hình ảnh)</param>
        /// <returns>Đối tượng Image hoặc null nếu lỗi</returns>
        public static Image ToImage(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }
            try
            {
                // Sử dụng MemoryStream thay cho ByteArrayInputStream
                using (MemoryStream ms = new MemoryStream(data))
                {
                    // Image.FromStream tương đương với ImageIO.read
                    Image img = Image.FromStream(ms);
                    return img;
                }
            }
            catch (Exception e)
            {
                // Ghi log lỗi (trong C# thường dùng Console.Error.WriteLine hoặc một logger chuyên dụng)
                Console.Error.WriteLine($"Lỗi chuyển byte[] sang Image: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Chuyển từ file ảnh (jpg, png, ...) thành byte[] để lưu vào CSDL.
        /// </summary>
        /// <param name="filePath">Đường dẫn đầy đủ đến file ảnh</param>
        /// <returns>Mảng byte[] hoặc null nếu lỗi</returns>
        public static byte[] ToByteArray(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }
            try
            {
                // C# có một phương thức cực kỳ tiện lợi cho việc này
                return File.ReadAllBytes(filePath);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine($"Lỗi đọc file thành byte[]: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Scale ảnh (Image) về kích thước mong muốn.
        /// </summary>
        /// <param name="image">Đối tượng Image gốc</param>
        /// <param name="width">Chiều rộng mới</param>
        /// <param name="height">Chiều cao mới</param>
        /// <returns>Image đã scale hoặc null nếu image == null</returns>
        public static Image ScaleImage(Image image, int width, int height)
        {
            if (image == null)
            {
                return null;
            }

            try
            {
                // Tạo một Bitmap (ảnh) mới với kích thước đích
                var destImage = new Bitmap(width, height);

                // Đặt độ phân giải (tùy chọn nhưng nên làm)
                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                // Tạo đối tượng Graphics để vẽ lại ảnh
                using (var graphics = Graphics.FromImage(destImage))
                {
                    // Đặt chất lượng scale (tương đương Image.SCALE_SMOOTH)
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;

                    // Vẽ ảnh gốc vào ảnh mới với kích thước mới
                    graphics.DrawImage(image, 0, 0, width, height);
                }

                return destImage;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Lỗi scale ảnh: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lưu byte[] thành file ảnh trên đĩa (đường dẫn chỉ định).
        /// Giống như bản Java, phương thức này giải mã byte[] thành Image
        /// rồi nén lại -> cho phép chuyển đổi định dạng (vd: byte[] PNG -> file JPG).
        /// </summary>
        /// <param name="data">Mảng byte của ảnh</param>
        /// <param name="outputPath">Đường dẫn file đầu ra (ví dụ: "C:\image.jpg")</param>
        /// <returns>True nếu lưu thành công, False nếu thất bại</returns>
        public static bool SaveImage(byte[] data, string outputPath)
        {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            try
            {
                // Sử dụng ToImage để giải mã byte[]
                using (Image img = ToImage(data))
                {
                    if (img == null)
                    {
                        return false;
                    }

                    // Phương thức Save của C# tự động xác định định dạng
                    // dựa trên phần mở rộng của outputPath (giống ImageIO.write)
                    img.Save(outputPath);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Lỗi lưu ảnh từ byte[]: {e.Message}");
                return false;
            }
        }
    }
}