using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
// using QLThuocWin.Models;    // TODO: ĐỔI nếu class KhachHang đã có trong Models
// using QLThuocWin.Services;  // TODO: ĐỔI nếu bạn có Service/Repo thật

namespace QLThuocWin.UI
{
    // ===== Nếu đã có entity KhachHang trong project, hãy xoá block class demo này =====
    public class KhachHang
    {
        public string IdKH { get; set; }
        public string HoTen { get; set; }
        public string Sdt { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgayThamGia { get; set; }
    }
    // ================================================================================

    public class KhachHangPanel : UserControl
    {
        // ===== Controls (khớp layout & chức năng với Java) =====
        DataGridView grid = new DataGridView();

        TextBox txtSearchHoTen = new TextBox();
        TextBox txtSearchSDT = new TextBox();
        Button btnSearch = new Button { Text = "  Tìm kiếm" }; // TODO: ĐỔI icon nếu có

        Button btnAdd = new Button { Text = "  Thêm" };
        Button btnEdit = new Button { Text = "  Sửa" };
        Button btnDelete = new Button { Text = "  Xóa" };
        Button btnRefresh = new Button { Text = "  Làm mới" };

        Panel inputPanel = new Panel();
        TextBox txtIdKH = new TextBox();
        TextBox txtHoTen = new TextBox();
        TextBox txtSDT = new TextBox();
        TextBox txtGioiTinh = new TextBox();
        TextBox txtNgayThamGia = new TextBox(); // dùng TextBox + parse dd/MM/yyyy

        Button btnSave = new Button { Text = "  Lưu" };
        Button btnCancel = new Button { Text = "  Hủy" };

        string currentMode = "NONE"; // "NONE" | "ADDING" | "EDITING"
        List<KhachHang> cache = new List<KhachHang>(); // dữ liệu hiển thị

        public KhachHangPanel()
        {
            Dock = DockStyle.Fill;
            BuildToolbar();
            BuildSearchPanel();
            BuildGrid();
            BuildInputPanel(visible: false);
            WireEvents();
            ReloadAll();
        }

        // ===== UI Builders =====
        private void BuildToolbar()
        {
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };

            // TODO: ĐỔI: gán Image = Properties.Resources.<icon> nếu bạn có file resx
            btnAdd.Width = 110; btnEdit.Width = 110; btnDelete.Width = 110; btnRefresh.Width = 120;

            bar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
            Controls.Add(bar);
        }

        private void BuildSearchPanel()
        {
            var search = new Panel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(8, 4, 8, 4) };

            var lblHoTen = new Label { Text = "Họ tên:", AutoSize = true, Left = 0, Top = 8 };
            txtSearchHoTen.SetBounds(lblHoTen.Right + 8, 4, 150, 24);

            var lblSdt = new Label { Text = "SĐT:", AutoSize = true, Left = txtSearchHoTen.Right + 16, Top = 8 };
            txtSearchSDT.SetBounds(lblSdt.Right + 8, 4, 120, 24);

            btnSearch.SetBounds(txtSearchSDT.Right + 16, 4, 120, 26);

            search.Controls.AddRange(new Control[] { lblHoTen, txtSearchHoTen, lblSdt, txtSearchSDT, btnSearch });
            Controls.Add(search);
        }

        private void BuildGrid()
        {
            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;

            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDKH", DataPropertyName = "IdKH", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ tên", DataPropertyName = "HoTen", Width = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SĐT", DataPropertyName = "Sdt", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giới tính", DataPropertyName = "GioiTinh", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày tham gia", DataPropertyName = "NgayThamGiaStr", Width = 140 });

            Controls.Add(grid);
        }

