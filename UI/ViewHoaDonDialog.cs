using System;
using System.Data;
using System.Drawing;
using System.Globalization; // MAPPED: dùng để format số & ngày giống Java
using System.Windows.Forms;

// MAPPED: File C# này được map từ 2 file Java:
// - gui.ViewHoaDonDetailDialog.java  (header hóa đơn + bảng chi tiết 5 cột)
// - gui.ViewChiTietHDDialog.java     (bảng chi tiết 3 cột + format đơn giá)

namespace QLThuocApp.UI
{
    public class ViewHoaDonDialog : Form
    {
        // ----- HEADER (MAPPED từ ViewHoaDonDetailDialog.java: IDHD, Thời gian, IDNV, IDKH, Tổng tiền, PT thanh toán, Trạng thái)
        TextBox txtMa = new TextBox { ReadOnly = true };
        TextBox txtNgay = new TextBox { ReadOnly = true };
        TextBox txtIdNV = new TextBox { ReadOnly = true };
        TextBox txtIdKH = new TextBox { ReadOnly = true };
        TextBox txtTongTien = new TextBox { ReadOnly = true };
        TextBox txtPTTT = new TextBox { ReadOnly = true };       // MAPPED: Phương thức thanh toán
        TextBox txtTrangThai = new TextBox { ReadOnly = true };  // MAPPED: Trạng thái đơn hàng

        DataGridView gridCT = new DataGridView();

        // --- OPTIONAL: inject service/repo để load theo idHD ---
        // TODO: thay interface/kiểu thật của bạn (DAO/Service)
        private readonly Func<string, object> _getHoaDonById;          // trả về HoaDon
        private readonly Func<string, DataTable> _getChiTietByIdHoaDon; // trả về DataTable chi tiết

        public ViewHoaDonDialog(
            // TODO: đổi kiểu tham số theo thực tế (vd: HoaDon hoaDon)
            object hoaDon = null,
            Func<string, object> getHoaDonById = null,                 // TODO: map tới HoaDonDAO/HoaDonController
            Func<string, DataTable> getChiTietByIdHoaDon = null        // TODO: map tới ChiTietHoaDonDAO.getByIdHD
        )
        {
            Text = "Xem chi tiết Hóa đơn";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(880, 560);

            _getHoaDonById = getHoaDonById;
            _getChiTietByIdHoaDon = getChiTietByIdHoaDon;

            var header = BuildHeaderTable();
            Controls.Add(header);

            // MAPPED: layout & label giống Java ViewHoaDonDetailDialog.java
            header.Controls.Add(new Label { Text = "Mã HĐ:", AutoSize = true }, 0, 0);
            header.Controls.Add(txtMa, 1, 0);
            header.Controls.Add(new Label { Text = "Thời gian:", AutoSize = true }, 2, 0);
            header.Controls.Add(txtNgay, 3, 0);

            header.Controls.Add(new Label { Text = "ID Nhân viên:", AutoSize = true }, 0, 1);
            header.Controls.Add(txtIdNV, 1, 1);
            header.Controls.Add(new Label { Text = "ID Khách hàng:", AutoSize = true }, 2, 1);
            header.Controls.Add(txtIdKH, 3, 1);

            header.Controls.Add(new Label { Text = "Tổng tiền:", AutoSize = true }, 0, 2);
            header.Controls.Add(txtTongTien, 1, 2);
            header.Controls.Add(new Label { Text = "PT Thanh toán:", AutoSize = true }, 2, 2);
            header.Controls.Add(txtPTTT, 3, 2);

            header.Controls.Add(new Label { Text = "Trạng thái:", AutoSize = true }, 0, 3);
            header.Controls.Add(txtTrangThai, 1, 3);

            // MAPPED: Bảng chi tiết 5 cột (giống ViewHoaDonDetailDialog.java)
            gridCT.Dock = DockStyle.Fill;
            gridCT.AutoGenerateColumns = false;
            gridCT.ReadOnly = true;
            gridCT.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCT.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID thuốc", DataPropertyName = "IdThuoc" });
            gridCT.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên thuốc", DataPropertyName = "TenThuoc" });
            gridCT.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", DataPropertyName = "SoLuong" });
            gridCT.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", DataPropertyName = "DonGia" });
            gridCT.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành tiền", DataPropertyName = "ThanhTien" });
            Controls.Add(gridCT);

            var panelBtn = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Padding = new Padding(12) };
            var btnClose = new Button { Text = "Đóng" };
            btnClose.Click += (s, e) => Close();
            panelBtn.Controls.Add(btnClose);
            Controls.Add(panelBtn);

            // DEMO hiển thị tạm nếu chưa truyền object
            if (hoaDon == null)
            {
                LoadDemo(); // TODO: bỏ khi nối dữ liệu thật
            }
            else
            {
                // TODO(MAPPED): Nếu bạn truyền entity HoaDon thật thì parse ra như dưới:
                // var hd = (HoaDon)hoaDon;
                // LoadHeader(hd.getIdHD(), hd.getThoiGian(), hd.getIdNV(), hd.getIdKH(), hd.getTongTien(), hd.getPhuongThucThanhToan(), hd.getTrangThaiDonHang());
                // LoadChiTiet(_getChiTietByIdHoaDon?.Invoke(hd.getIdHD()));
            }
        }

        private TableLayoutPanel BuildHeaderTable()
        {
            var t = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 4, AutoSize = true, Padding = new Padding(12) };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            return t;
        }

        // MAPPED: Hàm này “format” tương tự Java (DateHelper.toString + String.format)
        private void LoadHeader(string idHD, DateTime thoiGian, string idNV, string idKH, double tongTien, string pttt, string trangThai)
        {
            txtMa.Text = idHD ?? "";
            txtNgay.Text = thoiGian.ToString("dd/MM/yyyy HH:mm"); // MAPPED: dd/MM/yyyy HH:mm
            txtIdNV.Text = idNV ?? "";
            txtIdKH.Text = idKH ?? "";
            txtTongTien.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.0}", tongTien); // MAPPED: "%.1f"
            txtPTTT.Text = pttt ?? "";
            txtTrangThai.Text = trangThai ?? "";
        }

        // MAPPED: Nhận DataTable theo cột giống Java: IdThuoc, TenThuoc, SoLuong, DonGia, ThanhTien
        private void LoadChiTiet(DataTable dt)
        {
            gridCT.DataSource = dt;
        }

        // ===== DEMO =====
        private void LoadDemo()
        {
            LoadHeader("HD0001", DateTime.Now, "NV01", "KH01", 530000d, "Tiền mặt", "Đã thanh toán");

            var t = new DataTable();
            t.Columns.Add("IdThuoc");
            t.Columns.Add("TenThuoc");
            t.Columns.Add("SoLuong", typeof(int));
            t.Columns.Add("DonGia", typeof(double));
            t.Columns.Add("ThanhTien", typeof(double));
            t.Rows.Add("T001", "Paracetamol", 5, 12000.0, 60000.0);
            t.Rows.Add("T002", "Cefalexin", 10, 35000.0, 350000.0);
            t.Rows.Add("T010", "Vitamin C", 12, 10000.0, 120000.0);
            LoadChiTiet(t);
        }
    }
}
