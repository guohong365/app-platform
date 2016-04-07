namespace Platform.MultiMedia
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class Capture
    {
        private CAPDRIVERCAPS caps = new CAPDRIVERCAPS();
        private IntPtr hWndC = IntPtr.Zero;
        private BITMAPINFO info = new BITMAPINFO();
        private int m_DeviceIndex;
        private int m_Height;
        private int m_Width;
        private IntPtr mControlPtr;
        private int mHeight;
        private int mLeft;
        private int mTop;
        private int mWidth;
        private const int WM_CAP_ABORT = 0x445;
        private const int WM_CAP_DLG_VIDEOCOMPRESSION = 0x42e;
        private const int WM_CAP_DLG_VIDEODISPLAY = 0x42b;
        private const int WM_CAP_DLG_VIDEOFORMAT = 0x429;
        private const int WM_CAP_DLG_VIDEOSOURCE = 0x42a;
        private const int WM_CAP_DRIVER_CONNECT = 0x40a;
        private const int WM_CAP_DRIVER_DISCONNECT = 0x40b;
        private const int WM_CAP_DRIVER_GET_CAPS = 0x40e;
        private const int WM_CAP_DRIVER_GET_NAME = 0x470;
        private const int WM_CAP_DRIVER_GET_NAMEA = 0x40c;
        private const int WM_CAP_DRIVER_GET_NAMEW = 0x470;
        private const int WM_CAP_DRIVER_GET_VERSION = 0x471;
        private const int WM_CAP_DRIVER_GET_VERSIONA = 0x40d;
        private const int WM_CAP_DRIVER_GET_VERSIONW = 0x471;
        private const int WM_CAP_EDIT_COPY = 0x41e;
        private const int WM_CAP_END = 0x4b5;
        private const int WM_CAP_FILE_ALLOCATE = 0x416;
        private const int WM_CAP_FILE_GET_CAPTURE_FILE = 0x479;
        private const int WM_CAP_FILE_GET_CAPTURE_FILEA = 0x415;
        private const int WM_CAP_FILE_GET_CAPTURE_FILEW = 0x479;
        private const int WM_CAP_FILE_SAVEAS = 0x47b;
        private const int WM_CAP_FILE_SAVEASA = 0x417;
        private const int WM_CAP_FILE_SAVEASW = 0x47b;
        private const int WM_CAP_FILE_SAVEDIB = 0x47d;
        private const int WM_CAP_FILE_SAVEDIBA = 0x419;
        private const int WM_CAP_FILE_SAVEDIBW = 0x47d;
        private const int WM_CAP_FILE_SET_CAPTURE_FILE = 0x478;
        private const int WM_CAP_FILE_SET_CAPTURE_FILEA = 0x414;
        private const int WM_CAP_FILE_SET_CAPTURE_FILEW = 0x478;
        private const int WM_CAP_FILE_SET_INFOCHUNK = 0x418;
        private const int WM_CAP_GET_AUDIOFORMAT = 0x424;
        private const int WM_CAP_GET_CAPSTREAMPTR = 0x401;
        private const int WM_CAP_GET_MCI_DEVICE = 0x4a7;
        private const int WM_CAP_GET_MCI_DEVICEA = 0x443;
        private const int WM_CAP_GET_MCI_DEVICEW = 0x4a7;
        private const int WM_CAP_GET_SEQUENCE_SETUP = 0x441;
        private const int WM_CAP_GET_STATUS = 0x436;
        private const int WM_CAP_GET_USER_DATA = 0x408;
        private const int WM_CAP_GET_VIDEOFORMAT = 0x42c;
        private const int WM_CAP_GRAB_FRAME = 0x43c;
        private const int WM_CAP_GRAB_FRAME_NOSTOP = 0x43d;
        private const int WM_CAP_PAL_AUTOCREATE = 0x453;
        private const int WM_CAP_PAL_MANUALCREATE = 0x454;
        private const int WM_CAP_PAL_OPEN = 0x4b4;
        private const int WM_CAP_PAL_OPENA = 0x450;
        private const int WM_CAP_PAL_OPENW = 0x4b4;
        private const int WM_CAP_PAL_PASTE = 0x452;
        private const int WM_CAP_PAL_SAVE = 0x4b5;
        private const int WM_CAP_PAL_SAVEA = 0x451;
        private const int WM_CAP_PAL_SAVEW = 0x4b5;
        private const int WM_CAP_SEQUENCE = 0x43e;
        private const int WM_CAP_SEQUENCE_NOFILE = 0x43f;
        private const int WM_CAP_SET_AUDIOFORMAT = 0x423;
        private const int WM_CAP_SET_CALLBACK_CAPCONTROL = 0x455;
        private const int WM_CAP_SET_CALLBACK_ERROR = 0x466;
        private const int WM_CAP_SET_CALLBACK_ERRORA = 0x402;
        private const int WM_CAP_SET_CALLBACK_ERRORW = 0x466;
        private const int WM_CAP_SET_CALLBACK_FRAME = 0x405;
        private const int WM_CAP_SET_CALLBACK_STATUS = 0x467;
        private const int WM_CAP_SET_CALLBACK_STATUSA = 0x403;
        private const int WM_CAP_SET_CALLBACK_STATUSW = 0x467;
        private const int WM_CAP_SET_CALLBACK_VIDEOSTREAM = 0x406;
        private const int WM_CAP_SET_CALLBACK_WAVESTREAM = 0x407;
        private const int WM_CAP_SET_CALLBACK_YIELD = 0x404;
        private const int WM_CAP_SET_MCI_DEVICE = 0x4a6;
        private const int WM_CAP_SET_MCI_DEVICEA = 0x442;
        private const int WM_CAP_SET_MCI_DEVICEW = 0x4a6;
        private const int WM_CAP_SET_OVERLAY = 0x433;
        private const int WM_CAP_SET_PREVIEW = 0x432;
        private const int WM_CAP_SET_PREVIEWRATE = 0x434;
        private const int WM_CAP_SET_SCALE = 0x435;
        private const int WM_CAP_SET_SCROLL = 0x437;
        private const int WM_CAP_SET_SEQUENCE_SETUP = 0x440;
        private const int WM_CAP_SET_USER_DATA = 0x409;
        private const int WM_CAP_SET_VIDEOFORMAT = 0x42d;
        private const int WM_CAP_SINGLE_FRAME = 0x448;
        private const int WM_CAP_SINGLE_FRAME_CLOSE = 0x447;
        private const int WM_CAP_SINGLE_FRAME_OPEN = 0x446;
        private const int WM_CAP_START = 0x400;
        private const int WM_CAP_STOP = 0x444;
        private const int WM_CAP_UNICODE_END = 0x4b5;
        private const int WM_CAP_UNICODE_START = 0x464;
        private const int WM_USER = 0x400;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;

        public Capture()
        {
            //Çë¿¼ÂÇÊ¹ÓÃ System.Runtime.InteropServices.Marshal.SizeOf
            System.Runtime.InteropServices.Marshal.SizeOf(new BITMAPINFO());
            //this.info.biSize = sizeof(BITMAPINFO);
        }

        [DllImport("avicap32.dll")]
        private static extern IntPtr capCreateCaptureWindow(byte[] lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nID);
        [DllImport("avicap32.dll")]
        private static extern int capGetVideoFormat(IntPtr hWnd, IntPtr psVideoFormat, int wSize);
        public Bitmap GrabImage()
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return null;
                }
                if (!SendMessage(this.hWndC, 0x41e, 0, (long) 0))
                {
                    return null;
                }
                IDataObject dataObject = Clipboard.GetDataObject();
                if (!dataObject.GetDataPresent(typeof(Bitmap)))
                {
                    return null;
                }
                return (dataObject.GetData(typeof(Bitmap)) as Bitmap);
            }
            catch
            {
                return null;
            }
        }

        public bool GrabImage(string path)
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                IntPtr ptr = Marshal.StringToHGlobalAnsi(path);
                return SendMessage(this.hWndC, 0x47d, 0, ptr.ToInt64());
            }
            catch
            {
                return false;
            }
        }

        public bool Initialize(IntPtr handle, int left, int top, int width, int height)
        {
            return this.Initialize(handle, 0, left, top, width, height);
        }

        public unsafe bool Initialize(IntPtr handle, int deviceIndex, int left, int top, int width, int height)
        {
            try
            {
                if (this.hWndC != IntPtr.Zero)
                {
                    this.Stop();
                }
                this.mControlPtr = handle;
                this.mWidth = width;
                this.mHeight = height;
                this.mLeft = left;
                this.mTop = top;
                this.m_DeviceIndex = deviceIndex;
                byte[] lpszWindowName = new byte[100];
                this.hWndC = capCreateCaptureWindow(lpszWindowName, 0x50000000, this.mLeft, this.mTop, this.mWidth, this.mHeight, this.mControlPtr, 0);
                if (this.hWndC != IntPtr.Zero)
                {
                    bool flag = false;
                    if (!SendMessage(this.hWndC, 0x406, 0, (long) 0))
                    {
                        return false;
                    }
                    if (!SendMessage(this.hWndC, 0x466, 0, (long) 0))
                    {
                        return false;
                    }
                    if (!SendMessage(this.hWndC, 0x467, 0, (long) 0))
                    {
                        return false;
                    }
                    if (!SendMessage(this.hWndC, 0x40a, this.m_DeviceIndex, (long) 0))
                    {
                        return false;
                    }
                    flag = SendMessage(this.hWndC, 0x40e, sizeof(CAPDRIVERCAPS), ref this.caps);
                    if (!flag)
                    {
                        this.Stop();
                        return false;
                    }
                    fixed (BITMAPINFO* bitmapinfoRef = &this.info)
                    {
                        flag = SendMessage(this.hWndC, 0x42c, sizeof(BITMAPINFO), bitmapinfoRef);
                    }
                    if (flag)
                    {
                        this.m_Width = this.info.biWidth;
                        this.m_Height = this.info.biHeight;
                        goto Label_019C;
                    }
                    this.Stop();
                }
                return false;
            Label_019C:
                if (!SendMessage(this.hWndC, 0x435, 1, (long) 0))
                {
                    return false;
                }
                if (!SendMessage(this.hWndC, 0x434, 0x21, (long) 0))
                {
                    return false;
                }
                if (this.caps.fHasOverlay && !SendMessage(this.hWndC, 0x433, true, (long) 0))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Kinescope(string path)
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                IntPtr ptr = Marshal.StringToHGlobalAnsi(path);
                if (!SendMessage(this.hWndC, 0x478, 0, ptr.ToInt64()))
                {
                    return false;
                }
                if (!SendMessage(this.hWndC, 0x43e, 0, (long) 0))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("User32.dll")]
        private static extern void MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight);
        public bool ResizeWindow(int left, int top, int width, int height)
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                this.mWidth = width;
                this.mHeight = height;
                this.mLeft = left;
                this.mTop = top;
                MoveWindow(this.hWndC, this.mLeft, this.mTop, this.mWidth, this.mHeight);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int wMsg, bool wParam, long lParam);
        [DllImport("User32.dll")]
        private static extern unsafe bool SendMessage(IntPtr hWnd, int wMsg, int wSize, BITMAPINFO* info);
        [DllImport("User32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, long lParam);
        [DllImport("User32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int wMsg, int wSize, ref CAPDRIVERCAPS caps);
        [DllImport("User32.dll")]
        private static extern unsafe bool SendMessage(IntPtr hWnd, int wMsg, int wParam, byte* lParam);
        public bool SetOverly(bool overly)
        {
            try
            {
                if (!this.caps.fHasOverlay)
                {
                    return false;
                }
                return SendMessage(this.hWndC, 0x433, overly, (long) 0);
            }
            catch
            {
                return false;
            }
        }

        public bool ShowVideoCompressionDialog()
        {
            try
            {
                return SendMessage(this.hWndC, 0x42e, 0, (long) 0);
            }
            catch
            {
                return false;
            }
        }

        public bool ShowVideoDisplayDialog()
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                if (!this.caps.fHasDlgVideoDisplay)
                {
                    return false;
                }
                return SendMessage(this.hWndC, 0x42b, 0, (long) 0);
            }
            catch
            {
                return false;
            }
        }

        public bool ShowVideoFormatDialog()
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                if (!this.caps.fHasDlgVideoFormat)
                {
                    return false;
                }
                return SendMessage(this.hWndC, 0x429, 0, (long) 0);
            }
            catch
            {
                return false;
            }
        }

        public bool ShowVideoSourceDialog()
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                if (!this.caps.fHasDlgVideoSource)
                {
                    return false;
                }
                return SendMessage(this.hWndC, 0x42a, 0, (long) 0);
            }
            catch
            {
                return false;
            }
        }

        public bool Start()
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                return SendMessage(this.hWndC, 0x432, 1, (long) 0);
            }
            catch
            {
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                bool flag = SendMessage(this.hWndC, 0x40b, 0, (long) 0);
                if (flag)
                {
                    this.hWndC = IntPtr.Zero;
                }
                this.m_Width = 0;
                this.m_Height = 0;
                return flag;
            }
            catch
            {
                return false;
            }
        }

        public bool StopKinescope()
        {
            try
            {
                if (this.hWndC == IntPtr.Zero)
                {
                    return false;
                }
                return SendMessage(this.hWndC, 0x4b5, 0, (long) 0);
            }
            catch
            {
                return false;
            }
        }

        public int VideoHeight
        {
            get
            {
                return this.m_Height;
            }
        }

        public int VideoWidth
        {
            get
            {
                return this.m_Width;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct BITMAPINFO
        {
            [FieldOffset(14)]
            public short biBitCount;
            [FieldOffset(0x24)]
            public int biClrImportant;
            [FieldOffset(0x20)]
            public int biClrUsed;
            [FieldOffset(0x10)]
            public int biCompression;
            [FieldOffset(8)]
            public int biHeight;
            [FieldOffset(12)]
            public short biPlanes;
            [FieldOffset(0)]
            public int biSize;
            [FieldOffset(20)]
            public int biSizeImage;
            [FieldOffset(4)]
            public int biWidth;
            [FieldOffset(0x18)]
            public int biXPelsPerMeter;
            [FieldOffset(0x1c)]
            public int biYPelsPerMeter;
            [FieldOffset(40)]
            public IntPtr bmiColors;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct CAPDRIVERCAPS
        {
            [FieldOffset(20)]
            public bool fCaptureInitialized;
            [FieldOffset(0x18)]
            public bool fDriverSuppliesPalettes;
            [FieldOffset(0x10)]
            public bool fHasDlgVideoDisplay;
            [FieldOffset(12)]
            public bool fHasDlgVideoFormat;
            [FieldOffset(8)]
            public bool fHasDlgVideoSource;
            [FieldOffset(4)]
            public bool fHasOverlay;
            [FieldOffset(0x24)]
            public IntPtr hVideoExtIn;
            [FieldOffset(40)]
            public IntPtr hVideoExtOut;
            [FieldOffset(0x1c)]
            public IntPtr hVideoIn;
            [FieldOffset(0x20)]
            public IntPtr hVideoOut;
            [FieldOffset(0)]
            public uint wDeviceIndex;
        }
    }
}
