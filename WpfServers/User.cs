using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WpfServers
{
    public class User
    {
        #region - - - MoveWindow 窗体移动
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int IParam);
        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        public static void MoveWindow(IntPtr handle)//事件调用方法
        {
            ReleaseCapture();
            SendMessage(handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        #endregion

        #region  ---窗口置顶
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
        public static void SetWindowsFirst(IntPtr hWnd)
        {
            SetWindowPos(hWnd, -1, 0, 0, 0, 0, 1 | 2);
        }
        #endregion

        #region
        public static IntPtr GetWindowHandle(Window window)
        {
            return new WindowInteropHelper(window).Handle;
        }
        #endregion
    }
}
