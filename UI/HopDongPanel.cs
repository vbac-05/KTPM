using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
// using QLThuocWin.Services; // TODO: ĐỔI: namespace Service/Controller thật của bạn
// using QLThuocWin.Models;   // TODO: ĐỔI: nếu entity HopDong ở namespace khác

namespace QLThuocWin.UI
{
    public class HopDong
    {
        // TODO: ĐỔI: nếu bạn đã có class HopDong trong Models thì xóa block này
        public string IdHDong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string NoiDung { get; set; }
        public string IdNV { get; set; }
        public string IdNCC { get; set; }
        public string TrangThai { get; set; }
    }

    public class HopDongPanel : UserControl
    {
        // ====== Controls ======
        DataGridView grid = new DataGridView();
        TextBox txtSearchIdHDong = new TextBox();
        TextBox txtSearchIdNV = new TextBox();
        TextBox txtSearchIdNCC = new TextBox();
        Button btnSearch = new Button { Text = "Tìm kiếm" }; // TODO: ĐỔI icon nếu có

        Button btnAdd = new Button { Text = "   Thêm" };
        Button btnEdit = new Button { Text = "   Sửa" };
        Button btnDelete = new Button { Text = "   Xóa" };
        Button btnRefresh = new Button { Text = "   Làm mới" };

        // Input panel (ẩn/hiện khi Add/Edit)
        Panel inputPanel = new Panel();
        TextBox txtIdHDong = new TextBox();
        TextBox txtNgayBatDau = new TextBox();
        TextBox txtNgayKetThuc = new TextBox();
        TextBox txtNoiDung = new TextBox();
        TextBox txtIdNV = new TextBox();
        TextBox txtIdNCC = new TextBox();
        TextBox txtTrangThai = new TextBox();
        Button btnSave = new Button { Text = "Lưu" };
        Button btnCancel = new Button { Text = "Hủy" };

        // ====== Data ======
        string currentMode = "NONE"; // "NONE" | "ADDING" | "EDITING"
        List<HopDong> ds = new List<HopDong>(); // cache hiển thị

        public HopDongPanel()
        {
            Dock = DockStyle.Fill;
            BuildToolbar();
            BuildSearchPanel();
            BuildGrid();
            BuildInputPanel(visible: false);
            WireEvents();
            ReloadAll();
        }

        private void BuildToolbar()
        {
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };

            // TODO: ĐỔI: gán Image = Properties.Resources.xxx nếu bạn có icon như Java (getResource("/icon/..."))
            bar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
            Controls.Add(bar);
        }

        private void BuildSearchPanel()
        {
            var search = new Panel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(8, 4, 8, 4) };

            var lbl1 = new Label { Text = "IDHDong:", AutoSize = true, Left = 0, Top = 8 };
            txtSearchIdHDong.SetBounds(lbl1.Right + 8, 4, 120, 24);

            var lbl2 = new Label { Text = "IDNV:", AutoSize = true, Left = txtSearchIdHDong.Right + 16, Top = 8 };
            txtSearchIdNV.SetBounds(lbl2.Right + 8, 4, 120, 24);

            var lbl3 = new Label { Text = "IDNCC:", AutoSize = true, Left = txtSearchIdNV.Right + 16, Top = 8 };
            txtSearchIdNCC.SetBounds(lbl3.Right + 8, 4, 120, 24);

            btnSearch.SetBounds(txtSearchIdNCC.Right + 16, 4, 110, 26);

            search.Controls.AddRange(new Control[] { lbl1, txtSearchIdHDong, lbl2, txtSearchIdNV, lbl3, txtSearchIdNCC, btnSearch });
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

            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDHDong", DataPropertyName = "IdHDong", Width = 110 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày bắt đầu", DataPropertyName = "NgayBatDauStr", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày kết thúc", DataPropertyName = "NgayKetThucStr", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nội dung", DataPropertyName = "NoiDung", Width = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDNV", DataPropertyName = "IdNV", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "IDNCC", DataPropertyName = "IdNCC", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = "TrangThai", Width = 120 });

