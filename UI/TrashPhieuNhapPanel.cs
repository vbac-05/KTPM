using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLThuocApp.UI
{
    // CHANGED: Đặt đúng tên lớp để match Java: TrashPhieuNhapPanel (không phải TrashThuocPanel)
    public class TrashPhieuNhapPanel : UserControl
    {
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _txtSearch = new TextBox();
        private readonly Button _btnRefresh = new Button();
        private readonly Button _btnRestore = new Button();
        private readonly Button _btnDeleteForever = new Button();

        // CHANGED: nhận TabControl + tabIndex để cập nhật tiêu đề giống Java
        private readonly TabControl _parentTabbed;
        private readonly int _tabIndex;

        private CheckBox _headerCheck; // checkbox ở header cột 0

        // === DAO/Service ===
        // TODO: thay bằng DAO/service thật (tương đương PhieuNhapDAO.getDeleted()/restore/deleteForever bên Java)
        private readonly IPhieuNhapTrashService _service = new DummyPhieuNhapTrashService(); // CHANGED

        // CHANGED: ctor khớp Java: truyền TabControl + index
        public TrashPhieuNhapPanel(TabControl parentTabbed, int tabIndex)
        {
            _parentTabbed = parentTabbed;
            _tabIndex = tabIndex;

            Dock = DockStyle.Fill;
            BuildUI();
            WireEvents();
            LoadData(); // CHANGED: gọi sau khi build UI
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

            // Col 0: Checkbox chọn dòng
            var colCheck = new DataGridViewCheckBoxColumn
            {
                HeaderText = string.Empty,
                Width = 36,
                ReadOnly = false,
                Frozen = true
            };
            _grid.Columns.Add(colCheck);

            // Các cột còn lại (match Java: "", "ID Phiếu", "Thời gian", "ID Nhân viên", "Tổng tiền")
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID Phiếu", DataPropertyName = "IdPN", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thời gian", DataPropertyName = "ThoiGian", Width = 160 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID Nhân viên", DataPropertyName = "IdNV", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tổng tiền", DataPropertyName = "TongTien", Width = 120 });

            Controls.Add(_grid);

            // CHANGED: Header checkbox “select all” ở cột 0
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
                new DataColumn("IdPN", typeof(string)),
                new DataColumn("ThoiGian", typeof(string)), // TODO: nếu bạn dùng DateTime, format trước khi add
                new DataColumn("IdNV", typeof(string)),
                new DataColumn("TongTien", typeof(string)) // TODO: có thể để decimal và format ở CellFormatting
            });

            // TODO: thay lấy dữ liệu từ DAO/service thật
            var list = _service.GetDeleted(); // giống Java: dao.getDeleted()
            foreach (var pn in list)
            {
                table.Rows.Add(pn.IdPN, pn.ThoiGian, pn.IdNV, pn.TongTien);
            }

            _grid.DataSource = table;

            // Reset tất cả checkbox về false
            foreach (DataGridViewRow row in _grid.Rows)
                row.Cells[0].Value = false;

            // Cập nhật tiêu đề Tab: "Phiếu nhập (N)" — match Java
            if (_parentTabbed != null && _tabIndex >= 0 && _tabIndex < _parentTabbed.TabPages.Count)
            {
                _parentTabbed.TabPages[_tabIndex].Text = $"Phiếu nhập ({GetDeletedCount()})"; // CHANGED
            }
        }

        private int GetDeletedCount()
        {
            // TODO: trả về count thực từ DAO nếu cần
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

        // ============== Realtime search (match Java: chọn dòng khớp từ khóa) ==============
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

        // ============== Helpers cho hành động nút ==============
        private string[] GetSelectedIds()
        {
            var ids = _grid.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value is bool b && b)
                .Select(r => Convert.ToString(r.Cells[1].Value)) // cột 1 là "ID Phiếu" (IdPN)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
            return ids;
        }

        private void RestoreSelectedRows()
        {
            var ids = GetSelectedIds();
            if (ids.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập để khôi phục", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // TODO: gọi DAO thật – giống Java: dao.restore(id);
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
                MessageBox.Show("Vui lòng chọn phiếu nhập để xóa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var confirm = MessageBox.Show("Xóa vĩnh viễn các phiếu nhập đã chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                // TODO: gọi DAO thật – giống Java: dao.deleteForever(id);
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
    public interface IPhieuNhapTrashService
    {
        System.Collections.Generic.List<PhieuNhapRow> GetDeleted();
        void Restore(string id);
        void DeleteForever(string id);
    }

    // CHANGED: Model nhẹ để bind DataGridView
    public class PhieuNhapRow
    {
        public string IdPN { get; set; }
        public string ThoiGian { get; set; } // TODO: đổi DateTime nếu cần và format ở UI
        public string IdNV { get; set; }
        public string TongTien { get; set; } // TODO: decimal + định dạng tiền tệ nếu cần
    }

    // CHANGED: Dummy service cho chạy thử UI. TODO: bỏ đi và nối DAO thật.
    public class DummyPhieuNhapTrashService : IPhieuNhapTrashService
    {
        private readonly System.Collections.Generic.List<PhieuNhapRow> _data =
            new System.Collections.Generic.List<PhieuNhapRow>
            {
                new PhieuNhapRow{ IdPN="PN001", ThoiGian=DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy HH:mm"), IdNV="NV01", TongTien="123000.0" },
                new PhieuNhapRow{ IdPN="PN002", ThoiGian=DateTime.Now.AddDays(-2).ToString("dd/MM/yyyy HH:mm"), IdNV="NV02", TongTien="450000.0" },
                new PhieuNhapRow{ IdPN="PN003", ThoiGian=DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy HH:mm"), IdNV="NV03", TongTien="789000.0" },
            };

        public System.Collections.Generic.List<PhieuNhapRow> GetDeleted() => _data.ToList();

        public void Restore(string id)
        {
            var it = _data.FirstOrDefault(x => x.IdPN == id);
            if (it != null) _data.Remove(it);
        }

        public void DeleteForever(string id)
        {
            var it = _data.FirstOrDefault(x => x.IdPN == id);
            if (it != null) _data.Remove(it);
        }
    }
}
