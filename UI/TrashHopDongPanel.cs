using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
// TODO: đổi using/namespace nếu bạn đã có DAO thật
// using QLThuocWin.Services;

namespace QLThuocWin.UI
{
    public class TrashHopDongPanel : UserControl
    {
        // ===== Service & state =====
        // TODO: thay bằng DAO/Service thật dùng HopDongDAO
        private readonly ITrashHopDongService _service = new DummyTrashHopDongService();

        private readonly BindingList<HopDongRow> _rows = new BindingList<HopDongRow>();
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _txtSearch = new TextBox();

        private readonly Button _btnRefresh = new Button();
        private readonly Button _btnRestore = new Button();
        private readonly Button _btnDeleteForever = new Button();

        private CheckBox _headerCheck; // CHANGED: checkbox “chọn tất cả” ở header

        // CHANGED: hỗ trợ cập nhật tiêu đề tab như Java
        private readonly TabControl _parentTab;
        private readonly int _tabIndex;

        public TrashHopDongPanel(TabControl parentTab = null, int tabIndex = -1)
        {
            _parentTab = parentTab;
            _tabIndex = tabIndex;

            Dock = DockStyle.Fill;
            BuildSearchBar();
            BuildGrid();
            BuildButtons();

            LoadData();
        }

        // ===== UI =====
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
            _grid.MultiSelect = true; // để chọn nhiều dòng giống Java addRowSelectionInterval
            Controls.Add(_grid);

            // Cột 0: checkbox (không bind)
            var colCheck = new DataGridViewCheckBoxColumn
            {
                HeaderText = string.Empty,
                Width = 36,
                ReadOnly = false
            };
            _grid.Columns.Add(colCheck);

            // CHANGED: cột khớp Java: "", "ID", "Ngày bắt đầu", "Ngày kết thúc", "Nội dung", "ID Nhân viên", "ID Nhà cung cấp", "Trạng thái"
            _grid.Columns.Add(MakeText("ID", nameof(HopDongRow.Id), 100));
            _grid.Columns.Add(MakeText("Ngày bắt đầu", nameof(HopDongRow.NgayBatDau), 120));
            _grid.Columns.Add(MakeText("Ngày kết thúc", nameof(HopDongRow.NgayKetThuc), 120));
            _grid.Columns.Add(MakeText("Nội dung", nameof(HopDongRow.NoiDung), 220));
            _grid.Columns.Add(MakeText("ID Nhân viên", nameof(HopDongRow.IdNV), 110));
            _grid.Columns.Add(MakeText("ID Nhà cung cấp", nameof(HopDongRow.IdNCC), 130));
            _grid.Columns.Add(MakeText("Trạng thái", nameof(HopDongRow.TrangThai), 110));

            _grid.DataSource = _rows;

            // CHANGED: header checkbox “chọn tất cả”
            _grid.ColumnWidthChanged += (s, e) => PlaceHeaderCheckBox();
            _grid.Scroll += (s, e) => PlaceHeaderCheckBox();
            _grid.HandleCreated += (s, e) => PlaceHeaderCheckBox();
            _grid.DataBindingComplete += (s, e) => PlaceHeaderCheckBox();
        }

