namespace Platform.Resources
{
    using Platform.IO;
    using Platform.Utils;
    using System;
    using System.IO;
    using System.Reflection;

    public sealed class ResourceUtility
    {
        public static System.IO.MemoryStream GetResourceStream(Assembly asm, string resourceName)
        {
            if (resourceName == null)
            {
                return null;
            }
            Stream manifestResourceStream = Platform.Utils.ResourceUtility.GetManifestResourceStream(resourceName, asm);
            if (manifestResourceStream == null)
            {
                return null;
            }
            try
            {
                System.IO.MemoryStream stream2 = new System.IO.MemoryStream();
                byte[] buffer = new byte[0x400];
                int count = 0;
                while ((count = manifestResourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream2.Write(buffer, 0, count);
                }
                stream2.Flush();
                return stream2;
            }
            catch
            {
                return null;
            }
        }

        public static bool SaveResourceToFile(Assembly asm, string resourceName, string localFileName, bool overwrite)
        {
            if (resourceName == null)
            {
                return false;
            }
            if (!overwrite && File.Exists(localFileName))
            {
                return false;
            }
            Stream manifestResourceStream = Platform.Utils.ResourceUtility.GetManifestResourceStream(resourceName, asm);
            if (manifestResourceStream == null)
            {
                return false;
            }
            FileUtility.Delete(localFileName);
            try
            {
                using (FileStream stream2 = new FileStream(localFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[0x400];
                    int count = 0;
                    while ((count = manifestResourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream2.Write(buffer, 0, count);
                    }
                    stream2.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
