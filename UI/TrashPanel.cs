using System;
using System.Windows.Forms;
// TODO: thêm using tới namespace chứa các Trash*Panel của bạn (nếu khác)
using QLThuocWin.UI; // giữ nguyên nếu các lớp con cùng namespace

namespace QLThuocWin.UI
{
    public class TrashPanel : UserControl
    {
        private readonly TabControl _trashTabs;

        // Giữ reference để gọi LoadData sau khi addTab (giống Java)
        private readonly TrashPhanHoiPanel _phanHoiPanel;
        private readonly TrashHopDongPanel _hopDongPanel;
        private readonly TrashHoaDonPanel _hoaDonPanel;
        private readonly TrashNhanVienPanel _nhanVienPanel;
        private readonly TrashThuocPanel _thuocPanel;
        private readonly TrashNhaCungCapPanel _nccPanel;
        private readonly TrashPhieuNhapPanel _phieuNhapPanel;

        public TrashPanel()
        {
            Dock = DockStyle.Fill;
            _trashTabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(_trashTabs);

            // ===== Tạo panel con & add tab giống Java =====
            // Lưu ý: truyền (TabControl, tabIndex) để panel con có thể cập nhật tiêu đề tab "Tên (N)"
            // Nếu constructor thực tế khác, xem các dòng // TODO bên dưới.

            // Tab 0: Phản hồi
            // CHANGED: truyền TabControl + index
            // TODO: nếu constructor khác, đổi lại cho khớp (ví dụ: new TrashPhanHoiPanel())
            _phanHoiPanel = new TrashPhanHoiPanel(_trashTabs, 0);
            _trashTabs.TabPages.Add(new TabPage("Phản hồi") { Controls = { _phanHoiPanel } });

            // Tab 1: Hợp đồng
            _hopDongPanel = new TrashHopDongPanel(_trashTabs, 1);
            _trashTabs.TabPages.Add(new TabPage("Hợp đồng") { Controls = { _hopDongPanel } });

            // Tab 2: Hóa đơn
            _hoaDonPanel = new TrashHoaDonPanel(_trashTabs, 2);
            _trashTabs.TabPages.Add(new TabPage("Hóa đơn") { Controls = { _hoaDonPanel } });

            // Tab 3: Nhân viên
            _nhanVienPanel = new TrashNhanVienPanel(_trashTabs, 3);
            _trashTabs.TabPages.Add(new TabPage("Nhân viên") { Controls = { _nhanVienPanel } });

            // Tab 4: Thuốc
            _thuocPanel = new TrashThuocPanel(_trashTabs, 4);
            _trashTabs.TabPages.Add(new TabPage("Thuốc") { Controls = { _thuocPanel } });

            // Tab 5: Nhà cung cấp
            _nccPanel = new TrashNhaCungCapPanel(_trashTabs, 5);
            _trashTabs.TabPages.Add(new TabPage("Nhà cung cấp") { Controls = { _nccPanel } });

            // Tab 6: Phiếu nhập
            _phieuNhapPanel = new TrashPhieuNhapPanel(_trashTabs, 6);
            _trashTabs.TabPages.Add(new TabPage("Phiếu nhập") { Controls = { _phieuNhapPanel } });

            // ===== Gọi LoadData sau khi đã addTab (match Java) =====
            // CHANGED: gọi ngay để panel con đếm số record & tự cập nhật tiêu đề tab "Tên (N)"
            // TODO: nếu tên hàm khác (vd RefreshData/Reload), đổi cho đúng.
            SafeLoadData(_phanHoiPanel, nameof(TrashPhanHoiPanel));
            SafeLoadData(_hopDongPanel, nameof(TrashHopDongPanel));
            SafeLoadData(_hoaDonPanel, nameof(TrashHoaDonPanel));
            SafeLoadData(_nhanVienPanel, nameof(TrashNhanVienPanel));
            SafeLoadData(_thuocPanel, nameof(TrashThuocPanel));
            SafeLoadData(_nccPanel, nameof(TrashNhaCungCapPanel));
            SafeLoadData(_phieuNhapPanel, nameof(TrashPhieuNhapPanel));
        }

        // Helper để gọi LoadData() an toàn (nếu panel con có hàm public LoadData)
        private static void SafeLoadData(object panel, string panelName)
        {
            if (panel == null) return;
            var mi = panel.GetType().GetMethod("LoadData"); // CHANGED: tên hàm khớp Java: loadData()
            if (mi == null)
            {
                // TODO: nếu panel không có LoadData(), đổi đúng tên ở đây hoặc thêm hàm public LoadData() trong panel con
                // Ví dụ: var mi2 = panel.GetType().GetMethod("Reload");
                return;
            }
            mi.Invoke(panel, null);
        }
    }
}
