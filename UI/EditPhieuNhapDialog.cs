using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using QLThuocWin.Models;
// using QLThuocWin.Services; // TODO: ĐỔI namespace Services/DAO thật của bạn

namespace QLThuocWin.UI
{
    public class EditPhieuNhapDialog : Form
    {
        // ================== SERVICE/DAO (đặt theo dự án của bạn) ==================
        // Gợi ý nếu bạn dùng Service/DAO, bỏ comment và ĐỔI tên lớp phù hợp:
        // private readonly NhaCungCapService _ncc = new NhaCungCapService();            // TODO: ĐỔI lớp/ctor
        // private readonly ThuocService _thuoc = new ThuocService();                    // TODO: ĐỔI lớp/ctor
        // private readonly PhieuNhapService _pn = new PhieuNhapService();               // TODO: ĐỔI lớp/ctor
        // private readonly ChiTietPhieuNhapService _ct = new ChiTietPhieuNhapService(); // TODO: ĐỔI lớp/ctor

        // --------- Controls (match Java) ----------
        TextBox txtIdPN, txtThoiGian, txtIdNV, txtNCC;
        ComboBox cbNCC;
        DefaultComboBoxModel modelNCCAdapter; // adapter mô phỏng
        Panel nccDetailPanel;
        TextBox txtNewIdNCC, txtNewTenNCC, txtNewDiaChi, txtNewSdt;
        Button btnAddNCC;

        DataGridView tblThuoc;
        BindingList<RowThuoc> modelThuoc = new BindingList<RowThuoc>();
        Button btnXoaThuoc;

        Panel panelNhapThuoc;
        TextBox txtTenThuoc, txtThanhPhan, txtDonViTinh, txtDanhMuc, txtXuatXu;
        TextBox txtSoLuong, txtGiaNhap, txtDonGia, txtHanSuDung;
        Button btnThemThuoc;

        Button btnSave, btnCancel;
        Label lblTongTien;

        // --------- Data (match Java) ----------
        PhieuNhap phieuNhap;
        List<ChiTietPhieuNhap> listCTPN = new List<ChiTietPhieuNhap>();
        List<NhaCungCap> allNCCList = new List<NhaCungCap>();
        List<Thuoc> allThuocList = new List<Thuoc>();
        Dictionary<string, Thuoc> mapThuocMoi = new Dictionary<string, Thuoc>(); // idThuoc -> Thuoc đầy đủ

        bool nccPanelVisible = false;

        // Row hiển thị (match 6 cột)
        class RowThuoc
        {
            public string ID { get; set; }
            public string Ten { get; set; }
            public int SL { get; set; }
            public double GiaNhap { get; set; }
            public double DonGia { get; set; }
            public string HanSD { get; set; } // dd/MM/yyyy
        }

