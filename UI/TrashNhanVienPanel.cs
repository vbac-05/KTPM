using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
// TODO: nếu có DAO thật, thêm using tới namespace DAO của bạn
// using QLThuocWin.DAO;

namespace QLThuocWin.UI
{
    public class TrashNhanVienPanel : UserControl
    {
        // ===== Service/State =====
        // TODO: thay DummyTrashNhanVienService bằng adapter gọi NhanVienDAO thật
        private readonly ITrashNhanVienService _service = new DummyTrashNhanVienService();

        private readonly BindingList<NVRow> _rows = new BindingList<NVRow>();
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _txtSearch = new TextBox();

        private readonly Button _btnRefresh = new Button();
        private readonly Button _btnRestore = new Button();
        private readonly Button _btnDeleteForever = new Button();

        private CheckBox _headerCheck; // CHANGED: checkbox “chọn tất cả” ở header

        // CHANGED: hỗ trợ cập nhật tiêu đề tab như Java
        private readonly TabControl _parentTab;
        private readonly int _tabIndex;

        public TrashNhanVienPanel(TabControl parentTab = null, int tabIndex = -1)
        {
            _parentTab = parentTab;
            _tabIndex = tabIndex;

            Dock = DockStyle.Fill;
            BuildSearchBar();
            BuildGrid();
            BuildButtons();

            LoadData();
        }

        // ====== UI: thanh tìm kiếm realtime ======
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

        // ====== UI: DataGridView ======
        private void BuildGrid()
        {
            _grid.Dock = DockStyle.Fill;
            _grid.AutoGenerateColumns = false;
            _grid.AllowUserToAddRows = false;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = true; // CHANGED: cho phép chọn nhiều dòng
            Controls.Add(_grid);

            // Cột 0: checkbox (không bind)
            var colCheck = new DataGridViewCheckBoxColumn
            {
                HeaderText = string.Empty,
                Width = 36,
                ReadOnly = false
            };
            _grid.Columns.Add(colCheck);

            // CHANGED: cột khớp Java: "", "ID Nhân viên", "Họ tên","SĐT", "Giới tính", "Năm sinh", "Ngày vào làm", "Lương", "Trạng thái"
            _grid.Columns.Add(MakeText("ID Nhân viên", nameof(NVRow.IdNV), 120));
            _grid.Columns.Add(MakeText("Họ tên", nameof(NVRow.HoTen), 200));
            _grid.Columns.Add(MakeText("SĐT", nameof(NVRow.Sdt), 110));
            _grid.Columns.Add(MakeText("Giới tính", nameof(NVRow.GioiTinh), 80));
            _grid.Columns.Add(MakeText("Năm sinh", nameof(NVRow.NamSinh), 80));
            _grid.Columns.Add(MakeText("Ngày vào làm", nameof(NVRow.NgayVaoLam), 130));
            _grid.Columns.Add(MakeText("Lương", nameof(NVRow.Luong), 100));
            _grid.Columns.Add(MakeText("Trạng thái", nameof(NVRow.TrangThai), 100));

            _grid.DataSource = _rows;

            // CHANGED: đặt checkbox “chọn tất cả” lên header cột 0
            _grid.ColumnWidthChanged += (s, e) => PlaceHeaderCheckBox();
            _grid.Scroll += (s, e) => PlaceHeaderCheckBox();
            _grid.HandleCreated += (s, e) => PlaceHeaderCheckBox();
            _grid.DataBindingComplete += (s, e) => PlaceHeaderCheckBox();
        }

        private static DataGridViewTextBoxColumn MakeText(string header, string prop, int width)
            => new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = true
            };

        // ====== UI: Buttons ======
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

