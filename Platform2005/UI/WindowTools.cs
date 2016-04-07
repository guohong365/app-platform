namespace Platform.UI
{
    using Platform.Win32API;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public sealed class WindowTools
    {
        public static Control ControlFromPoint(Point pt)
        {
            IntPtr handle = HwndFromPoint(pt);
            if (handle == IntPtr.Zero)
            {
                return null;
            }
            return Control.FromHandle(handle);
        }

        public static IntPtr HwndFromPoint(Point pt)
        {
            IntPtr hWnd = User32.WindowFromPoint(pt);
            while (hWnd != IntPtr.Zero)
            {
                Point point = pt;
                User32.ScreenToClient(hWnd, ref point);
                IntPtr ptr2 = User32.ChildWindowFromPointEx(hWnd, point, 5);
                if (ptr2 == hWnd)
                {
                    return hWnd;
                }
                hWnd = ptr2;
            }
            return hWnd;
        }
    }
}