        // ========= Constructor =========
        public EditPhieuNhapDialog(PhieuNhap pn /* TODO: ĐỔI nếu bạn truyền thêm tham số */)
        {
            Text = "Sửa phiếu nhập";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(900, 650);
            var root = new Panel { Dock = DockStyle.Fill };
            Controls.Add(root);

            // ----- load dữ liệu từ DAO/Service -----
            phieuNhap = pn ?? new PhieuNhap();
            allNCCList = GetAllNCC();                     // TODO: ĐỔI: _ncc.GetAll()
            allThuocList = GetAllThuoc();                 // TODO: ĐỔI: _thuoc.GetAll()
            listCTPN = GetCTPNByPhieuNhap(phieuNhap.IdPN); // TODO: ĐỔI: _ct.GetByIdPN(pn.IdPN)

            // ===== Panel info phiếu nhập =====
            var pnlInfo = new Panel { Left = 0, Top = 0, Width = 900, Height = 210 };
            root.Controls.Add(pnlInfo);

            AddL(pnlInfo, "Mã phiếu nhập:", 20, 20);
            txtIdPN = new TextBox { Left = 140, Top = 20, Width = 150, ReadOnly = true, Text = phieuNhap.IdPN };
            pnlInfo.Controls.Add(txtIdPN);

            AddL(pnlInfo, "Thời gian:", 340, 20);
            txtThoiGian = new TextBox
            {
                Left = 420, Top = 20, Width = 180,
                Text = phieuNhap.ThoiGian == default ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : phieuNhap.ThoiGian.ToString("dd/MM/yyyy HH:mm")
            };
            pnlInfo.Controls.Add(txtThoiGian);

            AddL(pnlInfo, "ID Nhân viên:", 20, 60);
            txtIdNV = new TextBox { Left = 140, Top = 60, Width = 150, Text = phieuNhap.IdNV ?? "" };
            pnlInfo.Controls.Add(txtIdNV);

            AddL(pnlInfo, "Nhà cung cấp:", 340, 60);
            cbNCC = new ComboBox { Left = 440, Top = 60, Width = 200, DropDownStyle = ComboBoxStyle.DropDown };
            pnlInfo.Controls.Add(cbNCC);

            // build model NCC
            modelNCCAdapter = new DefaultComboBoxModel();
            foreach (var n in allNCCList) modelNCCAdapter.AddElement(n.TenNCC);
            cbNCC.Items.AddRange(allNCCList.Select(n => n.TenNCC).Cast<object>().ToArray());
            cbNCC.Text = GetTenNCCById(phieuNhap.IdNCC);

            // lấy editor text
            txtNCC = cbNCC.Controls.OfType<TextBox>().FirstOrDefault() ?? new TextBox();
            txtNCC.TextChanged += (s, e) =>
            {
                var input = txtNCC.Text.Trim().ToLowerInvariant();
                cbNCC.Items.Clear();
                bool found = false;
                foreach (var n in allNCCList)
                {
                    if (n.TenNCC?.ToLowerInvariant().Contains(input) == true)
                    {
                        cbNCC.Items.Add(n.TenNCC);
                        found = true;
                    }
                }
                if (!found && !string.IsNullOrEmpty(input))
                    cbNCC.Items.Add("Thêm nhà cung cấp");
                cbNCC.DroppedDown = true;
                cbNCC.IntegralHeight = true;
                cbNCC.SelectedIndex = -1;
            };

            cbNCC.SelectionChangeCommitted += (s, e) =>
            {
                var selected = (string)cbNCC.SelectedItem;
                if (selected == "Thêm nhà cung cấp") ShowNCCDetailPanel(true);
                else ShowNCCDetailPanel(false);
            };

            // panel thêm NCC
            nccDetailPanel = new Panel { Left = 20, Top = 100, Width = 850, Height = 55, BorderStyle = BorderStyle.FixedSingle, Visible = false };
            pnlInfo.Controls.Add(nccDetailPanel);

            AddL(nccDetailPanel, "IDNCC:", 10, 15);
            txtNewIdNCC = new TextBox { Left = 62, Top = 15, Width = 80 };
            nccDetailPanel.Controls.Add(txtNewIdNCC);

            AddL(nccDetailPanel, "Tên NCC:", 150, 15);
            txtNewTenNCC = new TextBox { Left = 210, Top = 15, Width = 150 };
            nccDetailPanel.Controls.Add(txtNewTenNCC);

            AddL(nccDetailPanel, "Địa chỉ:", 370, 15);
            txtNewDiaChi = new TextBox { Left = 420, Top = 15, Width = 140 };
            nccDetailPanel.Controls.Add(txtNewDiaChi);

            AddL(nccDetailPanel, "SĐT:", 568, 15);
            txtNewSdt = new TextBox { Left = 596, Top = 15, Width = 110 };
            nccDetailPanel.Controls.Add(txtNewSdt);

            btnAddNCC = new Button { Left = 730, Top = 13, Width = 110, Height = 30, Text = "  Lưu NCC" /*, Image = ...*/ };
            btnAddNCC.Click += (s, e) => AddNCCAction();
            nccDetailPanel.Controls.Add(btnAddNCC);

            // ===== Bảng thuốc =====
            tblThuoc = new DataGridView
            {
                Left = 20,
                Top = 210,
                Width = 850,
                Height = 250,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false,
                DataSource = modelThuoc
            };
            // 6 cột: ID, Tên, SL(edit), Giá nhập(edit), Đơn giá, Hạn SD
            tblThuoc.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.ID), HeaderText = "ID", Width = 90, ReadOnly = true });
            tblThuoc.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.Ten), HeaderText = "Tên", Width = 220, ReadOnly = true });
            tblThuoc.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.SL), HeaderText = "SL", Width = 60 });
            tblThuoc.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.GiaNhap), HeaderText = "Giá nhập", Width = 100 });
            tblThuoc.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.DonGia), HeaderText = "Đơn giá", Width = 100, ReadOnly = true });
            tblThuoc.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.HanSD), HeaderText = "Hạn SD", Width = 120, ReadOnly = true });
            Controls.Add(tblThuoc);

            tblThuoc.CellValueChanged += (s, e) => { if (e.RowIndex >= 0 && (e.ColumnIndex == 2 || e.ColumnIndex == 3)) UpdateTongTien(); };
            tblThuoc.CurrentCellDirtyStateChanged += (s, e) => { if (tblThuoc.IsCurrentCellDirty) tblThuoc.CommitEdit(DataGridViewDataErrorContexts.Commit); };

            // Fill từ listCTPN
            foreach (var ct in listCTPN)
            {
                var t = allThuocList.FirstOrDefault(x => x.IdThuoc == ct.IdThuoc);
                modelThuoc.Add(new RowThuoc
                {
                    ID = ct.IdThuoc,
                    Ten = string.IsNullOrEmpty(ct.TenThuoc) ? t?.TenThuoc ?? "" : ct.TenThuoc,
                    SL = ct.SoLuong,
                    GiaNhap = ct.GiaNhap,
                    DonGia = t?.DonGia ?? 0.0,
                    HanSD = t?.HanSuDung?.ToString("dd/MM/yyyy") ?? ""
                });
            }

            // ===== Panel nhập thuốc (giống Java) =====
            InitPanelNhapThuoc();

            // ===== Xóa thuốc =====
            btnXoaThuoc = new Button { Left = 726, Top = 1, Width = 136, Height = 25, Text = "  Xóa thuốc" /*, Image = ...*/ };
            btnXoaThuoc.Click += (s, e) =>
            {
                if (tblThuoc.CurrentRow != null)
                {
                    var row = (RowThuoc)tblThuoc.CurrentRow.DataBoundItem;
                    modelThuoc.Remove(row);
                    UpdateTongTien();
                }
                else
                {
                    MessageBox.Show("Bạn phải chọn một dòng thuốc để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            panelNhapThuoc.Controls.Add(btnXoaThuoc);

            // ===== Footer: Tổng tiền + Lưu/Hủy =====
            lblTongTien = new Label { Text = "Tổng tiền: 0", Left = 20, Top = 580, Width = 300, Font = new Font("Tahoma", 10f) };
            Controls.Add(lblTongTien);

            btnSave = new Button { Text = "Lưu", Left = 700, Top = 575, Width = 80, Height = 30 };
            btnCancel = new Button { Text = "  Hủy", Left = 790, Top = 575, Width = 80, Height = 30 };
            Controls.Add(btnSave); Controls.Add(btnCancel);

            btnSave.Click += (s, e) => OnSave();
            btnCancel.Click += (s, e) => Close();

            UpdateTongTien();
        }

        // ===== Helpers UI =====
        private static void AddL(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label { Text = text, Left = x, Top = y, AutoSize = true });
        }

        private void ShowNCCDetailPanel(bool visible)
        {
            nccPanelVisible = visible;
            nccDetailPanel.Visible = visible;
        }

        private string GetTenNCCById(string idNCC)
        {
            return allNCCList.FirstOrDefault(n => n.IdNCC == idNCC)?.TenNCC ?? "";
        }
        private string GetIdNCCByTen(string tenNCC)
        {
            return allNCCList.FirstOrDefault(n => n.TenNCC.Equals(tenNCC, StringComparison.OrdinalIgnoreCase))?.IdNCC;
        }

        // ===== Panel nhập thuốc (match Java) =====
        private void InitPanelNhapThuoc()
        {
            panelNhapThuoc = new Panel { Left = 10, Top = 470, Width = 870, Height = 90 };
            Controls.Add(panelNhapThuoc);

            var lblInfo = new Label { Text = "Thông tin thuốc", Left = 10, Top = 0, AutoSize = true };
            panelNhapThuoc.Controls.Add(lblInfo);

            // hàng 1
            AddL(panelNhapThuoc, "Tên thuốc:", 10, 25);
            txtTenThuoc = new TextBox { Left = 70, Top = 25, Width = 110 }; panelNhapThuoc.Controls.Add(txtTenThuoc);

            AddL(panelNhapThuoc, "Thành phần:", 190, 25);
            txtThanhPhan = new TextBox { Left = 260, Top = 25, Width = 80 }; panelNhapThuoc.Controls.Add(txtThanhPhan);

            AddL(panelNhapThuoc, "Đơn vị tính:", 350, 25);
            txtDonViTinh = new TextBox { Left = 415, Top = 25, Width = 60 }; panelNhapThuoc.Controls.Add(txtDonViTinh);

            AddL(panelNhapThuoc, "Danh mục:", 480, 25);
            txtDanhMuc = new TextBox { Left = 545, Top = 25, Width = 70 }; panelNhapThuoc.Controls.Add(txtDanhMuc);

            btnThemThuoc = new Button { Text = "  Thêm thuốc", Left = 726, Top = 25, Width = 136, Height = 25 /*, Image = ...*/ };
            btnThemThuoc.Click += (s, e) => ThemThuocVaoBang();
            panelNhapThuoc.Controls.Add(btnThemThuoc);

            // hàng 2
            AddL(panelNhapThuoc, "Xuất xứ:", 10, 55);
            txtXuatXu = new TextBox { Left = 70, Top = 55, Width = 80 }; panelNhapThuoc.Controls.Add(txtXuatXu);

            AddL(panelNhapThuoc, "Số lượng:", 190, 55);
            txtSoLuong = new TextBox { Left = 260, Top = 55, Width = 60 }; panelNhapThuoc.Controls.Add(txtSoLuong);

            AddL(panelNhapThuoc, "Giá nhập:", 338, 55);
            txtGiaNhap = new TextBox { Left = 395, Top = 55, Width = 80 }; panelNhapThuoc.Controls.Add(txtGiaNhap);

            AddL(panelNhapThuoc, "Đơn giá:", 490, 55);
            txtDonGia = new TextBox { Left = 545, Top = 55, Width = 70 }; panelNhapThuoc.Controls.Add(txtDonGia);

            AddL(panelNhapThuoc, "Hạn SD:", 624, 55);
            txtHanSuDung = new TextBox { Left = 682, Top = 55, Width = 90 }; panelNhapThuoc.Controls.Add(txtHanSuDung);
        }

        // ===== Sinh ID Thuốc theo max hiện có (match Java logic tổng quát) =====
        private string SinhIDThuocTuDong()
        {
            var ds = GetAllThuoc(); // TODO: ĐỔI: _thuoc.GetAll()
            int max = 0;
            foreach (var t in ds)
            {
                var id = t.IdThuoc;
                if (!string.IsNullOrEmpty(id) && id.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(id.Substring(1), out var num)) max = Math.Max(max, num);
                }
            }
            return $"T{max + 1:000}";
        }

        // ===== Thêm thuốc vào bảng + map phụ =====
        private void ThemThuocVaoBang()
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

            if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(slStr) || string.IsNullOrEmpty(giaNhapStr) || string.IsNullOrEmpty(donGiaStr))
            {
                MessageBox.Show("Phải nhập đủ tên thuốc, số lượng, giá nhập, đơn giá bán!");
                return;
            }
            if (!int.TryParse(slStr, out int sl) || !double.TryParse(giaNhapStr, NumberStyles.Number, CultureInfo.InvariantCulture, out double giaNhap) || !double.TryParse(donGiaStr, NumberStyles.Number, CultureInfo.InvariantCulture, out double donGia))
            {
                MessageBox.Show("Số lượng, giá nhập, đơn giá phải là số!");
                return;
            }

            var idThuoc = SinhIDThuocTuDong();

            // add row
            modelThuoc.Add(new RowThuoc
            {
                ID = idThuoc,
                Ten = ten,
                SL = sl,
                GiaNhap = giaNhap,
                DonGia = donGia,
                HanSD = hanSDStr
            });

            // add map đầy đủ
            var thuocMoi = new Thuoc
            {
                IdThuoc = idThuoc,
                TenThuoc = ten,
                ThanhPhan = tp,
                DonViTinh = dvt,
                DanhMuc = dm,
                XuatXu = xx,
                SoLuongTon = sl,
                GiaNhap = giaNhap,
                DonGia = donGia,
                IsDeleted = false
            };
            try
            {
                if (!string.IsNullOrWhiteSpace(hanSDStr))
                    thuocMoi.HanSuDung = DateTime.ParseExact(hanSDStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch { thuocMoi.HanSuDung = null; }

            mapThuocMoi[idThuoc] = thuocMoi;

            // reset
            txtTenThuoc.Clear(); txtThanhPhan.Clear(); txtDonViTinh.Clear(); txtDanhMuc.Clear(); txtXuatXu.Clear();
            txtSoLuong.Clear(); txtGiaNhap.Clear(); txtDonGia.Clear(); txtHanSuDung.Clear();

            UpdateTongTien();
        }

        private void UpdateTongTien()
        {
            double sum = 0;
            foreach (var r in modelThuoc)
            {
                sum += r.SL * r.GiaNhap;
            }
            lblTongTien.Text = $"Tổng tiền: {sum:0}";
        }

        // ===== Lưu (match onSave Java) =====
        private void OnSave()
        {
            try
            {
                // Danh sách id thuốc hiện tại (sau sửa)
                var idThuocMoi = new HashSet<string>(modelThuoc.Select(r => r.ID));

                // Danh sách id thuốc gốc của phiếu nhập
                var idThuocCu = new HashSet<string>(listCTPN.Select(ct => ct.IdThuoc));

                // Các tập chênh lệch
                var idThuocBiXoa = new HashSet<string>(idThuocCu);
                idThuocBiXoa.ExceptWith(idThuocMoi);

                var idThuocMoiThem = new HashSet<string>(idThuocMoi);
                idThuocMoiThem.ExceptWith(idThuocCu);

                // Validate phần header
                var idPN = (txtIdPN.Text ?? "").Trim();
                var thoiGianStr = (txtThoiGian.Text ?? "").Trim();
                var idNV = (txtIdNV.Text ?? "").Trim();
                var tenNCC = (cbNCC.Text ?? "").Trim();

                if (string.IsNullOrEmpty(idPN) || string.IsNullOrEmpty(thoiGianStr) || string.IsNullOrEmpty(idNV) || string.IsNullOrEmpty(tenNCC))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                var idNCC = GetIdNCCByTen(tenNCC);
                if (idNCC == null && !nccPanelVisible)
                {
                    MessageBox.Show("Vui lòng thêm nhà cung cấp mới hoặc chọn NCC hợp lệ!");
                    return;
                }
                if (idNCC == null && nccPanelVisible)
                {
                    MessageBox.Show("Bạn đang mở panel thêm NCC. Hãy lưu NCC rồi chọn lại.");
                    return;
                }

                // 1) Xử lý thuốc bị xóa: set isDeleted = true + xóa chi tiết
                foreach (var id in idThuocBiXoa)
                {
                    var t = allThuocList.FirstOrDefault(x => x.IdThuoc == id);
                    if (t != null)
                    {
                        t.IsDeleted = true;
                        UpdateThuoc(t); // TODO: ĐỔI: _thuoc.Update(t) để update IsDeleted
                    }
                    DeleteChiTietByPNAndThuoc(idPN, id); // TODO: ĐỔI: _ct.DeleteByPhieuNhapAndThuoc(idPN, id)
                }

                // 2) Update phiếu nhập header
                var pnUpdate = new PhieuNhap
                {
                    IdPN = idPN,
                    ThoiGian = DateTime.ParseExact(thoiGianStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    IdNV = idNV,
                    IdNCC = idNCC
                };
                // tính tổng tiền mới
                double tong = 0;
                foreach (var r in modelThuoc) tong += r.SL * r.GiaNhap;
                pnUpdate.TongTien = tong;

                if (!UpdatePhieuNhap(pnUpdate)) // TODO: ĐỔI: _pn.Update(pnUpdate)
                {
                    MessageBox.Show("Cập nhật phiếu nhập thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3) Xóa chi tiết cũ rồi ghi lại (đơn giản nhất)
                DeleteChiTietByPN(idPN); // TODO: ĐỔI: _ct.DeleteByPhieuNhap(idPN)

                foreach (var r in modelThuoc)
                {
                    // Nếu là thuốc mới: add vào bảng Thuoc
                    if (idThuocMoiThem.Contains(r.ID))
                    {
                        if (mapThuocMoi.TryGetValue(r.ID, out var tnew))
                        {
                            AddThuoc(tnew); // TODO: ĐỔI: _thuoc.Add(tnew)
                        }
                        else
                        {
                            var t2 = new Thuoc
                            {
                                IdThuoc = r.ID,
                                TenThuoc = r.Ten,
                                SoLuongTon = r.SL,
                                GiaNhap = r.GiaNhap,
                                DonGia = r.DonGia,
                                IsDeleted = false
                            };
                            AddThuoc(t2); // TODO: ĐỔI: _thuoc.Add(t2)
                        }
                    }

                    // Lưu chi tiết
                    var ct = new ChiTietPhieuNhap
                    {
                        IdPN = idPN,
                        IdThuoc = r.ID,
                        TenThuoc = r.Ten,
                        SoLuong = r.SL,
                        GiaNhap = r.GiaNhap
                    };
                    AddChiTietPhieuNhap(ct); // TODO: ĐỔI: _ct.Add(ct)
                }

                MessageBox.Show("Đã cập nhật phiếu nhập!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi cập nhật phiếu nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== Thêm NCC mới từ panel NCC =====
        private void AddNCCAction()
        {
            if (string.IsNullOrWhiteSpace(txtNewIdNCC.Text) || string.IsNullOrWhiteSpace(txtNewTenNCC.Text))
            {
                MessageBox.Show("IDNCC và Tên NCC không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var ncc = new NhaCungCap
            {
                IdNCC = txtNewIdNCC.Text.Trim(),
                TenNCC = txtNewTenNCC.Text.Trim(),
                DiaChi = txtNewDiaChi.Text.Trim(),
                Sdt = txtNewSdt.Text.Trim()
            };
            if (AddNCC(ncc)) // TODO: ĐỔI: _ncc.Add(ncc)
            {
                allNCCList = GetAllNCC(); // TODO: ĐỔI: _ncc.GetAll()
                cbNCC.Items.Clear();
                cbNCC.Items.AddRange(allNCCList.Select(x => x.TenNCC).Cast<object>().ToArray());
                cbNCC.Text = ncc.TenNCC;
                ShowNCCDetailPanel(false);
                MessageBox.Show("Đã thêm nhà cung cấp mới!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Thêm NCC thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== PLACEHOLDER DAO CALLS (để bạn dễ ĐỔI) ==================
        // Tất cả các hàm dưới đều có // TODO: ĐỔI — hãy thay thân hàm gọi đúng Service/DAO của bạn.

        private List<NhaCungCap> GetAllNCC()
        {
            // TODO: ĐỔI → return _ncc.GetAll();
            return new List<NhaCungCap>();
        }
        private bool AddNCC(NhaCungCap n)
        {
            // TODO: ĐỔI → return _ncc.Add(n);
            return true;
        }

        private List<Thuoc> GetAllThuoc()
        {
            // TODO: ĐỔI → return _thuoc.GetAll();
            return new List<Thuoc>();
        }
        private bool AddThuoc(Thuoc t)
        {
            // TODO: ĐỔI → return _thuoc.Add(t);
            return true;
        }
        private bool UpdateThuoc(Thuoc t)
        {
            // TODO: ĐỔI → return _thuoc.Update(t);
            return true;
        }

        private List<ChiTietPhieuNhap> GetCTPNByPhieuNhap(string idPN)
        {
            // TODO: ĐỔI → return _ct.GetByIdPN(idPN);
            return new List<ChiTietPhieuNhap>();
        }
        private bool AddChiTietPhieuNhap(ChiTietPhieuNhap ct)
        {
            // TODO: ĐỔI → return _ct.Add(ct);
            return true;
        }
        private bool DeleteChiTietByPN(string idPN)
        {
            // TODO: ĐỔI → return _ct.DeleteByPhieuNhap(idPN);
            return true;
        }
        private bool DeleteChiTietByPNAndThuoc(string idPN, string idThuoc)
        {
            // TODO: ĐỔI → return _ct.DeleteByPhieuNhapAndThuoc(idPN, idThuoc);
            return true;
        }

        private bool UpdatePhieuNhap(PhieuNhap pn)
        {
            // TODO: ĐỔI → return _pn.Update(pn);
            return true;
        }
    }

    // Adapter đơn giản để note lại ý tưởng DefaultComboBoxModel từ Swing
    internal class DefaultComboBoxModel
    {
        private readonly List<string> items = new List<string>();
        public void AddElement(string s) => items.Add(s);
        public void RemoveAllElements() => items.Clear();
        public IEnumerable<string> Items => items;
    }
}
