using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using QLThuocWin.Models;
// using QLThuocWin.Services; // TODO: ĐỔI namespace Services/DAO thực tế của bạn

namespace QLThuocWin.UI
{
    public class AddPhieuNhapDialog : Form
    {
        // ================== SERVICE/DAO (tùy dự án) ==================
        // Gợi ý: đổi sang lớp của bạn rồi bỏ comment ở dưới:
        // private readonly NhaCungCapService _ncc = new NhaCungCapService();  // TODO: ĐỔI lớp/constructor Service/DAO thực tế
        // private readonly PhieuNhapService _pn = new PhieuNhapService();      // TODO: ĐỔI lớp/constructor Service/DAO thực tế
        // private readonly ThuocService _thuoc = new ThuocService();           // TODO: ĐỔI lớp/constructor Service/DAO thực tế
        // private readonly ChiTietPhieuNhapService _ct = new ChiTietPhieuNhapService(); // TODO: ĐỔI lớp/constructor Service/DAO thực tế

        // Nếu bạn dùng DAO/Repository: thay tên & hàm gọi cho đúng; hoặc tiêm qua constructor (DI).

        // ========= KHAI BÁO CONTROL (match Java) =========
        TextBox txtIDPN, txtThoiGian, txtIDNV, txtNCC;
        TextBox txtTenThuoc, txtThanhPhan, txtDonViTinh, txtDanhMuc, txtXuatXu, txtSoLuong, txtGiaNhap, txtDonGia, txtHanSuDung;
        Button btnThemNCC, btnThemThuoc, btnLuu, btnHuy, btnXoaThuoc;
        ComboBox comboNCC;
        DataGridView grid;
        BindingList<RowThuoc> gridModel = new BindingList<RowThuoc>();
        Label lblTongTien;

        // ========= DỮ LIỆU TẠM =========
        private List<Thuoc> listThuocTam = new List<Thuoc>();
        private List<ChiTietPhieuNhap> listChiTietTam = new List<ChiTietPhieuNhap>();
        private double tongTien = 0.0;
        private List<NhaCungCap> dsNCC = new List<NhaCungCap>(); // gợi ý NCC

        class RowThuoc
        {
            public string ID { get; set; }
            public string Ten { get; set; }
            public int SL { get; set; }
            public decimal GiaNhap { get; set; }
            public decimal DonGia { get; set; }
            public string HanSD { get; set; } // dd/MM/yyyy
        }