        private static DataGridViewTextBoxColumn MakeText(string header, string prop, int width)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = true
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
                _parentTab.TabPages[_tabIndex].Text = $"Hợp đồng ({_service.GetDeleted().Count})";
            }
        }

        // ===== Search (realtime select như Java) =====
        private void ApplyRealtimeSearch()
        {
            var text = _txtSearch.Text.Trim().ToLowerInvariant();
            _grid.ClearSelection();
            if (string.IsNullOrEmpty(text)) return;

            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                // CHANGED: duyệt từ cột 1 trở đi (bỏ checkbox)
                if (Contains(r.Id, text) ||
                    Contains(r.NgayBatDau, text) ||
                    Contains(r.NgayKetThuc, text) ||
                    Contains(r.NoiDung, text) ||
                    Contains(r.IdNV, text) ||
                    Contains(r.IdNCC, text) ||
                    Contains(r.TrangThai, text))
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
            // cột 0: checkbox; cột 1: ID
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;
                var cell = row.Cells[0] as DataGridViewCheckBoxCell;
                var isChecked = cell?.Value is bool b && b;
                if (isChecked)
                    ids.Add(Convert.ToString(row.Cells[1].Value));
            }
            return ids;
        }

        private void RestoreSelectedRows()
        {
            var ids = GetCheckedIds();
            if (ids.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng để khôi phục");
                return;
            }

            // TODO: map sang dao.restore(id)
            foreach (var id in ids) _service.Restore(id);

            MessageBox.Show("Khôi phục thành công");
            LoadData();
        }

        private void DeleteSelectedRows()
        {
            var ids = GetCheckedIds();
            if (ids.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng để xóa");
                return;
            }

            var confirm = MessageBox.Show("Xóa vĩnh viễn các hợp đồng đã chọn?", "Xác nhận",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            // TODO: map sang dao.deleteForever(id)
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

            // sync trạng thái
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
        private static HopDongRow Map(HopDongDto h) => new HopDongRow
        {
            Id = h.Id,
            NgayBatDau = h.NgayBatDau,   // giữ kiểu string “dd/MM/yyyy” nếu DAO đã format
            NgayKetThuc = h.NgayKetThuc, // tương tự
            NoiDung = h.NoiDung,
            IdNV = h.IdNV,
            IdNCC = h.IdNCC,
            TrangThai = h.TrangThai
        };
    }

    // ====== Row bind cho DataGridView (string để search theo Java) ======
    public class HopDongRow
    {
        public string Id { get; set; }
        public string NgayBatDau { get; set; }
        public string NgayKetThuc { get; set; }
        public string NoiDung { get; set; }
        public string IdNV { get; set; }
        public string IdNCC { get; set; }
        public string TrangThai { get; set; }
    }

    // ====== DTO & Service demo – thay bằng HopDongDAO thật ======
    public class HopDongDto
    {
        public string Id { get; set; }
        public string NgayBatDau { get; set; }   // ví dụ "01/10/2025"
        public string NgayKetThuc { get; set; }  // ví dụ "01/11/2025"
        public string NoiDung { get; set; }
        public string IdNV { get; set; }
        public string IdNCC { get; set; }
        public string TrangThai { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface ITrashHopDongService
    {
        List<HopDongDto> GetDeleted();
        void Restore(string id);
        void DeleteForever(string id);
    }

    // Demo in-memory – chỉ để test UI; thay bằng HopDongDAO thật
    public class DummyTrashHopDongService : ITrashHopDongService
    {
        private readonly List<HopDongDto> _db = new List<HopDongDto>
        {
            new HopDongDto{ Id="HDG-001", NgayBatDau="01/10/2025", NgayKetThuc="01/11/2025", NoiDung="Cung cấp dược phẩm A", IdNV="NV01", IdNCC="NCC01", TrangThai="Đã hủy", IsDeleted=true },
            new HopDongDto{ Id="HDG-002", NgayBatDau="05/10/2025", NgayKetThuc="05/12/2025", NoiDung="Bảo trì thiết bị", IdNV="NV02", IdNCC="NCC02", TrangThai="Đã hủy", IsDeleted=true },
            new HopDongDto{ Id="HDG-003", NgayBatDau="10/10/2025", NgayKetThuc="10/01/2026", NoiDung="Cung ứng vật tư y tế", IdNV="NV03", IdNCC="NCC03", TrangThai="Đã hủy", IsDeleted=true },
        };

        public List<HopDongDto> GetDeleted() => _db.Where(x => x.IsDeleted).ToList();

        public void Restore(string id)
        {
            var it = _db.FirstOrDefault(x => x.Id == id);
            if (it != null) it.IsDeleted = false;
        }

        public void DeleteForever(string id)
        {
            var it = _db.FirstOrDefault(x => x.Id == id);
            if (it != null) _db.Remove(it);
        }
    }
}
