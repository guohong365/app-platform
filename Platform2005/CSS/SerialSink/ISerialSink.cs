namespace Platform.CSS.SerialSink
{
    using Platform.CSS.Communication.Packet;
    using Platform.IO;
    using System;

    public interface ISerialSink
    {
        MemoryStream Deserial(MemoryStream ms, int headerLen, int flagOffset);
        MemoryStream Serial(CommunicationPacketBase packet, MemoryStream ms, int headerLen, int flagOffset);
    }
}
