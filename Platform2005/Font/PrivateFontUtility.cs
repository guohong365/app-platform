namespace Platform.Font
{
    using System;
    using System.Drawing;
    using System.Drawing.Text;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public sealed class PrivateFontUtility
    {
        private static System.Type m_FontType = typeof(Font);
        private static PrivateFontCollection m_PrivateFonts = new PrivateFontCollection();
        private static FieldInfo nativeFontInfo = m_FontType.GetField("nativeFont", BindingFlags.SetField | BindingFlags.GetField | BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        public static void LoadFontFile(string fontFileName)
        {
            try
            {
                m_PrivateFonts.AddFontFile(fontFileName);
            }
            catch
            {
            }
        }

        private static void LoadFontFile(byte[] fontData)
        {
            try
            {
                IntPtr destination = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(byte)) * fontData.Length);
                Marshal.Copy(fontData, 0, destination, fontData.Length);
                m_PrivateFonts.AddMemoryFont(destination, fontData.Length);
                Marshal.FreeHGlobal(destination);
            }
            catch
            {
            }
        }

        private static void LoadFontFile1(string fontFileName)
        {
            try
            {
                FileStream stream = new FileStream(fontFileName, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                LoadFontFile(buffer);
            }
            catch
            {
            }
        }

        public static void SetupControlFont(Control ctrl)
        {
            int length = m_PrivateFonts.Families.Length;
            if ((length >= 1) && (ctrl.Font.FontFamily != m_PrivateFonts.Families[length - 1]))
            {
                Font font = ctrl.Font;
                Font font2 = new Font(m_PrivateFonts.Families[length - 1], font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
                ctrl.Font = font2;
            }
        }
    }
}
