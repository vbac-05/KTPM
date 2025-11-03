// File: controller/ChiTietPhieuNhapController.cs
// Controller xử lý logic nghiệp vụ cho ChiTietPhieuNhap

using QLThuocApp.DAO;
using QLThuocApp.Entities;

namespace QLThuocApp.Controllers
{
    public class ChiTietPhieuNhapController
    {
        // ✅ Chức năng đúng, nhưng khi namespace DAO đổi, class dưới cũng phải theo
        // ❌ TODO: Sau khi tạo DAO thật, đảm bảo ChiTietPhieuNhapDAO nằm trong QLThuocWin.DAO
        private ChiTietPhieuNhapDAO chiTietPhieuNhapDAO = new ChiTietPhieuNhapDAO();

        // ✅ Giữ nguyên logic, chỉ đổi namespace theo dự án
        public bool AddChiTietPhieuNhap(ChiTietPhieuNhap ct)
        {
            // ❌ TODO: Khi có DAO thật, có thể thêm try-catch hoặc validate tại đây
            return chiTietPhieuNhapDAO.Insert(ct);
        }

        // ✅ Đúng flow, không cần đổi trừ khi DAO method đổi tên
        public void DeleteByPhieuNhapAndThuoc(string idPN, string idThuoc)
        {
            // ❌ TODO: Khi có DAO thật, có thể thêm kiểm tra tồn tại trước khi xóa
            chiTietPhieuNhapDAO.DeleteByPhieuNhapAndThuoc(idPN, idThuoc);
        }

        // ✅ Có thể thêm các phương thức khác nếu GUI yêu cầu
    }
}

