namespace Platform.IO
{
    using Platform;
    using Platform.ExceptionHandling;
    using System;
    using System.IO;

    public sealed class PathUtility
    {
        private static readonly char[] InValidChars = new char[] { '/', '\\', '<', '>', '(', ')', '?', '*', '&', '^', '%', ':', ';' };

        public static string GetFullPath(string path)
        {
            return GetFullPath(path, PlatformConfig.BasePath);
        }

        public static string GetFullPath(string path, string basePath)
        {
            if (Path.IsPathRooted(path))
            {
                return Path.GetFullPath(path);
            }
            return Path.GetFullPath(basePath + @"\" + path.Replace("/", @"\"));
        }

        public static string GetValidName(string input)
        {
            string text = input;
            for (int i = 0; i < InValidChars.Length; i++)
            {
                text = text.Replace(InValidChars[i], '_');
            }
            return text;
        }

        public static void MakeDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        public static void MakeFileDirectory(string fileName)
        {
            MakeDirectory(Path.GetDirectoryName(Path.GetFullPath(fileName.Replace("/", @"\"))));
        }

        public static bool RemoveFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return true;
            }
            try
            {
                File.SetAttributes(filename, FileAttributes.Normal);
                File.Delete(filename);
                return !File.Exists(filename);
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return false;
            }
        }
    }
}
