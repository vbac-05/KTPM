// File: UI/EditHoaDonDialog.cs
// Nguồn gốc: chuyển từ Java gui/EditHoaDonDialog.java
// Mục tiêu: Giữ nguyên UX: header thông tin HĐ + bảng chi tiết (Tên thuốc, Số lượng, Đơn giá, Thành tiền),
//           có combobox chọn thuốc + tự cập nhật Đơn giá/Thành tiền/Tổng tiền + Lưu/Hủy.
// Mapping & khác biệt chính:
// - Java JDialog -> C# Form                                              // CHANGED
// - SimpleDateFormat -> DateTime.ToString("dd/MM/yyyy HH:mm:ss")         // CHANGED
// - JTable + DefaultTableModel -> DataGridView + DataTable               // CHANGED
// - DefaultCellEditor JComboBox (editable) -> ComboBox editor WinForms   // CHANGED (dùng EditingControlShowing)
// - Controller gọi updateHoaDonWithDetails(...) -> TODO: bơm service     // TODO(INJECT)
// - Kiểu tiền: Java double -> C# double (giữ nguyên)                     // MATCHED
// - Icon resource /icon/... -> TODO: đặt lại theo WinForms Resources     // TODO(RES)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLThuocWin.Models;

namespace QLThuocWin.UI
{
    public class EditHoaDonDialog : Form
    {
        // --- Services/Controllers ---
        private readonly Func<List<Thuoc>> _getAllThuoc;                         // TODO(INJECT): thay bằng ThuocService.GetAll()
        private readonly Func<string, List<ChiTietHoaDon>> _getChiTietByIdHD;    // TODO(INJECT): nếu cần load lại
        private readonly Func<HoaDon, List<ChiTietHoaDon>, bool> _updateHoaDon;  // TODO(INJECT): HoaDonService.UpdateWithDetails()

        // --- State ---
        private readonly HoaDon _hoaDon;                     // SOURCE(JAVA): truyền sẵn HoaDon vào ctor
        private readonly List<ChiTietHoaDon> _chiTietList;   // SOURCE(JAVA): truyền danh sách chi tiết ban đầu
        private List<Thuoc> _allThuoc = new();               // dữ liệu lookup

        // --- Header controls ---
        private readonly TextBox txtIdHD = new() { ReadOnly = true };
        private readonly TextBox txtThoiGian = new() { ReadOnly = false }; // CHANGED: Java auto-fill now, vẫn cho sửa tay nếu cần
        private readonly TextBox txtIdNV = new();
        private readonly TextBox txtIdKH = new();
        private readonly TextBox txtTongTien = new() { ReadOnly = true };
        private readonly TextBox txtPhuongThucThanhToan = new();
        private readonly TextBox txtTrangThai = new();

        // --- Grid chi tiết ---
        private readonly DataGridView grid = new() { Dock = DockStyle.Fill, AutoGenerateColumns = false };
        private readonly DataTable dtChiTiet = new(); // Cột: TenThuoc, SoLuong, DonGia, ThanhTien

        // --- Buttons ---
        private readonly Button btnAdd = new() { Text = "Thêm thuốc" };
        private readonly Button btnDel = new() { Text = "Xóa dòng" };
        private readonly Button btnSave = new() { Text = "Lưu" };
        private readonly Button btnCancel = new() { Text = "Hủy", DialogResult = DialogResult.Cancel };

        public bool Updated { get; private set; } // SOURCE(JAVA): isUpdated()

        // --- CTOR public dùng trong thực tế ---
        public EditHoaDonDialog(
            HoaDon hoaDon,
            List<ChiTietHoaDon> chiTietList,
            Func<List<Thuoc>> getAllThuoc,
            Func<HoaDon, List<ChiTietHoaDon>, bool> updateHoaDon,
            Func<string, List<ChiTietHoaDon>>? getChiTietByIdHD = null)
        {
            // SOURCE(JAVA): super(parent, "Sửa hóa đơn", true)
            Text = "Sửa hóa đơn";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(900, 600);

            _hoaDon = hoaDon;
            _chiTietList = chiTietList;
            _getAllThuoc = getAllThuoc;               // TODO(INJECT)
            _updateHoaDon = updateHoaDon;             // TODO(INJECT)
            _getChiTietByIdHD = getChiTietByIdHD ?? (_ => new List<ChiTietHoaDon>());

            BuildHeader();
            BuildGrid();
            BuildButtonsBar();

            // Nạp lookup & dữ liệu
            _allThuoc = _getAllThuoc?.Invoke() ?? new List<Thuoc>();   // CHANGED: gọi service thay vì new ThuocController()
            FillHeaderFromHoaDon(_hoaDon);
            LoadChiTietToGrid(_chiTietList);

            // Sự kiện tổng hợp
            grid.CellValueChanged += (_, __) => RecalcAll();
            grid.CurrentCellDirtyStateChanged += (_, __) =>
            {
                if (grid.IsCurrentCellDirty) grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            btnAdd.Click += (_, __) => AddRow();
            btnDel.Click += (_, __) => DeleteRow();
            btnSave.Click += (_, __) => OnSave();
        }

        // --- UI layout ---

        private void BuildHeader()
        {
            var pnlInfo = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 4,
                RowCount = 3,
                Padding = new Padding(8),
                AutoSize = true
            };
            pnlInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            pnlInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            pnlInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            pnlInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));

