using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
// TODO: đổi namespace/using cho đúng dự án thật (DAO/Repos)
// using QLThuocApp.Services;

namespace QLThuocApp.UI
{
    public class TrashHoaDonPanel : UserControl
    {
        // ===== Services & state =====
        // TODO: thay bằng DAO/Service thật (ví dụ HoaDonDAO) và map GetDeleted/Restore/DeleteForever
        private readonly ITrashHoaDonService _service = new DummyTrashHoaDonService();

        private readonly BindingList<HoaDonRow> _rows = new BindingList<HoaDonRow>(); // datasource
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _txtSearch = new TextBox();

        private readonly Button _btnRefresh = new Button();
        private readonly Button _btnRestore = new Button();
        private readonly Button _btnDeleteForever = new Button();

        private CheckBox _headerCheck; // CHANGED: checkbox chọn tất cả ở header

        // CHANGED: hỗ trợ cập nhật tên tab như Java
        private readonly TabControl _parentTab;
        private readonly int _tabIndex;

        public TrashHoaDonPanel(TabControl parentTab = null, int tabIndex = -1)
        {
            _parentTab = parentTab;
            _tabIndex = tabIndex;

            Dock = DockStyle.Fill;
            BuildSearchBar();
            BuildGrid();
            BuildButtons();

            LoadData(); // initial
        }

        // ===== UI builders =====
        private void BuildSearchBar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(5) };
            Controls.Add(bar);

            var lbl = new Label { Text = "Tìm kiếm: ", AutoSize = true, Dock = DockStyle.Left, TextAlign = ContentAlignment.MiddleLeft };
            bar.Controls.Add(lbl);