        public AddPhieuNhapDialog()
        {
            Text = "Thêm phiếu nhập mới";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(900, 600);

            var root = new Panel { Dock = DockStyle.Fill };
            Controls.Add(root);

            // ========= Panel Phiếu nhập (y=10, h=90) =========
            var pnPN = new Panel { Left = 10, Top = 10, Width = 870, Height = 90 };
            root.Controls.Add(pnPN);

            var lblIDPN = new Label { Left = 10, Top = 10, Width = 50, Text = "IDPN:" };
            txtIDPN = new TextBox { Left = 110, Top = 10, Width = 80 };

            var lblThoiGian = new Label { Left = 221, Top = 10, Width = 70, Text = "Thời gian:" };
            txtThoiGian = new TextBox { Left = 299, Top = 10, Width = 150 };

            var lblIDNV = new Label { Left = 480, Top = 10, Width = 50, Text = "IDNV:" };
            txtIDNV = new TextBox { Left = 538, Top = 10, Width = 80 };

            var lblNCC = new Label { Left = 10, Top = 45, Width = 100, Text = "Nhà cung cấp:" };
            txtNCC = new TextBox { Left = 110, Top = 45, Width = 200 };

            comboNCC = new ComboBox { Left = 320, Top = 45, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            btnThemNCC = new Button { Left = 603, Top = 45, Width = 200, Height = 25, Text = "   Thêm nhà cung cấp" /*, Image = ...*/ };
            btnThemNCC.Visible = false;

            pnPN.Controls.AddRange(new Control[] {
                lblIDPN, txtIDPN, lblThoiGian, txtThoiGian, lblIDNV, txtIDNV,
                lblNCC, txtNCC, comboNCC, btnThemNCC
            });

            // ========= Group Thuốc (y=110, h=130) =========
            var pnThuoc = new GroupBox { Left = 10, Top = 110, Width = 860, Height = 130, Text = "Thông tin thuốc" };
            root.Controls.Add(pnThuoc);

            AddL(pnThuoc, "Tên thuốc:", 10, 20, out _);
            txtTenThuoc = new TextBox { Left = 80, Top = 20, Width = 120 }; pnThuoc.Controls.Add(txtTenThuoc);

            AddL(pnThuoc, "Thành phần:", 210, 20, out _);
            txtThanhPhan = new TextBox { Left = 290, Top = 20, Width = 80 }; pnThuoc.Controls.Add(txtThanhPhan);

            AddL(pnThuoc, "Đơn vị tính:", 378, 20, out _);
            txtDonViTinh = new TextBox { Left = 444, Top = 20, Width = 80 }; pnThuoc.Controls.Add(txtDonViTinh);

            AddL(pnThuoc, "Danh mục:", 542, 20, out _);
            txtDanhMuc = new TextBox { Left = 603, Top = 20, Width = 90 }; pnThuoc.Controls.Add(txtDanhMuc);

            btnThemThuoc = new Button { Left = 707, Top = 20, Width = 145, Height = 25, Text = "  Thêm thuốc" /*, Image = ...*/ };
            pnThuoc.Controls.Add(btnThemThuoc);

            AddL(pnThuoc, "Xuất xứ:", 10, 55, out _);
            txtXuatXu = new TextBox { Left = 80, Top = 55, Width = 120 }; pnThuoc.Controls.Add(txtXuatXu);

            AddL(pnThuoc, "Số lượng:", 210, 55, out _);
            txtSoLuong = new TextBox { Left = 290, Top = 55, Width = 60 }; pnThuoc.Controls.Add(txtSoLuong);

            AddL(pnThuoc, "Giá nhập:", 360, 55, out _);
            txtGiaNhap = new TextBox { Left = 420, Top = 55, Width = 80 }; pnThuoc.Controls.Add(txtGiaNhap);

            AddL(pnThuoc, "Đơn giá:", 510, 55, out _);
            txtDonGia = new TextBox { Left = 570, Top = 55, Width = 80 }; pnThuoc.Controls.Add(txtDonGia);

            AddL(pnThuoc, "Hạn SD:", 660, 55, out _);
            txtHanSuDung = new TextBox { Left = 710, Top = 55, Width = 80 }; pnThuoc.Controls.Add(txtHanSuDung);

            // ========= Bảng (y=250, h=130) =========
            grid = new DataGridView
            {
                Left = 10, Top = 250, Width = 860, Height = 130,
                ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, AutoGenerateColumns = false, AllowUserToAddRows = false,
                DataSource = gridModel
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.ID), HeaderText = "ID", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.Ten), HeaderText = "Tên", Width = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.SL), HeaderText = "SL", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.GiaNhap), HeaderText = "Giá nhập", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "0.##" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.DonGia), HeaderText = "Đơn giá", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "0.##" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.HanSD), HeaderText = "Hạn SD", Width = 120 });
            root.Controls.Add(grid);

            // ========= Tổng tiền & nút Xóa =========
            btnXoaThuoc = new Button { Left = 10, Top = 390, Width = 150, Height = 30, Text = "   Xóa thuốc" /*, Image = ...*/ };
            root.Controls.Add(btnXoaThuoc);

            lblTongTien = new Label { Left = 700, Top = 390, Width = 160, Height = 25, Text = "Tổng tiền: 0" };
            root.Controls.Add(lblTongTien);

            // ========= Nút Lưu/Hủy =========
            btnLuu = new Button { Left = 650, Top = 440, Width = 100, Height = 35, Text = "    Lưu" /*, Image = ...*/ };
            btnHuy = new Button { Left = 770, Top = 440, Width = 100, Height = 35, Text = "    Hủy" /*, Image = ...*/ };
            root.Controls.Add(btnLuu); root.Controls.Add(btnHuy);

            // ========= XỬ LÝ DỮ LIỆU MẶC ĐỊNH =========
            txtIDPN.ReadOnly = true; txtThoiGian.ReadOnly = true;
            SinhIDPNTuDong();
            txtThoiGian.Text = DateTime.Now.ToString("dd/MM/yy HH:mm", CultureInfo.InvariantCulture);

            // Lấy danh sách NCC (giống Java: nccController.getAllNhaCungCap())
            dsNCC = GetAllNCC(); // TODO: ĐỔI: gọi Service/DAO thật (VD: _ncc.GetAll())
                                 // ↑ Tìm theo tag TODO: ĐỔI để thay đúng dòng

            // Gợi ý NCC khi gõ trong txtNCC (match Java)
            txtNCC.TextChanged += (s, e) =>
            {
                var key = (txtNCC.Text ?? "").Trim().ToLowerInvariant();
                comboNCC.Items.Clear();
                bool found = false;
                foreach (var n in dsNCC)
                {
                    if (!string.IsNullOrEmpty(key) && n.TenNCC?.ToLowerInvariant().Contains(key) == true)
                    {
                        comboNCC.Items.Add($"{n.TenNCC} - {n.IdNCC}");
                        found = true;
                    }
                }
                btnThemNCC.Visible = !found && key.Length > 0;
                comboNCC.DroppedDown = found && comboNCC.Items.Count > 0;
                if (comboNCC.DroppedDown && comboNCC.SelectedIndex < 0 && comboNCC.Items.Count > 0)
                    comboNCC.SelectedIndex = 0;
            };

            btnThemNCC.Click += (s, e) => ShowThemNCCDialog();

            // Thêm thuốc
            btnThemThuoc.Click += (s, e) => ThemThuocVaoTam();

            // Xóa thuốc (match Java)
            btnXoaThuoc.Click += (s, e) =>
            {
                if (grid.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng thuốc muốn xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var row = grid.CurrentRow.DataBoundItem as RowThuoc;
                if (row == null) return;

                listThuocTam.RemoveAll(t => string.Equals(t.IdThuoc, row.ID, StringComparison.OrdinalIgnoreCase));
                listChiTietTam.RemoveAll(ct => string.Equals(ct.IdThuoc, row.ID, StringComparison.OrdinalIgnoreCase));

                tongTien -= (double)row.GiaNhap * row.SL;
                if (tongTien < 0) tongTien = 0;
                lblTongTien.Text = $"Tổng tiền: {tongTien}";

                gridModel.Remove(row);
            };

            // Lưu
            btnLuu.Click += (s, e) => LuuPhieuNhap();

            // Hủy
            btnHuy.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn hủy?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            };
        }

        private static void AddL(Control parent, string text, int x, int y, out Label lbl)
        {
            lbl = new Label { Left = x, Top = y, Text = text, AutoSize = true };
            parent.Controls.Add(lbl);
        }

        // ============= TỰ SINH IDPN (match Java) =============
        private void SinhIDPNTuDong()
        {
            var ds = GetAllPhieuNhap(); // TODO: ĐỔI: gọi Service/DAO thật (VD: _pn.GetAll())
            var lastID = "PN000";
            if (ds != null && ds.Count > 0)
                lastID = ds[^1].IdPN ?? "PN000";
            var num = ParseTrailingNumber(lastID);
            txtIDPN.Text = $"PN{num + 1:000}";
        }

        // ============= TỰ SINH ID THUỐC (match Java) =============
        private string SinhIDThuocTuDong()
        {
            var ds = GetAllThuoc(); // TODO: ĐỔI: gọi Service/DAO thật (VD: _thuoc.GetAll())
            var lastID = "T000";
            if (ds != null && ds.Count > 0)
                lastID = ds[^1].IdThuoc ?? "T000";
            var num = ParseTrailingNumber(lastID);
            return $"T{num + listThuocTam.Count + 1:000}";
        }

        private static int ParseTrailingNumber(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            var digits = new string(id.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var n) ? n : 0;
        }

        // ============= Dialog thêm NCC (match Java) =============
        private void ShowThemNCCDialog()
        {
            using var f = new Form
            {
                Text = "Thêm nhà cung cấp",
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(380, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var txtId = new TextBox();
            var txtTen = new TextBox { Text = txtNCC.Text };
            var txtDiaChi = new TextBox();
            var txtSdt = new TextBox();

            var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(10) };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

            tbl.Controls.Add(new Label { Text = "IDNCC:", AutoSize = true }, 0, 0); tbl.Controls.Add(txtId, 1, 0);
            tbl.Controls.Add(new Label { Text = "Tên NCC:", AutoSize = true }, 0, 1); tbl.Controls.Add(txtTen, 1, 1);
            tbl.Controls.Add(new Label { Text = "Địa chỉ:", AutoSize = true }, 0, 2); tbl.Controls.Add(txtDiaChi, 1, 2);
            tbl.Controls.Add(new Label { Text = "SĐT:", AutoSize = true }, 0, 3); tbl.Controls.Add(txtSdt, 1, 3);

            var pnlBtn = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Bottom, Padding = new Padding(10) };
            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel };
            pnlBtn.Controls.Add(ok); pnlBtn.Controls.Add(cancel);

            f.Controls.Add(tbl);
            f.Controls.Add(pnlBtn);

            if (f.ShowDialog(this) == DialogResult.OK)
            {
                var ncc = new NhaCungCap
                {
                    IdNCC = (txtId.Text ?? "").Trim(),
                    TenNCC = (txtTen.Text ?? "").Trim(),
                    DiaChi = (txtDiaChi.Text ?? "").Trim(),
                    Sdt = (txtSdt.Text ?? "").Trim()
                };
                if (!string.IsNullOrWhiteSpace(ncc.IdNCC) && !string.IsNullOrWhiteSpace(ncc.TenNCC))
                {
                    var okAdd = AddNCC(ncc); // TODO: ĐỔI: gọi Service/DAO thật (VD: _ncc.Add(ncc) hoặc _nccDAO.Insert(ncc))
                    if (okAdd)
                    {
                        dsNCC.Add(ncc);
                        comboNCC.Items.Add($"{ncc.TenNCC} - {ncc.IdNCC}");
                        comboNCC.SelectedItem = $"{ncc.TenNCC} - {ncc.IdNCC}";
                        MessageBox.Show("Đã thêm nhà cung cấp mới!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi thêm NCC!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ============= Thêm thuốc (match Java) =============
        private void ThemThuocVaoTam()
        {
            var ten = (txtTenThuoc.Text ?? "").Trim();
            var tp = (txtThanhPhan.Text ?? "").Trim();
            var dvt = (txtDonViTinh.Text ?? "").Trim();
            var dm = (txtDanhMuc.Text ?? "").Trim();
            var xx = (txtXuatXu.Text ?? "").Trim();
            var slStr = (txtSoLuong.Text ?? "").Trim();
            var giaNhapStr = (txtGiaNhap.Text ?? "").Trim();
            var donGiaStr = (txtDonGia.Text ?? "").Trim();
            var hanSDStr = (txtHanSuDung.Text ?? "").Trim();

            if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(dvt) || string.IsNullOrEmpty(dm)
                || string.IsNullOrEmpty(slStr) || string.IsNullOrEmpty(giaNhapStr)
                || string.IsNullOrEmpty(donGiaStr) || string.IsNullOrEmpty(hanSDStr))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin thuốc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(slStr, out var sl) || sl <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(giaNhapStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var giaNhap) || giaNhap < 0)
            {
                MessageBox.Show("Giá nhập không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(donGiaStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParseExact(hanSDStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var hanSD))
            {
                MessageBox.Show("Hạn sử dụng phải đúng định dạng dd/MM/yyyy!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var idThuoc = SinhIDThuocTuDong();

            var t = new Thuoc
            {
                IdThuoc = idThuoc,
                TenThuoc = ten,
                ThanhPhan = tp,
                DonViTinh = dvt,
                DanhMuc = dm,
                XuatXu = xx,
                SoLuongTon = sl,
                GiaNhap = (double)giaNhap,
                DonGia = (double)donGia,
                HanSuDung = hanSD
            };
            listThuocTam.Add(t);

            var ct = new ChiTietPhieuNhap
            {
                IdPN = txtIDPN.Text,
                IdThuoc = idThuoc,
                SoLuong = sl,
                GiaNhap = (double)giaNhap
            };
            listChiTietTam.Add(ct);

            gridModel.Add(new RowThuoc
            {
                ID = idThuoc,
                Ten = ten,
                SL = sl,
                GiaNhap = giaNhap,
                DonGia = donGia,
                HanSD = hanSD.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
            });

            tongTien += (double)giaNhap * sl;
            lblTongTien.Text = $"Tổng tiền: {tongTien}";

            // reset
            txtTenThuoc.Clear(); txtThanhPhan.Clear(); txtDonViTinh.Clear(); txtDanhMuc.Clear(); txtXuatXu.Clear();
            txtSoLuong.Clear(); txtGiaNhap.Clear(); txtDonGia.Clear(); txtHanSuDung.Clear();
        }

        // ============= Lưu PN + CT + Thuốc (match Java) =============
        private void LuuPhieuNhap()
        {
            if (string.IsNullOrWhiteSpace(txtIDNV.Text))
            {
                MessageBox.Show("Vui lòng nhập IDNV!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboNCC.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (listThuocTam.Count == 0)
            {
                MessageBox.Show("Chưa có thuốc nào được nhập!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sel = comboNCC.SelectedItem.ToString();
            var idNCC = sel.Substring(sel.LastIndexOf('-') + 1).Trim();

            // 1) Lưu thuốc mới
            foreach (var t in listThuocTam)
            {
                var ok = AddThuoc(t); // TODO: ĐỔI: gọi Service/DAO thật (VD: _thuoc.Add(t) hoặc _thuocDAO.Insert(t))
                if (!ok)
                {
                    MessageBox.Show($"Lưu thuốc {t.TenThuoc} thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 2) Lưu phiếu nhập
            var pn = new PhieuNhap
            {
                IdPN = txtIDPN.Text,
                ThoiGian = ParseDateTime(txtThoiGian.Text, "dd/MM/yy HH:mm"),
                IdNV = txtIDNV.Text.Trim(),
                IdNCC = idNCC,
                TongTien = tongTien
            };

            if (!AddPhieuNhap(pn)) // TODO: ĐỔI: gọi Service/DAO thật (VD: _pn.Add(pn) hoặc _pnDAO.Insert(pn))
            {
                MessageBox.Show("Lưu phiếu nhập thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3) Lưu chi tiết phiếu nhập
            foreach (var c in listChiTietTam)
            {
                if (!AddChiTietPhieuNhap(c)) // TODO: ĐỔI: gọi Service/DAO thật (VD: _ct.Add(c) hoặc _ctDAO.Insert(c))
                {
                    MessageBox.Show("Lưu chi tiết phiếu nhập thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            MessageBox.Show("Lưu phiếu nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private static DateTime ParseDateTime(string s, string fmt)
        {
            return DateTime.TryParseExact(s, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                ? dt : DateTime.Now;
        }

        // ================== CÁC HÀM PLACEHOLDER (đặt TODO) ==================
        // Bạn chỉ cần thay phần thân các hàm này gọi sang Service/DAO thực tế
        // Hoặc xóa nhóm này và gọi thẳng Service/DAO tại chỗ (như comment TODO ở trên)

        private List<NhaCungCap> GetAllNCC()
        {
            // TODO: ĐỔI: trả về _ncc.GetAll(); hoặc _nccDAO.GetAll();
            return new List<NhaCungCap>(); // ← placeholder
        }

        private List<PhieuNhap> GetAllPhieuNhap()
        {
            // TODO: ĐỔI: trả về _pn.GetAll(); hoặc _pnDAO.GetAll();
            return new List<PhieuNhap>(); // ← placeholder
        }

        private List<Thuoc> GetAllThuoc()
        {
            // TODO: ĐỔI: trả về _thuoc.GetAll(); hoặc _thuocDAO.GetAll();
            return new List<Thuoc>(); // ← placeholder
        }

        private bool AddNCC(NhaCungCap ncc)
        {
            // TODO: ĐỔI: return _ncc.Add(ncc); hoặc _nccDAO.Insert(ncc);
            return true; // ← placeholder
        }

        private bool AddThuoc(Thuoc t)
        {
            // TODO: ĐỔI: return _thuoc.Add(t); hoặc _thuocDAO.Insert(t);
            return true; // ← placeholder
        }

        private bool AddPhieuNhap(PhieuNhap pn)
        {
            // TODO: ĐỔI: return _pn.Add(pn); hoặc _pnDAO.Insert(pn);
            return true; // ← placeholder
        }

        private bool AddChiTietPhieuNhap(ChiTietPhieuNhap ct)
        {
            // TODO: ĐỔI: return _ct.Add(ct); hoặc _ctDAO.Insert(ct);
            return true; // ← placeholder
        }
    }
}