        private void BuildInputPanel(bool visible)
        {
            inputPanel.Dock = DockStyle.Top;
            inputPanel.Height = 100;
            inputPanel.Padding = new Padding(8);
            inputPanel.Visible = visible;

            // Tọa độ giống Java (absolute positioning)
            var lblId = new Label { Text = "IDKH:", AutoSize = true, Left = 10, Top = 10 };
            txtIdKH.SetBounds(lblId.Right + 8, 8, 100, 24);

            var lblHoTen = new Label { Text = "Họ tên:", AutoSize = true, Left = txtIdKH.Right + 30, Top = 10 };
            txtHoTen.SetBounds(lblHoTen.Right + 8, 8, 200, 24);

            var lblSdt = new Label { Text = "SĐT:", AutoSize = true, Left = txtHoTen.Right + 30, Top = 10 };
            txtSDT.SetBounds(lblSdt.Right + 8, 8, 120, 24);

            var lblGt = new Label { Text = "Giới tính:", AutoSize = true, Left = 10, Top = 45 };
            txtGioiTinh.SetBounds(lblGt.Right + 8, 41, 100, 24);

            var lblNgay = new Label { Text = "Ngày tham gia:", AutoSize = true, Left = txtGioiTinh.Right + 30, Top = 45 };
            txtNgayThamGia.SetBounds(lblNgay.Right + 8, 41, 200, 24); // nhập dd/MM/yyyy

            btnSave.SetBounds(760, 8, 100, 28);
            btnCancel.SetBounds(760, 40, 100, 28);
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            inputPanel.Controls.AddRange(new Control[]
            {
                lblId, txtIdKH, lblHoTen, txtHoTen, lblSdt, txtSDT, lblGt, txtGioiTinh, lblNgay, txtNgayThamGia,
                btnSave, btnCancel
            });

            Controls.Add(inputPanel);
            inputPanel.BringToFront();
        }

