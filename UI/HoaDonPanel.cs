using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
// using QLThuocApp.Services; // TODO: ĐỔI: namespace Service/DAO thật của bạn
using QLThuocApp.Entities;        // TODO: ĐỔI: nếu entity ở namespace khác

namespace QLThuocApp.UI
{
    public class HoaDonPanel : UserControl
    {
        // ===== Controls =====
        TextBox txtSearchIdHD = new TextBox();
        TextBox txtSearchIdNV = new TextBox();
        TextBox txtSearchIdKH = new TextBox();
        Button btnSearch = new Button { Text = "Tìm kiếm" /*, Image = ...*/ }; // TODO: ĐỔI icon nếu cần
        Button btnAdd = new Button { Text = "     Thêm" };    // TODO: ĐỔI: Image = Image.FromFile(...) nếu dùng icon
        Button btnEdit = new Button { Text = "     Sửa" };
        Button btnDelete = new Button { Text = "     Xóa" };
        Button btnViewDetail = new Button { Text = "  Xem chi tiết" };
        Button btnRefresh = new Button { Text = "   Làm mới" };
        Button btnDTNgay = new Button { Text = "   Doanh thu ngày" };
        Button btnDTThang = new Button { Text = "   Doanh thu tháng" };

        DataGridView grid = new DataGridView();
        BindingSource bs = new BindingSource();

        // ===== Dữ liệu =====
        List<HoaDon> dsHoaDon = new List<HoaDon>();

        public HoaDonPanel()
        {
            Dock = DockStyle.Fill;
            BuildToolbar();
            BuildSearchBar();
            BuildGrid();
            WireEvents();
            ReloadAll();
        }

