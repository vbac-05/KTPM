using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLThuocApp.UI
{
    public class LineChartPanel : UserControl
    {
        // ====== DATA ======
        private IDictionary<string, int> _data = new Dictionary<string, int>();

        // ====== STYLE (tùy biến) ======
        private int padding = 60;
        private int labelPadding = 40;
        private int pointSize = 6;

        public LineChartPanel()
        {
            DoubleBuffered = true; // mượt khi vẽ
            BackColor = Color.White;
            Size = new Size(800, 400);
        }

        /// <summary>
        /// Bơm dữ liệu cho chart. Key = nhãn (ngày), Value = doanh thu.
        /// </summary>
        public void SetData(IDictionary<string, int> data)
        {
            _data = data ?? new Dictionary<string, int>();
            Invalidate();
        }

        // (tuỳ chọn) nếu muốn truy cập data hiện tại
        public IDictionary<string, int> GetData() => _data;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_data == null || _data.Count == 0)
            {
                using (var f = new Font("SansSerif", 10f))
                using (var b = new SolidBrush(Color.Black))
                {
                    g.DrawString("Không có dữ liệu để hiển thị.", f, b, 50, 50);
                }
                return;
            }

            // Lấy labels & values theo thứ tự đưa vào (giống Java)
            var labels = _data.Keys.ToList();
            var values = _data.Values.ToList();

            int width = ClientSize.Width;
            int height = ClientSize.Height;

            int maxVal = values.Max();
            int minVal = 0; // giống Java, chốt min = 0

            // Tránh chia cho 0 khi chỉ có 1 điểm
            double xScale = labels.Count > 1
                ? (double)(width - 2 * padding - labelPadding) / (labels.Count - 1)
                : 1.0;

            // Nếu maxVal == minVal (mọi điểm đều bằng nhau) -> yScale 1 để khỏi NaN
            double yScale = (maxVal - minVal) != 0
                ? (double)(height - 2 * padding) / (maxVal - minVal)
                : 1.0;

            // ===== Vẽ lưới ngang =====
            using (var gridPen = new Pen(Color.FromArgb(220, 220, 220)))
            {
                for (int i = 0; i <= 5; i++)
                {
                    int y = height - padding - (int)(i * (height - 2 * padding) / 5.0);
                    g.DrawLine(gridPen, padding, y, width - padding, y);
                }
            }

            // ===== Vẽ đường nối =====
            using (var linePen = new Pen(Color.FromArgb(0, 120, 215), 2f)) // xanh giống Java
            {
                for (int i = 0; i < values.Count - 1; i++)
                {
                    int x1 = (int)(padding + i * xScale);
                    int y1 = height - padding - (int)((values[i] - minVal) * yScale);
                    int x2 = (int)(padding + (i + 1) * xScale);
                    int y2 = height - padding - (int)((values[i + 1] - minVal) * yScale);
                    g.DrawLine(linePen, x1, y1, x2, y2);
                }
            }

            // ===== Vẽ điểm + giá trị =====
            using (var pointBrush = new SolidBrush(Color.Red))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var valueFont = new Font("SansSerif", 12f))
            {
                for (int i = 0; i < values.Count; i++)
                {
                    int x = (int)(padding + i * xScale);
                    int y = height - padding - (int)((values[i] - minVal) * yScale);
                    g.FillEllipse(pointBrush, x - pointSize / 2, y - pointSize / 2, pointSize, pointSize);

                    g.DrawString(values[i].ToString(), valueFont, textBrush, x - 5, y - 10);
                }
            }

            // ===== Vẽ nhãn trục X =====
            using (var axisFont = new Font("SansSerif", 12f))
            using (var textBrush = new SolidBrush(Color.Black))
            {
                for (int i = 0; i < labels.Count; i++)
                {
                    int x = (int)(padding + i * xScale);
                    // Bỏ nhãn xen kẽ nếu quá nhiều (giữ nguyên logic Java)
                    if (labels.Count > 15 && i % 2 != 0) continue;
                    g.DrawString(labels[i], axisFont, textBrush, x - 10, height - padding + 15);
                }
            }

            // ===== Vẽ trục X & Y =====
            using (var axisPen = new Pen(Color.Black))
            {
                g.DrawLine(axisPen, padding, height - padding, padding, padding);               // Y
                g.DrawLine(axisPen, padding, height - padding, width - padding, height - padding); // X
            }

            // ===== Vẽ tiêu đề =====
            using (var titleFont = new Font("SansSerif", 14f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.Black))
            {
                // TODO: ĐỔI tiêu đề nếu cần
                string title = "Biểu đồ doanh thu theo ngày";
                var titleSize = g.MeasureString(title, titleFont);
                g.DrawString(title, titleFont, titleBrush, (width - titleSize.Width) / 2f, 15);
            }

            // ===== Tính & vẽ ô trung bình, tổng =====
            double avg = values.Average();
            int total = values.Sum();
            int n = values.Count;

            string avgLabel = $"Doanh thu trung bình trong {n} ngày: {avg:0}";
            string sumLabel = $"Tổng doanh thu trong {n} ngày: {total}";

            using (var infoFont = new Font("SansSerif", 12f, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var backBrush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            using (var borderPen = new Pen(Color.Gray))
            {
                var metrics = g.MeasureString(avgLabel, infoFont);
                var metrics2 = g.MeasureString(sumLabel, infoFont);

                int x = 60;
                int y = 50;
                int lineSpacing = 20;

                int boxW = (int)Math.Max(metrics.Width, metrics2.Width) + 20;
                int boxH = lineSpacing * 2 + 20;

                var rect = new Rectangle(x - 10, y - 25, boxW, boxH);
                // Nền & viền bo góc
                // (Không có API fillRoundRect sẵn; vẽ đơn giản bằng FillRectangle + DrawRectangle)
                g.FillRectangle(backBrush, rect);
                g.DrawRectangle(borderPen, rect);

                g.DrawString(avgLabel, infoFont, textBrush, x, y);
                g.DrawString(sumLabel, infoFont, textBrush, x, y + lineSpacing);
            }
        }

        // ====== Helper (nếu muốn đổi màu/tiêu đề theo theme) ======
        // TODO: ĐỔI màu/thickness/title theo Settings toàn app nếu cần
    }
}
