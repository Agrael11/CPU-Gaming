using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CPU_Gaming
{
    internal static class ProcessExtender
    {
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow(); // Gets the active (focused) window

        public static bool TryGetProcessWindow(this Process process, out string title)
        {
            title = "";
            if (process.MainWindowHandle == IntPtr.Zero || !IsWindowVisible(process.MainWindowHandle))
            {
                return false;
            }

            var titleBuilder = new StringBuilder(256);
            _ = GetWindowText(process.MainWindowHandle, titleBuilder, titleBuilder.Capacity);
            title = titleBuilder.ToString();
            if (title.Length == 0)
            {
                IntPtr foregroundWindow = GetForegroundWindow();
                _= GetWindowText(foregroundWindow, titleBuilder, titleBuilder.Capacity);
                title = titleBuilder.ToString();
                if (title.Length == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
