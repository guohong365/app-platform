namespace Platform.CSS.SerialSink
{
    using Platform.CSS.Communication.Packet;
    using Platform.IO;
    using Platform.Security;
    using System;
    using System.IO;

    [SerialSink(0)]
    public sealed class HashSerialSink : ISerialSink
    {
        public Platform.IO.MemoryStream Deserial(Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            ms.Seek((long) headerLen, SeekOrigin.Begin);
            byte[] buffer = Platform.Security.Security.ComputeHash(ms, (((int) ms.Length) - headerLen) - 8);
            byte[] buffer2 = new byte[8];
            ms.Seek((long) 8, SeekOrigin.End);
            ms.Read(buffer2, 0, 8);
            for (int i = 0; i < 8; i++)
            {
                if (buffer[i] != buffer2[i])
                {
                    throw new Exception("安全性校验错误！");
                }
            }
            ms.SetLength(ms.Length - 8);
            return ms;
        }

        public Platform.IO.MemoryStream Serial(CommunicationPacketBase packet, Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            ms.Seek((long) headerLen, SeekOrigin.Begin);
            byte[] buffer = Platform.Security.Security.ComputeHash(ms, ((int) ms.Length) - headerLen);
            ms.Seek((long) 0, SeekOrigin.End);
            ms.Write(buffer, 0, 8);
            return ms;
        }
    }
}
