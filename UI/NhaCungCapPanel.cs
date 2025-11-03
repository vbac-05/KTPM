using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
// TODO: đổi namespace dịch vụ/DTO theo project của bạn
// using QLThuocApp.Services;
// using QLThuocWin.DTO;

namespace QLThuocApp.UI
{
    public class NhaCungCapPanel : UserControl
    {
        // ====== Services & state ======
        // TODO: thay thế bằng controller/service thật của bạn
        private readonly INccService _service = new DummyNccService();

        private string _mode = "NONE"; // "NONE" | "ADDING" | "EDITING"
        private readonly BindingList<NhaCungCapDto> _binding = new BindingList<NhaCungCapDto>();

        // ====== Controls ======
        private DataGridView grid;
        private TextBox txtSearchIdNCC, txtSearchTenNCC;
        private Button btnSearch, btnAdd, btnEdit, btnDelete, btnRefresh;

        private Panel inputPanel;
        private TextBox txtIdNCC, txtTenNCC, txtSdt, txtDiaChi;
        private Button btnSave, btnCancel;

        public NhaCungCapPanel()
        {
            Dock = DockStyle.Fill;
            BuildToolbar();
            BuildSearchPanel();
            BuildInputPanel(visible: false);
            BuildGrid();

            ReloadTable();
        }

        // ====== UI builder ======
        private void BuildToolbar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 44 };
            Controls.Add(bar);

            btnAdd = new Button { Text = "Thêm", Width = 100, Height = 30, Location = new Point(10, 8) };
            // TODO: gắn icon nếu có Resources: btnAdd.Image = Properties.Resources.add;
            btnAdd.Click += (s, e) => OnAdd();
            bar.Controls.Add(btnAdd);

            btnEdit = new Button { Text = "Sửa", Width = 100, Height = 30, Location = new Point(131, 8) };
            // TODO: icon
            btnEdit.Click += (s, e) => OnEdit();
            bar.Controls.Add(btnEdit);

            btnDelete = new Button { Text = "Xóa", Width = 100, Height = 30, Location = new Point(257, 8) };
            // TODO: icon
            btnDelete.Click += (s, e) => OnDelete();
            bar.Controls.Add(btnDelete);

