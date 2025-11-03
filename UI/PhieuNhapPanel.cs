using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
// TODO: đổi namespace/using cho đúng dự án thật của bạn
// using QLThuocApp.Services;
// using QLThuocApp.DTO;

namespace QLThuocApp.UI
{
    public class PhieuNhapPanel : UserControl
    {
        // ====== Services & state ======
        // TODO: thay bằng controller/service thật của bạn (PhieuNhapController C#)
        private readonly IPhieuNhapService _service = new DummyPhieuNhapService();
        private readonly BindingList<PhieuNhapRow> _binding = new BindingList<PhieuNhapRow>();

        // ====== Controls ======
        private DataGridView grid;
        private Button btnAdd, btnEdit, btnDelete, btnViewDetail, btnRefresh, btnSearch; // CHANGED: thêm ViewDetail, Search để khớp Java
        private TextBox txtSearchIdPN, txtSearchIdNV, txtSearchIdNCC;                    // CHANGED: 3 ô tìm kiếm như Java

        public PhieuNhapPanel()
        {
            Dock = DockStyle.Fill;

            BuildToolbar();
            BuildSearchPanel();   // CHANGED: thêm panel tìm kiếm để match Java
            BuildGrid();

            ReloadTable();
        }

        // ====== UI builders ======
        private void BuildToolbar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 44 };
            Controls.Add(bar);

            btnAdd = new Button { Text = "Thêm", Width = 93, Height = 30, Location = new Point(10, 8) };
            // TODO: icon Add.png
            btnAdd.Click += (s, e) =>
            {
                // CHANGED: mở dialog Add (khớp Java)
                // TODO: gọi dialog thật AddPhieuNhapDialog và truyền owner nếu cần
                var dlg = new AddPhieuNhapDialog(); // TODO: thay constructor thật
                dlg.ShowDialog(this);
                ReloadTable();
            };
            bar.Controls.Add(btnAdd);

            btnEdit = new Button { Text = "Sửa", Width = 80, Height = 30, Location = new Point(113, 8) };
            // TODO: icon chungEdit.png
            btnEdit.Click += (s, e) => OnEdit();
            bar.Controls.Add(btnEdit);

            btnDelete = new Button { Text = "Xóa", Width = 91, Height = 30, Location = new Point(203, 8) };
            // TODO: icon chungDelete.png
            btnDelete.Click += (s, e) => OnDelete();
            bar.Controls.Add(btnDelete);

            btnViewDetail = new Button { Text = "Xem chi tiết", Width = 137, Height = 30, Location = new Point(304, 8) };
            // TODO: icon chungDetail.png
            btnViewDetail.Click += (s, e) => OnViewDetail(); // CHANGED: có nút xem chi tiết
            bar.Controls.Add(btnViewDetail);

