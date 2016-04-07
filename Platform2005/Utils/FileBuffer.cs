namespace Platform.Utils
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class FileBuffer
    {
        private byte[] m_Buffer;
        private static Hashtable m_Files = new Hashtable();
        private static GetFileDataEventHandler m_Handler = new GetFileDataEventHandler(FileBuffer.OnGetFileData);
        private long m_Length;

        public static FileBuffer GetFileBuffer(string localFileName)
        {
            FileBuffer buffer3;
            FileBuffer buffer = m_Files[localFileName] as FileBuffer;
            if (buffer != null)
            {
                return buffer;
            }
            Monitor.Enter(m_Files.SyncRoot);
            try
            {
                buffer = m_Files[localFileName] as FileBuffer;
                if (buffer != null)
                {
                    return buffer;
                }
                byte[] buffer2 = m_Handler(localFileName);
                if (buffer2 == null)
                {
                    return null;
                }
                buffer = new FileBuffer();
                buffer.m_Buffer = buffer2;
                buffer.m_Length = buffer2.Length;
                m_Files[localFileName] = buffer;
                buffer3 = buffer;
            }
            catch
            {
                buffer3 = null;
            }
            finally
            {
                Monitor.Exit(m_Files.SyncRoot);
            }
            return buffer3;
        }

        private static byte[] OnGetFileData(string localFileName)
        {
            if (!File.Exists(localFileName))
            {
                return null;
            }
            try
            {
                FileStream stream = new FileStream(localFileName, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                return buffer;
            }
            catch
            {
                return null;
            }
        }

        public int Read(out byte[] output, long offset, int count)
        {
            int num = ((this.Length - offset) > count) ? count : ((int) (this.Length - offset));
            if (num < 0)
            {
                num = 0;
            }
            output = new byte[num];
            if (this.Length > offset)
            {
                System.Buffer.BlockCopy(this.m_Buffer, (int) offset, output, 0, num);
            }
            return num;
        }

        public static void RegisterGetFileDataHandler(GetFileDataEventHandler handler)
        {
            if (handler != null)
            {
                m_Handler = handler;
            }
        }

        public byte[] Buffer
        {
            get
            {
                return this.m_Buffer;
            }
        }

        public long Length
        {
            get
            {
                return this.m_Length;
            }
        }
    }
}
