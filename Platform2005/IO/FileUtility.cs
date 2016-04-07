namespace Platform.IO
{
    using System;
    using System.IO;

    public class FileUtility
    {
        public static bool Copy(string sourceFile, string destFile, bool overWrite)
        {
            try
            {
                if (!File.Exists(sourceFile))
                {
                    return false;
                }
                if (File.Exists(destFile))
                {
                    if (!overWrite)
                    {
                        return false;
                    }
                    SetFileAttributes(destFile, FileAttributes.Normal);
                }
                PathUtility.MakeDirectory(Path.GetDirectoryName(destFile));
                File.Copy(sourceFile, destFile, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    SetFileAttributes(fileName, FileAttributes.Normal);
                    File.Delete(fileName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    SetDirectoryAllAttributes(path, FileAttributes.Normal);
                    Directory.Delete(path, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteDirectory(string path, bool includePath)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    PathUtility.MakeDirectory(path);
                    return true;
                }
                SetDirectoryAllAttributes(path, FileAttributes.Normal);
                if (!DeleteDirectory(path))
                {
                    return false;
                }
                if (!includePath)
                {
                    PathUtility.MakeDirectory(path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Move(string sourceFile, string destFile, bool overWrite)
        {
            try
            {
                if (Path.GetFullPath(sourceFile.Replace("/", @"\")).ToUpper() != Path.GetFullPath(destFile.Replace("/", @"\")).ToUpper())
                {
                    if (!File.Exists(sourceFile))
                    {
                        return false;
                    }
                    if (File.Exists(destFile))
                    {
                        if (!overWrite)
                        {
                            return false;
                        }
                        Delete(destFile);
                    }
                    PathUtility.MakeDirectory(Path.GetDirectoryName(Path.GetFullPath(destFile.Replace("/", @"\"))));
                    File.Move(sourceFile, destFile);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SetDirectoryAllAttributes(string path, FileAttributes attrs)
        {
            try
            {
                foreach (string text in Directory.GetFiles(path))
                {
                    SetFileAttributes(text, attrs);
                }
                foreach (string text2 in Directory.GetDirectories(path))
                {
                    SetDirectoryAttributes(text2, attrs);
                    SetDirectoryAllAttributes(text2, attrs);
                }
            }
            catch
            {
            }
        }

        public static bool SetDirectoryAttributes(string path, FileAttributes attrs)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    return false;
                }
                DirectoryInfo info = new DirectoryInfo(path);
                info.Attributes = attrs;
                info.Refresh();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SetFileAttributes(string fileName, FileAttributes attrs)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return false;
                }
                File.SetAttributes(fileName, attrs);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
