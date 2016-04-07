namespace Platform.CSS.SerialSink
{
    using Platform.CSS.Communication.Packet;
    using Platform.Identity;
    using Platform.IO;
    using System;
    using System.IO;

    [SerialSink(0x10)]
    public sealed class UserIDSink : ISerialSink
    {
        public Platform.IO.MemoryStream Deserial(Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            byte[] buffer = new byte[0x10];
            ms.Seek((long) flagOffset, SeekOrigin.Begin);
            ms.Read(buffer, 0, 0x10);
            Guid userID = new Guid(buffer);
            UserManager.SetCurrentThreadUserID(userID);
            return ms;
        }

        public Platform.IO.MemoryStream Serial(CommunicationPacketBase packet, Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            Guid currentThreadUserID = UserManager.GetCurrentThreadUserID();
            ms.Seek((long) flagOffset, SeekOrigin.Begin);
            ms.Write(currentThreadUserID.ToByteArray(), 0, 0x10);
            return ms;
        }
    }
}
