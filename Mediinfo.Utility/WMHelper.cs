using System;
using System.Runtime.InteropServices;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 使用COPYDATASTRUCT来传递字符串
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }

    public class WMHelper
    {
        public static IntPtr ParentHandler { get; set; }
        public static IntPtr Handler { get; set; }
        public const int WM_COPYDATA = 0x004A;

        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr hWnd, int msg, int wParam, ref COPYDATASTRUCT lParam);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref COPYDATASTRUCT lParam);
    }
}
