using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VisPrWindowsDesktopRecorder.Algorithms
{
    public static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        public static Win32Exception GetLastWin32Error(string functionName)
        {
            var code = Marshal.GetLastWin32Error();
            var ex = new Win32Exception(Marshal.GetLastWin32Error());
            return new Win32Exception(Marshal.GetLastWin32Error(), functionName + "::" + ex.Message);
        }
    }
}