            btnRefresh = new Button { Text = "Làm mới", Width = 120, Height = 30, Location = new Point(386, 8) };
            // TODO: icon
            btnRefresh.Click += (s, e) => OnRefresh();
            bar.Controls.Add(btnRefresh);
        }

        private void BuildSearchPanel()
        {
            var search = new Panel { Dock = DockStyle.Top, Height = 36 };
            Controls.Add(search);

            var lblId = new Label { Text = "IDNCC:", AutoSize = true, Location = new Point(10, 9) };
            search.Controls.Add(lblId);

            txtSearchIdNCC = new TextBox { Width = 120, Location = new Point(65, 6) };
            search.Controls.Add(txtSearchIdNCC);

            var lblTen = new Label { Text = "Tên NCC:", AutoSize = true, Location = new Point(200, 9) };
            search.Controls.Add(lblTen);

            txtSearchTenNCC = new TextBox { Width = 150, Location = new Point(265, 6) };
            search.Controls.Add(txtSearchTenNCC);

            btnSearch = new Button { Text = "Tìm kiếm", Width = 120, Height = 26, Location = new Point(430, 5) };
            // TODO: icon
            btnSearch.Click += (s, e) => OnSearch();
            search.Controls.Add(btnSearch);
        }

        private void BuildInputPanel(bool visible)
        {
            inputPanel = new Panel { Dock = DockStyle.Top, Height = 100, Visible = visible };
            Controls.Add(inputPanel);

            var lblId = new Label { Text = "IDNCC:", AutoSize = true, Location = new Point(10, 14) };
            inputPanel.Controls.Add(lblId);
            txtIdNCC = new TextBox { Width = 120, Location = new Point(70, 10) };
            inputPanel.Controls.Add(txtIdNCC);

            var lblTen = new Label { Text = "Tên NCC:", AutoSize = true, Location = new Point(210, 14) };
            inputPanel.Controls.Add(lblTen);
            txtTenNCC = new TextBox { Width = 380, Location = new Point(275, 10) };
            inputPanel.Controls.Add(txtTenNCC);

            var lblDiaChi = new Label { Text = "Địa chỉ:", AutoSize = true, Location = new Point(10, 48) };
            inputPanel.Controls.Add(lblDiaChi);
            txtDiaChi = new TextBox { Width = 400, Location = new Point(70, 44) };
            inputPanel.Controls.Add(txtDiaChi);

            var lblSdt = new Label { Text = "SĐT:", AutoSize = true, Location = new Point(490, 48) };
            inputPanel.Controls.Add(lblSdt);
            txtSdt = new TextBox { Width = 120, Location = new Point(525, 44) };
            inputPanel.Controls.Add(txtSdt);

            btnSave = new Button { Text = "Lưu", Width = 110, Height = 30, Location = new Point(750, 10) };
            // TODO: icon
            btnSave.Click += (s, e) => OnSave();
            inputPanel.Controls.Add(btnSave);

            btnCancel = new Button { Text = "Hủy", Width = 110, Height = 30, Location = new Point(750, 50) };
            // TODO: icon
            btnCancel.Click += (s, e) => OnCancel();
            inputPanel.Controls.Add(btnCancel);
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

            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDNCC", DataPropertyName = "IdNCC", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên NCC", DataPropertyName = "TenNCC", Width = 240 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SĐT", DataPropertyName = "Sdt", Width = 140 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Địa chỉ", DataPropertyName = "DiaChi", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            grid.DataSource = _binding;

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && _mode == "NONE")
                {
                    var item = _binding[e.RowIndex];
                    PopulateInput(item);
                }
            };
        }

        // ====== Actions ======
        private void OnAdd()
        {
            _mode = "ADDING";
            inputPanel.Visible = true;

            txtIdNCC.Text = "";
            txtTenNCC.Text = "";
            txtSdt.Text = "";
            txtDiaChi.Text = "";

            txtIdNCC.ReadOnly = false;

            EnableTopAndSearch(false);
            grid.Enabled = false;
        }

        private void OnEdit()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _mode = "EDITING";
            inputPanel.Visible = true;

            var item = grid.CurrentRow?.DataBoundItem as NhaCungCapDto;
            if (item != null) PopulateInput(item);

            txtIdNCC.ReadOnly = true;

            EnableTopAndSearch(false);
            grid.Enabled = false;
        }

        private void OnDelete()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var item = grid.CurrentRow?.DataBoundItem as NhaCungCapDto;
            if (item == null) return;

            var ok = MessageBox.Show($"Bạn có chắc muốn xóa Nhà cung cấp {item.IdNCC}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            if (!ok) return;

            // TODO: gọi controller/service thật
            if (_service.Delete(item.IdNCC))
            {
                MessageBox.Show("Xóa thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadTable();
            }
            else
            {
                MessageBox.Show("Xóa thất bại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnRefresh()
        {
            HideInputPanel();
            ReloadTable();
        }

        private void OnSearch()
        {
            var id = txtSearchIdNCC.Text.Trim();
            var ten = txtSearchTenNCC.Text.Trim();

            // TODO: gọi controller.searchNhaCungCap(id, ten)
            var results = _service.Search(id, ten);

            _binding.Clear();
            foreach (var ncc in results) _binding.Add(ncc);

            if (_binding.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
                PopulateInput(_binding[0]);
            }
        }

        private void OnSave()
        {
            var id = txtIdNCC.Text.Trim();
            var ten = txtTenNCC.Text.Trim();
            var sdt = txtSdt.Text.Trim();
            var dc  = txtDiaChi.Text.Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("IDNCC không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Tên Nhà cung cấp không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // TODO: thay bằng utils.Validator.isPhone(...) của bạn
            if (!IsPhoneVN(sdt))
            {
                MessageBox.Show("SĐT không hợp lệ!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new NhaCungCapDto { IdNCC = id, TenNCC = ten, Sdt = sdt, DiaChi = dc };

            bool ok;
            if (_mode == "ADDING")
            {
                // TODO: controller.addNhaCungCap(...)
                ok = _service.Add(dto);
                if (!ok)
                {
                    MessageBox.Show("Thêm thất bại! Kiểm tra ID hoặc kết nối DB.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Thêm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else // EDITING
            {
                // TODO: controller.updateNhaCungCap(...)
                ok = _service.Update(dto);
                if (!ok)
                {
                    MessageBox.Show("Cập nhật thất bại! Kiểm tra lại dữ liệu.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Cập nhật thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            HideInputPanel();
            ReloadTable();
        }

        private void OnCancel()
        {
            HideInputPanel();
        }

        // ====== Helpers ======
        private void ReloadTable()
        {
            var all = _service.GetAll(); // TODO: controller.getAllNhaCungCap()
            _binding.Clear();
            foreach (var ncc in all) _binding.Add(ncc);
        }

        private void HideInputPanel()
        {
            txtIdNCC.Text = "";
            txtTenNCC.Text = "";
            txtSdt.Text = "";
            txtDiaChi.Text = "";

            inputPanel.Visible = false;
            _mode = "NONE";

            EnableTopAndSearch(true);
            grid.Enabled = true;
        }

        private void EnableTopAndSearch(bool enabled)
        {
            btnAdd.Enabled = enabled;
            btnEdit.Enabled = enabled;
            btnDelete.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            btnSearch.Enabled = enabled;
            txtSearchIdNCC.Enabled = enabled;
            txtSearchTenNCC.Enabled = enabled;
        }

        private void PopulateInput(NhaCungCapDto ncc)
        {
            txtIdNCC.Text = ncc.IdNCC;
            txtTenNCC.Text = ncc.TenNCC;
            txtSdt.Text = ncc.Sdt;
            txtDiaChi.Text = ncc.DiaChi;
        }

        // TODO: thay bằng utils.Validator.isPhone(...) của bạn
        private bool IsPhoneVN(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            // đơn giản: 9-11 chữ số, có thể bắt đầu bằng 0/+84
            var p = phone.Replace(" ", "");
            return Regex.IsMatch(p, @"^(\+?84|0)\d{8,10}$");
        }
    }

    // ====== DTO & Service mẫu để bạn chạy thử giao diện ======
    // TODO: xoá khối này và thay bằng entities/controller thật của bạn
    public class NhaCungCapDto
    {
        public string IdNCC { get; set; }
        public string TenNCC { get; set; }
        public string Sdt { get; set; }
        public string DiaChi { get; set; }
    }

    public interface INccService
    {
        List<NhaCungCapDto> GetAll();
        List<NhaCungCapDto> Search(string id, string ten);
        bool Add(NhaCungCapDto dto);
        bool Update(NhaCungCapDto dto);
        bool Delete(string id);
    }

    // Service giả lập (in-memory)
    public class DummyNccService : INccService
    {
        private readonly List<NhaCungCapDto> _db = new List<NhaCungCapDto>
        {
            new NhaCungCapDto{ IdNCC="NCC001", TenNCC="Dược A", Sdt="0901234567", DiaChi="Hà Nội"},
            new NhaCungCapDto{ IdNCC="NCC002", TenNCC="Dược B", Sdt="0912345678", DiaChi="TP.HCM"},
        };

        public List<NhaCungCapDto> GetAll() => _db.Select(x => Copy(x)).ToList();

        public List<NhaCungCapDto> Search(string id, string ten)
        {
            var q = _db.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(id))
                q = q.Where(x => x.IdNCC.IndexOf(id, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(ten))
                q = q.Where(x => x.TenNCC.IndexOf(ten, StringComparison.OrdinalIgnoreCase) >= 0);
            return q.Select(x => Copy(x)).ToList();
        }

        public bool Add(NhaCungCapDto dto)
        {
            if (_db.Any(x => string.Equals(x.IdNCC, dto.IdNCC, StringComparison.OrdinalIgnoreCase))) return false;
            _db.Add(Copy(dto));
            return true;
        }

        public bool Update(NhaCungCapDto dto)
        {
            var old = _db.FirstOrDefault(x => string.Equals(x.IdNCC, dto.IdNCC, StringComparison.OrdinalIgnoreCase));
            if (old == null) return false;
            old.TenNCC = dto.TenNCC;
            old.Sdt = dto.Sdt;
            old.DiaChi = dto.DiaChi;
            return true;
        }

        public bool Delete(string id)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdNCC, id, StringComparison.OrdinalIgnoreCase));
            if (it == null) return false;
            _db.Remove(it);
            return true;
        }

        private static NhaCungCapDto Copy(NhaCungCapDto s) => new NhaCungCapDto
        {
            IdNCC = s.IdNCC, TenNCC = s.TenNCC, Sdt = s.Sdt, DiaChi = s.DiaChi
        };
    }
}
