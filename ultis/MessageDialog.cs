using System.Windows.Forms;

namespace QLThuocApp.Utils
{
    /// <summary>
    /// Các phương thức tiện ích hiển thị thông báo bằng MessageBox.
    /// (Tương đương C# của JOptionPane trong Java Swing)
    /// </summary>
    public static class MessageDialog
    {
        /// <summary>
        /// Hiển thị thông báo thông tin (Information).
        /// </summary>
        public static void ShowInfo(IWin32Window parent, string message, string title)
        {
            MessageBox.Show(parent, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị cảnh báo (Warning).
        /// </summary>
        public static void ShowWarning(IWin32Window parent, string message, string title)
        {
            MessageBox.Show(parent, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Hiển thị lỗi (Error).
        /// </summary>
        public static void ShowError(IWin32Window parent, string message, string title)
        {
            MessageBox.Show(parent, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hiển thị hộp thoại xác nhận (Yes/No).
        /// </summary>
        /// <returns>true nếu người dùng chọn Yes</returns>
        public static bool ShowConfirm(IWin32Window parent, string message, string title)
        {
            DialogResult result = MessageBox.Show(parent, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }
    }
}