            _txtSearch.Dock = DockStyle.Fill;
            _txtSearch.Margin = new Padding(5, 8, 5, 8);
            _txtSearch.TextChanged += (s, e) => ApplyRealtimeSearch(); // CHANGED: realtime chọn dòng
            bar.Controls.Add(_txtSearch);
        }

        private void BuildGrid()
        {
            _grid.Dock = DockStyle.Fill;
            _grid.AutoGenerateColumns = false;
            _grid.AllowUserToAddRows = false;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = true; // để chọn nhiều dòng giống behavior “addRowSelectionInterval” Java
            Controls.Add(_grid);

            // Cột 0: checkbox (không bind)
            var colCheck = new DataGridViewCheckBoxColumn
            {
                HeaderText = string.Empty,
                Width = 36,
                ReadOnly = false
            };
            _grid.Columns.Add(colCheck);

            // CHANGED: thêm các cột đúng thứ tự & tên Java
            _grid.Columns.Add(MakeText("ID Hóa đơn", nameof(HoaDonRow.IdHD), 110));
            _grid.Columns.Add(MakeText("Thời gian", nameof(HoaDonRow.ThoiGian), 140));
            _grid.Columns.Add(MakeText("ID Nhân viên", nameof(HoaDonRow.IdNV), 110));
            _grid.Columns.Add(MakeText("ID Khách hàng", nameof(HoaDonRow.IdKH), 120));
            _grid.Columns.Add(MakeText("Tổng tiền", nameof(HoaDonRow.TongTien), 100, DataGridViewContentAlignment.MiddleRight));
            _grid.Columns.Add(MakeText("Phương thức thanh toán", nameof(HoaDonRow.PhuongThucThanhToan), 170));
            _grid.Columns.Add(MakeText("Trạng thái", nameof(HoaDonRow.TrangThaiDonHang), 120));

            _grid.DataSource = _rows;

            // CHANGED: header checkbox “chọn tất cả”
            _grid.ColumnWidthChanged += (s, e) => PlaceHeaderCheckBox();
            _grid.Scroll += (s, e) => PlaceHeaderCheckBox();
            _grid.HandleCreated += (s, e) => PlaceHeaderCheckBox();
            _grid.DataBindingComplete += (s, e) => PlaceHeaderCheckBox();

            // bỏ chọn khi click header checkbox; còn lại hành vi chọn dòng bình thường
        }

        private static DataGridViewTextBoxColumn MakeText(string header, string prop, int width,
            DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = align }
            };
        }

        private void BuildButtons()
        {
            var btns = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(5) };
            Controls.Add(btns);

            _btnRefresh.Text = "Làm mới";
            _btnRefresh.Width = 110;
            // TODO: _btnRefresh.Image = ... // icon chungRefresh.png
            _btnRefresh.Click += (s, e) => LoadData();

            _btnDeleteForever.Text = "Xóa vĩnh viễn";
            _btnDeleteForever.Width = 130;
            // TODO: _btnDeleteForever.Image = ... // icon chungTrash.png
            _btnDeleteForever.Click += (s, e) => DeleteSelectedRows();

            _btnRestore.Text = "Khôi phục";
            _btnRestore.Width = 110;
            // TODO: _btnRestore.Image = ... // icon ChungUndo.png
            _btnRestore.Click += (s, e) => RestoreSelectedRows();

            btns.Controls.AddRange(new Control[] { _btnRefresh, _btnDeleteForever, _btnRestore });
        }

        // ===== Data =====
        private void LoadData()
        {
            _rows.Clear();
            foreach (var x in _service.GetDeleted().Select(Map))
                _rows.Add(x);

            _grid.ClearSelection();
            UpdateTabTitle(); // CHANGED: cập nhật tiêu đề tab như Java
        }

        private void UpdateTabTitle()
        {
            if (_parentTab != null && _tabIndex >= 0 && _tabIndex < _parentTab.TabCount)
            {
                _parentTab.TabPages[_tabIndex].Text = $"Hóa đơn ({_service.GetDeleted().Count})";
            }
        }

        // ===== Search (realtime select like Java) =====
        private void ApplyRealtimeSearch()
        {
            var text = _txtSearch.Text.Trim().ToLowerInvariant();
            _grid.ClearSelection();

            if (string.IsNullOrEmpty(text))
                return;

            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                // CHANGED: duyệt từ cột 1 đến hết (bỏ checkbox) => match Java
                if (Contains(r.IdHD, text) ||
                    Contains(r.ThoiGian, text) ||
                    Contains(r.IdNV, text) ||
                    Contains(r.IdKH, text) ||
                    Contains(r.TongTien, text) ||
                    Contains(r.PhuongThucThanhToan, text) ||
                    Contains(r.TrangThaiDonHang, text))
                {
                    _grid.Rows[i].Selected = true;
                }
            }
        }

        private static bool Contains(string src, string q)
            => !string.IsNullOrEmpty(src) && src.ToLowerInvariant().Contains(q);

        // ===== Select helpers =====
        private List<string> GetCheckedIds()
        {
            var ids = new List<string>();
            // cột 0: checkbox; cột 1: IdHD
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;
                var isChecked = false;
                var cell = row.Cells[0] as DataGridViewCheckBoxCell;
                if (cell?.Value is bool b && b) isChecked = true;

                if (isChecked)
                {
                    ids.Add(Convert.ToString(row.Cells[1].Value));
                }
            }
            return ids;
        }

        private void RestoreSelectedRows()
        {
            var ids = GetCheckedIds();
            if (ids.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn để khôi phục");
                return;
            }

            // TODO: map sang dao.restore(id) thật
            foreach (var id in ids) _service.Restore(id);

            MessageBox.Show("Khôi phục thành công");
            LoadData();
        }

        private void DeleteSelectedRows()
        {
            var ids = GetCheckedIds();
            if (ids.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn để xóa");
                return;
            }

            var confirm = MessageBox.Show("Xóa vĩnh viễn các hóa đơn đã chọn?", "Xác nhận",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            // TODO: map sang dao.deleteForever(id) thật
            foreach (var id in ids) _service.DeleteForever(id);

            MessageBox.Show("Đã xóa vĩnh viễn");
            LoadData();
        }

        // ===== Header “chọn tất cả” =====
        private void PlaceHeaderCheckBox()
        {
            if (_grid.Columns.Count == 0) return;

            if (_headerCheck == null)
            {
                _headerCheck = new CheckBox
                {
                    Size = new Size(16, 16),
                    BackColor = Color.Transparent
                };
                _headerCheck.CheckedChanged += (s, e) => ToggleAll(_headerCheck.Checked);
                _grid.Controls.Add(_headerCheck);
            }

            var rect = _grid.GetCellDisplayRectangle(0, -1, true); // header cell of col 0
            var x = rect.X + (rect.Width - _headerCheck.Width) / 2;
            var y = rect.Y + (rect.Height - _headerCheck.Height) / 2;
            _headerCheck.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
            _headerCheck.BringToFront();

            // sync trạng thái theo tất cả dòng
            _headerCheck.CheckedChanged -= (s, e) => ToggleAll(_headerCheck.Checked);
            _headerCheck.Checked = AreAllChecked();
            _headerCheck.CheckedChanged += (s, e) => ToggleAll(_headerCheck.Checked);
        }

        private bool AreAllChecked()
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;
                var cell = row.Cells[0] as DataGridViewCheckBoxCell;
                if (!(cell?.Value is bool b) || !b) return false;
            }
            return _grid.Rows.Count > 0;
        }

        private void ToggleAll(bool state)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;
                var cell = row.Cells[0] as DataGridViewCheckBoxCell;
                if (cell != null) cell.Value = state;
            }
            _grid.Invalidate();
        }

        // ===== Mapping =====
        private static HoaDonRow Map(HoaDonDto h) => new HoaDonRow
        {
            IdHD = h.IdHD,
            ThoiGian = h.ThoiGian, // giữ nguyên string “dd/MM/yyyy HH:mm” nếu sẵn có
            IdNV = h.IdNV,
            IdKH = h.IdKH,
            TongTien = h.TongTien, // để dạng chuỗi như Java (có thể format phía Service/DAO)
            PhuongThucThanhToan = h.PhuongThucThanhToan,
            TrangThaiDonHang = h.TrangThaiDonHang
        };
    }

    // ====== Row bind cho DataGridView (string giống Java để search dễ) ======
    public class HoaDonRow
    {
        public string IdHD { get; set; }
        public string ThoiGian { get; set; }
        public string IdNV { get; set; }
        public string IdKH { get; set; }
        public string TongTien { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string TrangThaiDonHang { get; set; }
    }

    // ====== DTO & Service demo – thay bằng DAO thật của bạn ======
    public class HoaDonDto
    {
        public string IdHD { get; set; }
        public string ThoiGian { get; set; }                // ví dụ "23/10/2025 14:30"
        public string IdNV { get; set; }
        public string IdKH { get; set; }
        public string TongTien { get; set; }                // CHANGED: dùng string để khớp Java hiển thị/tìm kiếm
        public string PhuongThucThanhToan { get; set; }
        public string TrangThaiDonHang { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface ITrashHoaDonService
    {
        List<HoaDonDto> GetDeleted();
        void Restore(string id);
        void DeleteForever(string id);
    }

    // Demo in-memory – chỉ để bạn test UI; thay bằng HoaDonDAO thật
    public class DummyTrashHoaDonService : ITrashHoaDonService
    {
        private readonly List<HoaDonDto> _db = new List<HoaDonDto>
        {
            new HoaDonDto{ IdHD="HD001", ThoiGian="25/10/2025 10:15", IdNV="NV01", IdKH="KH10", TongTien="123000.0", PhuongThucThanhToan="Tiền mặt", TrangThaiDonHang="Đã hủy", IsDeleted=true },
            new HoaDonDto{ IdHD="HD002", ThoiGian="26/10/2025 08:20", IdNV="NV02", IdKH="KH11", TongTien="450000.0", PhuongThucThanhToan="Chuyển khoản", TrangThaiDonHang="Đã hủy", IsDeleted=true },
            new HoaDonDto{ IdHD="HD003", ThoiGian="26/10/2025 16:05", IdNV="NV03", IdKH="KH12", TongTien="789000.0", PhuongThucThanhToan="QR", TrangThaiDonHang="Đã hủy", IsDeleted=true },
        };

        public List<HoaDonDto> GetDeleted() => _db.Where(x => x.IsDeleted).ToList();

        public void Restore(string id)
        {
            var it = _db.FirstOrDefault(x => x.IdHD == id);
            if (it != null) it.IsDeleted = false;
        }

        public void DeleteForever(string id)
        {
            var it = _db.FirstOrDefault(x => x.IdHD == id);
            if (it != null) _db.Remove(it);
        }
    }
}
