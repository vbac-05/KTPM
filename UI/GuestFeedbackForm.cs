using System;
using System.Drawing;
using System.Windows.Forms;
// using QLThuocWin.Services; // TODO: ĐỔI: namespace Service/DAO thực tế của bạn
// using QLThuocWin.Models;   // TODO: ĐỔI: nếu cần entity KhachHang/PhanHoi

namespace QLThuocWin.UI
{
    public class GuestFeedbackForm : Form
    {
        // ====== Controls (match Java fields) ======
        TextBox txtSDT = new TextBox();
        TextBox txtIdHD = new TextBox();
        TextBox txtNoiDung = new TextBox();
        TextBox txtDanhGia = new TextBox();
        Button btnSubmit = new Button();
        Button btnCancel = new Button();

        // ====== Services/DAO placeholders ======
        // Gợi ý: bạn có thể tiêm qua constructor để test dễ hơn
        // private readonly PhanHoiService _phanHoi;          // TODO: ĐỔI: lớp Controller/Service thật (vd: PhanHoiController)
        // private readonly ChiTietHoaDonService _cthd;       // TODO: ĐỔI: lớp DAO/Service thật (vd: ChiTietHoaDonDAO)
        // private readonly KhachHangService _kh;             // TODO: ĐỔI: lớp DAO/Service thật (vd: KhachHangDAO)

        public GuestFeedbackForm()
        {
            Text = "Chế độ khách - Gửi phản hồi";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(480, 320);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(16),
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            Controls.Add(root);

            var lblTitle = new Label
            {
                Text = "Gửi phản hồi (Chế độ khách)",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            // hàng tiêu đề (span 2 cột)
            root.Controls.Add(lblTitle, 0, 0);
            root.SetColumnSpan(lblTitle, 2);

            // SĐT
            root.Controls.Add(new Label { Text = "Số điện thoại:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            txtSDT.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            root.Controls.Add(txtSDT, 1, 1);

            // IDHD
            root.Controls.Add(new Label { Text = "ID Hóa đơn:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
            txtIdHD.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            root.Controls.Add(txtIdHD, 1, 2);

            // Nội dung
            root.Controls.Add(new Label { Text = "Nội dung:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
            txtNoiDung.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            root.Controls.Add(txtNoiDung, 1, 3);

            // Đánh giá
            root.Controls.Add(new Label { Text = "Đánh giá (1-5):", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
            txtDanhGia.Width = 60;
            root.Controls.Add(txtDanhGia, 1, 4);

            // Buttons
            var pnlBtns = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
            btnSubmit.Text = "Gửi";
            // btnSubmit.Image = Image.FromFile("..."); // TODO: ĐỔI: icon nếu bạn dùng resource như Java (GuestFeedbackForm.class.getResource("/icon/sent.png"))
            btnSubmit.AutoSize = true;
            btnSubmit.Click += (s, e) => SubmitFeedback(); // sự kiện chính

            btnCancel.Text = "Hủy";
            // btnCancel.Image = Image.FromFile("..."); // TODO: ĐỔI: icon nếu cần
            btnCancel.AutoSize = true;
            btnCancel.Click += (s, e) =>
            {
                // TODO: ĐỔI: điều hướng về màn hình đăng nhập thực tế của bạn
                // new LoginForm().Show(); // nếu có form này
                Close();
            };

            pnlBtns.Controls.Add(btnSubmit);
            pnlBtns.Controls.Add(btnCancel);
            root.Controls.Add(new Label(), 0, 5); // filler
            root.Controls.Add(pnlBtns, 1, 5);
        }

        /// <summary>
        /// SubmitFeedback – match logic Java:
        /// 1) Lấy sdt, idHD, nội dung, đánh giá
        /// 2) Validate rỗng + validate đánh giá 1..5
        /// 3) Lookup idThuoc đầu tiên theo idHD (để đảm bảo hóa đơn có thuốc)  // TODO: ĐỔI: gọi DAO/Service thật
        /// 4) Gọi controller: addPhanHoiGuest(idHD, sdt, noiDung, danhGia)     // TODO: ĐỔI: gọi Controller thật
        /// </summary>
        private void SubmitFeedback()
        {
            var sdt = (txtSDT.Text ?? "").Trim();
            var idHD = (txtIdHD.Text ?? "").Trim();
            var noiDung = (txtNoiDung.Text ?? "").Trim();
            var strDanhGia = (txtDanhGia.Text ?? "").Trim();

            if (string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(idHD) || string.IsNullOrEmpty(noiDung) || string.IsNullOrEmpty(strDanhGia))
            {
                MessageBox.Show("Vui lòng điền hết các trường", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(strDanhGia, out var danhGia) || danhGia < 1 || danhGia > 5)
            {
                MessageBox.Show("Đánh giá phải là số nguyên từ 1 đến 5", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ===== BƯỚC 3: Lookup idThuoc đầu tiên trong hóa đơn để đảm bảo idHD hợp lệ =====
            // string idThuoc = _cthd.GetFirstIdThuocByHD(idHD); // TODO: ĐỔI: gọi DAO/Service thật
            string idThuoc = GetFirstIdThuocByHD(idHD);          // ← placeholder
            if (idThuoc == null)
            {
                MessageBox.Show($"Không tìm thấy thuốc trong hóa đơn {idHD}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ===== BƯỚC 4: Gọi controller để thêm phản hồi guest =====
            // bool ok = _phanHoi.AddPhanHoiGuest(idHD, sdt, noiDung, danhGia); // TODO: ĐỔI: gọi Controller/Service thật (match Java: phanHoiController.addPhanHoiGuest)
            bool ok = AddPhanHoiGuest(idHD, sdt, noiDung, danhGia);            // ← placeholder

            if (ok)
            {
                MessageBox.Show("Gửi phản hồi thành công. Cảm ơn bạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // TODO: ĐỔI: điều hướng về LoginForm thực tế
                // new LoginForm().Show();
                Close();
            }
            else
            {
                MessageBox.Show("Gửi phản hồi thất bại. Vui lòng kiểm tra lại thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== PLACEHOLDER cho DAO/Service =====================
        // Các hàm dưới chỉ để bạn dễ tìm & thay. Hãy search "TODO: ĐỔI" trong file.

        private string GetFirstIdThuocByHD(string idHD)
        {
            // TODO: ĐỔI: return _cthd.GetFirstIdThuocByHD(idHD);
            // Ý tưởng đúng như Java: ChiTietHoaDonDAO#getFirstIdThuocByHD(idHD)
            return "T001"; // giả lập có dữ liệu
        }

        private bool AddPhanHoiGuest(string idHD, string sdt, string noiDung, int danhGia)
        {
            // TODO: ĐỔI: return _phanHoi.AddPhanHoiGuest(idHD, sdt, noiDung, danhGia);
            // Controller của bạn sẽ tự:
            //  - idPH = "PH" + timestamp
            //  - tìm idKH theo sdt (KhachHangDAO#getBySDT)
            //  - tìm idThuoc đầu tiên theo idHD (ChiTietHoaDonDAO)
            //  - set thoiGian = now
            //  - insert PhanHoi
            return true; // giả lập thành công
        }
    }
}
