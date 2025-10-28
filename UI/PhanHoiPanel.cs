using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
// TODO: đổi namespace/using theo dự án thật của bạn
// using QLThuocWin.Services;
// using QLThuocWin.DTO;

namespace QLThuocWin.UI
{
    public class PhanHoiPanel : UserControl
    {
        // ====== Services & state ======
        // TODO: thay bằng controller/service thật (ví dụ PhanHoiController)
        private readonly IPhanHoiService _service = new DummyPhanHoiService();
        private readonly BindingList<PhanHoiDto> _binding = new BindingList<PhanHoiDto>();
        private string _mode = "NONE"; // "NONE" | "ADDING" | "EDITING"

        // ====== Controls ======
        private DataGridView grid;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh, btnSearch, btnSave, btnCancel;
        private TextBox txtSearchIdPH, txtSearchIdKH;

        private Panel inputPanel;
        private TextBox txtIdPH, txtIdKH, txtIdHD, txtNoiDung, txtThoiGian, txtDanhGia;

        public PhanHoiPanel()
        {
            Dock = DockStyle.Fill;

            BuildToolbar();
            BuildSearchPanel();
            BuildInputPanel(visible: false);
            BuildGrid();

            ReloadTable();
        }

        // ====== UI builders ======
        private void BuildToolbar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 44 };
            Controls.Add(bar);

            btnAdd = new Button { Text = "Thêm", Width = 118, Height = 30, Location = new Point(10, 8) };
            // TODO: icon
            btnAdd.Click += (s, e) => OnAdd();
            bar.Controls.Add(btnAdd);

            btnEdit = new Button { Text = "Sửa", Width = 126, Height = 30, Location = new Point(138, 8) };
            // TODO: icon
            btnEdit.Click += (s, e) => OnEdit();
            bar.Controls.Add(btnEdit);

            btnDelete = new Button { Text = "Xóa", Width = 126, Height = 30, Location = new Point(274, 8) };
            // TODO: icon
            btnDelete.Click += (s, e) => OnDelete();
            bar.Controls.Add(btnDelete);

