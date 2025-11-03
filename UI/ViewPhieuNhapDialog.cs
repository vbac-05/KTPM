using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace QLThuocApp.UI
{

    public class ViewPhieuNhapDialog : Form
    {
        TextBox txtMa = new TextBox{ ReadOnly = true };
        TextBox txtNCC = new TextBox{ ReadOnly = true };
        TextBox txtNgay = new TextBox{ ReadOnly = true };
        TextBox txtGhiChu = new TextBox{ ReadOnly = true };
        TextBox txtTongTien = new TextBox{ ReadOnly = true };
        DataGridView gridCT = new DataGridView();

        public ViewPhieuNhapDialog()
        {
            Text = "Xem chi tiết Phiếu nhập";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(880, 560);

            var header = BuildHeaderTable();
            Controls.Add(header);

            header.Controls.Add(new Label{Text="Mã PN:", AutoSize=true}, 0, 0);
            header.Controls.Add(txtMa, 1, 0);
            header.Controls.Add(new Label{Text="Nhà cung cấp:", AutoSize=true}, 2, 0);
            header.Controls.Add(txtNCC, 3, 0);

            header.Controls.Add(new Label{Text="Ngày nhập:", AutoSize=true}, 0, 1);
            header.Controls.Add(txtNgay, 1, 1);
            header.Controls.Add(new Label{Text="Ghi chú:", AutoSize=true}, 2, 1);
            header.Controls.Add(txtGhiChu, 3, 1);

            header.Controls.Add(new Label{Text="Tổng tiền:", AutoSize=true}, 0, 2);
            header.Controls.Add(txtTongTien, 1, 2);

            gridCT.Dock = DockStyle.Fill;
            gridCT.AutoGenerateColumns = false;
            gridCT.ReadOnly = true;
            gridCT.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCT.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Mã thuốc", DataPropertyName="MaThuoc"});
            gridCT.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Tên thuốc", DataPropertyName="TenThuoc"});
            gridCT.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Số lượng", DataPropertyName="SoLuong"});
            gridCT.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Đơn giá", DataPropertyName="DonGia"});
            gridCT.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Thành tiền", DataPropertyName="ThanhTien"});
            Controls.Add(gridCT);

            var panelBtn = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Padding = new Padding(12) };
            var btnClose = new Button { Text = "Đóng" };
            btnClose.Click += (s,e) => Close();
            panelBtn.Controls.Add(btnClose);
            Controls.Add(panelBtn);

            LoadDemo();
        }


        private TableLayoutPanel BuildHeaderTable()
        {
            var t = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 4, AutoSize = true, Padding = new Padding(12) };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            return t;
        }


        private void LoadDemo()
        {
            txtMa.Text = "PN0005";
            txtNCC.Text = "Công ty Dược ABC";
            txtNgay.Text = DateTime.Today.ToString("dd/MM/yyyy");
            txtGhiChu.Text = "Nhập lô mới";
            txtTongTien.Text = "1,250,000";

            var t = new DataTable();
            t.Columns.Add("MaThuoc");
            t.Columns.Add("TenThuoc");
            t.Columns.Add("SoLuong", typeof(int));
            t.Columns.Add("DonGia", typeof(decimal));
            t.Columns.Add("ThanhTien", typeof(decimal));
            t.Rows.Add("T050", "Kháng sinh X", 20, 50000, 1000000);
            t.Rows.Add("T099", "Bông gạc", 50, 5000, 250000);
            gridCT.DataSource = t;
        }
    }

}

