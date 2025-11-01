using System;
using System.IO; // Để dùng StreamWriter, File...
using System.Text; // Để dùng Encoding
using System.Windows.Forms; // Để dùng DataGridView

namespace QLThuocWin.Utils
{
    /**
     * Lớp tiện ích C# để xuất dữ liệu.
     * Tương đương với 'ExportHelper.java'.
     *
     * Ghi chú từ file Java:
     * Xuất dữ liệu ra file CSV (có thể mở bằng Excel).
     * Nếu muốn xuất PDF hoặc định dạng Excel (.xlsx) chuyên sâu, bạn cần bổ sung thư viện .NET như:
     * - Excel: EPPlus, NPOI (phiên bản .NET của Apache POI)
     * - PDF: iTextSharp (phiên bản .NET của iText), QuestPDF, hoặc PdfSharp.
     */
    public static class ExportHelper
    {
        /// <summary>
        /// Xuất DataGridView ra CSV (ngăn cách bằng dấu phẩy).
        /// Tương đương: exportTableToCSV(JTable table, File outputFile)
        /// </summary>
        /// <param name="dataGridView">DataGridView cần xuất (thay vì JTable).</param>
        /// <param name="outputFilePath">Đường dẫn file đích (ví dụ: "C:\exports\data.csv").</param>
        /// <exception cref="ArgumentException">Nếu tham số đầu vào bị null.</exception>
        /// <exception cref="IOException">Ném ra nếu có lỗi khi ghi file.</exception>
        public static void ExportTableToCSV(DataGridView dataGridView, string outputFilePath)
        {
            if (dataGridView == null || string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentException("DataGridView hoặc outputFilePath không được null hoặc rỗng");
            }

            // 1. Sử dụng 'using' (tương đương try-with-resources của Java) để đảm bảo file được đóng.
            // 2. StreamWriter là lớp C# tương đương với BufferedWriter(OutputStreamWriter(...))
            // 3. Encoding.UTF8 tương đương StandardCharsets.UTF_8 để hỗ trợ tiếng Việt.
            // 4. 'false' trong hàm dựng của StreamWriter nghĩa là "Overwrite" (Ghi đè file nếu đã tồn tại).
            using (StreamWriter sw = new StreamWriter(outputFilePath, false, Encoding.UTF8))
            {
                int columnCount = dataGridView.Columns.Count;

                // Viết header (Tiêu đề cột)
                for (int col = 0; col < columnCount; col++)
                {
                    // Lấy HeaderText của cột (tương đương model.getColumnName(col))
                    sw.Write(dataGridView.Columns[col].HeaderText);
                    if (col < columnCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                // Ghi xuống dòng (tương đương bw.newLine())
                sw.WriteLine(); 

                // Lấy số dòng thực tế
                // (Phòng trường hợp DataGridView cho phép thêm dòng mới 'AllowUserToAddRows')
                int rowCount = dataGridView.Rows.Count;
                if (dataGridView.AllowUserToAddRows)
                {
                    rowCount--; // Bỏ qua dòng mới (thường là dòng cuối)
                }

                // Viết dữ liệu từng dòng
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < columnCount; col++)
                    {
                        // Lấy giá trị ô (tương đương model.getValueAt(row, col))
                        object value = dataGridView.Rows[row].Cells[col].Value;

                        // Xử lý giá trị (giống logic Java)
                        // Thay thế dấu phẩy (nếu có) bằng dấu cách để không làm vỡ cấu trúc CSV
                        // .Replace() trong C# tương đương .replaceAll() (cho chuỗi) trong Java
                        string cell = value != null ? value.ToString().Replace(",", " ") : "";
                        
                        sw.Write(cell);
                        if (col < columnCount - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.WriteLine(); // Xuống dòng
                }
            }
        }

        /**
         * (Tùy chọn) Xuất danh sách entity ra file CSV.
         * Phương thức này chưa được cài đặt.
         * 'IEnumerable<T>' là C# tương đương với 'Iterable<T>' của Java.
         * 'NotImplementedException' là C# tương đương với 'UnsupportedOperationException'
         */
        public static void ExportListToCSV<T>(System.Collections.Generic.IEnumerable<T> list, string outputFilePath)
        {
            // TODO: Cài đặt theo từng entity cụ thể hoặc sử dụng reflection.
            throw new NotImplementedException("Phương thức ExportListToCSV cần được cài đặt riêng theo entity.");
        }

        /**
         * (Placeholder) Xuất DataGridView ra file PDF.
         * Để làm việc này, cần bổ sung thư viện .NET như iTextSharp hoặc QuestPDF.
         *
         * Method này chưa cài đặt sẵn để tránh phụ thuộc thư viện.
         */
        public static void ExportTableToPDF(DataGridView dataGridView, string outputFilePath)
        {
            throw new NotImplementedException("Muốn xuất PDF, phải thêm thư viện .NET như iTextSharp hoặc QuestPDF và cài đặt logic ở đây.");
        }
    }
}