            btnRefresh = new Button { Text = "Làm mới", Width = 150, Height = 30, Location = new Point(451, 8) };
            // TODO: icon chungRefresh.png
            btnRefresh.Click += (s, e) => ReloadTable();
            bar.Controls.Add(btnRefresh);
        }

        private void BuildSearchPanel()
        {
            var search = new Panel { Dock = DockStyle.Top, Height = 36 };
            Controls.Add(search);

            var lblIdPN = new Label { Text = "IDPN:", AutoSize = true, Location = new Point(10, 9) };
            search.Controls.Add(lblIdPN);
            txtSearchIdPN = new TextBox { Width = 120, Location = new Point(55, 6) };
            search.Controls.Add(txtSearchIdPN);

            var lblIdNV = new Label { Text = "IDNV:", AutoSize = true, Location = new Point(200, 9) };
            search.Controls.Add(lblIdNV);
            txtSearchIdNV = new TextBox { Width = 120, Location = new Point(245, 6) };
            search.Controls.Add(txtSearchIdNV);

            var lblIdNCC = new Label { Text = "IDNCC:", AutoSize = true, Location = new Point(390, 9) };
            search.Controls.Add(lblIdNCC);
            txtSearchIdNCC = new TextBox { Width = 120, Location = new Point(445, 6) };
            search.Controls.Add(txtSearchIdNCC);

            btnSearch = new Button { Text = "Tìm kiếm", Width = 120, Height = 26, Location = new Point(590, 5) };
            // TODO: icon chungSearch.png
            btnSearch.Click += (s, e) => OnSearch(); // CHANGED: nút tìm kiếm
            search.Controls.Add(btnSearch);
        }

        private void BuildGrid()
        {
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true
            };
            Controls.Add(grid);

            // CHANGED: Bảng 5 cột đúng thứ tự Java: IDPN, Thời gian, IDNV, IDNCC, Tổng tiền
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDPN", DataPropertyName = "IdPN", Width = 110 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thời gian", DataPropertyName = "ThoiGianStr", Width = 160 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDNV", DataPropertyName = "IdNV", Width = 110 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDNCC", DataPropertyName = "IdNCC", Width = 110 });

            var colTong = new DataGridViewTextBoxColumn { HeaderText = "Tổng tiền", DataPropertyName = "TongTienStr", Width = 130 };
            grid.Columns.Add(colTong);
            // Ghi chú: ta hiển thị qua string để mô phỏng String.format(\"%.1f\", ...) của Java

            grid.DataSource = _binding;

            grid.CellClick += (s, e) =>
            {
                // (Java chỉ dùng click để lấy row cho Edit/Delete/View; không bật input inline)
                // Không cần populate vào input vì Java bản của bạn dùng dialog cho Add/Edit.
            };
        }

        // ====== Actions ======
        private void OnEdit()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = grid.CurrentRow.DataBoundItem as PhieuNhapRow;
            if (row == null) return;

            // Lấy entity đầy đủ để đưa vào dialog Edit (giống Java)
            // TODO: nếu service có GetById thì dùng trực tiếp
            var item = _service.GetAll().FirstOrDefault(p => string.Equals(p.IdPN, row.IdPN, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu phiếu nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // CHANGED: mở dialog Edit như Java
            // TODO: truyền item thật vào EditPhieuNhapDialog
            var dlg = new EditPhieuNhapDialog(item); // TODO: thay constructor/mapper thật
            dlg.ShowDialog(this);
            ReloadTable();
        }

        private void OnDelete()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = grid.CurrentRow.DataBoundItem as PhieuNhapRow;
            if (row == null) return;

            var ok = MessageBox.Show($"Bạn có chắc muốn xóa phiếu nhập {row.IdPN}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            if (!ok) return;

            // CHANGED: gọi xóa thực (khớp Java)
            // TODO: gọi controller/service thật
            if (_service.Delete(row.IdPN))
            {
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadTable();
            }
            else
            {
                MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnViewDetail()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập để xem chi tiết!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = grid.CurrentRow.DataBoundItem as PhieuNhapRow;
            if (row == null) return;

            // CHANGED: mở dialog chi tiết (khớp Java)
            // TODO: truyền idPN thật vào dialog chi tiết
            var dlg = new ViewChiTietPNDialog(row.IdPN); // TODO: thay constructor thật
            dlg.ShowDialog(this);
        }

        private void OnSearch()
        {
            var idPN = txtSearchIdPN.Text.Trim();
            var idNV = txtSearchIdNV.Text.Trim();
            var idNCC = txtSearchIdNCC.Text.Trim();

            // CHANGED: gọi search theo 3 tiêu chí (khớp Java)
            // TODO: gọi controller.searchPhieuNhap(idPN, idNV, idNCC)
            var results = _service.Search(idPN, idNV, idNCC);

            _binding.Clear();
            foreach (var it in results.Select(MapToRow)) _binding.Add(it);

            if (_binding.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
            }
        }

        private void ReloadTable()
        {
            var all = _service.GetAll();
            _binding.Clear();
            foreach (var it in all.Select(MapToRow)) _binding.Add(it);
        }

        // ====== Mappers & helpers ======
        private static PhieuNhapRow MapToRow(PhieuNhapDto d) => new PhieuNhapRow
        {
            IdPN = d.IdPN,
            ThoiGianStr = d.ThoiGian.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture), // CHANGED: match Java DateHelper.toString
            IdNV = d.IdNV,
            IdNCC = d.IdNCC,
            TongTienStr = d.TongTien.ToString("0.0", CultureInfo.InvariantCulture) // CHANGED: mô phỏng String.format("%.1f")
        };
    }

    // ====== View row for grid (match Java formatting) ======
    public class PhieuNhapRow
    {
        public string IdPN { get; set; }
        public string ThoiGianStr { get; set; } // dd/MM/yyyy HH:mm
        public string IdNV { get; set; }
        public string IdNCC { get; set; }
        public string TongTienStr { get; set; } // one decimal like Java
    }

    // ====== DTO & Service mẫu (demo). Thay bằng Controller/DAO thật của bạn ======
    // TODO: thay bằng entities.PhieuNhap + PhieuNhapController C# của bạn
    public class PhieuNhapDto
    {
        public string IdPN { get; set; }
        public DateTime ThoiGian { get; set; }
        public string IdNV { get; set; }
        public string IdNCC { get; set; }
        public double TongTien { get; set; }
    }

    public interface IPhieuNhapService
    {
        List<PhieuNhapDto> GetAll();
        List<PhieuNhapDto> Search(string idPN, string idNV, string idNCC);
        bool Delete(string idPN);
    }

    // In-memory demo service
    public class DummyPhieuNhapService : IPhieuNhapService
    {
        private readonly List<PhieuNhapDto> _db = new List<PhieuNhapDto>
        {
            new PhieuNhapDto{ IdPN="PN001", ThoiGian=new DateTime(2025,10,1,8,30,0), IdNV="NV01", IdNCC="NCC01", TongTien=123000.0 },
            new PhieuNhapDto{ IdPN="PN002", ThoiGian=new DateTime(2025,10,5,14,15,0), IdNV="NV02", IdNCC="NCC02", TongTien=9876543.2 },
        };

        public List<PhieuNhapDto> GetAll() => _db.Select(Clone).ToList();

        public List<PhieuNhapDto> Search(string idPN, string idNV, string idNCC)
        {
            var q = _db.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(idPN))
                q = q.Where(x => x.IdPN.IndexOf(idPN, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(idNV))
                q = q.Where(x => x.IdNV.IndexOf(idNV, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(idNCC))
                q = q.Where(x => x.IdNCC.IndexOf(idNCC, StringComparison.OrdinalIgnoreCase) >= 0);
            return q.Select(Clone).ToList();
        }

        public bool Delete(string idPN)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdPN, idPN, StringComparison.OrdinalIgnoreCase));
            if (it == null) return false;
            _db.Remove(it);
            return true;
        }

        private static PhieuNhapDto Clone(PhieuNhapDto s) => new PhieuNhapDto
        {
            IdPN = s.IdPN,
            ThoiGian = s.ThoiGian,
            IdNV = s.IdNV,
            IdNCC = s.IdNCC,
            TongTien = s.TongTien
        };
    }

    // ====== Stub dialog classes để biên dịch chạy thử ======
    // TODO: thay bằng dialog thật của bạn
    public class AddPhieuNhapDialog : Form
    {
        public AddPhieuNhapDialog()
        {
            Text = "Thêm phiếu nhập (demo)";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(400, 200);
            Controls.Add(new Label { Text = "Dialog thêm PN (demo)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
        }
    }

    public class EditPhieuNhapDialog : Form
    {
        public EditPhieuNhapDialog(PhieuNhapDto pn)
        {
            Text = "Sửa phiếu nhập (demo)";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(400, 200);
            Controls.Add(new Label { Text = $"Sửa: {pn?.IdPN}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
        }
    }

    public class ViewChiTietPNDialog : Form
    {
        public ViewChiTietPNDialog(string idPN)
        {
            Text = "Chi tiết phiếu nhập (demo)";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(480, 300);
            Controls.Add(new Label { Text = $"Chi tiết PN: {idPN}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
        }
    }
}
