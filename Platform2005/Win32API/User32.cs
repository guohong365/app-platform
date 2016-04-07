namespace Platform.Win32API
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public sealed class User32
    {
        public const uint CWP_ALL = 0;
        public const uint CWP_SKIPDISABLED = 2;
        public const uint CWP_SKIPINVISIBLE = 1;
        public const uint CWP_SKIPTRANSPARENT = 4;
        public const int GWL_EXSTYLE = -20;
        public const int GWL_ID = -12;
        public const int GWL_STYLE = -16;
        public const int GWL_USERDATA = -21;
        public const uint MB_ICONASTERISK = 0x40;
        public const uint MB_ICONEXCLAMATION = 0x30;
        public const uint MB_ICONHAND = 0x10;
        public const uint MB_ICONQUESTION = 0x20;
        public const uint MB_OK = 0;
        public const int MF_BYCOMMAND = 0;
        public const int MF_BYPOSITION = 0x400;
        public const int SC_CLOSE = 0xf060;
        public const int SC_MINIMIZE = 0xf020;
        public const uint SIMPLEBEEP = uint.MaxValue;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_HIDE = 0;
        public const int SW_MAX = 11;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_NORMAL = 1;
        public const int SW_RESTORE = 9;
        public const int SW_SHOW = 5;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOWNORMAL = 1;
        public const int WM_INITMENU = 0x116;
        public const int WM_INITMENUPOPUP = 0x117;
        public const int WM_SYSCOMMAND = 0x112;
        public const uint WS_BORDER = 0x800000;
        public const uint WS_CAPTION = 0xc00000;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_CHILDWINDOW = 0x40000000;
        public const uint WS_CLIPCHILDREN = 0x2000000;
        public const uint WS_CLIPSIBLINGS = 0x4000000;
        public const uint WS_DISABLED = 0x8000000;
        public const uint WS_DLGFRAME = 0x400000;
        public const uint WS_EX_ACCEPTFILES = 0x10;
        public const uint WS_EX_APPWINDOW = 0x40000;
        public const uint WS_EX_CLIENTEDGE = 0x200;
        public const uint WS_EX_CONTEXTHELP = 0x400;
        public const uint WS_EX_CONTROLPARENT = 0x10000;
        public const uint WS_EX_DLGMODALFRAME = 1;
        public const uint WS_EX_LEFT = 0;
        public const uint WS_EX_LEFTSCROLLBAR = 0x4000;
        public const uint WS_EX_LTRREADING = 0;
        public const uint WS_EX_MDICHILD = 0x40;
        public const uint WS_EX_NOPARENTNOTIFY = 4;
        public const uint WS_EX_OVERLAPPEDWINDOW = 0x300;
        public const uint WS_EX_PALETTEWINDOW = 0x188;
        public const uint WS_EX_RIGHT = 0x1000;
        public const uint WS_EX_RIGHTSCROLLBAR = 0;
        public const uint WS_EX_RTLREADING = 0x2000;
        public const uint WS_EX_STATICEDGE = 0x20000;
        public const uint WS_EX_TOOLWINDOW = 0x80;
        public const uint WS_EX_TOPMOST = 8;
        public const uint WS_EX_TRANSPARENT = 0x20;
        public const uint WS_EX_WINDOWEDGE = 0x100;
        public const uint WS_GROUP = 0x20000;
        public const uint WS_HSCROLL = 0x100000;
        public const uint WS_ICONIC = 0x20000000;
        public const uint WS_MAXIMIZE = 0x1000000;
        public const uint WS_MAXIMIZEBOX = 0x10000;
        public const uint WS_MINIMIZE = 0x20000000;
        public const uint WS_MINIMIZEBOX = 0x20000;
        public const uint WS_OVERLAPPED = 0;
        public const uint WS_OVERLAPPEDWINDOW = 0xcf0000;
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_POPUPWINDOW = 0x80880000;
        public const uint WS_SIZEBOX = 0x40000;
        public const uint WS_SYSMENU = 0x80000;
        public const uint WS_TABSTOP = 0x10000;
        public const uint WS_THICKFRAME = 0x40000;
        public const uint WS_TILED = 0;
        public const uint WS_TILEDWINDOW = 0xcf0000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_VSCROLL = 0x200000;

        [DllImport("User32.dll")]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr hParentWnd, Point pt, uint flag);
        [DllImport("User32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point pt);
        [DllImport("User32.dll")]
        public static extern bool DeleteMenu(IntPtr hMenu, uint nPos, uint nCmd);
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        [DllImport("User32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);
        [DllImport("User32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32")]
        public static extern bool MessageBeep(uint uType);
        [DllImport("User32.dll")]
        private static extern void MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight);
        [DllImport("User32.dll")]
        public static extern bool RemoveMenu(IntPtr hMenu, uint nPos, uint nCmd);
        [DllImport("User32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point pt);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hChildWnd, IntPtr hParentWnd);
        [DllImport("user32.dll")]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint newLong);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowText(IntPtr hwnd, string title);
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("User32.dll")]
        public static extern void UpdateWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);
    }
}
