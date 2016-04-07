namespace Platform.Resources
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public sealed class SystemIconHelper
    {
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private static Hashtable m_LargeIcons = new Hashtable();
        private static Hashtable m_SmallIcons = new Hashtable();
        public const uint SHGFI_ADDOVERLAYS = 0x20;
        public const uint SHGFI_ATTR_SPECIFIED = 0x20000;
        public const uint SHGFI_ATTRIBUTES = 0x800;
        public const uint SHGFI_DISPLAYNAME = 0x200;
        public const uint SHGFI_EXETYPE = 0x2000;
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_ICONLOCATION = 0x1000;
        public const uint SHGFI_LARGEICON = 0;
        public const uint SHGFI_LINKOVERLAY = 0x8000;
        public const uint SHGFI_OPENICON = 2;
        public const uint SHGFI_OVERLAYINDEX = 0x40;
        public const uint SHGFI_PIDL = 8;
        public const uint SHGFI_SELECTED = 0x10000;
        public const uint SHGFI_SHELLICONSIZE = 4;
        public const uint SHGFI_SMALLICON = 1;
        public const uint SHGFI_SYSICONINDEX = 0x4000;
        public const uint SHGFI_TYPENAME = 0x400;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;

        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
        public static Icon GetSystemFileIcon(string extension, bool smallIcon)
        {
            string key = extension.ToLower();
            if (smallIcon && m_SmallIcons.ContainsKey(key))
            {
                return (m_SmallIcons[key] as Icon);
            }
            if (m_LargeIcons.ContainsKey(key))
            {
                return (m_LargeIcons[key] as Icon);
            }
            Icon systemFileIcon = InternalGetSystemFileIcon(key, smallIcon);
            if (smallIcon)
            {
                m_SmallIcons[key] = systemFileIcon;
                return systemFileIcon;
            }
            m_LargeIcons[key] = systemFileIcon;
            return systemFileIcon;
        }

        public static void GetSystemFileIcon(string extension, out Icon samllIcon, out Icon largeIcon)
        {
            string key = extension.ToLower();
            if (!m_SmallIcons.ContainsKey(key))
            {
                m_SmallIcons[key] = InternalGetSystemFileIcon(key, true);
            }
            if (!m_LargeIcons.ContainsKey(key))
            {
                m_LargeIcons[key] = InternalGetSystemFileIcon(key, false);
            }
            samllIcon = m_SmallIcons[key] as Icon;
            largeIcon = m_LargeIcons[key] as Icon;
        }

        public static void GetSystemFolderIcon(out Icon samllIcon, out Icon samllOpenIcon, out Icon largeIcon, out Icon largeOpenIcon)
        {
            samllIcon = InternalGetSystemFolderIcon(true, false);
            samllOpenIcon = InternalGetSystemFolderIcon(true, true);
            largeIcon = InternalGetSystemFolderIcon(false, false);
            largeOpenIcon = InternalGetSystemFolderIcon(false, true);
        }

        private static Icon InternalGetSystemFileIcon(string extension, bool smallIcon)
        {
            SHFILEINFO psfi = new SHFILEINFO();
            SHGetFileInfo(extension, 0x80, ref psfi, (uint) Marshal.SizeOf(psfi), (uint) (0x110 | (smallIcon ? 1 : 0)));
            Icon icon = (Icon) Icon.FromHandle(psfi.hIcon).Clone();
            DestroyIcon(psfi.hIcon);
            return icon;
        }

        private static Icon InternalGetSystemFolderIcon(bool smallIcon, bool openIcon)
        {
            SHFILEINFO psfi = new SHFILEINFO();
            SHGetFileInfo(null, 0x10, ref psfi, (uint) Marshal.SizeOf(psfi), (uint) ((0x110 | (smallIcon ? 1 : 0)) | (openIcon ? 2 : 0)));
            Icon icon = (Icon) Icon.FromHandle(psfi.hIcon).Clone();
            DestroyIcon(psfi.hIcon);
            return icon;
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=80)]
            public string szTypeName;
        }
    }
}