        private void WireEvents()
        {
            btnSearch.Click += (s, e) => DoSearch();
            btnRefresh.Click += (s, e) => { HideInputPanel(); ReloadAll(); };
            btnAdd.Click += (s, e) => OnAdd();
            btnEdit.Click += (s, e) => OnEdit();
            btnDelete.Click += (s, e) => OnDelete();
            btnSave.Click += (s, e) => OnSave();
            btnCancel.Click += (s, e) => HideInputPanel();

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && currentMode == "NONE")
                    PopulateFromRow(e.RowIndex);
            };

            // giữ 2 nút bên phải khi resize
            inputPanel.Resize += (s, e) =>
            {
                btnSave.Left = inputPanel.Width - 120;
                btnCancel.Left = inputPanel.Width - 120;
                btnSave.Top = 8;
                btnCancel.Top = 40;
            };
        }

        // ======= Actions (khớp Java) =======
        private void OnAdd()
        {
            currentMode = "ADDING";
            inputPanel.Visible = true;

            txtIdKH.Text = "";
            txtHoTen.Text = "";
            txtSDT.Text = "";
            txtGioiTinh.Text = "";
            txtNgayThamGia.Text = "";

            txtIdKH.ReadOnly = false;
            ToggleEnabled(false);
        }

        private void OnEdit()
        {
            var row = grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            currentMode = "EDITING";
            inputPanel.Visible = true;
            PopulateFromRow(row.Index);
            txtIdKH.ReadOnly = true;
            ToggleEnabled(false);
        }

        private void OnDelete()
        {
            var row = grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var id = Convert.ToString(row.Cells[0].Value);
            if (MessageBox.Show($"Bạn có chắc muốn xóa khách hàng {id}?", "Xác nhận",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // TODO: ĐỔI: gọi controller/Repo thật để xóa
                var ok = DeleteKhachHang(id);
                if (ok)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo");
                    ReloadAll();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnSave()
        {
            var id = (txtIdKH.Text ?? "").Trim();
            var hoten = (txtHoTen.Text ?? "").Trim();
            var sdt = (txtSDT.Text ?? "").Trim();
            var gt = (txtGioiTinh.Text ?? "").Trim();
            var ngayStr = (txtNgayThamGia.Text ?? "").Trim();

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("IDKH không được để trống!", "Cảnh báo"); return;
            }
            if (string.IsNullOrEmpty(hoten))
            {
                MessageBox.Show("Họ tên không được để trống!", "Cảnh báo"); return;
            }
            if (!string.IsNullOrEmpty(ngayStr) && !TryParseDate(ngayStr, out var ngay))
            {
                MessageBox.Show("Ngày tham gia phải đúng định dạng dd/MM/yyyy!", "Cảnh báo"); return;
            }
            DateTime? ngayTG = null;
            if (!string.IsNullOrEmpty(ngayStr))
            {
                TryParseDate(ngayStr, out var d); ngayTG = d;
            }

            var kh = new KhachHang
            {
                IdKH = id,
                HoTen = hoten,
                Sdt = sdt,
                GioiTinh = gt,
                NgayThamGia = ngayTG
            };

            bool ok;
            if (currentMode == "ADDING")
            {
                // TODO: ĐỔI: gọi controller.addKhachHang(kh, out errorMsg)
                ok = AddKhachHang(kh, out var err);
                if (!ok)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(err) ? "Thêm thất bại!" : err, "Lỗi");
                    return;
                }
                MessageBox.Show("Thêm thành công!", "Thông báo");
            }
            else // EDITING
            {
                // TODO: ĐỔI: gọi controller.updateKhachHang(kh, out errorMsg)
                ok = UpdateKhachHang(kh, out var err);
                if (!ok)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(err) ? "Cập nhật thất bại!" : err, "Lỗi");
                    return;
                }
                MessageBox.Show("Cập nhật thành công!", "Thông báo");
            }

            HideInputPanel();
            ReloadAll();
        }

        private void HideInputPanel()
        {
            txtIdKH.Text = "";
            txtHoTen.Text = "";
            txtSDT.Text = "";
            txtGioiTinh.Text = "";
            txtNgayThamGia.Text = "";

            inputPanel.Visible = false;
            currentMode = "NONE";
            ToggleEnabled(true);
        }

        private void ToggleEnabled(bool enabled)
        {
            btnAdd.Enabled = enabled;
            btnEdit.Enabled = enabled;
            btnDelete.Enabled = enabled;
            btnRefresh.Enabled = enabled;

            btnSearch.Enabled = enabled;
            txtSearchHoTen.Enabled = enabled;
            txtSearchSDT.Enabled = enabled;

            grid.Enabled = enabled;
        }

        private void PopulateFromRow(int rowIdx)
        {
            var row = grid.Rows[rowIdx];
            txtIdKH.Text = Convert.ToString(row.Cells[0].Value);
            txtHoTen.Text = Convert.ToString(row.Cells[1].Value);
            txtSDT.Text = Convert.ToString(row.Cells[2].Value);
            txtGioiTinh.Text = Convert.ToString(row.Cells[3].Value);
            txtNgayThamGia.Text = Convert.ToString(row.Cells[4].Value);
        }

        private void DoSearch()
        {
            var hoTen = (txtSearchHoTen.Text ?? "").Trim();
            var sdt = (txtSearchSDT.Text ?? "").Trim();

            // TODO: ĐỔI: gọi controller.searchKhachHang(hoTen, sdt)
            var results = SearchKhachHang(hoTen, sdt);
            BindGrid(results);
        }

        private void ReloadAll()
        {
            // TODO: ĐỔI: gọi controller.getAllKhachHang()
            cache = GetAllKhachHang();
            BindGrid(cache);
        }

        private void BindGrid(List<KhachHang> source)
        {
            var rows = source.Select(k => new
            {
                k.IdKH,
                k.HoTen,
                k.Sdt,
                k.GioiTinh,
                NgayThamGiaStr = k.NgayThamGia.HasValue ? k.NgayThamGia.Value.ToString("dd/MM/yyyy") : ""
            }).ToList();

            grid.DataSource = rows;
            if (grid.Rows.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
            }
        }

        private static bool TryParseDate(string ddMMyyyy, out DateTime dt)
        {
            return DateTime.TryParseExact(ddMMyyyy, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        }

        // ================== PLACEHOLDERS – ĐỔI sang Repos/Service/DAO thật ==================

        private List<KhachHang> GetAllKhachHang()
        {
            // TODO: ĐỔI -> return new KhachHangService().GetAll();
            return new List<KhachHang>
            {
                new KhachHang{ IdKH="KH001", HoTen="Nguyễn Văn A", Sdt="0901234567", GioiTinh="Nam", NgayThamGia=DateTime.Today.AddDays(-30) },
                new KhachHang{ IdKH="KH002", HoTen="Trần Thị B", Sdt="0912345678", GioiTinh="Nữ",  NgayThamGia=DateTime.Today.AddDays(-10) },
            };
        }

        private List<KhachHang> SearchKhachHang(string hoTen, string sdt)
        {
            // TODO: ĐỔI -> return new KhachHangService().Search(hoTen, sdt);
            var q = GetAllKhachHang().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(hoTen)) q = q.Where(x => (x.HoTen ?? "").IndexOf(hoTen, StringComparison.OrdinalIgnoreCase) >= 0);
            if (!string.IsNullOrWhiteSpace(sdt)) q = q.Where(x => (x.Sdt ?? "").Contains(sdt));
            return q.ToList();
        }

        private bool AddKhachHang(KhachHang kh, out string error)
        {
            // TODO: ĐỔI -> return new KhachHangService().Add(kh, out error);
            error = null;
            return true;
        }

        private bool UpdateKhachHang(KhachHang kh, out string error)
        {
            // TODO: ĐỔI -> return new KhachHangService().Update(kh, out error);
            error = null;
            return true;
        }

        private bool DeleteKhachHang(string id)
        {
            // TODO: ĐỔI -> return new KhachHangService().Delete(id);
            return true;
        }
    }
}
