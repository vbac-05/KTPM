using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
// TODO: thay namespace theo project thật của bạn
// using QLThuocWin.Services;
// using QLThuocWin.DTO;

namespace QLThuocWin.UI
{
    public class NhanVienPanel : UserControl
    {
        // ====== Services & state ======
        // TODO: thay bằng controller/service thật (ví dụ NhanVienController)
        private readonly INvService _service = new DummyNvService();
        private readonly BindingList<NhanVienDto> _binding = new BindingList<NhanVienDto>();
        private string _mode = "NONE"; // "NONE" | "ADDING" | "EDITING"

        // ====== Controls ======
        private DataGridView grid;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh, btnSearch, btnSave, btnCancel;
        private TextBox txtSearchIdNV, txtSearchHoTen;

        private Panel inputPanel;
        private TextBox txtIdNV, txtHoTen, txtSdt, txtGioiTinh, txtNamSinh, txtNgayVaoLam;
        private TextBox txtLuong, txtTrangThai, txtUsername, txtPassword;

        public NhanVienPanel()
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

            btnAdd = new Button { Text = "  Thêm", Width = 106, Height = 30, Location = new Point(10, 8) };
            // TODO: icon btnAdd.Image = Properties.Resources.Add;
            btnAdd.Click += (s, e) => OnAdd();
            bar.Controls.Add(btnAdd);

            btnEdit = new Button { Text = "   Sửa", Width = 106, Height = 30, Location = new Point(150, 8) };
            // TODO: icon
            btnEdit.Click += (s, e) => OnEdit();
            bar.Controls.Add(btnEdit);

            btnDelete = new Button { Text = "  Xóa", Width = 106, Height = 30, Location = new Point(280, 8) };
            // TODO: icon
            btnDelete.Click += (s, e) => OnDelete();
            bar.Controls.Add(btnDelete);

            btnRefresh = new Button { Text = "  Làm mới", Width = 114, Height = 30, Location = new Point(415, 8) };
            // TODO: icon
            btnRefresh.Click += (s, e) => OnRefresh();
            bar.Controls.Add(btnRefresh);
        }

        private void BuildSearchPanel()
        {
            var search = new Panel { Dock = DockStyle.Top, Height = 36 };
            Controls.Add(search);

            var lblId = new Label { Text = "IDNV:", AutoSize = true, Location = new Point(10, 9) };
            search.Controls.Add(lblId);
            txtSearchIdNV = new TextBox { Width = 120, Location = new Point(55, 6) };
            search.Controls.Add(txtSearchIdNV);

            var lblTen = new Label { Text = "Họ tên:", AutoSize = true, Location = new Point(200, 9) };
            search.Controls.Add(lblTen);
            txtSearchHoTen = new TextBox { Width = 150, Location = new Point(255, 6) };
            search.Controls.Add(txtSearchHoTen);

            btnSearch = new Button { Text = "Tìm kiếm", Width = 100, Height = 26, Location = new Point(420, 5) };
            // TODO: icon
            btnSearch.Click += (s, e) => OnSearch();
            search.Controls.Add(btnSearch);
        }

