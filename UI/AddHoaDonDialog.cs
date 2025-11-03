using System;using System;using System;

using System.Collections.Generic;

using System.ComponentModel;using System.Collections.Generic;using System.Collections.Generic;

using System.Drawing;

using System.Globalization;using System.ComponentModel;using System.ComponentModel;

using System.Linq;

using System.Windows.Forms;using System.Drawing;using System.Globalization;

using QLThuocApp.Entities;

using System.Globalization;using System.Linq;

namespace QLThuocApp.UI

{using System.Linq;using System.Drawing;

    /// <summary>

    /// Dialog thêm hóa đơn mớiusing System.Windows.Forms;using System.Windows.Forms;

    /// </summary>

    public class AddHoaDonDialog : Formusing QLThuocApp.Entities;using QLThuocApp.Entities;

    {

        // TODO: Inject DAO/Service thực tế

        // private readonly IHoaDonService _hoaDonService;

        // private readonly IKhachHangService _khachHangService;namespace QLThuocApp.UInamespace QLThuocApp.UI

        // private readonly IThuocService _thuocService;

{{

        TextBox txtIDHD, txtThoiGian, txtIDNV, txtIDKH;

        TextBox txtPhuongThuc, txtTrangThai;    /// <summary>    public class AddHoaDonDialog : Form

        Label lblTongTien;

        Button btnLuu, btnHuy;    /// Dialog thêm hóa đơn mới    {



        public AddHoaDonDialog()    /// </summary>        // ================== KHAI BÁO SERVICE/DAO ==================

        {

            Text = "Thêm hóa đơn mới";    public class AddHoaDonDialog : Form        // Bạn có thể đổi sang DAO/Repository tuỳ dự án:

            StartPosition = FormStartPosition.CenterParent;

            ClientSize = new Size(500, 350);    {        // VD: private readonly NhaCungCapDAO _ncc = new NhaCungCapDAO();

            FormBorderStyle = FormBorderStyle.FixedDialog;

            MaximizeBox = false;        // TODO: Inject DAO/Service thực tế        // HOẶC dùng DI qua constructor (mục cuối file)

            MinimizeBox = false;

        // private readonly IHoaDonService _hoaDonService;        private readonly NhaCungCapService _ncc = new NhaCungCapService();               // TODO: ĐỔI sang Service/DAO thật của bạn

            var tbl = new TableLayoutPanel

            {        // private readonly IKhachHangService _khachHangService;        private readonly PhieuNhapService _pn = new PhieuNhapService();                  // TODO: ĐỔI sang Service/DAO thật của bạn

                Dock = DockStyle.Fill,

                ColumnCount = 2,        // private readonly IThuocService _thuocService;        private readonly ThuocService _thuoc = new ThuocService();                       // TODO: ĐỔI sang Service/DAO thật của bạn

                RowCount = 8,

                Padding = new Padding(15)        private readonly ChiTietPhieuNhapService _ct = new ChiTietPhieuNhapService();    // TODO: ĐỔI sang Service/DAO thật của bạn

            };

            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));        TextBox txtIDHD, txtThoiGian, txtIDNV, txtIDKH;

            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

        TextBox txtPhuongThuc, txtTrangThai;        // --- Khu vực "phiếu nhập"

            // Row 0: IDHD

            tbl.Controls.Add(new Label { Text = "Mã hóa đơn:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);        Label lblTongTien;        TextBox txtIDPN = new TextBox { ReadOnly = true };

            txtIDHD = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };

            tbl.Controls.Add(txtIDHD, 1, 0);        Button btnLuu, btnHuy;        TextBox txtThoiGian = new TextBox { ReadOnly = true };



            // Row 1: Thời gian        TextBox txtIDNV = new TextBox();

            tbl.Controls.Add(new Label { Text = "Thời gian:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);

            txtThoiGian = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };        public AddHoaDonDialog()        TextBox txtNCC = new TextBox();                  // ô gõ để gợi ý

            tbl.Controls.Add(txtThoiGian, 1, 1);

        {        ComboBox comboNCC = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

            // Row 2: IDNV

            tbl.Controls.Add(new Label { Text = "Mã nhân viên:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);            Text = "Thêm hóa đơn mới";        Button btnThemNCC = new Button { Text = "   Thêm nhà cung cấp" };

            txtIDNV = new TextBox { Dock = DockStyle.Fill };

            tbl.Controls.Add(txtIDNV, 1, 2);            StartPosition = FormStartPosition.CenterParent;



            // Row 3: IDKH            ClientSize = new Size(500, 350);        // --- Khu vực "thông tin thuốc"

            tbl.Controls.Add(new Label { Text = "Mã khách hàng:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);

            txtIDKH = new TextBox { Dock = DockStyle.Fill };            FormBorderStyle = FormBorderStyle.FixedDialog;        TextBox txtTenThuoc = new TextBox();

            tbl.Controls.Add(txtIDKH, 1, 3);

            MaximizeBox = false;        TextBox txtThanhPhan = new TextBox();

            // Row 4: Phương thức thanh toán

            tbl.Controls.Add(new Label { Text = "Phương thức TT:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 4);            MinimizeBox = false;        TextBox txtDonViTinh = new TextBox();

            txtPhuongThuc = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Tiền mặt, Chuyển khoản..." };

            tbl.Controls.Add(txtPhuongThuc, 1, 4);        TextBox txtDanhMuc = new TextBox();



            // Row 5: Trạng thái            var tbl = new TableLayoutPanel        TextBox txtXuatXu = new TextBox();

            tbl.Controls.Add(new Label { Text = "Trạng thái:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 5);

            txtTrangThai = new TextBox { Dock = DockStyle.Fill, Text = "Đã thanh toán" };            {        TextBox txtSoLuong = new TextBox();

            tbl.Controls.Add(txtTrangThai, 1, 5);

                Dock = DockStyle.Fill,        TextBox txtGiaNhap = new TextBox();

            // Row 6: Tổng tiền

            tbl.Controls.Add(new Label { Text = "Tổng tiền:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 6);                ColumnCount = 2,        TextBox txtDonGia = new TextBox();

            lblTongTien = new Label { Text = "0", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font(Font.FontFamily, 10, FontStyle.Bold) };

            tbl.Controls.Add(lblTongTien, 1, 6);                RowCount = 8,        TextBox txtHanSuDung = new TextBox(); // dd/MM/yyyy



            // Row 7: Buttons                Padding = new Padding(15)        Button btnThemThuoc = new Button { Text = "  Thêm thuốc" };

            var pnlBtn = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };

            btnHuy = new Button { Text = "Hủy", Width = 80 };            };        Button btnXoaThuoc = new Button { Text = "   Xóa thuốc" };

            btnLuu = new Button { Text = "Lưu", Width = 80 };

            pnlBtn.Controls.Add(btnHuy);            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            pnlBtn.Controls.Add(btnLuu);

            tbl.SetColumnSpan(pnlBtn, 2);            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));        // --- Bảng & tổng tiền

            tbl.Controls.Add(pnlBtn, 0, 7);

        DataGridView grid = new DataGridView();

            Controls.Add(tbl);

            // Row 0: IDHD        BindingList<RowThuoc> viewRows = new BindingList<RowThuoc>();

            // Khởi tạo giá trị mặc định

            GenerateIDHD();            tbl.Controls.Add(new Label { Text = "Mã hóa đơn:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);        Label lblTongTien = new Label { Text = "Tổng tiền: 0" };

            txtThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            txtIDHD = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };

            // Events

            btnLuu.Click += BtnLuu_Click;            tbl.Controls.Add(txtIDHD, 1, 0);        // --- Nút Lưu/Hủy

            btnHuy.Click += (s, e) => Close();

        }        Button btnLuu = new Button { Text = "    Lưu" };



        private void GenerateIDHD()            // Row 1: Thời gian        Button btnHuy = new Button { Text = "    Hủy" };

        {

            // TODO: Lấy từ database để sinh ID tự động            tbl.Controls.Add(new Label { Text = "Thời gian:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);

            // var lastID = _hoaDonService.GetLastID();

            // txtIDHD.Text = GenerateNextID(lastID);            txtThoiGian = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };        // --- Dữ liệu tạm (giống Java)

            txtIDHD.Text = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");

        }            tbl.Controls.Add(txtThoiGian, 1, 1);        List<Thuoc> listThuocTam = new List<Thuoc>();



        private void BtnLuu_Click(object sender, EventArgs e)        List<ChiTietPhieuNhap> listChiTietTam = new List<ChiTietPhieuNhap>();

        {

            // Validation            // Row 2: IDNV        double tongTien = 0.0;

            if (string.IsNullOrWhiteSpace(txtIDNV.Text))

            {            tbl.Controls.Add(new Label { Text = "Mã nhân viên:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);        List<NhaCungCap> dsNCC = new List<NhaCungCap>();

                MessageBox.Show("Vui lòng nhập mã nhân viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtIDNV.Focus();            txtIDNV = new TextBox { Dock = DockStyle.Fill };

                return;

            }            tbl.Controls.Add(txtIDNV, 1, 2);        // --- Row hiển thị trong grid



            if (string.IsNullOrWhiteSpace(txtIDKH.Text))        class RowThuoc

            {

                MessageBox.Show("Vui lòng nhập mã khách hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);            // Row 3: IDKH        {

                txtIDKH.Focus();

                return;            tbl.Controls.Add(new Label { Text = "Mã khách hàng:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);            public string Id { get; set; }

            }

            txtIDKH = new TextBox { Dock = DockStyle.Fill };            public string Ten { get; set; }

            // TODO: Tạo đối tượng HoaDon và lưu vào database

            var hoaDon = new HoaDon            tbl.Controls.Add(txtIDKH, 1, 3);            public int SL { get; set; }

            {

                IdHD = txtIDHD.Text.Trim(),            public decimal GiaNhap { get; set; }

                ThoiGian = DateTime.Now,

                IdNV = txtIDNV.Text.Trim(),            // Row 4: Phương thức thanh toán            public decimal DonGia { get; set; }

                IdKH = txtIDKH.Text.Trim(),

                TongTien = 0, // TODO: Tính từ chi tiết hóa đơn            tbl.Controls.Add(new Label { Text = "Phương thức TT:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 4);            public string HanSD { get; set; } // dd/MM/yyyy (hiển thị)

                PhuongThucThanhToan = txtPhuongThuc.Text.Trim(),

                TrangThaiDonHang = txtTrangThai.Text.Trim()            txtPhuongThuc = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Tiền mặt, Chuyển khoản..." };        }

            };

            tbl.Controls.Add(txtPhuongThuc, 1, 4);

            try

            {        public AddPhieuNhapDialog()

                // TODO: Lưu vào database

                // bool success = _hoaDonService.Add(hoaDon);            // Row 5: Trạng thái        {

                // if (success)

                // {            tbl.Controls.Add(new Label { Text = "Trạng thái:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 5);            Text = "Thêm phiếu nhập mới";

                    MessageBox.Show("Thêm hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;            txtTrangThai = new TextBox { Dock = DockStyle.Fill, Text = "Đã thanh toán" };            StartPosition = FormStartPosition.CenterParent;

                    Close();

                // }            tbl.Controls.Add(txtTrangThai, 1, 5);            ClientSize = new Size(940, 520);

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Lỗi khi thêm hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);            // Row 6: Tổng tiền            // ===== Layout bám sát Java =====

            }

        }            tbl.Controls.Add(new Label { Text = "Tổng tiền:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 6);            var root = new Panel { Dock = DockStyle.Fill };

    }

}            lblTongTien = new Label { Text = "0", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font(Font.FontFamily, 10, FontStyle.Bold) };            Controls.Add(root);


            tbl.Controls.Add(lblTongTien, 1, 6);

            // Panel phiếu nhập

            // Row 7: Buttons            var pnPN = new Panel { Left = 10, Top = 10, Width = 900, Height = 90 };

            var pnlBtn = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };            root.Controls.Add(pnPN);

            btnHuy = new Button { Text = "Hủy", Width = 80 };

            btnLuu = new Button { Text = "Lưu", Width = 80 };            var lblIDPN = new Label { Left = 10, Top = 10, Width = 50, Text = "IDPN:" };

            pnlBtn.Controls.Add(btnHuy);            var lblThoiGian = new Label { Left = 221, Top = 10, Width = 70, Text = "Thời gian:" };

            pnlBtn.Controls.Add(btnLuu);            var lblIDNV = new Label { Left = 480, Top = 10, Width = 50, Text = "IDNV:" };

            tbl.SetColumnSpan(pnlBtn, 2);            var lblNCC = new Label { Left = 10, Top = 45, Width = 100, Text = "Nhà cung cấp:" };

            tbl.Controls.Add(pnlBtn, 0, 7);

            txtIDPN.SetBounds(110, 10, 80, 25);

            Controls.Add(tbl);            txtThoiGian.SetBounds(299, 10, 150, 25);

            txtIDNV.SetBounds(538, 10, 80, 25);

            // Khởi tạo giá trị mặc định

            GenerateIDHD();            txtNCC.SetBounds(110, 45, 200, 25);

            txtThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");            comboNCC.SetBounds(320, 45, 240, 25);

            btnThemNCC.SetBounds(603, 45, 200, 25);

            // Events            btnThemNCC.Visible = false;

            btnLuu.Click += BtnLuu_Click;

            btnHuy.Click += (s, e) => Close();            pnPN.Controls.AddRange(new Control[]

        }            {

                lblIDPN, txtIDPN, lblThoiGian, txtThoiGian, lblIDNV, txtIDNV,

        private void GenerateIDHD()                lblNCC, txtNCC, comboNCC, btnThemNCC

        {            });

            // TODO: Lấy từ database để sinh ID tự động

            // var lastID = _hoaDonService.GetLastID();            // Panel thuốc

            // txtIDHD.Text = GenerateNextID(lastID);            var pnThuoc = new GroupBox { Left = 10, Top = 110, Width = 900, Height = 130, Text = "Thông tin thuốc" };

            txtIDHD.Text = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");            root.Controls.Add(pnThuoc);

        }

            AddL(pnThuoc, "Tên thuốc:", 10, 20, out _);

        private void BtnLuu_Click(object sender, EventArgs e)            txtTenThuoc.SetBounds(80, 20, 120, 25); pnThuoc.Controls.Add(txtTenThuoc);

        {

            // Validation            AddL(pnThuoc, "Thành phần:", 210, 20, out _);

            if (string.IsNullOrWhiteSpace(txtIDNV.Text))            txtThanhPhan.SetBounds(290, 20, 80, 25); pnThuoc.Controls.Add(txtThanhPhan);

            {

                MessageBox.Show("Vui lòng nhập mã nhân viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);            AddL(pnThuoc, "Đơn vị tính:", 378, 20, out _);

                txtIDNV.Focus();            txtDonViTinh.SetBounds(444, 20, 80, 25); pnThuoc.Controls.Add(txtDonViTinh);

                return;

            }            AddL(pnThuoc, "Danh mục:", 542, 20, out _);

            txtDanhMuc.SetBounds(603, 20, 90, 25); pnThuoc.Controls.Add(txtDanhMuc);

            if (string.IsNullOrWhiteSpace(txtIDKH.Text))

            {            btnThemThuoc.SetBounds(707, 20, 145, 25); pnThuoc.Controls.Add(btnThemThuoc);

                MessageBox.Show("Vui lòng nhập mã khách hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtIDKH.Focus();            AddL(pnThuoc, "Xuất xứ:", 10, 55, out _);

                return;            txtXuatXu.SetBounds(80, 55, 120, 25); pnThuoc.Controls.Add(txtXuatXu);

            }

            AddL(pnThuoc, "Số lượng:", 210, 55, out _);

            // TODO: Tạo đối tượng HoaDon và lưu vào database            txtSoLuong.SetBounds(290, 55, 60, 25); pnThuoc.Controls.Add(txtSoLuong);

            var hoaDon = new HoaDon

            {            AddL(pnThuoc, "Giá nhập:", 360, 55, out _);

                IdHD = txtIDHD.Text.Trim(),            txtGiaNhap.SetBounds(420, 55, 80, 25); pnThuoc.Controls.Add(txtGiaNhap);

                ThoiGian = DateTime.Now,

                IdNV = txtIDNV.Text.Trim(),            AddL(pnThuoc, "Đơn giá:", 510, 55, out _);

                IdKH = txtIDKH.Text.Trim(),            txtDonGia.SetBounds(570, 55, 80, 25); pnThuoc.Controls.Add(txtDonGia);

                TongTien = 0, // TODO: Tính từ chi tiết hóa đơn

                PhuongThucThanhToan = txtPhuongThuc.Text.Trim(),            AddL(pnThuoc, "Hạn SD:", 660, 55, out _);

                TrangThaiDonHang = txtTrangThai.Text.Trim()            txtHanSuDung.SetBounds(710, 55, 80, 25); pnThuoc.Controls.Add(txtHanSuDung);

            };

            // Grid

            try            grid.Left = 10; grid.Top = 250; grid.Width = 900; grid.Height = 130;

            {            grid.ReadOnly = true;

                // TODO: Lưu vào database            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // bool success = _hoaDonService.Add(hoaDon);            grid.MultiSelect = false;

                // if (success)            grid.AutoGenerateColumns = false;

                // {            grid.AllowUserToAddRows = false;

                    MessageBox.Show("Thêm hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);            grid.DataSource = viewRows;

                    DialogResult = DialogResult.OK;

                    Close();            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.Id), HeaderText = "ID", Width = 80 });

                // }            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.Ten), HeaderText = "Tên", Width = 220 });

            }            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.SL), HeaderText = "SL", Width = 60 });

            catch (Exception ex)            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.GiaNhap), HeaderText = "Giá nhập", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "0.##" } });

            {            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.DonGia), HeaderText = "Đơn giá", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "0.##" } });

                MessageBox.Show($"Lỗi khi thêm hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RowThuoc.HanSD), HeaderText = "Hạn SD", Width = 120 });

            }            root.Controls.Add(grid);

        }

    }            // Tổng tiền + nút xóa

}            btnXoaThuoc.SetBounds(10, 390, 150, 30); root.Controls.Add(btnXoaThuoc);

            lblTongTien.SetBounds(750, 390, 160, 25); root.Controls.Add(lblTongTien);

            // Nút Lưu/Hủy
            btnLuu.SetBounds(700, 430, 100, 35);
            btnHuy.SetBounds(810, 430, 100, 35);
            root.Controls.Add(btnLuu);
            root.Controls.Add(btnHuy);

            // ===== Khởi tạo dữ liệu mặc định =====
            SinhIDPNTuDong();
            txtThoiGian.Text = DateTime.Now.ToString("dd/MM/yy HH:mm", CultureInfo.InvariantCulture);

            // Lấy danh sách NCC (map Java: nccController.getAllNhaCungCap())
            dsNCC = _ncc.GetAll(); // TODO: ĐỔI nếu hàm/lớp khác tên (VD: _nccDAO.GetAll())
            comboNCC.Items.Clear();

            // Gợi ý NCC khi gõ trong txtNCC
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

            // Xóa thuốc
            btnXoaThuoc.Click += (s, e) =>
            {
                if (grid.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng thuốc muốn xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var row = grid.CurrentRow.DataBoundItem as RowThuoc;
                if (row == null) return;

                // Xóa khỏi list tạm
                listThuocTam.RemoveAll(t => string.Equals(t.IdThuoc, row.Id, StringComparison.OrdinalIgnoreCase));
                listChiTietTam.RemoveAll(ct => string.Equals(ct.IdThuoc, row.Id, StringComparison.OrdinalIgnoreCase));

                // Trừ tổng tiền = giaNhap * soLuong
                tongTien -= (double)row.GiaNhap * row.SL;
                if (tongTien < 0) tongTien = 0;
                lblTongTien.Text = $"Tổng tiền: {tongTien}";

                // Xóa khỏi view
                viewRows.Remove(row);
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

        // ====== Sinh IDPN tự động (match Java) ======
        private void SinhIDPNTuDong()
        {
            // Java: lấy ds PN, lấy lastID, tăng số
            var ds = _pn.GetAll(); // TODO: ĐỔI nếu dùng DAO khác tên/hàm (VD: _pnDAO.GetAll())
            var lastID = "PN000";
            if (ds != null && ds.Count > 0)
                lastID = ds[^1].IdPN ?? "PN000";

            var num = ParseTrailingNumber(lastID);
            txtIDPN.Text = $"PN{num + 1:000}";
        }

        // ====== Sinh ID thuốc tự động (match Java) ======
        private string SinhIDThuocTuDong()
        {
            var ds = _thuoc.GetAll(); // TODO: ĐỔI nếu dùng DAO khác tên/hàm (VD: _thuocRepo.GetAll())
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
            // Nếu ID của bạn có format khác, hãy đổi logic cắt số ở đây cho phù hợp.
        }

        // ====== Dialog thêm NCC mới (match Java) ======
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
                    var okAdd = _ncc.Add(ncc); // TODO: ĐỔI tên hàm nếu khác (VD: _nccDAO.Insert(ncc))
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

        // ====== Thêm thuốc vào danh sách tạm (match Java) ======
        private void ThemThuocVaoTam()
        {
            var ten = txtTenThuoc.Text?.Trim() ?? "";
            var tp = txtThanhPhan.Text?.Trim() ?? "";
            var dvt = txtDonViTinh.Text?.Trim() ?? "";
            var dm = txtDanhMuc.Text?.Trim() ?? "";
            var xx = txtXuatXu.Text?.Trim() ?? "";
            var slStr = txtSoLuong.Text?.Trim() ?? "";
            var giaNhapStr = txtGiaNhap.Text?.Trim() ?? "";
            var donGiaStr = txtDonGia.Text?.Trim() ?? "";
            var hanSDStr = txtHanSuDung.Text?.Trim() ?? "";

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

            viewRows.Add(new RowThuoc
            {
                Id = idThuoc,
                Ten = ten,
                SL = sl,
                GiaNhap = giaNhap,
                DonGia = donGia,
                HanSD = hanSD.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
            });

            tongTien += (double)giaNhap * sl;
            lblTongTien.Text = $"Tổng tiền: {tongTien}";

            // Reset ô nhập
            txtTenThuoc.Clear(); txtThanhPhan.Clear(); txtDonViTinh.Clear(); txtDanhMuc.Clear(); txtXuatXu.Clear();
            txtSoLuong.Clear(); txtGiaNhap.Clear(); txtDonGia.Clear(); txtHanSuDung.Clear();
        }

        // ====== Lưu phiếu nhập (match Java) ======
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

            // Lưu THUỐC trước
            foreach (var t in listThuocTam)
            {
                var ok = _thuoc.Add(t); // TODO: ĐỔI tên hàm nếu khác (VD: _thuocDAO.Insert(t))
                if (!ok)
                {
                    MessageBox.Show($"Lưu thuốc {t.TenThuoc} thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Lưu PHIẾU NHẬP
            var pn = new PhieuNhap
            {
                IdPN = txtIDPN.Text,
                ThoiGian = ParseDateTime(txtThoiGian.Text, "dd/MM/yy HH:mm"),
                IdNV = txtIDNV.Text.Trim(),
                IdNCC = idNCC,
                TongTien = tongTien
            };

            if (!_pn.Add(pn)) // TODO: ĐỔI tên hàm nếu khác (VD: _pnRepo.Insert(pn))
            {
                MessageBox.Show("Lưu phiếu nhập thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lưu CHI TIẾT PHIẾU NHẬP
            foreach (var cti in listChiTietTam)
            {
                if (!_ct.Add(cti)) // TODO: ĐỔI tên hàm nếu khác (VD: _ctDAO.Insert(cti))
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
            if (DateTime.TryParseExact(s, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            return DateTime.Now;
        }

        // ====== (Tuỳ chọn) Dùng DI qua constructor ======
        // Nếu bạn có interface & container, dùng mẫu này thay cho 4 field ở đầu:
        //
        // private readonly INhaCungCapService _ncc;
        // private readonly IPhieuNhapService _pn;
        // private readonly IThuocService _thuoc;
        // private readonly IChiTietPhieuNhapService _ct;
        //
        // public AddPhieuNhapDialog(
        //     INhaCungCapService ncc,
        //     IPhieuNhapService pn,
        //     IThuocService thuoc,
        //     IChiTietPhieuNhapService ct)
        // {
        //     _ncc = ncc; _pn = pn; _thuoc = thuoc; _ct = ct;
        //     // ... phần còn lại giữ nguyên
        // }

    }
}