            Controls.Add(grid);
        }

        private void BuildInputPanel(bool visible)
        {
            inputPanel.Dock = DockStyle.Top;
            inputPanel.Height = 110;
            inputPanel.Padding = new Padding(8);
            inputPanel.Visible = visible;

            // Layout tương tự Java (tọa độ tuyệt đối)
            var lblId = new Label { Text = "IDHDong:", AutoSize = true, Left = 8, Top = 10 };
            txtIdHDong.SetBounds(lblId.Right + 8, 8, 120, 24);

            var lblNBD = new Label { Text = "Ngày bắt đầu:", AutoSize = true, Left = txtIdHDong.Right + 16, Top = 10 };
            txtNgayBatDau.SetBounds(lblNBD.Right + 8, 8, 120, 24);

            var lblNKT = new Label { Text = "Ngày kết thúc:", AutoSize = true, Left = txtNgayBatDau.Right + 16, Top = 10 };
            txtNgayKetThuc.SetBounds(lblNKT.Right + 8, 8, 120, 24);

            var lblNoiDung = new Label { Text = "Nội dung:", AutoSize = true, Left = 8, Top = 70 };
            txtNoiDung.SetBounds(lblNoiDung.Right + 8, 66, 400, 24);

            var lblIdNV = new Label { Text = "IDNV:", AutoSize = true, Left = txtNgayKetThuc.Right + 16, Top = 38 };
            txtIdNV.SetBounds(lblIdNV.Right + 8, 34, 120, 24);

            var lblIdNCC = new Label { Text = "IDNCC:", AutoSize = true, Left = txtIdNV.Right + 16, Top = 38 };
            txtIdNCC.SetBounds(lblIdNCC.Right + 8, 34, 120, 24);

            var lblTT = new Label { Text = "Trạng thái:", AutoSize = true, Left = 8, Top = 38 };
            txtTrangThai.SetBounds(lblTT.Right + 8, 34, 120, 24);

            btnSave.SetBounds(inputPanel.Width - 220, 8, 100, 28);
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.SetBounds(inputPanel.Width - 110, 8, 100, 28);
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            inputPanel.Controls.AddRange(new Control[]
            {
                lblId, txtIdHDong, lblNBD, txtNgayBatDau, lblNKT, txtNgayKetThuc,
                lblNoiDung, txtNoiDung, lblIdNV, txtIdNV, lblIdNCC, txtIdNCC,
                lblTT, txtTrangThai, btnSave, btnCancel
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
                    PopulateInputFromRow(e.RowIndex);
            };

            // Resize để giữ vị trí 2 nút Lưu/Hủy
            inputPanel.Resize += (s, e) =>
            {
                btnSave.Left = inputPanel.Width - 220;
                btnCancel.Left = inputPanel.Width - 110;
            };
        }

        // =================== Actions ===================
        private void OnAdd()
        {
            currentMode = "ADDING";
            inputPanel.Visible = true;

            txtIdHDong.Text = "";
            txtNgayBatDau.Text = "";
            txtNgayKetThuc.Text = "";
            txtNoiDung.Text = "";
            txtIdNV.Text = "";
            txtIdNCC.Text = "";
            txtTrangThai.Text = "";

            txtIdHDong.ReadOnly = false;

            ToggleEnabled(false);
        }

        private void OnEdit()
        {
            var row = grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            currentMode = "EDITING";
            inputPanel.Visible = true;
            PopulateInputFromRow(row.Index);
            txtIdHDong.ReadOnly = true;
            ToggleEnabled(false);
        }

        private void OnDelete()
        {
            var row = grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var id = Convert.ToString(row.Cells[0].Value);
            if (MessageBox.Show($"Bạn có chắc muốn xóa hợp đồng {id}?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // TODO: ĐỔI: gọi controller.deleteHopDong(id)
                var ok = DeleteHopDong(id);
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
            var idHDong = (txtIdHDong.Text ?? "").Trim();
            var ngayBDStr = (txtNgayBatDau.Text ?? "").Trim();
            var ngayKTStr = (txtNgayKetThuc.Text ?? "").Trim();
            var noiDung = (txtNoiDung.Text ?? "").Trim();
            var idNV = (txtIdNV.Text ?? "").Trim();
            var idNCC = (txtIdNCC.Text ?? "").Trim();
            var trangThai = (txtTrangThai.Text ?? "").Trim();

            if (string.IsNullOrEmpty(idHDong))
            {
                MessageBox.Show("IDHDong không được để trống!", "Cảnh báo"); return;
            }
            if (!TryParseDate(ngayBDStr, out var ngayBD))
            {
                MessageBox.Show("Ngày bắt đầu phải đúng định dạng dd/MM/yyyy!", "Cảnh báo"); return;
            }
            if (!TryParseDate(ngayKTStr, out var ngayKT))
            {
                MessageBox.Show("Ngày kết thúc phải đúng định dạng dd/MM/yyyy!", "Cảnh báo"); return;
            }
            if (string.IsNullOrEmpty(trangThai))
            {
                MessageBox.Show("Trạng thái không được để trống!", "Cảnh báo"); return;
            }

            var hd = new HopDong
            {
                IdHDong = idHDong,
                NgayBatDau = ngayBD,
                NgayKetThuc = ngayKT,
                NoiDung = string.IsNullOrWhiteSpace(noiDung) ? null : noiDung,
                IdNV = string.IsNullOrWhiteSpace(idNV) ? null : idNV,
                IdNCC = string.IsNullOrWhiteSpace(idNCC) ? null : idNCC,
                TrangThai = trangThai
            };

            bool ok;
            if (currentMode == "ADDING")
            {
                // TODO: ĐỔI: controller.addHopDong(hd)
                ok = AddHopDong(hd);
                if (!ok) { MessageBox.Show("Thêm hợp đồng thất bại!", "Lỗi"); return; }
                MessageBox.Show("Thêm hợp đồng thành công!", "Thông báo");
            }
            else
            {
                // TODO: ĐỔI: controller.updateHopDong(hd)
                ok = UpdateHopDong(hd);
                if (!ok) { MessageBox.Show("Cập nhật hợp đồng thất bại!", "Lỗi"); return; }
                MessageBox.Show("Cập nhật hợp đồng thành công!", "Thông báo");
            }

            HideInputPanel();
            ReloadAll();
        }

        private void HideInputPanel()
        {
            txtIdHDong.Text = "";
            txtNgayBatDau.Text = "";
            txtNgayKetThuc.Text = "";
            txtNoiDung.Text = "";
            txtIdNV.Text = "";
            txtIdNCC.Text = "";
            txtTrangThai.Text = "";

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
            txtSearchIdHDong.Enabled = enabled;
            txtSearchIdNV.Enabled = enabled;
            txtSearchIdNCC.Enabled = enabled;
            grid.Enabled = enabled;
        }

        private void PopulateInputFromRow(int rowIdx)
        {
            var row = grid.Rows[rowIdx];
            txtIdHDong.Text = Convert.ToString(row.Cells[0].Value);
            txtNgayBatDau.Text = Convert.ToString(row.Cells[1].Value);
            txtNgayKetThuc.Text = Convert.ToString(row.Cells[2].Value);
            txtNoiDung.Text = Convert.ToString(row.Cells[3].Value);
            txtIdNV.Text = Convert.ToString(row.Cells[4].Value);
            txtIdNCC.Text = Convert.ToString(row.Cells[5].Value);
            txtTrangThai.Text = Convert.ToString(row.Cells[6].Value);
        }

        private void DoSearch()
        {
            var idHDong = (txtSearchIdHDong.Text ?? "").Trim();
            var idNV = (txtSearchIdNV.Text ?? "").Trim();
            var idNCC = (txtSearchIdNCC.Text ?? "").Trim();

            // TODO: ĐỔI: gọi controller.searchHopDong(idHDong, idNV, idNCC)
            var results = SearchHopDong(idHDong, idNV, idNCC);

            BindGrid(results);
        }

        private void ReloadAll()
        {
            // TODO: ĐỔI: gọi controller.getAllHopDong()
            ds = GetAllHopDong();

            BindGrid(ds);
        }

        private void BindGrid(List<HopDong> source)
        {
            var rows = source.Select(h => new
            {
                h.IdHDong,
                NgayBatDauStr = h.NgayBatDau == default ? "" : h.NgayBatDau.ToString("dd/MM/yyyy"),
                NgayKetThucStr = h.NgayKetThuc == default ? "" : h.NgayKetThuc.ToString("dd/MM/yyyy"),
                h.NoiDung,
                h.IdNV,
                h.IdNCC,
                h.TrangThai
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
            return DateTime.TryParseExact(ddMMyyyy, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out dt);
        }

        // ================= PLACEHOLDER: thay bằng Controller/DAO thật =================

        private List<HopDong> GetAllHopDong()
        {
            // TODO: ĐỔI: return new HopDongController().getAllHopDong();
            return new List<HopDong>
            {
                // demo
                new HopDong{ IdHDong="HDG001", NgayBatDau=DateTime.Today.AddDays(-14), NgayKetThuc=DateTime.Today.AddDays(16), NoiDung="Cung cấp NVL", IdNV="NV01", IdNCC="NCC01", TrangThai="Hiệu lực"},
                new HopDong{ IdHDong="HDG002", NgayBatDau=DateTime.Today.AddDays(-30), NgayKetThuc=DateTime.Today.AddDays(60), NoiDung="Bảo trì máy", IdNV="NV02", IdNCC="NCC03", TrangThai="Chờ ký"},
            };
        }

        private List<HopDong> SearchHopDong(string idHDong, string idNV, string idNCC)
        {
            // TODO: ĐỔI: return new HopDongController().searchHopDong(idHDong, idNV, idNCC);
            var q = GetAllHopDong().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(idHDong)) q = q.Where(x => (x.IdHDong ?? "").Contains(idHDong));
            if (!string.IsNullOrWhiteSpace(idNV)) q = q.Where(x => (x.IdNV ?? "").Contains(idNV));
            if (!string.IsNullOrWhiteSpace(idNCC)) q = q.Where(x => (x.IdNCC ?? "").Contains(idNCC));
            return q.ToList();
        }

        private bool AddHopDong(HopDong hd)
        {
            // TODO: ĐỔI: return new HopDongController().addHopDong(hd);
            return true;
        }

        private bool UpdateHopDong(HopDong hd)
        {
            // TODO: ĐỔI: return new HopDongController().updateHopDong(hd);
            return true;
        }

        private bool DeleteHopDong(string id)
        {
            // TODO: ĐỔI: return new HopDongController().deleteHopDong(id);
            return true;
        }
    }
}