        private void BuildToolbar()
        {
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };
            // TODO: ĐỔI: set Image cho các nút nếu bạn có resource (vd: Properties.Resources.xxx)
            bar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnViewDetail, btnRefresh, btnDTNgay, btnDTThang });
            Controls.Add(bar);
        }

        private void BuildSearchBar()
        {
            var search = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 8, AutoSize = true, Padding = new Padding(8) };
            search.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            search.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            search.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            search.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            search.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            search.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            search.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            search.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            search.Controls.Add(new Label { Text = "IDHD:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            search.Controls.Add(txtSearchIdHD, 1, 0);

            search.Controls.Add(new Label { Text = "IDNV:", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
            search.Controls.Add(txtSearchIdNV, 3, 0);

            search.Controls.Add(new Label { Text = "IDKH:", AutoSize = true, Anchor = AnchorStyles.Left }, 4, 0);
            search.Controls.Add(txtSearchIdKH, 5, 0);

            search.Controls.Add(new Label(), 6, 0);
            search.Controls.Add(btnSearch, 7, 0);

            Controls.Add(search);
        }

        private void BuildGrid()
        {
            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;

            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDHD", DataPropertyName = "IdHD", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thời gian", DataPropertyName = "ThoiGianStr", Width = 160 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDNV", DataPropertyName = "IdNV", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDKH", DataPropertyName = "IdKH", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tổng tiền", DataPropertyName = "TongTienStr", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "PT Thanh toán", DataPropertyName = "PhuongThucThanhToan", Width = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = "TrangThaiDonHang", Width = 140 });

            grid.DataSource = bs;
            Controls.Add(grid);
        }

        private void WireEvents()
        {
            btnRefresh.Click += (s, e) => ReloadAll();
            btnSearch.Click += (s, e) => DoSearch();

            btnAdd.Click += (s, e) =>
            {
                // TODO: ĐỔI: gọi dialog thật của bạn (bạn đã có AddHoaDonDialog)
                var dlg = new AddHoaDonDialog(); // giả định có ctor không tham số
                var owner = FindForm();
                if (owner != null) dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ShowDialog(owner);

                // Sau khi thêm xong, reload
                ReloadAll();
            };

            btnEdit.Click += (s, e) =>
            {
                var id = GetCurrentIdHD();
                if (id == null) { MessageBox.Show("Vui lòng chọn hóa đơn cần sửa!", "Cảnh báo"); return; }

                // TODO: ĐỔI: lấy HoaDon + Chi tiết từ service/dao thật
                var hd = GetHoaDonById(id); // TODO: ĐỔI: controller.getHoaDonById(id)
                var ct = GetChiTietByIdHD(id); // TODO: ĐỔI: ChiTietHoaDonDAO().getByIdHD(id)

                // TODO: ĐỔI: dùng dialog sửa thật của bạn
                var dlg = new EditHoaDonDialog(/* truyền hd, ct nếu dialog của bạn cần */);
                var owner = FindForm();
                if (owner != null) dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ShowDialog(owner);

                ReloadAll();
            };

            btnDelete.Click += (s, e) =>
            {
                var id = GetCurrentIdHD();
                if (id == null) { MessageBox.Show("Vui lòng chọn hóa đơn cần xóa!", "Cảnh báo"); return; }

                if (MessageBox.Show($"Bạn có chắc muốn xóa hóa đơn {id}?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    var err = "";
                    var ok = DeleteHoaDon(id, ref err); // TODO: ĐỔI: controller.deleteHoaDon(id, out err)
                    if (ok)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        ReloadAll();
                    }
                    else
                    {
                        MessageBox.Show(string.IsNullOrWhiteSpace(err) ? "Xóa thất bại!" : err, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            btnViewDetail.Click += (s, e) =>
            {
                var id = GetCurrentIdHD();
                if (id == null) { MessageBox.Show("Vui lòng chọn hóa đơn để xem chi tiết!", "Cảnh báo"); return; }

                var hd = GetHoaDonById(id); // TODO: ĐỔI: controller.getHoaDonById(id)
                if (hd == null) { MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi"); return; }

                // TODO: ĐỔI: dialog chi tiết thật
                var dlg = new ViewHoaDonDetailDialog(FindForm(), hd);
                dlg.ShowDialog(FindForm());
            };

            btnDTNgay.Click += (s, e) => ShowRevenueByDay();
            btnDTThang.Click += (s, e) => ShowRevenueByMonth();
        }

        // ====== Actions ======
        private void ReloadAll()
        {
            dsHoaDon = GetAllHoaDon(); // TODO: ĐỔI: controller.getAllHoaDon()

            // map -> view model
            var rows = dsHoaDon.Select(h => new
            {
                h.IdHD,
                ThoiGianStr = (h.ThoiGian == default) ? "" : h.ThoiGian.ToString("dd/MM/yyyy HH:mm:ss"),
                h.IdNV,
                h.IdKH,
                TongTienStr = string.Format(CultureInfo.InvariantCulture, "{0:0.0}", h.TongTien),
                h.PhuongThucThanhToan,
                h.TrangThaiDonHang
            }).ToList();

            bs.DataSource = rows;
            if (grid.Rows.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
            }
        }

        private void DoSearch()
        {
            var idHD = (txtSearchIdHD.Text ?? "").Trim();
            var idNV = (txtSearchIdNV.Text ?? "").Trim();
            var idKH = (txtSearchIdKH.Text ?? "").Trim();

            var results = SearchHoaDon(idHD, idNV, idKH); // TODO: ĐỔI: controller.searchHoaDon(...)
            var rows = results.Select(h => new
            {
                h.IdHD,
                ThoiGianStr = (h.ThoiGian == default) ? "" : h.ThoiGian.ToString("dd/MM/yyyy HH:mm:ss"),
                h.IdNV,
                h.IdKH,
                TongTienStr = string.Format(CultureInfo.InvariantCulture, "{0:0.0}", h.TongTien),
                h.PhuongThucThanhToan,
                h.TrangThaiDonHang
            }).ToList();

            bs.DataSource = rows;
            if (grid.Rows.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
            }
        }

        private string GetCurrentIdHD()
        {
            if (grid.CurrentRow == null) return null;
            var obj = grid.CurrentRow.Cells[0].Value;
            return obj?.ToString();
        }

        private void ShowRevenueByDay()
        {
            var from = Prompt2Text("Từ ngày (yyyy-MM-dd):", "Chọn khoảng ngày");
            if (from == null) return;
            var to = Prompt2Text("Đến ngày (yyyy-MM-dd):", "Chọn khoảng ngày");
            if (to == null) return;

            var data = TinhDoanhThuTheoNgay(from, to); // TODO: ĐỔI: HoaDonController.tinhDoanhThuTheoNgay(from, to)
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trong khoảng ngày đã chọn.");
                return;
            }
            ShowLineChart("Biểu đồ Doanh thu", "Ngày", "VNĐ", data);
        }

        private void ShowRevenueByMonth()
        {
            var yearStr = Prompt("Nhập năm (VD: 2024):", "Chọn năm");
            if (string.IsNullOrWhiteSpace(yearStr)) return;

            if (!int.TryParse(yearStr.Trim(), out var year))
            {
                MessageBox.Show("Năm không hợp lệ!");
                return;
            }

            var data = TinhDoanhThuTheoThang(year); // TODO: ĐỔI: HoaDonController.tinhDoanhThuTheoThang(year)
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu năm đã chọn.");
                return;
            }

            var tong = data.Values.Sum();
            var tb = data.Count == 0 ? 0 : tong / data.Count;
            var title = $"Biểu đồ doanh thu theo tháng - {year}";
            var tongText = $"Tổng doanh thu: {tong:N0} VNĐ";
            var tbText = $"Trung bình: {tb:N0} VNĐ/tháng";

            ShowLineChart(title, "Tháng", "VNĐ", data, tongText, tbText);
        }

        private static string Prompt(string text, string title)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(text, title, "");
        }
        private static string Prompt2Text(string label, string title)
        {
            using (var f = new Form { Text = title, StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(360, 110), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                var lbl = new Label { Text = label, Left = 12, Top = 15, AutoSize = true };
                var txt = new TextBox { Left = 12, Top = 40, Width = 330 };
                var ok = new Button { Text = "OK", Left = 190, Top = 70, DialogResult = DialogResult.OK };
                var cancel = new Button { Text = "Hủy", Left = 268, Top = 70, DialogResult = DialogResult.Cancel };
                f.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
                f.AcceptButton = ok; f.CancelButton = cancel;
                return f.ShowDialog() == DialogResult.OK ? txt.Text : null;
            }
        }

        private void ShowLineChart(string title, string xTitle, string yTitle, IDictionary<string, int> data, string subtitle1 = null, string subtitle2 = null)
        {
            var chart = new Chart { Dock = DockStyle.Fill };
            chart.ChartAreas.Add(new ChartArea("ca"));
            var s = new Series("Revenue")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Int32
            };
            foreach (var kv in data)
                s.Points.AddXY(kv.Key, kv.Value);

            chart.Series.Add(s);
            chart.Titles.Add(title);
            if (!string.IsNullOrEmpty(subtitle1)) chart.Titles.Add(subtitle1);
            if (!string.IsNullOrEmpty(subtitle2)) chart.Titles.Add(subtitle2);
            chart.ChartAreas["ca"].AxisX.Title = xTitle;
            chart.ChartAreas["ca"].AxisY.Title = yTitle;

            var frm = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                Width = 800,
                Height = 500
            };
            frm.Controls.Add(chart);
            frm.Show(FindForm());
        }

        // ================== PLACEHOLDER SERVICE/DAO CALLS ==================
        // Tất cả dưới đây có gắn // TODO: ĐỔI – bạn thay bằng Controller/DAO thật của dự án

        private List<HoaDon> GetAllHoaDon()
        {
            // TODO: ĐỔI: return new HoaDonController().getAllHoaDon();
            // hoặc Repos.HoaDonRepo.ToList();
            return new List<HoaDon>(); // placeholder
        }

        private List<HoaDon> SearchHoaDon(string idHD, string idNV, string idKH)
        {
            // TODO: ĐỔI: return new HoaDonController().searchHoaDon(idHD, idNV, idKH);
            // placeholder: lọc tạm trên dsHoaDon đã có
            var q = dsHoaDon.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(idHD)) q = q.Where(h => (h.IdHD ?? "").Contains(idHD));
            if (!string.IsNullOrWhiteSpace(idNV)) q = q.Where(h => (h.IdNV ?? "").Contains(idNV));
            if (!string.IsNullOrWhiteSpace(idKH)) q = q.Where(h => (h.IdKH ?? "").Contains(idKH));
            return q.ToList();
        }

        private HoaDon GetHoaDonById(string idHD)
        {
            // TODO: ĐỔI: return new HoaDonController().getHoaDonById(idHD);
            return dsHoaDon.FirstOrDefault(h => string.Equals(h.IdHD, idHD, StringComparison.OrdinalIgnoreCase));
        }

        private List<ChiTietHoaDon> GetChiTietByIdHD(string idHD)
        {
            // TODO: ĐỔI: return new ChiTietHoaDonDAO().getByIdHD(idHD);
            return new List<ChiTietHoaDon>(); // placeholder
        }

        private bool DeleteHoaDon(string idHD, ref string errorMsg)
        {
            // TODO: ĐỔI: return new HoaDonController().deleteHoaDon(idHD, out errorMsg);
            errorMsg = "";
            return true; // placeholder
        }

        private Dictionary<string, int> TinhDoanhThuTheoNgay(string fromDate, string toDate)
        {
            // TODO: ĐỔI: return HoaDonController.tinhDoanhThuTheoNgay(fromDate, toDate);
            // placeholder demo
            return new Dictionary<string, int>
            {
                ["2024-10-01"] = 5000000,
                ["2024-10-02"] = 6500000,
                ["2024-10-03"] = 4200000,
            };
        }

        private Dictionary<string, int> TinhDoanhThuTheoThang(int year)
        {
            // TODO: ĐỔI: return HoaDonController.tinhDoanhThuTheoThang(year);
            // placeholder demo
            return Enumerable.Range(1, 12).ToDictionary(m => m.ToString("00"), m => 3_000_000 * m);
        }
    }
}
