namespace Platform.Font
{
    using Microsoft.Win32;
    using Platform;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public sealed class EUDCUtility
    {
        [DllImport("gdi32.dll")]
        public static extern bool EnableEUDC(bool bEnable);
        public static bool InstallEUDC(string fontFamilyName, string fileName)
        {
            return InstallEUDC(PlatformConfig.TextEncoding.CodePage, fontFamilyName, fileName);
        }

        public static bool InstallEUDC(int codePage, string fontFamilyName, string fileName)
        {
            bool flag;
            fileName = Path.GetFullPath(fileName.Replace("/", @"\"));
            string name = codePage.ToString();
            EnableEUDC(false);
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("EUDC", true))
                {
                    RegistryKey key2 = key.OpenSubKey(name, true);
                    if (key2 == null)
                    {
                        key2 = key.CreateSubKey(name);
                    }
                    if (key2.GetValue("SystemDefaultEUDCFont") == null)
                    {
                        key2.SetValue("SystemDefaultEUDCFont", "");
                    }
                    key2.SetValue(fontFamilyName, fileName);
                }
                flag = true;
            }
            catch
            {
                flag = false;
            }
            finally
            {
                EnableEUDC(true);
            }
            return flag;
        }
    }
}
