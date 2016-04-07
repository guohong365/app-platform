namespace Platform.LiveUpdate
{
    using Platform.Compress;
    using System;
    using System.Data;
    using System.IO;
    using System.Net;

    public class LiveUpdateWebServiceHelper
    {
        private static CookieContainer Cookie;
        private static bool Logined;
        private static string Password;
        private static LiveupdateService Service;
        public static string SettingName;
        public static string Url;
        private static string UserName;

        public static bool AddUpdateFile(string path, string fileName, bool restart, bool must, int fileType, string uploadFileName)
        {
            if (!Logined && !Login())
            {
                return false;
            }
            byte[] buffer = null;
            using (FileStream stream = new FileStream(uploadFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
            int num = Service.UploadFile(SettingName, path, fileName, restart, must, fileType, buffer);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.UploadFile(SettingName, path, fileName, restart, must, fileType, buffer) == 0);
        }

        public static string CheckFile(string id)
        {
            if (!Logined && !Login())
            {
                return null;
            }
            string text = Service.CheckFile(id);
            if (text != null)
            {
                return text;
            }
            if (!Login())
            {
                return null;
            }
            return Service.CheckFile(id);
        }

        public static bool CleanUpdateFiles()
        {
            if (!Logined && !Login())
            {
                return false;
            }
            int num = Service.CleanUpdateFiles(SettingName);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.CleanUpdateFiles(SettingName) == 0);
        }

        public static bool CreateSettingName(string settingName, string updateFolder, string updateZipFolder)
        {
            if (!Logined && !Login())
            {
                return false;
            }
            int num = Service.CreateSettingName(settingName, updateFolder, updateZipFolder);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.CreateSettingName(settingName, updateFolder, updateZipFolder) == 0);
        }

        public static bool DeleteSettingName(string settingName)
        {
            if (!Logined && !Login())
            {
                return false;
            }
            int num = Service.DeleteSettingName(settingName);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.DeleteSettingName(settingName) == 0);
        }

        public static byte[] GetFile(string id, bool zipflag)
        {
            byte[] file = Service.GetFile(id, zipflag);
            if (file == null)
            {
                return null;
            }
            if (!zipflag)
            {
                return file;
            }
            return GZipUtility.GUnzip(file, 0, file.Length);
        }

        public static DataSet GetFileList()
        {
            DataSet set;
            if (Service.GetFileListNew(SettingName, out set) == 0)
            {
                return set;
            }
            return null;
        }

        public static DataSet GetFileListByFileType(string filetype)
        {
            DataSet set;
            if (Service.GetFileListTyped(SettingName, filetype, out set) == 0)
            {
                return set;
            }
            return null;
        }

        public static byte[] GetFileOrg(string id, bool zipflag)
        {
            return Service.GetFile(id, zipflag);
        }

        public static string[] GetSettingNames()
        {
            if (!Logined && !Login())
            {
                return null;
            }
            string[] settingNames = Service.GetSettingNames();
            if (settingNames != null)
            {
                return settingNames;
            }
            if (!Login())
            {
                return null;
            }
            return Service.GetSettingNames();
        }

        public static DataSet GetUpdateFileList()
        {
            DataSet set;
            if (!Logined && !Login())
            {
                return null;
            }
            int fullFileList = Service.GetFullFileList(SettingName, out set);
            if (fullFileList != 0)
            {
                if (fullFileList != -1)
                {
                    return null;
                }
                if (!Login())
                {
                    return null;
                }
                fullFileList = Service.GetFullFileList(SettingName, out set);
            }
            return set;
        }

        public static void Initialize(string userName, string passrowd, string url, string settingName)
        {
            Logined = false;
            UserName = userName;
            Password = passrowd;
            Url = url;
            SettingName = settingName;
            Cookie = new CookieContainer();
            Service = new LiveupdateService();
            Service.Url = Url;
            Service.CookieContainer = Cookie;
        }

        private static bool Login()
        {
            bool flag1 = Service.Login(UserName, Password);
            Logined = flag1;
            return flag1;
        }

        public static bool RemoveFileById(string id)
        {
            if (!Logined && !Login())
            {
                return false;
            }
            int num = Service.RemoveFileById(id);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.RemoveFileById(id) == 0);
        }

        public static bool UpdateFile(string id, bool restart, bool must, int fileType)
        {
            if (!Logined && !Login())
            {
                return false;
            }
            int num = Service.UpdateFile(id, restart, must, fileType);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.UpdateFile(id, restart, must, fileType) == 0);
        }

        public static bool UpdateFileContent(string id, byte[] fileData)
        {
            if (fileData == null)
            {
                return false;
            }
            if (!Logined && !Login())
            {
                return false;
            }
            int num = Service.UpdateFileContent(id, fileData);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.UpdateFileContent(id, fileData) == 0);
        }

        public static bool UpdateSettingName(string settingName, string updateFolder, string updateZipFolder)
        {
            if (!Logined && !Login())
            {
                return false;
            }
            int num = Service.UpdateSettingName(settingName, updateFolder, updateZipFolder);
            if (num == 0)
            {
                return true;
            }
            if (num != -1)
            {
                return false;
            }
            if (!Login())
            {
                return false;
            }
            return (Service.UpdateSettingName(settingName, updateFolder, updateZipFolder) == 0);
        }
    }
}