        private void BuildInputPanel(bool visible)
        {
            inputPanel = new Panel { Dock = DockStyle.Top, Height = 100, Visible = visible };
            Controls.Add(inputPanel);

            // IDNV
            inputPanel.Controls.Add(new Label { Text = "IDNV:", AutoSize = true, Location = new Point(10, 14) });
            txtIdNV = new TextBox { Width = 80, Location = new Point(80, 10) };
            inputPanel.Controls.Add(txtIdNV);

            // Họ tên
            inputPanel.Controls.Add(new Label { Text = "Họ tên:", AutoSize = true, Location = new Point(200, 14) });
            txtHoTen = new TextBox { Width = 200, Location = new Point(260, 10) };
            inputPanel.Controls.Add(txtHoTen);

            // SĐT
            inputPanel.Controls.Add(new Label { Text = "SĐT:", AutoSize = true, Location = new Point(480, 14) });
            txtSdt = new TextBox { Width = 120, Location = new Point(528, 10) };
            inputPanel.Controls.Add(txtSdt);

            // Giới tính
            inputPanel.Controls.Add(new Label { Text = "Giới tính:", AutoSize = true, Location = new Point(10, 44) });
            txtGioiTinh = new TextBox { Width = 80, Location = new Point(80, 40) };
            inputPanel.Controls.Add(txtGioiTinh);

            // Ngày vào làm (dd/MM/yyyy)
            inputPanel.Controls.Add(new Label { Text = "Ngày vào:", AutoSize = true, Location = new Point(200, 44) });
            txtNgayVaoLam = new TextBox { Width = 120, Location = new Point(260, 40) };
            inputPanel.Controls.Add(txtNgayVaoLam);

            // Trạng thái
            inputPanel.Controls.Add(new Label { Text = "Trạng thái:", AutoSize = true, Location = new Point(424, 44) }) ;
            txtTrangThai = new TextBox { Width = 130, Location = new Point(502, 40) };
            inputPanel.Controls.Add(txtTrangThai);

            // Username
            inputPanel.Controls.Add(new Label { Text = "Tài khoản:", AutoSize = true, Location = new Point(10, 74) });
            txtUsername = new TextBox { Width = 103, Location = new Point(79, 70) };
            inputPanel.Controls.Add(txtUsername);

            // Password
            inputPanel.Controls.Add(new Label { Text = "Mật khẩu:", AutoSize = true, Location = new Point(200, 74) });
            txtPassword = new TextBox { Width = 150, Location = new Point(260, 70) };
            inputPanel.Controls.Add(txtPassword);

            // Lương
            inputPanel.Controls.Add(new Label { Text = "Lương:", AutoSize = true, Location = new Point(440, 74) });
            txtLuong = new TextBox { Width = 97, Location = new Point(505, 70) };
            inputPanel.Controls.Add(txtLuong);

            // Năm sinh
            inputPanel.Controls.Add(new Label { Text = "Năm sinh:", AutoSize = true, Location = new Point(623, 74) });
            txtNamSinh = new TextBox { Width = 65, Location = new Point(691, 70) };
            inputPanel.Controls.Add(txtNamSinh);

            // Nút Lưu
            btnSave = new Button { Text = "  Lưu", Width = 90, Height = 30, Location = new Point(762, 7) };
            // TODO: icon
            btnSave.Click += (s, e) => OnSave();
            inputPanel.Controls.Add(btnSave);

            // Nút Hủy
            btnCancel = new Button { Text = "  Hủy", Width = 96, Height = 35, Location = new Point(764, 45) };
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

            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDNV", DataPropertyName = "IdNV", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ tên", DataPropertyName = "HoTen", Width = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SĐT", DataPropertyName = "Sdt", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giới tính", DataPropertyName = "GioiTinh", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Năm sinh", DataPropertyName = "NamSinh", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày vào làm", DataPropertyName = "NgayVaoLamStr", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Lương", DataPropertyName = "Luong", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = "TrangThai", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tài khoản", DataPropertyName = "Username", Width = 110 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mật khẩu", DataPropertyName = "Password", Width = 110 });

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

            txtIdNV.Text = "";
            txtHoTen.Text = "";
            txtSdt.Text = "";
            txtGioiTinh.Text = "";
            txtNamSinh.Text = "";
            txtNgayVaoLam.Text = "";
            txtLuong.Text = "";
            txtTrangThai.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";

            txtIdNV.ReadOnly = false;
            EnableTopAndSearch(false);
            grid.Enabled = false;
        }

        private void OnEdit()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _mode = "EDITING";
            inputPanel.Visible = true;

            var it = grid.CurrentRow?.DataBoundItem as NhanVienDto;
            if (it != null) PopulateInput(it);

