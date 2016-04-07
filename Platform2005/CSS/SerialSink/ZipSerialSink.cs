namespace Platform.CSS.SerialSink
{
    using Platform.Compress;
    using Platform.CSS.Communication.Packet;
    using Platform.IO;
    using System;
    using System.IO;

    [SerialSink(1)]
    public sealed class ZipSerialSink : ISerialSink
    {
        public Platform.IO.MemoryStream Deserial(Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            ms.Seek((long) flagOffset, SeekOrigin.Begin);
            int num = ms.ReadByte();
            if (num < 0)
            {
                return null;
            }
            if (num != 1)
            {
                return ms;
            }
            ms.Seek((long) 0, SeekOrigin.Begin);
            Platform.IO.MemoryStream outms = new Platform.IO.MemoryStream(ms, headerLen);
            if (!GZipUtility.GUnzipData(ms, headerLen, outms))
            {
                return null;
            }
            return outms;
        }

        public Platform.IO.MemoryStream Serial(CommunicationPacketBase packet, Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            if ((SerialSinkConfig.ZipLimitation < 0) || ((ms.Length - headerLen) < SerialSinkConfig.ZipLimitation))
            {
                ms.Seek((long) flagOffset, SeekOrigin.Begin);
                ms.WriteByte(0);
                return ms;
            }
            ms.Seek((long) 0, SeekOrigin.Begin);
            Platform.IO.MemoryStream outms = new Platform.IO.MemoryStream(ms, headerLen);
            if (!GZipUtility.GZipData(ms, headerLen, outms))
            {
                return null;
            }
            outms.Seek((long) flagOffset, SeekOrigin.Begin);
            outms.WriteByte(1);
            return outms;
        }
    }
}
