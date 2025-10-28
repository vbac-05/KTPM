
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QLThuocWin.Utils
{
    public static class MessageDialog
    {
        public static void Info(string msg, string title="Thông báo") => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        public static void Error(string msg, string title="Lỗi") => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        public static bool Confirm(string msg, string title="Xác nhận") => MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    public static class Validator
    {
        public static bool NotEmpty(Control c, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(c.Text))
            {
                MessageDialog.Error($"{fieldName} không được để trống!");
                c.Focus();
                return false;
            }
            return true;
        }
        public static bool IsPhone(string s) => Regex.IsMatch(s ?? "", @"^0\d{9,10}$");
        public static bool IsEmail(string s) => Regex.IsMatch(s ?? "", @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
    }
}