            btnRefresh = new Button { Text = "Làm mới", Width = 158, Height = 30, Location = new Point(410, 8) };
            // TODO: icon
            btnRefresh.Click += (s, e) => OnRefresh();
            bar.Controls.Add(btnRefresh);
        }

        private void BuildSearchPanel()
        {
            var search = new Panel { Dock = DockStyle.Top, Height = 36 };
            Controls.Add(search);

            var lblIdPH = new Label { Text = "IDPH:", AutoSize = true, Location = new Point(10, 9) };
            search.Controls.Add(lblIdPH);
            txtSearchIdPH = new TextBox { Width = 120, Location = new Point(55, 6) };
            search.Controls.Add(txtSearchIdPH);

            var lblIdKH = new Label { Text = "IDKH:", AutoSize = true, Location = new Point(200, 9) };
            search.Controls.Add(lblIdKH);
            txtSearchIdKH = new TextBox { Width = 120, Location = new Point(255, 6) };
            search.Controls.Add(txtSearchIdKH);

            btnSearch = new Button { Text = "Tìm kiếm", Width = 134, Height = 26, Location = new Point(400, 5) };
            // TODO: icon
            btnSearch.Click += (s, e) => OnSearch();
            search.Controls.Add(btnSearch);
        }

        private void BuildInputPanel(bool visible)
        {
            inputPanel = new Panel { Dock = DockStyle.Top, Height = 100, Visible = visible };
            Controls.Add(inputPanel);

            // IDPH
            inputPanel.Controls.Add(new Label { Text = "IDPH:", AutoSize = true, Location = new Point(10, 14) });
            txtIdPH = new TextBox { Width = 120, Location = new Point(70, 10) };
            inputPanel.Controls.Add(txtIdPH);

            // IDKH
            inputPanel.Controls.Add(new Label { Text = "IDKH:", AutoSize = true, Location = new Point(220, 14) });
            txtIdKH = new TextBox { Width = 100, Location = new Point(280, 10) };
            inputPanel.Controls.Add(txtIdKH);

            // IDHD
            inputPanel.Controls.Add(new Label { Text = "IDHD:", AutoSize = true, Location = new Point(400, 14) });
            txtIdHD = new TextBox { Width = 60, Location = new Point(460, 10) };
            inputPanel.Controls.Add(txtIdHD);

            // Nội dung
            inputPanel.Controls.Add(new Label { Text = "Nội dung:", AutoSize = true, Location = new Point(10, 48) });
            txtNoiDung = new TextBox { Width = 300, Location = new Point(80, 45) };
            inputPanel.Controls.Add(txtNoiDung);

            // Thời gian (dd/MM/yyyy HH:mm)
            inputPanel.Controls.Add(new Label { Text = "Thời gian:", AutoSize = true, Location = new Point(400, 48) });
            txtThoiGian = new TextBox { Width = 137, Location = new Point(460, 45) };
            inputPanel.Controls.Add(txtThoiGian);

            // Đánh giá (int)
            inputPanel.Controls.Add(new Label { Text = "Đánh giá:", AutoSize = true, Location = new Point(618, 48) });
            txtDanhGia = new TextBox { Width = 50, Location = new Point(680, 45) };
            inputPanel.Controls.Add(txtDanhGia);

            // Nút Lưu
            btnSave = new Button { Text = "Lưu", Width = 90, Height = 30, Location = new Point(770, 10) };
            // TODO: icon
            btnSave.Click += (s, e) => OnSave();
            inputPanel.Controls.Add(btnSave);

            // Nút Hủy
            btnCancel = new Button { Text = "Hủy", Width = 90, Height = 30, Location = new Point(770, 50) };
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

            // CHANGED: bám đúng 6 cột Java: IDPH, IDKH, IDHD, Nội dung, Thời gian, Đánh giá
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDPH", DataPropertyName = "IdPH", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDKH", DataPropertyName = "IdKH", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDHD", DataPropertyName = "IdHD", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nội dung", DataPropertyName = "NoiDung", Width = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thời gian", DataPropertyName = "ThoiGianStr", Width = 140 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đánh giá", DataPropertyName = "DanhGia", Width = 90 });

            grid.DataSource = _binding;

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && _mode == "NONE")
                {
                    var it = _binding[e.RowIndex];
                    PopulateInput(it);
                }
            };
        }

        // ====== Actions ======
        private void OnAdd()
        {
            _mode = "ADDING";
            inputPanel.Visible = true;

            txtIdPH.Text = "";
            txtIdKH.Text = "";
            txtIdHD.Text = "";
            txtNoiDung.Text = "";
            txtThoiGian.Text = "";
            txtDanhGia.Text = "";

            txtIdPH.ReadOnly = false;

            EnableTopAndSearch(false);
            grid.Enabled = false;
        }

        private void OnEdit()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn phản hồi cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _mode = "EDITING";
            inputPanel.Visible = true;

            var it = grid.CurrentRow?.DataBoundItem as PhanHoiDto;
            if (it != null) PopulateInput(it);

            txtIdPH.ReadOnly = true;
            EnableTopAndSearch(false);
            grid.Enabled = false;
        }

        private void OnDelete()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn phản hồi cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var it = grid.CurrentRow?.DataBoundItem as PhanHoiDto;
            if (it == null) return;

            var ok = MessageBox.Show($"Bạn có chắc muốn xóa phản hồi {it.IdPH}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            if (!ok) return;

            // TODO: thay bằng controller.deletePhanHoi(id)
            if (_service.Delete(it.IdPH))
            {
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadTable();
            }
            else
            {
                MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnRefresh()
        {
            HideInputPanel();
            ReloadTable();
        }

        private void OnSearch()
        {
            var idPH = txtSearchIdPH.Text.Trim();
            var idKH = txtSearchIdKH.Text.Trim();

            // TODO: thay bằng controller.searchPhanHoi(idPH, idKH)
            var results = _service.Search(idPH, idKH);

            _binding.Clear();
            foreach (var it in results) _binding.Add(it);

            if (_binding.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
                PopulateInput(_binding[0]);
            }
        }

        private void OnSave()
        {
            // ====== Validate (match Java) ======
            if (!IsDateDdMMyyyyHHmm(txtThoiGian.Text.Trim()))
            {
                MessageBox.Show("Thời gian phải đúng định dạng dd/MM/yyyy HH:mm", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsInteger(txtDanhGia.Text.Trim()))
            {
                MessageBox.Show("Đánh giá phải là số nguyên", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new PhanHoiDto
            {
                IdPH = txtIdPH.Text.Trim(),
                IdKH = txtIdKH.Text.Trim(),
                IdHD = txtIdHD.Text.Trim(),
                NoiDung = txtNoiDung.Text.Trim(),
                ThoiGianStr = txtThoiGian.Text.Trim(), // CHANGED: lưu string để bám format Java
                DanhGia = int.Parse(txtDanhGia.Text.Trim())
            };

            bool ok;
            if (_mode == "ADDING")
            {
                // TODO: controller.addPhanHoi(ph)
                ok = _service.Add(dto);
                if (!ok)
                {
                    MessageBox.Show("Thêm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else // EDITING
            {
                // TODO: controller.updatePhanHoi(ph)
                ok = _service.Update(dto);
                if (!ok)
                {
                    MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // TODO: thay bằng controller.getAllPhanHoi()
            var all = _service.GetAll();
            _binding.Clear();
            foreach (var it in all) _binding.Add(it);
        }

        private void HideInputPanel()
        {
            txtIdPH.Text = "";
            txtIdKH.Text = "";
            txtIdHD.Text = "";
            txtNoiDung.Text = "";
            txtThoiGian.Text = "";
            txtDanhGia.Text = "";

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
            txtSearchIdPH.Enabled = enabled;
            txtSearchIdKH.Enabled = enabled;
        }

        private void PopulateInput(PhanHoiDto it)
        {
            txtIdPH.Text = it.IdPH;
            txtIdKH.Text = it.IdKH;
            txtIdHD.Text = it.IdHD;
            txtNoiDung.Text = it.NoiDung;
            txtThoiGian.Text = it.ThoiGianStr;
            txtDanhGia.Text = it.DanhGia.ToString();
        }

        // ====== Simple validators (thay bằng utils.Validator của bạn nếu đã port sang C#) ======
        // TODO: Validator.isDate("dd/MM/yyyy HH:mm")
        private bool IsDateDdMMyyyyHHmm(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return DateTime.TryParseExact(s, "dd/MM/yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None, out _);
        }
        // TODO: Validator.isInteger(...)
        private bool IsInteger(string s) => Regex.IsMatch(s ?? "", @"^\s*-?\d+\s*$");
    }

    // ====== DTO & Service mẫu (demo) ======
    // TODO: thay bằng entities.PhanHoi + controller thật
    public class PhanHoiDto
    {
        public string IdPH { get; set; }
        public string IdKH { get; set; }
        public string IdHD { get; set; }
        public string NoiDung { get; set; }
        public string ThoiGianStr { get; set; } // match Java toString "dd/MM/yyyy HH:mm"
        public int DanhGia { get; set; }
    }

    public interface IPhanHoiService
    {
        List<PhanHoiDto> GetAll();
        List<PhanHoiDto> Search(string idPH, string idKH);
        bool Add(PhanHoiDto dto);
        bool Update(PhanHoiDto dto);
        bool Delete(string idPH);
    }

    // Service in-memory demo
    public class DummyPhanHoiService : IPhanHoiService
    {
        private readonly List<PhanHoiDto> _db = new List<PhanHoiDto>
        {
            new PhanHoiDto{ IdPH="PH01", IdKH="KH01", IdHD="HD01", NoiDung="Thuốc tốt", ThoiGianStr="01/10/2025 08:30", DanhGia=5 },
            new PhanHoiDto{ IdPH="PH02", IdKH="KH02", IdHD="HD02", NoiDung="Giao hàng nhanh", ThoiGianStr="05/10/2025 14:15", DanhGia=4 },
        };

        public List<PhanHoiDto> GetAll() => _db.Select(Copy).ToList();

        public List<PhanHoiDto> Search(string idPH, string idKH)
        {
            var q = _db.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(idPH))
                q = q.Where(x => x.IdPH.IndexOf(idPH, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(idKH))
                q = q.Where(x => x.IdKH.IndexOf(idKH, StringComparison.OrdinalIgnoreCase) >= 0);
            return q.Select(Copy).ToList();
        }

        public bool Add(PhanHoiDto dto)
        {
            if (_db.Any(x => string.Equals(x.IdPH, dto.IdPH, StringComparison.OrdinalIgnoreCase))) return false;
            _db.Add(Copy(dto));
            return true;
        }

        public bool Update(PhanHoiDto dto)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdPH, dto.IdPH, StringComparison.OrdinalIgnoreCase));
            if (it == null) return false;
            it.IdKH = dto.IdKH;
            it.IdHD = dto.IdHD;
            it.NoiDung = dto.NoiDung;
            it.ThoiGianStr = dto.ThoiGianStr;
            it.DanhGia = dto.DanhGia;
            return true;
        }

        public bool Delete(string idPH)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdPH, idPH, StringComparison.OrdinalIgnoreCase));
            if (it == null) return false;
            _db.Remove(it);
            return true;
        }

        private static PhanHoiDto Copy(PhanHoiDto s) => new PhanHoiDto
        {
            IdPH = s.IdPH, IdKH = s.IdKH, IdHD = s.IdHD,
            NoiDung = s.NoiDung, ThoiGianStr = s.ThoiGianStr, DanhGia = s.DanhGia
        };
    }
}
