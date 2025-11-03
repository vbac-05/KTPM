using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
// TODO: đổi using/namespace nếu bạn đã có DAO thật
// using QLThuocApp.Services;

namespace QLThuocApp.UI
{
    public class TrashNhaCungCapPanel : UserControl
    {
        // ===== Service & state =====
        // TODO: thay bằng DAO/Service thật dùng NhaCungCapDAO
        private readonly ITrashNCCService _service = new DummyTrashNCCService();

        private readonly BindingList<NCCRow> _rows = new BindingList<NCCRow>();
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _txtSearch = new TextBox();

        private readonly Button _btnRefresh = new Button();
        private readonly Button _btnRestore = new Button();
        private readonly Button _btnDeleteForever = new Button();

        private CheckBox _headerCheck; // CHANGED: checkbox “chọn tất cả” ở header

        // CHANGED: hỗ trợ cập nhật tiêu đề tab như Java
        private readonly TabControl _parentTab;
        private readonly int _tabIndex;

        public TrashNhaCungCapPanel(TabControl parentTab = null, int tabIndex = -1)
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
            _txtSearch.TextChanged += (s, e) => ApplyRealtimeSearch(); // CHANGED: realtime như Java
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

            // CHANGED: cột khớp Java: "", "ID Nhà cung cấp", "Tên NCC", "SĐT", "Địa chỉ"
            _grid.Columns.Add(MakeText("ID Nhà cung cấp", nameof(NCCRow.IdNCC), 130));
            _grid.Columns.Add(MakeText("Tên NCC", nameof(NCCRow.TenNCC), 200));
            _grid.Columns.Add(MakeText("SĐT", nameof(NCCRow.Sdt), 110));
            _grid.Columns.Add(MakeText("Địa chỉ", nameof(NCCRow.DiaChi), 260));

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
                _parentTab.TabPages[_tabIndex].Text = $"Nhà cung cấp ({_service.GetDeleted().Count})";
            }
        }

        // ===== Search (realtime – chọn các dòng khớp) =====
        private void ApplyRealtimeSearch()
        {
            var text = _txtSearch.Text.Trim().ToLowerInvariant();
            _grid.ClearSelection();
            if (string.IsNullOrEmpty(text)) return;

            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                // CHANGED: duyệt từ cột 1 trở đi (bỏ checkbox)
                if (Contains(r.IdNCC, text) ||
                    Contains(r.TenNCC, text) ||
                    Contains(r.Sdt, text) ||
                    Contains(r.DiaChi, text))
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
            // cột 0: checkbox; cột 1: ID Nhà cung cấp
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
                MessageBox.Show("Vui lòng chọn nhà cung cấp để khôi phục");
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
                MessageBox.Show("Vui lòng chọn nhà cung cấp để xóa");
                return;
            }

            var confirm = MessageBox.Show("Xóa vĩnh viễn các nhà cung cấp đã chọn?", "Xác nhận",
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
        private static NCCRow Map(NCCDto n) => new NCCRow
        {
            IdNCC = n.IdNCC,
            TenNCC = n.TenNCC,
            Sdt = n.Sdt,
            DiaChi = n.DiaChi
        };
    }

    // ====== Row bind cho DataGridView (string để search theo Java) ======
    public class NCCRow
    {
        public string IdNCC { get; set; }
        public string TenNCC { get; set; }
        public string Sdt { get; set; }
        public string DiaChi { get; set; }
    }

    // ====== DTO & Service demo – thay bằng NhaCungCapDAO thật ======
    public class NCCDto
    {
        public string IdNCC { get; set; }
        public string TenNCC { get; set; }
        public string Sdt { get; set; }
        public string DiaChi { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface ITrashNCCService
    {
        List<NCCDto> GetDeleted();
        void Restore(string id);
        void DeleteForever(string id);
    }

    // Demo in-memory – chỉ để test UI; thay bằng DAO thật
    public class DummyTrashNCCService : ITrashNCCService
    {
        private readonly List<NCCDto> _db = new List<NCCDto>
        {
            new NCCDto{ IdNCC="NCC01", TenNCC="Cty Dược A", Sdt="0901234567", DiaChi="Hà Nội", IsDeleted=true },
            new NCCDto{ IdNCC="NCC02", TenNCC="Cty Dược B", Sdt="0902345678", DiaChi="Đà Nẵng", IsDeleted=true },
            new NCCDto{ IdNCC="NCC03", TenNCC="Cty Dược C", Sdt="0903456789", DiaChi="TP.HCM", IsDeleted=true },
        };

        public List<NCCDto> GetDeleted() => _db.Where(x => x.IsDeleted).ToList();

        public void Restore(string id)
        {
            var it = _db.FirstOrDefault(x => x.IdNCC == id);
            if (it != null) it.IsDeleted = false;
        }

        public void DeleteForever(string id)
        {
            var it = _db.FirstOrDefault(x => x.IdNCC == id);
            if (it != null) _db.Remove(it);
        }
    }
}