        // ====== Data ======
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
                _parentTab.TabPages[_tabIndex].Text = $"Nhân viên ({_service.GetDeleted().Count})";
        }

        // ====== Search realtime: chọn các dòng khớp ======
        private void ApplyRealtimeSearch()
        {
            var text = _txtSearch.Text.Trim().ToLowerInvariant();
            _grid.ClearSelection();
            if (string.IsNullOrEmpty(text)) return;

            for (int i = 0; i < _rows.Count; i++)
            {
                var r = _rows[i];
                // CHANGED: đối chiếu từ cột 1 trở đi (bỏ checkbox)
                if (Contains(r.IdNV, text) ||
                    Contains(r.HoTen, text) ||
                    Contains(r.Sdt, text) ||
                    Contains(r.GioiTinh, text) ||
                    Contains(r.NamSinh, text) ||
                    Contains(r.NgayVaoLam, text) ||
                    Contains(r.Luong, text) ||
                    Contains(r.TrangThai, text))
                {
                    _grid.Rows[i].Selected = true;
                }
            }
        }

        private static bool Contains(string src, string q)
            => !string.IsNullOrEmpty(src) && src.ToLowerInvariant().Contains(q);

        // ====== Header checkbox (chọn tất cả) ======
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

            var rect = _grid.GetCellDisplayRectangle(0, -1, true); // header cell col 0
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

        // ====== Actions ======
        private List<string> GetCheckedIds()
        {
            var ids = new List<string>();
            // cột 0: checkbox; cột 1: ID Nhân viên
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
                MessageBox.Show("Vui lòng chọn nhân viên để khôi phục");
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
                MessageBox.Show("Vui lòng chọn nhân viên để xóa");
                return;
            }

            var confirm = MessageBox.Show("Xóa vĩnh viễn nhân viên đã chọn?", "Xác nhận",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            // TODO: map sang dao.deleteForever(id)
            foreach (var id in ids) _service.DeleteForever(id);

            MessageBox.Show("Đã xóa vĩnh viễn");
            LoadData();
        }

        // ====== Mapping bind row (string để search thống nhất) ======
        private static NVRow Map(NVDto n) => new NVRow
        {
            IdNV       = n.IdNV,
            HoTen      = n.HoTen,
            Sdt        = n.Sdt,
            GioiTinh   = n.GioiTinh,
            NamSinh    = n.NamSinh?.ToString() ?? string.Empty,
            NgayVaoLam = n.NgayVaoLam,      // đã format sẵn trong DTO (nếu cần)
            Luong      = n.Luong,           // để dạng string cho search đồng nhất
            TrangThai  = n.TrangThai
        };
    }

    // ====== Row bind lên DataGridView ======
    public class NVRow
    {
        public string IdNV { get; set; }
        public string HoTen { get; set; }
        public string Sdt { get; set; }
        public string GioiTinh { get; set; }
        public string NamSinh { get; set; }
        public string NgayVaoLam { get; set; }
        public string Luong { get; set; }
        public string TrangThai { get; set; }
    }

    // ====== DTO & Service demo – thay bằng NhanVienDAO thật ======
    public class NVDto
    {
        public string IdNV { get; set; }
        public string HoTen { get; set; }
        public string Sdt { get; set; }
        public string GioiTinh { get; set; }
        public int?   NamSinh { get; set; }
        public string NgayVaoLam { get; set; } // dd/MM/yyyy theo Java, nếu muốn
        public string Luong { get; set; }      // để string cho thống nhất grid/search
        public string TrangThai { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface ITrashNhanVienService
    {
        List<NVDto> GetDeleted();
        void Restore(string id);
        void DeleteForever(string id);
    }

    // Demo in-memory – chỉ để test UI; thay bằng DAO thật
    public class DummyTrashNhanVienService : ITrashNhanVienService
    {
        private readonly List<NVDto> _db = new List<NVDto>
        {
            new NVDto{ IdNV="NV01", HoTen="Nguyễn Văn A", Sdt="0901234567", GioiTinh="Nam", NamSinh=1998, NgayVaoLam="01/01/2023", Luong="12,000,000", TrangThai="Đã nghỉ", IsDeleted=true },
            new NVDto{ IdNV="NV02", HoTen="Trần Thị B",   Sdt="0902345678", GioiTinh="Nữ",  NamSinh=1999, NgayVaoLam="15/03/2023", Luong="10,500,000", TrangThai="Đã nghỉ", IsDeleted=true },
            new NVDto{ IdNV="NV03", HoTen="Lê Văn C",     Sdt="0903456789", GioiTinh="Nam", NamSinh=1997, NgayVaoLam="20/06/2022", Luong="14,000,000", TrangThai="Đã nghỉ", IsDeleted=true },
        };

        public List<NVDto> GetDeleted() => _db.Where(x => x.IsDeleted).ToList();

        public void Restore(string id)
        {
            var it = _db.FirstOrDefault(x => x.IdNV == id);
            if (it != null) it.IsDeleted = false;
        }

        public void DeleteForever(string id)
        {
            var it = _db.FirstOrDefault(x => x.IdNV == id);
            if (it != null) _db.Remove(it);
        }
    }

    // ====== Gợi ý adapter DAO thật (CHỈ LÀ MẪU – bạn map theo DAO của bạn) ======
    // public class DaoTrashNhanVienService : ITrashNhanVienService
    // {
    //     private readonly NhanVienDAO _dao;
    //     public DaoTrashNhanVienService(NhanVienDAO dao) { _dao = dao; }
    //
    //     public List<NVDto> GetDeleted()
    //         => _dao.getDeleted() // CHANGED: tên hàm giống Java
    //               .Select(nv => new NVDto {
    //                   IdNV       = nv.getIdNV(),
    //                   HoTen      = nv.getHoTen(),
    //                   Sdt        = nv.getSdt(),
    //                   GioiTinh   = nv.getGioiTinh(),
    //                   NamSinh    = nv.getNamSinh(),
    //                   NgayVaoLam = /* CHANGED: format dd/MM/yyyy nếu cần */ (nv.getNgayVaoLam()!=null ? new DateTime(nv.getNgayVaoLam().getTime()).ToString("dd/MM/yyyy") : ""),
    //                   Luong      = nv.getLuong(),   // nếu double -> nv.getLuong().ToString(CultureInfo)
    //                   TrangThai  = nv.getTrangThai(),
    //                   IsDeleted  = true
    //               }).ToList();
    //
    //     public void Restore(string id) => _dao.restore(id);          // CHANGED
    //     public void DeleteForever(string id) => _dao.deleteForever(id); // CHANGED
    // }
}
