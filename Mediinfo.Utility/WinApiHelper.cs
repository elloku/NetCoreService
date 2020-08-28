using System;
using System.Runtime.InteropServices;

namespace Mediinfo.Utility
{
    /// <summary>
    /// Windows API帮助类
    /// </summary>
    public class WinApiHelper
    {
        #region const

        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int HTCAPTION = 2;
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        #endregion

        #region extern

        /// <summary>
        /// 获得当前活动窗体句柄
        /// </summary>
        /// <returns>返回值是一个前台窗口的句柄</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 获得本窗体的句柄
        /// </summary>
        /// <returns>返回值是一个前台窗口的句柄</returns>
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetF();

        /// <summary>
        /// 设置此窗体为活动窗体
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetF(IntPtr hWnd);

        /// <summary>
        /// 根据句柄获取进程PID
        /// </summary>
        /// <param name="hwnd">句饼</param>
        /// <param name="lpdwProcessId">引用 返回进程PID</param>
        /// <returns>未知</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd, ref int lpdwProcessId);

        /// <summary>
        /// 运行一个外部程序
        /// </summary>
        /// <param name="hwnd">用于指定父窗口句柄</param>
        /// <param name="lpOperation">用于指定要进行的操作</param>
        /// <param name="lpFile">用于指定要打开的文件名、要执行的程序文件名或要浏览的文件夹名</param>
        /// <param name="lpParameters">若FileName参数是一个可执行程序，则此参数指定命令行参数，否则此参数应为nil或PChar(0)</param>
        /// <param name="lpDirectory">用于指定默认目录</param>
        /// <param name="nShowCmd">若FileName参数是一个可执行程序，则此参数指定程序窗口的初始显示方式，否则此参数应设置为0</param>
        /// <returns>执行成功会返回应用程序句柄</returns>
        [DllImport("SHELL32.dll")]
        public static extern int ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        /// <summary>
        /// 将指定的消息发送到一个或多个窗口
        /// </summary>
        /// <param name="hWnd">指定要接收消息的窗口的句柄</param>
        /// <param name="Msg">指定被发送的消息</param>
        /// <param name="wParam">指定附加的消息特定信息</param>
        /// <param name="lParam">指定附加的消息特定信息</param>
        /// <returns>返回值：返回值指定消息处理的结果，依赖于所发送的消息</returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// 该函数把光标移到屏幕的指定位置
        /// </summary>
        /// <param name="x">指定光标的新的X坐标，以屏幕坐标表示</param>
        /// <param name="y">指定光标的新的Y坐标，以屏幕坐标表示</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// 综合鼠标移动和按钮点击
        /// </summary>
        /// <param name="dwFlags">标志位集，指定点击按钮和鼠标动作的多种情况</param>
        /// <param name="dx">指定鼠标沿x轴的绝对位置或者从上次鼠标事件产生以来移动的数量，依赖于MOUSEEVENTF_ABSOLUTE的设置</param>
        /// <param name="dy">指定鼠标沿y轴的绝对位置或者从上次鼠标事件产生以来移动的数量，依赖于MOUSEEVENTF_ABSOLUTE的设置</param>
        /// <param name="cButtons">如果dwFlags为MOUSEEVENTF_WHEEL，则dwData指定鼠标轮移动的数量</param>
        /// <param name="dwExtraInfo">指定与鼠标事件相关的附加32位值</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        #endregion
    }
}
