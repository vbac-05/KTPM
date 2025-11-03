using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLThuocApp.UI
{
    public class LineChartPanel2 : UserControl
    {
        // ===== DATA =====
        private List<KeyValuePair<string, int>> _pairs = new(); // giữ thứ tự
        private string _title = "Biểu đồ";       // TODO: ĐỔI tiêu đề mặc định nếu cần
        private string _tongText = "";           // TODO: ĐỔI nội dung hiển thị tổng
        private string _tbText = "";             // TODO: ĐỔI nội dung hiển thị trung bình

        // ===== STYLE =====
        private int padding = 60;
        private int labelPadding = 40;

        public LineChartPanel2()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            Size = new Size(800, 500);
            Dock = DockStyle.Fill;
        }

        public LineChartPanel2(IDictionary<string, int> data, string title, string tongText, string tbText) : this()
        {
            SetData(data, title, tongText, tbText);
        }

        /// <summary>
        /// Bơm dữ liệu + text hiển thị.
        /// </summary>
        public void SetData(IDictionary<string, int> data, string title, string tongText, string tbText)
        {
            _pairs = (data ?? new Dictionary<string, int>()).ToList(); // giữ thứ tự enumeration
            _title = title ?? _title;
            _tongText = tongText ?? "";
            _tbText = tbText ?? "";
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g2 = e.Graphics;
            g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_pairs == null || _pairs.Count == 0)
            {
                using var f = new Font("SansSerif", 10f);
                using var b = new SolidBrush(Color.Black);
                g2.DrawString("Không có dữ liệu để hiển thị.", f, b, 50, 50);
                return;
            }

            int width = ClientSize.Width;
            int height = ClientSize.Height;

            int maxValue = Math.Max(1, _pairs.Max(p => p.Value)); // tránh 0
            int xCount = _pairs.Count;

            // Nếu chỉ có 1 điểm, tránh chia 0
            int xStep = xCount > 1 ? (width - 2 * padding) / (xCount - 1) : 1;
            int yStep = (height - 2 * padding) / 10; // 10 mức Y

            // Lưới ngang + nhãn trục Y
            using var gridPen = new Pen(Color.LightGray);
            using var yTextBrush = new SolidBrush(Color.DarkGray);
            using var yFont = new Font("SansSerif", 10f);
            for (int i = 0; i <= 10; i++)
            {
                int y = height - padding - i * yStep;
                g2.DrawLine(gridPen, padding, y, width - padding, y);
                int value = maxValue * i / 10;
                var yStr = string.Format("{0:n0}", value);
                g2.DrawString(yStr, yFont, yTextBrush, padding - 50, y - yFont.Height / 2f + 5);
            }

            // Trục X & Y
            using (var axisPen = new Pen(Color.Black))
            {
                g2.DrawLine(axisPen, padding, height - padding, width - padding, height - padding); // X
                g2.DrawLine(axisPen, padding, padding, padding, height - padding);                   // Y
            }

            // Tính toạ độ
            var xPoints = new int[xCount];
            var yPoints = new int[xCount];
            for (int i = 0; i < xCount; i++)
            {
                int x = padding + i * xStep;
                int y = height - padding - (int)((double)_pairs[i].Value / maxValue * (height - 2 * padding));
                xPoints[i] = x;
                yPoints[i] = y;
            }

            // Polyline
            using (var linePen = new Pen(Color.Blue, 2f))
            {
                if (xCount >= 2) g2.DrawLines(linePen, xPoints.Zip(yPoints, (xx, yy) => new Point(xx, yy)).ToArray());
                else if (xCount == 1) // chỉ 1 điểm, không vẽ line
                {
                    // no-op
                }
            }

            // Điểm + nhãn X + giá trị điểm
            using var pointBrush = new SolidBrush(Color.Red);
            using var xFont = new Font("SansSerif", 9f);
            using var valueFont = new Font("SansSerif", 9f);
            using var textBrush = new SolidBrush(Color.Black);

            for (int i = 0; i < xCount; i++)
            {
                int x = xPoints[i];
                int y = yPoints[i];
                g2.FillEllipse(pointBrush, x - 3, y - 3, 6, 6);

                g2.DrawString($"{_pairs[i].Value}", valueFont, textBrush, x - 8, y - 14);
                g2.DrawString(_pairs[i].Key, xFont, textBrush, x - 15, height - padding + 15);
            }

            // Tiêu đề
            using (var titleFont = new Font("SansSerif", 16f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.Black))
            {
                // TODO: ĐỔI căn lề tiêu đề: đang vẽ tại (padding, 30)
                g2.DrawString(_title, titleFont, titleBrush, padding, 30);
            }

            // Khung Tổng/Trung bình (gradient + bo góc)
            using var infoFont = new Font("SansSerif", 12f, FontStyle.Bold);
            var fm = g2.MeasureString(_tongText ?? "", infoFont);
            var fm2 = g2.MeasureString(_tbText ?? "", infoFont);
            int maxTextWidth = (int)Math.Max(fm.Width, fm2.Width);

            int boxWidth = maxTextWidth + 20;
            int boxHeight = 45;
            int boxX = padding;
            int boxY = 40;

            // Gradient
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            var rect = new Rectangle(boxX, boxY, boxWidth, boxHeight);
            int radius = 15;
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect,
                Color.FromArgb(200, 200, 255, 200),
                Color.FromArgb(255, 255, 255, 200),
                0f))
            using var borderPen = new Pen(Color.Black);
            using var infoBrush = new SolidBrush(Color.Black);
            {
                g2.FillPath(brush, path);
                g2.DrawPath(borderPen, path);

                g2.DrawString(_tongText ?? "", infoFont, infoBrush, boxX + 10, boxY + 20);
                g2.DrawString(_tbText ?? "", infoFont, infoBrush, boxX + 10, boxY + 40);
            }
        }

        // ===== GỢI Ý TÍCH HỢP =====
        // TODO: Nếu muốn đảm bảo thứ tự thời gian (tháng 1..12), hãy sort:
        // _pairs = data.OrderBy(kv => kv.Key, StringComparer.Create(new CultureInfo("vi-VN"), true)).ToList();
    }
}