            Controls.Add(pnlInfo);

            // Hàng 1
            pnlInfo.Controls.Add(new Label { Text = "Mã hóa đơn:", AutoSize = true }, 0, 0);
            pnlInfo.Controls.Add(txtIdHD, 1, 0);
            pnlInfo.Controls.Add(new Label { Text = "ID Nhân viên:", AutoSize = true }, 2, 0);
            pnlInfo.Controls.Add(txtIdNV, 3, 0);

            // Hàng 2 (Java để txtThoiGian & IDKH)
            pnlInfo.Controls.Add(new Label { Text = "Thời gian:", AutoSize = true }, 0, 1);
            pnlInfo.Controls.Add(txtThoiGian, 1, 1);
            pnlInfo.Controls.Add(new Label { Text = "ID Khách hàng:", AutoSize = true }, 2, 1);
            pnlInfo.Controls.Add(txtIdKH, 3, 1);

            // Hàng 3 (Tổng tiền & PT thanh toán & Trạng thái)
            pnlInfo.Controls.Add(new Label { Text = "Tổng tiền:", AutoSize = true }, 0, 2);
            pnlInfo.Controls.Add(txtTongTien, 1, 2);

            var flowRight = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            flowRight.Controls.Add(new Label { Text = "PT Thanh toán:", AutoSize = true });
            flowRight.Controls.Add(txtPhuongThucThanhToan);
            flowRight.Controls.Add(new Label { Text = "Trạng thái:", AutoSize = true, Margin = new Padding(16, 3, 3, 3) });
            flowRight.Controls.Add(txtTrangThai);
            pnlInfo.Controls.Add(flowRight, 2, 2);
            pnlInfo.SetColumnSpan(flowRight, 2);
        }

        private void BuildGrid()
        {
            // DataTable schema
            dtChiTiet.Columns.Add("TenThuoc", typeof(string));
            dtChiTiet.Columns.Add("SoLuong", typeof(int));
            dtChiTiet.Columns.Add("DonGia", typeof(double));
            dtChiTiet.Columns.Add("ThanhTien", typeof(double));

            grid.DataSource = dtChiTiet;

            // Cột 0: Tên thuốc -> ComboBox editable + autocomplete
            var colTen = new DataGridViewComboBoxColumn
            {
                HeaderText = "Tên thuốc",
                DataPropertyName = "TenThuoc",
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                AutoComplete = true // gợi ý trong danh sách
            };
            colTen.Items.AddRange(_allThuoc.Select(t => t.TenThuoc).Distinct().OrderBy(x => x).ToArray());
            grid.Columns.Add(colTen);

            // Cột 1: Số lượng (editable)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong"
            });

            // Cột 2: Đơn giá (readonly – cập nhật theo Tên thuốc)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Đơn giá",
                DataPropertyName = "DonGia",
                ReadOnly = true // MATCHED: Java tự động cập nhật
            });

            // Cột 3: Thành tiền (readonly)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Thành tiền",
                DataPropertyName = "ThanhTien",
                ReadOnly = true
            });

            // Thay vì tạo cột tự động 2 lần, xóa cột autogen mặc định
            grid.AutoGenerateColumns = false;
            // Style
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Gắn vào form
            Controls.Add(grid);

            // Cho phép gõ để lọc (gần tương đương Java KeyListener)
            grid.EditingControlShowing += (s, e) =>
            {
                if (grid.CurrentCell?.ColumnIndex == 0 && e.Control is ComboBox cb)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                }
            };
        }

        private void BuildButtonsBar()
        {
            var left = new FlowLayoutPanel { Dock = DockStyle.Left, AutoSize = true, Padding = new Padding(8) };
            left.Controls.Add(btnAdd);
            left.Controls.Add(btnDel);
            Controls.Add(left);

            var right = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Padding = new Padding(8) };
            right.Controls.Add(btnCancel);
            right.Controls.Add(btnSave);
            Controls.Add(right);

            // TODO(RES): set Image cho nút nếu dùng Resources
            // btnAdd.Image = Properties.Resources.Add;
            // ...
        }

        // --- Data binding helpers ---

        private void FillHeaderFromHoaDon(HoaDon hd)
        {
            txtIdHD.Text = hd.IdHD;
            // Java: new SimpleDateFormat("dd/MM/yyyy HH:mm:ss").format(new Date())
            txtThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // CHANGED: time now
            txtIdNV.Text = hd.IdNV;
            txtIdKH.Text = hd.IdKH;
            txtTongTien.Text = (hd.TongTien).ToString("0");      // MATCHED: "%.0f"
            txtPhuongThucThanhToan.Text = hd.PhuongThucThanhToan ?? string.Empty;
            txtTrangThai.Text = hd.TrangThaiDonHang ?? string.Empty;
        }

        private void LoadChiTietToGrid(List<ChiTietHoaDon> list)
        {
            dtChiTiet.Rows.Clear();
            foreach (var ct in list)
            {
                var donGia = ResolveDonGia(ct.TenThuoc);
                var soLuong = ct.SoLuong <= 0 ? 1 : ct.SoLuong;
                var thanhTien = soLuong * donGia;
                dtChiTiet.Rows.Add(ct.TenThuoc, soLuong, donGia, thanhTien);
            }
            RecalcAll();
        }

        private double ResolveDonGia(string tenThuoc)
        {
            // SOURCE(JAVA): duyệt allThuocList để lấy đơn giá theo tên
            var found = _allThuoc.FirstOrDefault(t => string.Equals(t.TenThuoc, tenThuoc, StringComparison.OrdinalIgnoreCase));
            return found?.DonGia ?? 0.0;
        }

        private string ResolveIdThuoc(string tenThuoc)
        {
            var found = _allThuoc.FirstOrDefault(t => string.Equals(t.TenThuoc, tenThuoc, StringComparison.OrdinalIgnoreCase));
            return found?.IdThuoc ?? "";
        }

        private void AddRow()
        {
            // SOURCE(JAVA): addRow("", 1, 0.0, 0.0)
            dtChiTiet.Rows.Add("", 1, 0.0, 0.0);
            if (grid.Rows.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[^1].Selected = true;
                grid.CurrentCell = grid.Rows[^1].Cells[0];
                grid.BeginEdit(true);
            }
        }

        private void DeleteRow()
        {
            if (grid.CurrentRow == null) return;
            grid.Rows.Remove(grid.CurrentRow);
            RecalcAll();
        }

        private void RecalcAll()
        {
            double total = 0;
            foreach (DataRow row in dtChiTiet.Rows)
            {
                var ten = Convert.ToString(row["TenThuoc"]) ?? "";
                // Số lượng
                int sl = 1;
                try
                {
                    sl = Convert.ToInt32(row["SoLuong"]);
                    if (sl < 0) sl = 1; // SOURCE(JAVA)
                }
                catch { sl = 1; }

                // Đơn giá theo tên thuốc
                var dg = ResolveDonGia(ten);

                var tt = sl * dg;
                // Ghi ngược lại nếu khác
                if (!Equals(row["DonGia"], dg)) row["DonGia"] = dg;
                if (!Equals(row["ThanhTien"], tt)) row["ThanhTien"] = tt;

                total += tt;
            }
            txtTongTien.Text = total.ToString("0"); // SOURCE(JAVA): "%.0f"
        }

        private void OnSave()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIdHD.Text))
                {
                    MessageBox.Show("Mã hóa đơn không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dtChiTiet.Rows.Count == 0)
                {
                    MessageBox.Show("Hóa đơn cần ít nhất một loại thuốc!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Header -> HoaDon
                var hd = new HoaDon
                {
                    IdHD = txtIdHD.Text.Trim(),
                    // Java parse "dd/MM/yyyy HH:mm:ss"
                    ThoiGian = DateTime.ParseExact(txtThoiGian.Text.Trim(), "dd/MM/yyyy HH:mm:ss", null), // CHANGED
                    IdNV = txtIdNV.Text.Trim(),
                    IdKH = txtIdKH.Text.Trim(),
                    TongTien = double.TryParse(txtTongTien.Text.Trim(), out var t) ? t : 0.0,
                    PhuongThucThanhToan = txtPhuongThucThanhToan.Text.Trim(),
                    TrangThaiDonHang = txtTrangThai.Text.Trim()
                };

                // Grid -> List<ChiTietHoaDon>
                var list = new List<ChiTietHoaDon>();
                foreach (DataRow row in dtChiTiet.Rows)
                {
                    var ten = Convert.ToString(row["TenThuoc"]) ?? "";
                    var idThuoc = ResolveIdThuoc(ten);
                    var dg = row["DonGia"] is DBNull ? 0.0 : Convert.ToDouble(row["DonGia"]);
                    var sl = row["SoLuong"] is DBNull ? 1 : Math.Max(1, Convert.ToInt32(row["SoLuong"]));

                    list.Add(new ChiTietHoaDon
                    {
                        IdHD = hd.IdHD,
                        IdThuoc = idThuoc,
                        TenThuoc = ten,
                        SoLuong = sl,
                        DonGia = dg
                    });
                }

                // Gọi service cập nhật
                // SOURCE(JAVA): hoaDonController.updateHoaDonWithDetails(hd, list)
                var ok = _updateHoaDon?.Invoke(hd, list) ?? false; // TODO(INJECT)
                if (ok)
                {
                    MessageBox.Show("Cập nhật hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Updated = true; // SOURCE(JAVA): updated = true
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
