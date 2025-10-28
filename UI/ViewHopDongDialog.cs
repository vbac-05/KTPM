using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace QLThuocWin.UI
{

    public class ViewHopDongDialog : Form
    {
        TextBox txtMa = new TextBox{ ReadOnly = true };
        TextBox txtNCC = new TextBox{ ReadOnly = true };
        TextBox txtNgayKy = new TextBox{ ReadOnly = true };
        TextBox txtHieuLuc = new TextBox{ ReadOnly = true };
        TextBox txtGhiChu = new TextBox{ ReadOnly = true };

        public ViewHopDongDialog()
        {
            Text = "Xem chi tiết Hợp đồng";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(720, 340);

            var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6, Padding = new Padding(12) };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            Controls.Add(header);

            header.Controls.Add(new Label{Text="Mã HĐG:", AutoSize=true}, 0, 0); header.Controls.Add(txtMa, 1, 0);
            header.Controls.Add(new Label{Text="Nhà cung cấp:", AutoSize=true}, 0, 1); header.Controls.Add(txtNCC, 1, 1);
            header.Controls.Add(new Label{Text="Ngày ký:", AutoSize=true}, 0, 2); header.Controls.Add(txtNgayKy, 1, 2);
            header.Controls.Add(new Label{Text="Hiệu lực:", AutoSize=true}, 0, 3); header.Controls.Add(txtHieuLuc, 1, 3);
            header.Controls.Add(new Label{Text="Ghi chú:", AutoSize=true}, 0, 4); header.Controls.Add(txtGhiChu, 1, 4);

            var panelBtn = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Padding = new Padding(12) };
            var btnClose = new Button { Text = "Đóng" };
            btnClose.Click += (s,e) => Close();
            panelBtn.Controls.Add(btnClose);
            Controls.Add(panelBtn);

            LoadDemo();
        }

        private void LoadDemo()
        {
            txtMa.Text = "HĐG-2025-01";
            txtNCC.Text = "Dược phẩm XYZ";
            txtNgayKy.Text = DateTime.Today.ToString("dd/MM/yyyy");
            txtHieuLuc.Text = "12 tháng";
            txtGhiChu.Text = "Điều khoản giao hàng theo quý.";
        }
    }

}

