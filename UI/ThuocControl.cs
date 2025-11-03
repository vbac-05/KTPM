using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
// TODO: đổi namespace/using cho đúng dự án thật của bạn
// using QLThuocApp.Services;
// using QLThuocWin.DTO;

namespace QLThuocApp.UI
{
    public class ThuocControl : UserControl
    {
        // ====== Services & state ======
        // TODO: thay bằng ThuocController/IThuocService thật của bạn
        private readonly IThuocService _service = new DummyThuocService();

        private readonly BindingList<ThuocRow> _binding = new BindingList<ThuocRow>();
        private string _mode = "NONE"; // "NONE" | "ADDING" | "EDITING"  // CHANGED: bám Java

        // ====== Controls ======
        private DataGridView grid;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh, btnSearch; // CHANGED: thêm btnSearch
        private Panel inputPanel;                                          // CHANGED: panel nhập liệu ẩn/hiện
        private TextBox txtIdThuoc, txtTenThuoc, txtThanhPhan, txtDonViTinh, txtDanhMuc, txtXuatXu,
                        txtSoLuongTon, txtGiaNhap, txtDonGia, txtHanSuDung;
        private Button btnSave, btnCancel;
        private TextBox txtSearchIdThuoc, txtSearchTenThuoc;              // CHANGED: 2 ô tìm kiếm như Java

        public ThuocControl()
        {
            Dock = DockStyle.Fill;

            BuildToolbar();
            BuildSearchPanel();   // CHANGED
            BuildGrid();          // CHANGED
            BuildInputPanel(false); // CHANGED

            ReloadTable();
        }

        // ====== UI builders ======
        private void BuildToolbar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 44 };
            Controls.Add(bar);

            btnAdd = new Button { Text = "Thêm", Width = 118, Height = 30, Left = 10, Top = 8 };
            // TODO: icon Add.png
            btnAdd.Click += (s, e) => OnAdd();
            bar.Controls.Add(btnAdd);

            btnEdit = new Button { Text = "Sửa", Width = 110, Height = 30, Left = 138, Top = 8 };
            // TODO: icon chungEdit.png
            btnEdit.Click += (s, e) => OnEdit();
            bar.Controls.Add(btnEdit);

            btnDelete = new Button { Text = "Xóa", Width = 110, Height = 30, Left = 258, Top = 8 };
            // TODO: icon chungDelete.png
            btnDelete.Click += (s, e) => OnDelete();
            bar.Controls.Add(btnDelete);

