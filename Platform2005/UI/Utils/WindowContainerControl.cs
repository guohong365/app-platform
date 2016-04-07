namespace Platform.UI.Utils
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class WindowContainerControl : UserControl
    {
        private Container components;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_ID = -12;
        private const int GWL_STYLE = -16;
        private const int GWL_USERDATA = -21;
        private int m_ButtomOffset;
        private IntPtr m_ChildHwnd = IntPtr.Zero;
        private int m_LeftOffset;
        private int m_RightOffset;
        private int m_TopOffset;
        private const int MF_BYCOMMAND = 0;
        private const int MF_BYPOSITION = 0x400;
        private Panel panel_Container;
        private const int SC_CLOSE = 0xf060;
        private const int SC_MINIMIZE = 0xf020;
        private const int SW_FORCEMINIMIZE = 11;
        private const int SW_HIDE = 0;
        private const int SW_MAX = 11;
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        private const int SW_NORMAL = 1;
        private const int SW_RESTORE = 9;
        private const int SW_SHOW = 5;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOWNORMAL = 1;
        private const int WM_INITMENU = 0x116;
        private const int WM_INITMENUPOPUP = 0x117;
        private const int WM_SYSCOMMAND = 0x112;
        private const uint WS_BORDER = 0x800000;
        private const uint WS_CAPTION = 0xc00000;
        private const uint WS_CHILD = 0x40000000;
        private const uint WS_CHILDWINDOW = 0x40000000;
        private const uint WS_CLIPCHILDREN = 0x2000000;
        private const uint WS_CLIPSIBLINGS = 0x4000000;
        private const uint WS_DISABLED = 0x8000000;
        private const uint WS_DLGFRAME = 0x400000;
        private const uint WS_EX_ACCEPTFILES = 0x10;
        private const uint WS_EX_APPWINDOW = 0x40000;
        private const uint WS_EX_CLIENTEDGE = 0x200;
        private const uint WS_EX_CONTEXTHELP = 0x400;
        private const uint WS_EX_CONTROLPARENT = 0x10000;
        private const uint WS_EX_DLGMODALFRAME = 1;
        private const uint WS_EX_LEFT = 0;
        private const uint WS_EX_LEFTSCROLLBAR = 0x4000;
        private const uint WS_EX_LTRREADING = 0;
        private const uint WS_EX_MDICHILD = 0x40;
        private const uint WS_EX_NOPARENTNOTIFY = 4;
        private const uint WS_EX_OVERLAPPEDWINDOW = 0x300;
        private const uint WS_EX_PALETTEWINDOW = 0x188;
        private const uint WS_EX_RIGHT = 0x1000;
        private const uint WS_EX_RIGHTSCROLLBAR = 0;
        private const uint WS_EX_RTLREADING = 0x2000;
        private const uint WS_EX_STATICEDGE = 0x20000;
        private const uint WS_EX_TOOLWINDOW = 0x80;
        private const uint WS_EX_TOPMOST = 8;
        private const uint WS_EX_TRANSPARENT = 0x20;
        private const uint WS_EX_WINDOWEDGE = 0x100;
        private const uint WS_GROUP = 0x20000;
        private const uint WS_HSCROLL = 0x100000;
        private const uint WS_ICONIC = 0x20000000;
        private const uint WS_MAXIMIZE = 0x1000000;
        private const uint WS_MAXIMIZEBOX = 0x10000;
        private const uint WS_MINIMIZE = 0x20000000;
        private const uint WS_MINIMIZEBOX = 0x20000;
        private const uint WS_OVERLAPPED = 0;
        private const uint WS_OVERLAPPEDWINDOW = 0xcf0000;
        private const uint WS_POPUP = 0x80000000;
        private const uint WS_POPUPWINDOW = 0x80880000;
        private const uint WS_SIZEBOX = 0x40000;
        private const uint WS_SYSMENU = 0x80000;
        private const uint WS_TABSTOP = 0x10000;
        private const uint WS_THICKFRAME = 0x40000;
        private const uint WS_TILED = 0;
        private const uint WS_TILEDWINDOW = 0xcf0000;
        private const uint WS_VISIBLE = 0x10000000;
        private const uint WS_VSCROLL = 0x200000;

        public WindowContainerControl()
        {
            this.InitializeComponent();
        }

        [DllImport("User32.dll")]
        public static extern bool DeleteMenu(IntPtr hMenu, uint nPos, uint nCmd);
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FillWindow()
        {
            if (this.m_ChildHwnd != IntPtr.Zero)
            {
                MoveWindow(this.m_ChildHwnd, -this.m_LeftOffset, -this.m_TopOffset, (base.Width + this.m_LeftOffset) + this.m_RightOffset, (base.Height + this.m_TopOffset) + this.m_ButtomOffset, true);
                RECT lpRect = new RECT();
                GetWindowRect(this.m_ChildHwnd, ref lpRect);
                this.panel_Container.Width = ((lpRect.right - lpRect.left) - this.m_LeftOffset) - this.m_RightOffset;
                this.panel_Container.Left = (base.Width - this.panel_Container.Width) / 2;
                this.panel_Container.Height = ((lpRect.bottom - lpRect.top) - this.m_TopOffset) - this.m_ButtomOffset;
                this.panel_Container.Top = (base.Height - this.panel_Container.Height) / 2;
            }
        }

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        [DllImport("User32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);
        [DllImport("User32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("User32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        private void InitializeComponent()
        {
            components = new Container();
            this.panel_Container = new Panel();
            base.SuspendLayout();
            this.panel_Container.Location = new Point(0x20, 0x18);
            this.panel_Container.Name = "panel_Container";
            this.panel_Container.Size = new Size(0x68, 100);
            this.panel_Container.TabIndex = 0;
            base.Controls.Add(this.panel_Container);
            base.Name = "WindowContainerControl";
            base.Size = new Size(160, 150);
            base.VisibleChanged += new EventHandler(this.WindowContainerControl_VisibleChanged);
            base.SizeChanged += new EventHandler(this.WindowContainerControl_SizeChanged);
            base.ResumeLayout(false);
        }

        [DllImport("User32.dll")]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool repaint);
        [DllImport("User32.dll")]
        public static extern bool RemoveMenu(IntPtr hMenu, uint nPos, uint nCmd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hChildWnd, IntPtr hParentWnd);
        public void SetWindowHandle(IntPtr childHwnd, int topOffset, int buttomOffset, int leftOffset, int rightOffset)
        {
            if (this.m_ChildHwnd != IntPtr.Zero)
            {
                SetParent(this.m_ChildHwnd, IntPtr.Zero);
            }
            this.m_ChildHwnd = childHwnd;
            this.m_TopOffset = topOffset;
            this.m_ButtomOffset = buttomOffset;
            this.m_LeftOffset = leftOffset;
            this.m_RightOffset = rightOffset;
            SetWindowLong(this.m_ChildHwnd, -16, 0x40000000);
            IntPtr systemMenu = GetSystemMenu(this.m_ChildHwnd, false);
            for (int i = GetMenuItemCount(systemMenu); i >= 0; i--)
            {
                DeleteMenu(systemMenu, (uint) i, 0x400);
            }
            SetParent(this.m_ChildHwnd, this.panel_Container.Handle);
            ShowWindow(this.m_ChildHwnd, 1);
            this.FillWindow();
        }

        [DllImport("user32.dll")]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint newLong);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowText(IntPtr hwnd, string title);
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("User32.dll")]
        public static extern void UpdateWindow(IntPtr hWnd);
        private void WindowContainerControl_SizeChanged(object sender, EventArgs e)
        {
            this.FillWindow();
        }

        private void WindowContainerControl_VisibleChanged(object sender, EventArgs e)
        {
            this.FillWindow();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static WindowContainerControl.RECT FromXYWH(int x, int y, int width, int height)
            {
                return new WindowContainerControl.RECT(x, y, x + width, y + height);
            }
        }
    }
}
