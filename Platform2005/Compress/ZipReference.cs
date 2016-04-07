using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;

namespace Platform.Compress
{
    public sealed class ZipReference
    {
        public static byte[] GUnzipData(byte[] input)
        {
            return GUnzipData(input, 0, input.Length);
        }

        public static byte[] GUnzipData(byte[] input, int offset, int length)
        {
            try
            {
                byte[] buffer = new byte[0x400];
                var baseInputStream = new MemoryStream(input, offset, length);
                GZipInputStream stream2 = new GZipInputStream(baseInputStream);
                var stream3 = new MemoryStream();
                int count;
                while ((count = stream2.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream3.Write(buffer, 0, count);
                }
                stream2.Close();
                return stream3.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GUnzipData(byte[] input, int headerOffset, int headerLen, int offset, int length)
        {
            try
            {
                byte[] buffer = new byte[0x400];
                var baseInputStream = new MemoryStream(input, offset, length);
                GZipInputStream stream2 = new GZipInputStream(baseInputStream);
                var stream3 = new MemoryStream();
                stream3.Write(input, headerOffset, headerLen);
                int count;
                while ((count = stream2.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream3.Write(buffer, 0, count);
                }
                stream2.Close();
                return stream3.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GZipData(byte[] input)
        {
            return GZipData(input, 0, input.Length);
        }

        public static byte[] GZipData(byte[] input, int offset, int length)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                GZipOutputStream stream2 = new GZipOutputStream(baseOutputStream);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GZipData(byte header, byte[] input, int offset, int length)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                baseOutputStream.WriteByte(header);
                GZipOutputStream stream2 = new GZipOutputStream(baseOutputStream);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GZipData(byte[] header, byte[] input, int offset, int length)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                baseOutputStream.Write(header, 0, header.Length);
                GZipOutputStream stream2 = new GZipOutputStream(baseOutputStream);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GZipData(byte[] input, int headerOffset, int headerLen, int offset, int length)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                baseOutputStream.Write(input, headerOffset, headerLen);
                GZipOutputStream stream2 = new GZipOutputStream(baseOutputStream);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GZipData(byte[] header, int hoff, int hlen, byte[] input, int offset, int length)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                baseOutputStream.Write(header, hoff, hlen);
                GZipOutputStream stream2 = new GZipOutputStream(baseOutputStream);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] UnzipData(byte[] input, int offset, int length, int nFileIndex)
        {
            try
            {
                var baseInputStream = new MemoryStream(input, offset, length);
                ZipInputStream stream2 = new ZipInputStream(baseInputStream);
                ZipEntry nextEntry;
                for (int i = 0; i < nFileIndex; i++)
                {
                    nextEntry = stream2.GetNextEntry();
                }
                nextEntry = stream2.GetNextEntry();
                if (nextEntry == null)
                {
                    return null;
                }
                int size = (int) nextEntry.Size;
                if (size < 0)
                {
                    return null;
                }
                if (size == 0)
                {
                    return new byte[0];
                }
                byte[] buffer = new byte[size];
                var num3 = 0;
                while (num3 < size)
                {
                    int num4 = stream2.Read(buffer, num3, size - num3);
                    if (num4 < 1)
                    {
                        stream2.Close();
                        return null;
                    }
                    num3 += num4;
                }
                stream2.Close();
                return buffer;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] UnzipData(byte[] input, int offset, int length, string fileName)
        {
            try
            {
                ZipEntry entry;
                MemoryStream baseInputStream = new MemoryStream(input, offset, length);
                ZipInputStream stream2 = new ZipInputStream(baseInputStream);
                while ((entry = stream2.GetNextEntry()) != null)
                {
                    if (entry.Name == fileName)
                    {
                        int size = (int) entry.Size;
                        if (size < 0)
                        {
                            return null;
                        }
                        if (size == 0)
                        {
                            return new byte[0];
                        }
                        byte[] buffer = new byte[size];
                        int num2 = 0;
                        while (num2 < size)
                        {
                            int num3 = stream2.Read(buffer, num2, size - num2);
                            if (num3 < 1)
                            {
                                stream2.Close();
                                return null;
                            }
                            num2 += num3;
                        }
                        stream2.Close();
                        return buffer;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static void UnzipFiles(string strZipFile, string strPath, bool createSubDir)
        {
            ZipInputStream stream = null;
            byte[] buffer = new byte[0x400];
            try
            {
                ZipEntry entry;
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                stream = new ZipInputStream(new FileStream(strZipFile, FileMode.Open, FileAccess.Read));
                while ((entry = stream.GetNextEntry()) != null)
                {
                    if (entry.Size >= 0)
                    {
                        int num;
                        string text;
                        if (createSubDir)
                        {
                            text = strPath + @"\" + entry.Name;
                            if (entry.IsDirectory)
                            {
                                Directory.CreateDirectory(text);
                                continue;
                            }
                            string directoryName = Path.GetDirectoryName(text);
                            Debug.Assert(directoryName != null, "directoryName != null");
                            if (!Directory.Exists(directoryName))
                            {
                                Directory.CreateDirectory(directoryName);
                            }
                        }
                        else
                        {
                            if (entry.IsDirectory)
                            {
                                continue;
                            }
                            text = strPath + @"\" + Path.GetFileName(entry.Name);
                        }
                        FileStream stream2 = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write);
                        while ((num = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream2.Write(buffer, 0, num);
                        }
                        stream2.Flush();
                        stream2.Close();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                catch
                {
                }
            }
        }

        public static void UnzipFiles(string strZipFile, string strPath, bool createSubDir, ZipEventHandler handler)
        {
            ZipInputStream stream = null;
            byte[] buffer = new byte[0x400];
            try
            {
                ZipEntry entry;
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                stream = new ZipInputStream(new FileStream(strZipFile, FileMode.Open, FileAccess.Read));
                while ((entry = stream.GetNextEntry()) != null)
                {
                    if (entry.Size >= 0)
                    {
                        int num;
                        string text;
                        if (createSubDir)
                        {
                            text = strPath + @"\" + entry.Name;
                            if (entry.IsDirectory)
                            {
                                Directory.CreateDirectory(text);
                                continue;
                            }
                            string directoryName = Path.GetDirectoryName(text);
                            if (!Directory.Exists(directoryName))
                            {
                                Directory.CreateDirectory(directoryName);
                            }
                        }
                        else
                        {
                            if (entry.IsDirectory)
                            {
                                continue;
                            }
                            text = strPath + @"\" + Path.GetFileName(entry.Name);
                        }
                        FileStream stream2 = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write);
                        while ((num = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream2.Write(buffer, 0, num);
                        }
                        stream2.Flush();
                        stream2.Close();
                        if (handler != null)
                        {
                            handler(text, entry.CompressedSize, entry.Size, false);
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                catch
                {
                }
            }
        }

        public static byte[] ZipFile(byte[] input, string FileName)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                ZipOutputStream stream2 = new ZipOutputStream(baseOutputStream);
                ZipEntry entry = new ZipEntry(FileName);
                stream2.PutNextEntry(entry);
                stream2.Write(input, 0, input.Length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ZipFile(byte[] input, int offset, int length, string FileName)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                ZipOutputStream stream2 = new ZipOutputStream(baseOutputStream);
                ZipEntry entry = new ZipEntry(FileName);
                stream2.PutNextEntry(entry);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ZipFile(byte header, byte[] input, int offset, int length, string FileName)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                baseOutputStream.WriteByte(header);
                ZipOutputStream stream2 = new ZipOutputStream(baseOutputStream);
                ZipEntry entry = new ZipEntry(FileName);
                stream2.PutNextEntry(entry);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ZipFile(byte[] header, byte[] input, int offset, int length, string FileName)
        {
            try
            {
                MemoryStream baseOutputStream = new MemoryStream();
                baseOutputStream.Write(header, 0, header.Length);
                ZipOutputStream stream2 = new ZipOutputStream(baseOutputStream);
                ZipEntry entry = new ZipEntry(FileName);
                stream2.PutNextEntry(entry);
                stream2.Write(input, offset, length);
                stream2.Finish();
                stream2.Close();
                return baseOutputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static void ZipFiles(string[] files, string outFile, bool fullPath)
        {
            ZipOutputStream stream = null;
            byte[] buffer = new byte[0x400];
            try
            {
                string directoryName = Path.GetDirectoryName(outFile);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                stream = new ZipOutputStream(new FileStream(outFile, FileMode.Create, FileAccess.Write));
                foreach (string text3 in files)
                {
                    int num;
                    FileStream stream2;
                    string fileName;
                    try
                    {
                        stream2 = new FileStream(text3, FileMode.Open, FileAccess.Read);
                    }
                    catch
                    {
                        goto Label_0096;
                    }
                    if (fullPath)
                    {
                        fileName = text3;
                    }
                    else
                    {
                        fileName = Path.GetFileName(text3);
                    }
                    ZipEntry entry = new ZipEntry(fileName);
                    stream.PutNextEntry(entry);
                    while ((num = stream2.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, num);
                    }
                    stream2.Close();
                Label_0096:;
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                catch
                {
                }
            }
        }

        public static void ZipFiles(string[] files, string outFile, bool fullPath, ZipEventHandler handler)
        {
            ZipOutputStream stream = null;
            byte[] buffer = new byte[0x400];
            try
            {
                string directoryName = Path.GetDirectoryName(outFile);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                stream = new ZipOutputStream(new FileStream(outFile, FileMode.Create, FileAccess.Write));
                foreach (string text3 in files)
                {
                    FileStream stream2;
                    string fileName;
                    int num;
                    try
                    {
                        stream2 = new FileStream(text3, FileMode.Open, FileAccess.Read);
                    }
                    catch
                    {
                        goto Label_00AF;
                    }
                    if (fullPath)
                    {
                        fileName = text3;
                    }
                    else
                    {
                        fileName = Path.GetFileName(text3);
                    }
                    ZipEntry entry = new ZipEntry(fileName);
                    stream.PutNextEntry(entry);
                    while ((num = stream2.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, num);
                    }
                    stream2.Close();
                    if (handler != null)
                    {
                        handler(text3, entry.CompressedSize, entry.Size, true);
                    }
                Label_00AF:;
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                catch
                {
                }
            }
        }
    }
}