            btnRefresh = new Button { Text = "Làm mới", Width = 147, Height = 30, Left = 378, Top = 8 };
            // TODO: icon chungRefresh.png
            btnRefresh.Click += (s, e) => OnRefresh();
            bar.Controls.Add(btnRefresh);
        }

        private void BuildSearchPanel()
        {
            var search = new Panel { Dock = DockStyle.Top, Height = 36 };
            Controls.Add(search);

            var lblId = new Label { Text = "IDThuoc:", AutoSize = true, Left = 10, Top = 9 };
            search.Controls.Add(lblId);
            txtSearchIdThuoc = new TextBox { Width = 120, Left = 75, Top = 6 };
            search.Controls.Add(txtSearchIdThuoc);

            var lblTen = new Label { Text = "Tên thuốc:", AutoSize = true, Left = 210, Top = 9 };
            search.Controls.Add(lblTen);
            txtSearchTenThuoc = new TextBox { Width = 150, Left = 285, Top = 6 };
            search.Controls.Add(txtSearchTenThuoc);

            btnSearch = new Button { Text = "Tìm kiếm", Width = 127, Height = 26, Left = 450, Top = 5 };
            // TODO: icon chungSearch.png
            btnSearch.Click += (s, e) => OnSearch(); // CHANGED
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

            // CHANGED: 10 cột đúng thứ tự Java
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDThuoc", DataPropertyName = "IdThuoc", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên thuốc", DataPropertyName = "TenThuoc", Width = 160 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành phần", DataPropertyName = "ThanhPhan", Width = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn vị tính", DataPropertyName = "DonViTinh", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Danh mục", DataPropertyName = "DanhMuc", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Xuất xứ", DataPropertyName = "XuatXu", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SL tồn", DataPropertyName = "SoLuongTon", Width = 70 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá nhập", DataPropertyName = "GiaNhap", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", DataPropertyName = "DonGia", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hạn sử dụng", DataPropertyName = "HanSuDungStr", Width = 110 });

            grid.DataSource = _binding;

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && _mode == "NONE")
                {
                    PopulateInputFromRow(e.RowIndex); // CHANGED: giống Java
                }
            };
        }

        private void BuildInputPanel(bool visible)
        {
            inputPanel = new Panel { Dock = DockStyle.Bottom, Height = 110, Visible = visible }; // CHANGED: cao ~100
            Controls.Add(inputPanel);

            // Tạo control nhanh với helper
            Label L(string t, int x, int y, int w = 80) => new Label { Text = t, Left = x, Top = y, Width = w, AutoSize = false };
            TextBox T(out TextBox tb, int x, int y, int w = 100) { tb = new TextBox { Left = x, Top = y, Width = w }; return tb; }

            inputPanel.Controls.Add(L("IDThuoc:", 10, 6, 65));
            T(out txtIdThuoc, 75, 4, 100); inputPanel.Controls.Add(txtIdThuoc);

            inputPanel.Controls.Add(L("Tên thuốc:", 180, 6, 70));
            T(out txtTenThuoc, 250, 4, 200); inputPanel.Controls.Add(txtTenThuoc);

            inputPanel.Controls.Add(L("SL tồn:", 460, 6, 50));
            T(out txtSoLuongTon, 510, 4, 80); inputPanel.Controls.Add(txtSoLuongTon);

            inputPanel.Controls.Add(L("Thành phần:", 395, 40, 80));
            T(out txtThanhPhan, 483, 38, 250); inputPanel.Controls.Add(txtThanhPhan);

            inputPanel.Controls.Add(L("ĐVT:", 10, 40, 40));
            T(out txtDonViTinh, 50, 38, 95); inputPanel.Controls.Add(txtDonViTinh);

            inputPanel.Controls.Add(L("DM:", 150, 40, 30));
            T(out txtDanhMuc, 185, 38, 100); inputPanel.Controls.Add(txtDanhMuc);

            inputPanel.Controls.Add(L("Xuất xứ:", 300, 74, 60));
            T(out txtXuatXu, 360, 72, 100); inputPanel.Controls.Add(txtXuatXu);

            inputPanel.Controls.Add(L("Giá nhập:", 468, 74, 70));
            T(out txtGiaNhap, 540, 72, 90); inputPanel.Controls.Add(txtGiaNhap);

            inputPanel.Controls.Add(L("Đơn giá:", 10, 74, 60));
            T(out txtDonGia, 70, 72, 95); inputPanel.Controls.Add(txtDonGia);

            inputPanel.Controls.Add(L("Hạn SD:", 170, 74, 60));
            T(out txtHanSuDung, 230, 72, 100); inputPanel.Controls.Add(txtHanSuDung);
            // CHANGED: HSD nhập theo chuỗi "dd/MM/yyyy" giống Java

            btnSave = new Button { Text = "Lưu", Width = 100, Height = 30, Left = 760, Top = 8 };
            // TODO: icon chungSave.png
            btnSave.Click += (s, e) => OnSave();
            inputPanel.Controls.Add(btnSave);

            btnCancel = new Button { Text = "Hủy", Width = 100, Height = 30, Left = 760, Top = 48 };
            // TODO: icon chungCancel.png
            btnCancel.Click += (s, e) => OnCancel();
            inputPanel.Controls.Add(btnCancel);
        }

        // ====== Data ops ======
        private void ReloadTable()
        {
            _binding.Clear();
            foreach (var t in _service.GetAll().Select(MapToRow))
                _binding.Add(t);
        }

        private void OnSearch()
        {
            var id = txtSearchIdThuoc.Text.Trim();
            var ten = txtSearchTenThuoc.Text.Trim();

            var results = _service.Search(id, ten);

            _binding.Clear();
            foreach (var t in results.Select(MapToRow))
                _binding.Add(t);

            if (_binding.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
            }
        }

        private void OnAdd()
        {
            _mode = "ADDING";            // CHANGED
            inputPanel.Visible = true;   // CHANGED
            ClearInputs();
            txtIdThuoc.ReadOnly = false; // CHANGED

            EnableSearch(false);
            EnableToolbar(false);
            grid.Enabled = false;
        }

        private void OnEdit()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _mode = "EDITING";           // CHANGED
            inputPanel.Visible = true;   // CHANGED

            PopulateInputFromRow(grid.CurrentRow.Index);
            txtIdThuoc.ReadOnly = true;  // CHANGED

            EnableSearch(false);
            EnableToolbar(false);
            grid.Enabled = false;
        }

        private void OnDelete()
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = (ThuocRow)grid.CurrentRow.DataBoundItem;
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa thuốc {row.IdThuoc}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            if (!confirm) return;

            // TODO: gọi controller.deleteThuoc(id, out errorMessage)
            string error = null;
            var ok = _service.Delete(row.IdThuoc, out error); // CHANGED
            if (ok)
            {
                MessageBox.Show("Xóa thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadTable();
            }
            else
            {
                MessageBox.Show(string.IsNullOrEmpty(error) ? "Xóa thất bại!" : error,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnRefresh()
        {
            HideInputPanel(); // CHANGED
            ReloadTable();
        }

        private void OnSave()
        {
            // ====== Validate như Java ======
            var idThuoc = txtIdThuoc.Text.Trim();
            var tenThuoc = txtTenThuoc.Text.Trim();
            var thanhPhan = txtThanhPhan.Text.Trim();
            var donViTinh = txtDonViTinh.Text.Trim();
            var danhMuc = txtDanhMuc.Text.Trim();
            var xuatXu = txtXuatXu.Text.Trim();
            var soLuongTonStr = txtSoLuongTon.Text.Trim();
            var giaNhapStr = txtGiaNhap.Text.Trim();
            var donGiaStr = txtDonGia.Text.Trim();
            var hanSuDungStr = txtHanSuDung.Text.Trim();

            if (string.IsNullOrEmpty(idThuoc))
            { MessageBox.Show("IDThuoc không được để trống!", "Cảnh báo"); return; }
            if (string.IsNullOrEmpty(tenThuoc))
            { MessageBox.Show("Tên thuốc không được để trống!", "Cảnh báo"); return; }
            if (string.IsNullOrEmpty(donViTinh))
            { MessageBox.Show("Đơn vị tính không được để trống!", "Cảnh báo"); return; }
            if (string.IsNullOrEmpty(danhMuc))
            { MessageBox.Show("Danh mục không được để trống!", "Cảnh báo"); return; }
            if (string.IsNullOrEmpty(xuatXu))
            { MessageBox.Show("Xuất xứ không được để trống!", "Cảnh báo"); return; }

            if (!int.TryParse(soLuongTonStr, out var soLuongTon))
            { MessageBox.Show("Số lượng tồn phải là số nguyên!", "Cảnh báo"); return; }
            if (!double.TryParse(giaNhapStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var giaNhap))
            { MessageBox.Show("Giá nhập phải là số!", "Cảnh báo"); return; }
            if (!double.TryParse(donGiaStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var donGia))
            { MessageBox.Show("Đơn giá phải là số!", "Cảnh báo"); return; }
            if (!TryParseDate(hanSuDungStr, "dd/MM/yyyy", out var hsd))
            { MessageBox.Show("Hạn sử dụng phải đúng định dạng dd/MM/yyyy!", "Cảnh báo"); return; }

            var dto = new ThuocDto
            {
                IdThuoc = idThuoc,
                TenThuoc = tenThuoc,
                ThanhPhan = string.IsNullOrWhiteSpace(thanhPhan) ? null : thanhPhan,
                DonViTinh = donViTinh,
                DanhMuc = danhMuc,
                XuatXu = xuatXu,
                SoLuongTon = soLuongTon,
                GiaNhap = giaNhap,
                DonGia = donGia,
                HanSuDung = hsd
            };

            bool success;
            if (_mode == "ADDING")
            {
                // TODO: controller.addThuoc(dto)
                success = _service.Add(dto);
                if (!success) { MessageBox.Show("Thêm thất bại! Kiểm tra IDThuoc hoặc kết nối DB.", "Lỗi"); return; }
                MessageBox.Show("Thêm thành công!", "Thông báo");
            }
            else // EDITING
            {
                // TODO: controller.updateThuoc(dto)
                success = _service.Update(dto);
                if (!success) { MessageBox.Show("Cập nhật thất bại! Kiểm tra lại dữ liệu.", "Lỗi"); return; }
                MessageBox.Show("Cập nhật thành công!", "Thông báo");
            }

            HideInputPanel();
            ReloadTable();
        }

        private void OnCancel() => HideInputPanel();

        // ====== Helpers ======
        private void EnableSearch(bool enable)
        {
            txtSearchIdThuoc.Enabled = enable;
            txtSearchTenThuoc.Enabled = enable;
            btnSearch.Enabled = enable;
        }

        private void EnableToolbar(bool enable)
        {
            btnAdd.Enabled = enable;
            btnEdit.Enabled = enable;
            btnDelete.Enabled = enable;
            btnRefresh.Enabled = enable;
        }

        private void HideInputPanel()
        {
            ClearInputs();
            inputPanel.Visible = false;
            _mode = "NONE";

            EnableSearch(true);
            EnableToolbar(true);
            grid.Enabled = true;
        }

        private void ClearInputs()
        {
            txtIdThuoc.Text = "";
            txtTenThuoc.Text = "";
            txtThanhPhan.Text = "";
            txtDonViTinh.Text = "";
            txtDanhMuc.Text = "";
            txtXuatXu.Text = "";
            txtSoLuongTon.Text = "";
            txtGiaNhap.Text = "";
            txtDonGia.Text = "";
            txtHanSuDung.Text = "";
        }

        private void PopulateInputFromRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _binding.Count) return;
            var r = _binding[rowIndex];

            txtIdThuoc.Text = r.IdThuoc;
            txtTenThuoc.Text = r.TenThuoc;
            txtThanhPhan.Text = r.ThanhPhan;
            txtDonViTinh.Text = r.DonViTinh;
            txtDanhMuc.Text = r.DanhMuc;
            txtXuatXu.Text = r.XuatXu;
            txtSoLuongTon.Text = r.SoLuongTon.ToString(CultureInfo.InvariantCulture);
            txtGiaNhap.Text = r.GiaNhap.ToString(CultureInfo.InvariantCulture);
            txtDonGia.Text = r.DonGia.ToString(CultureInfo.InvariantCulture);
            txtHanSuDung.Text = r.HanSuDungStr;
        }

        private static bool TryParseDate(string s, string format, out DateTime value)
        {
            return DateTime.TryParseExact(s, format, CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out value);
        }

        private static ThuocRow MapToRow(ThuocDto t) => new ThuocRow
        {
            IdThuoc = t.IdThuoc,
            TenThuoc = t.TenThuoc,
            ThanhPhan = t.ThanhPhan,
            DonViTinh = t.DonViTinh,
            DanhMuc = t.DanhMuc,
            XuatXu = t.XuatXu,
            SoLuongTon = t.SoLuongTon,
            GiaNhap = t.GiaNhap,
            DonGia = t.DonGia,
            HanSuDungStr = t.HanSuDung.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) // CHANGED: match DateHelper.toString
        };
    }

    // ====== View row (DataGridView binding) ======
    public class ThuocRow
    {
        public string IdThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string ThanhPhan { get; set; }
        public string DonViTinh { get; set; }
        public string DanhMuc { get; set; }
        public string XuatXu { get; set; }
        public int SoLuongTon { get; set; }
        public double GiaNhap { get; set; }
        public double DonGia { get; set; }
        public string HanSuDungStr { get; set; } // dd/MM/yyyy
    }

    // ====== DTO & Service mẫu (demo) – thay bằng Controller/DAO thật ======
    public class ThuocDto
    {
        public string IdThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string ThanhPhan { get; set; }
        public string DonViTinh { get; set; }
        public string DanhMuc { get; set; }
        public string XuatXu { get; set; }
        public int SoLuongTon { get; set; }
        public double GiaNhap { get; set; }
        public double DonGia { get; set; }
        public DateTime HanSuDung { get; set; }
    }

    public interface IThuocService
    {
        List<ThuocDto> GetAll();
        List<ThuocDto> Search(string idThuoc, string tenThuoc);
        bool Add(ThuocDto t);
        bool Update(ThuocDto t);
        bool Delete(string idThuoc, out string errorMessage);
    }

    // In-memory demo service
    public class DummyThuocService : IThuocService
    {
        private readonly List<ThuocDto> _db = new List<ThuocDto>
        {
            new ThuocDto{ IdThuoc="T001", TenThuoc="Paracetamol", ThanhPhan="Acetaminophen 500mg",
                DonViTinh="Viên", DanhMuc="Giảm đau", XuatXu="VN",
                SoLuongTon=100, GiaNhap=2000, DonGia=3000, HanSuDung=new DateTime(2027,12,31)},
            new ThuocDto{ IdThuoc="T002", TenThuoc="Cefalexin", ThanhPhan="Cefalexin 500mg",
                DonViTinh="Viên", DanhMuc="Kháng sinh", XuatXu="VN",
                SoLuongTon=50, GiaNhap=3500, DonGia=5000, HanSuDung=new DateTime(2026,6,30)},
        };

        public List<ThuocDto> GetAll() => _db.Select(Clone).ToList();

        public List<ThuocDto> Search(string idThuoc, string tenThuoc)
        {
            var q = _db.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(idThuoc))
                q = q.Where(x => x.IdThuoc.IndexOf(idThuoc, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(tenThuoc))
                q = q.Where(x => x.TenThuoc.IndexOf(tenThuoc, StringComparison.OrdinalIgnoreCase) >= 0);
            return q.Select(Clone).ToList();
        }

        public bool Add(ThuocDto t)
        {
            if (_db.Any(x => string.Equals(x.IdThuoc, t.IdThuoc, StringComparison.OrdinalIgnoreCase))) return false;
            _db.Add(Clone(t));
            return true;
        }

        public bool Update(ThuocDto t)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdThuoc, t.IdThuoc, StringComparison.OrdinalIgnoreCase));
            if (it == null) return false;
            it.TenThuoc = t.TenThuoc;
            it.ThanhPhan = t.ThanhPhan;
            it.DonViTinh = t.DonViTinh;
            it.DanhMuc = t.DanhMuc;
            it.XuatXu = t.XuatXu;
            it.SoLuongTon = t.SoLuongTon;
            it.GiaNhap = t.GiaNhap;
            it.DonGia = t.DonGia;
            it.HanSuDung = t.HanSuDung;
            return true;
        }

        public bool Delete(string idThuoc, out string errorMessage)
        {
            var it = _db.FirstOrDefault(x => string.Equals(x.IdThuoc, idThuoc, StringComparison.OrdinalIgnoreCase));
            if (it == null) { errorMessage = "Không tìm thấy thuốc!"; return false; }
            // TODO: nếu cần kiểm tra ràng buộc Hóa đơn/Chi tiết… thì xử lý ở đây
            _db.Remove(it);
            errorMessage = null;
            return true;
        }

        private static ThuocDto Clone(ThuocDto s) => new ThuocDto
        {
            IdThuoc = s.IdThuoc,
            TenThuoc = s.TenThuoc,
            ThanhPhan = s.ThanhPhan,
            DonViTinh = s.DonViTinh,
            DanhMuc = s.DanhMuc,
            XuatXu = s.XuatXu,
            SoLuongTon = s.SoLuongTon,
            GiaNhap = s.GiaNhap,
            DonGia = s.DonGia,
            HanSuDung = s.HanSuDung
        };
    }
}
