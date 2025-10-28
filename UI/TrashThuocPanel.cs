using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLThuocWin.UI
{
    public class TrashThuocPanel : UserControl
    {
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _txtSearch = new TextBox();
        private readonly Button _btnRefresh = new Button();
        private readonly Button _btnRestore = new Button();
        private readonly Button _btnDeleteForever = new Button();

        private readonly TabControl _parentTabbed; // CHANGED: để cập nhật tiêu đề tab "Thuốc (N)"
        private readonly int _tabIndex;

        private CheckBox _headerCheck; // checkbox ở header cột 0

        // === DAO/Service ===
        // TODO: thay bằng DAO/service thật (tương đương ThuocDAO.getDeleted()/restore()/deleteForever() bên Java)
        private readonly IThuocTrashService _service = new DummyThuocTrashService(); // CHANGED

        // CHANGED: ctor nhận TabControl + index giống Java
        public TrashThuocPanel(TabControl parentTabbed, int tabIndex)
        {
            _parentTabbed = parentTabbed;
            _tabIndex = tabIndex;

            Dock = DockStyle.Fill;
            BuildUI();
            WireEvents();
            LoadData(); // CHANGED
        }

        private void BuildUI()
        {
            // ============== SEARCH PANEL (match Java) ==============
            var searchPanel = new Panel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(5) };
            var lblSearch = new Label { Text = "Tìm kiếm: ", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            lblSearch.Location = new Point(8, 10);
            _txtSearch.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _txtSearch.Location = new Point(80, 7);
            _txtSearch.Width = 500;
            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(_txtSearch);
            Controls.Add(searchPanel);

            // ============== BUTTON BAR (match Java) ==============
            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(5),
                Height = 46
            };
            _btnRefresh.Text = "Làm mới";
            _btnRestore.Text = "Khôi phục";
            _btnDeleteForever.Text = "Xóa vĩnh viễn";
            buttons.Controls.AddRange(new Control[] { _btnDeleteForever, _btnRestore, _btnRefresh });
            Controls.Add(buttons);

            // ============== GRID ==============
            _grid.Dock = DockStyle.Fill;
            _grid.RowHeadersVisible = false;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.MultiSelect = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoGenerateColumns = false;
            _grid.RowTemplate.Height = 25;

            // Col 0: Checkbox
            var colCheck = new DataGridViewCheckBoxColumn
            {
                HeaderText = string.Empty,
                Width = 36,
                ReadOnly = false,
                Frozen = true
            };
            _grid.Columns.Add(colCheck);

            // Các cột còn lại (match Java: "", "ID Thuốc", "Tên thuốc", "Giá nhập", "Hạn sử dụng", "Số lượng", "Xuất xứ")
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID Thuốc", DataPropertyName = "IdThuoc", Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên thuốc", DataPropertyName = "TenThuoc", Width = 200 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá nhập", DataPropertyName = "GiaNhap", Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hạn sử dụng", DataPropertyName = "HanSuDung", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", DataPropertyName = "SoLuongTon", Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Xuất xứ", DataPropertyName = "XuatXu", Width = 110 });

            Controls.Add(_grid);

            // CHANGED: Header checkbox “chọn tất cả”
            _grid.HandleCreated += (s, e) => AddHeaderCheckBox();
            _grid.ColumnWidthChanged += (s, e) => RepositionHeaderCheckBox();
            _grid.Scroll += (s, e) => RepositionHeaderCheckBox();
        }

        private void WireEvents()
        {
            _btnRefresh.Click += (s, e) => LoadData();
            _btnRestore.Click += (s, e) => RestoreSelectedRows();
            _btnDeleteForever.Click += (s, e) => DeleteSelectedRows();
            _txtSearch.TextChanged += (s, e) => ApplyRealtimeSearchSelection(); // CHANGED: realtime search
        }

        // ============== Data Loading (match Java loadData) ==============
        public void LoadData() // CHANGED: public để TrashPanel gọi ngay sau addTab
        {
            var table = new DataTable();
            table.Columns.AddRange(new[]
            {
                new DataColumn("IdThuoc", typeof(string)),
                new DataColumn("TenThuoc", typeof(string)),
                new DataColumn("GiaNhap", typeof(string)),     // TODO: decimal + CellFormatting nếu muốn
                new DataColumn("HanSuDung", typeof(string)),   // TODO: DateTime + format nếu muốn
                new DataColumn("SoLuongTon", typeof(string)),  // TODO: int
                new DataColumn("XuatXu", typeof(string))
            });

            // TODO: thay bằng DAO/service thật
            var list = _service.GetDeleted();
            foreach (var t in list)
            {
                table.Rows.Add(
                    t.IdThuoc,
                    t.TenThuoc,
                    t.GiaNhap,       // đã format sẵn thành chuỗi
                    t.HanSuDung,     // đã format sẵn dd/MM/yyyy
                    t.SoLuongTon,
                    t.XuatXu
                );
            }

            _grid.DataSource = table;

            // Reset checkbox về false
            foreach (DataGridViewRow row in _grid.Rows)
                row.Cells[0].Value = false;

            // Cập nhật tiêu đề Tab: "Thuốc (N)" — match Java
            if (_parentTabbed != null && _tabIndex >= 0 && _tabIndex < _parentTabbed.TabPages.Count)
            {
                _parentTabbed.TabPages[_tabIndex].Text = $"Thuốc ({GetDeletedCount()})"; // CHANGED
            }
        }

        private int GetDeletedCount()
        {
            // TODO: trả về count thực từ DAO
            return _service.GetDeleted().Count;
        }

        // ============== Header checkbox (select all) ==============
        private void AddHeaderCheckBox()
        {
            if (_headerCheck != null) return;

            _headerCheck = new CheckBox
            {
                Size = new Size(15, 15),
                BackColor = Color.Transparent
            };
            _headerCheck.CheckedChanged += (s, e) =>
            {
                var state = _headerCheck.Checked;
                foreach (DataGridViewRow row in _grid.Rows)
                    row.Cells[0].Value = state;
            };
            _grid.Controls.Add(_headerCheck);
            RepositionHeaderCheckBox();
        }

        private void RepositionHeaderCheckBox()
        {
            if (_headerCheck == null) return;
            var rect = _grid.GetCellDisplayRectangle(0, -1, true);
            _headerCheck.Location = new Point(
                rect.X + (rect.Width - _headerCheck.Width) / 2,
                rect.Y + (rect.Height - _headerCheck.Height) / 2
            );
        }

        // ============== Realtime search (chọn các dòng khớp từ khóa) ==============
        private void ApplyRealtimeSearchSelection()
        {
            var text = (_txtSearch.Text ?? string.Empty).Trim().ToLowerInvariant();
            _grid.ClearSelection();
            if (string.IsNullOrEmpty(text))
                return;

            foreach (DataGridViewRow row in _grid.Rows)
            {
                bool match = false;
                // Duyệt các cột 1..n (bỏ cột checkbox ở 0)
                for (int j = 1; j < _grid.Columns.Count; j++)
                {
                    var val = row.Cells[j].Value?.ToString();
                    if (!string.IsNullOrEmpty(val) && val.ToLowerInvariant().Contains(text))
                    {
                        match = true;
                        break;
                    }
                }
                if (match)
                    row.Selected = true;
            }
        }

        // ============== Helpers hành động nút ==============
        private string[] GetSelectedIds()
        {
            var ids = _grid.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value is bool b && b)
                .Select(r => Convert.ToString(r.Cells[1].Value)) // cột 1: "ID Thuốc"
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
            return ids;
        }

        private void RestoreSelectedRows()
        {
            var ids = GetSelectedIds();
            if (ids.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn thuốc để khôi phục", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // TODO: gọi DAO thật – giống Java: dao.restore(id)
            foreach (var id in ids)
                _service.Restore(id);

            MessageBox.Show("Khôi phục thành công", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
        }

        private void DeleteSelectedRows()
        {
            var ids = GetSelectedIds();
            if (ids.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn thuốc để xóa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var confirm = MessageBox.Show("Xóa vĩnh viễn các thuốc đã chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                // TODO: gọi DAO thật – giống Java: dao.deleteForever(id)
                foreach (var id in ids)
                    _service.DeleteForever(id);

                MessageBox.Show("Đã xóa vĩnh viễn", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
        }
    }

    // ================== DEMO SERVICE + MODEL (TODO: thay bằng DAO của bạn) ==================
    // CHANGED: Interface giống API Java (getDeleted/restore/deleteForever)
    public interface IThuocTrashService
    {
        System.Collections.Generic.List<ThuocTrashRow> GetDeleted();
        void Restore(string id);
        void DeleteForever(string id);
    }

    // CHANGED: Model nhẹ để bind DataGridView
    public class ThuocTrashRow
    {
        public string IdThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string GiaNhap { get; set; }     // ví dụ "12000.0"
        public string HanSuDung { get; set; }   // "dd/MM/yyyy"
        public string SoLuongTon { get; set; }  // ví dụ "100"
        public string XuatXu { get; set; }
    }

    // CHANGED: Dummy service cho chạy thử UI. TODO: bỏ và nối DAO thật.
    public class DummyThuocTrashService : IThuocTrashService
    {
        private readonly System.Collections.Generic.List<ThuocTrashRow> _data =
            new System.Collections.Generic.List<ThuocTrashRow>
            {
                new ThuocTrashRow{ IdThuoc="T001", TenThuoc="Paracetamol", GiaNhap="12000.0", HanSuDung=DateTime.Today.AddYears(2).ToString("dd/MM/yyyy"), SoLuongTon="100", XuatXu="VN" },
                new ThuocTrashRow{ IdThuoc="T002", TenThuoc="Cefalexin",   GiaNhap="35000.0", HanSuDung=DateTime.Today.AddYears(1).ToString("dd/MM/yyyy"), SoLuongTon="50",  XuatXu="VN" },
                new ThuocTrashRow{ IdThuoc="T003", TenThuoc="Alpha-Choay", GiaNhap="42000.0", HanSuDung=DateTime.Today.AddMonths(18).ToString("dd/MM/yyyy"), SoLuongTon="30",  XuatXu="FR" },
            };

        public System.Collections.Generic.List<ThuocTrashRow> GetDeleted() => _data.ToList();

        public void Restore(string id)
        {
            var it = _data.FirstOrDefault(x => x.IdThuoc == id);
            if (it != null) _data.Remove(it);
        }

        public void DeleteForever(string id)
        {
            var it = _data.FirstOrDefault(x => x.IdThuoc == id);
            if (it != null) _data.Remove(it);
        }
    }
}