            txtIdNV.ReadOnly = true;
            EnableTopAndSearch(false);
            grid.Enabled = false;
        }

        private void OnDelete()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var it = grid.CurrentRow?.DataBoundItem as NhanVienDto;
            if (it == null) return;

            var ok = MessageBox.Show($"Bạn có chắc muốn xóa nhân viên {it.IdNV}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            if (!ok) return;

            // TODO: thay bằng controller.deleteNhanVien(id)
            if (_service.Delete(it.IdNV))
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
            var id = txtSearchIdNV.Text.Trim();
            var ten = txtSearchHoTen.Text.Trim();

            // TODO: thay bằng controller.searchNhanVien(id, ten)
            var results = _service.Search(id, ten);

            _binding.Clear();
            foreach (var nv in results) _binding.Add(nv);

            if (_binding.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
                PopulateInput(_binding[0]);
            }
        }

        private void OnSave()
        {
            var idNV = txtIdNV.Text.Trim();
            var hoTen = txtHoTen.Text.Trim();
            var sdt = txtSdt.Text.Trim();
            var gioiTinh = txtGioiTinh.Text.Trim();
            var namSinhStr = txtNamSinh.Text.Trim();
            var ngayVaoLamStr = txtNgayVaoLam.Text.Trim(); // dd/MM/yyyy
            var luong = txtLuong.Text.Trim();
            var trangThai = txtTrangThai.Text.Trim();
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();

            // ====== Validate (match Java) ======
            if (string.IsNullOrWhiteSpace(idNV))
            {
                MessageBox.Show("IDNV không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Họ tên không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // TODO: thay bằng utils.Validator.isPhone(...)
            if (!IsPhoneVN(sdt))
            {
                MessageBox.Show("SĐT phải là 10–11 chữ số!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(gioiTinh))
            {
                MessageBox.Show("Giới tính không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(namSinhStr, out var namSinh))
            {
                MessageBox.Show("Năm sinh phải là số!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // TODO: thay bằng utils.Validator.isDate(ngay, "dd/MM/yyyy")
            if (!IsDateDdMMyyyy(ngayVaoLamStr))
            {
                MessageBox.Show("Ngày vào làm phải đúng định dạng dd/MM/yyyy!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(luong))
            {
                MessageBox.Show("Lương không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(trangThai))
            {
                MessageBox.Show("Trạng thái không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new NhanVienDto
            {
                IdNV = idNV,
                HoTen = hoTen,
                Sdt = sdt,
                GioiTinh = gioiTinh,
                NamSinh = namSinh,
                NgayVaoLamStr = ngayVaoLamStr,
                Luong = luong,
                TrangThai = trangThai,
                Username = username,
                Password = password,
                RoleId = "VT02" // match Java giả định
            };

            bool ok;
            if (_mode == "ADDING")
            {
                // TODO: controller.addNhanVien(nv)
                ok = _service.Add(dto);
                if (!ok)
                {
                    MessageBox.Show("Thêm thất bại! Kiểm tra lại IDNV hoặc tài khoản.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else // EDITING
            {
                // TODO: controller.updateNhanVien(nv)
                ok = _service.Update(dto);
                if (!ok)
                {
                    MessageBox.Show("Cập nhật thất bại! Kiểm tra lại dữ liệu.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // TODO: thay bằng controller.getAllNhanVien()
            var all = _service.GetAll();
            _binding.Clear();
            foreach (var nv in all) _binding.Add(nv);
        }

        private void HideInputPanel()
        {
            txtIdNV.Text = "";
            txtHoTen.Text = "";
            txtSdt.Text = "";
            txtGioiTinh.Text = "";
            txtNamSinh.Text = "";
            txtNgayVaoLam.Text = "";
            txtLuong.Text = "";
            txtTrangThai.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";

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
            txtSearchIdNV.Enabled = enabled;
            txtSearchHoTen.Enabled = enabled;
        }

        private void PopulateInput(NhanVienDto nv)
        {
            txtIdNV.Text = nv.IdNV;
            txtHoTen.Text = nv.HoTen;
            txtSdt.Text = nv.Sdt;
            txtGioiTinh.Text = nv.GioiTinh;
            txtNamSinh.Text = nv.NamSinh.ToString();
            txtNgayVaoLam.Text = nv.NgayVaoLamStr;
            txtLuong.Text = nv.Luong;
            txtTrangThai.Text = nv.TrangThai;
            txtUsername.Text = nv.Username;
            txtPassword.Text = nv.Password;
        }

        // ====== Simple validators (thay bằng utils.Validator của bạn nếu có) ======
        // TODO: Validator.isPhone(...)
        private bool IsPhoneVN(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var p = phone.Replace(" ", "");
            return Regex.IsMatch(p, @"^\d{10,11}$");
        }
        // TODO: Validator.isDate(ngay, "dd/MM/yyyy")
        private bool IsDateDdMMyyyy(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return DateTime.TryParseExact(s, "dd/MM/yyyy", null,
                System.Globalization.DateTimeStyles.None, out _);
        }
    }

    // ====== DTO & Service mẫu cho demo UI ======
    // TODO: thay bằng entities.NhanVien + controller thật
    public class NhanVienDto
    {
        public string IdNV { get; set; }
        public string HoTen { get; set; }
        public string Sdt { get; set; }
        public string GioiTinh { get; set; }
        public int NamSinh { get; set; }
        public string NgayVaoLamStr { get; set; } // Java render string dd/MM/yyyy
        public string Luong { get; set; }         // giữ dạng chuỗi như Java đang dùng
        public string TrangThai { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }        // "VT02"
    }

    public interface INvService
    {
        List<NhanVienDto> GetAll();
        List<NhanVienDto> Search(string id, string hoTen);
        bool Add(NhanVienDto dto);
        bool Update(NhanVienDto dto);
        bool Delete(string id);
    }

    // Service in-memory demo
    public class DummyNvService : INvService
    {
        private readonly List<NhanVienDto> _db = new List<NhanVienDto>
        {
            new NhanVienDto{ IdNV="NV01", HoTen="Nguyễn Văn A", Sdt="0912345678", GioiTinh="Nam", NamSinh=1999,
                             NgayVaoLamStr="01/01/2024", Luong="12,000,000", TrangThai="Đang làm",
                             Username="nva", Password="123", RoleId="VT02"},
            new NhanVienDto{ IdNV="NV02", HoTen="Trần Thị B", Sdt="0987654321", GioiTinh="Nữ", NamSinh=2000,
                             NgayVaoLamStr="15/03/2024", Luong="10,500,000", TrangThai="Thử việc",
                             Username="ttb", Password="456", RoleId="VT02"},
        };

        public List<NhanVienDto> GetAll() => _db.Select(Copy).ToList();

        public List<NhanVienDto> Search(string id, string hoTen)
        {
            var q = _db.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(id))
                q = q.Where(x => x.IdNV.IndexOf(id, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(hoTen))
                q = q.Where(x => x.HoTen.IndexOf(hoTen, StringComparison.OrdinalIgnoreCase) >= 0);
            return q.Select(Copy).ToList();
        }

        public bool Add(NhanVienDto dto)
        {
            if (_db.Any(x => string.Equals(x.IdNV, dto.IdNV, StringComparison.OrdinalIgnoreCase))) return false;
            _db.Add(Copy(dto));
            return true;
        }

        public bool Update(NhanVienDto dto)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdNV, dto.IdNV, StringComparison.OrdinalIgnoreCase));
            if (it == null) return false;
            it.HoTen = dto.HoTen;
            it.Sdt = dto.Sdt;
            it.GioiTinh = dto.GioiTinh;
            it.NamSinh = dto.NamSinh;
            it.NgayVaoLamStr = dto.NgayVaoLamStr;
            it.Luong = dto.Luong;
            it.TrangThai = dto.TrangThai;
            it.Username = dto.Username;
            it.Password = dto.Password;
            it.RoleId = dto.RoleId;
            return true;
        }

        public bool Delete(string id)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdNV, id, StringComparison.OrdinalIgnoreCase));
            if (it == null) return false;
            _db.Remove(it);
            return true;
        }

        private static NhanVienDto Copy(NhanVienDto s) => new NhanVienDto
        {
            IdNV = s.IdNV, HoTen = s.HoTen, Sdt = s.Sdt, GioiTinh = s.GioiTinh, NamSinh = s.NamSinh,
            NgayVaoLamStr = s.NgayVaoLamStr, Luong = s.Luong, TrangThai = s.TrangThai,
            Username = s.Username, Password = s.Password, RoleId = s.RoleId
        };
    }
}
