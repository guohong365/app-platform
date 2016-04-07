namespace Platform.CSS.Communication.PacketHandler
{
    using Platform.CSS.Communication.Packet;
    using System;

    public interface ICommunicationPacketHandler
    {
        CommunicationPacketBase HandlePacket(Guid packetID, CommunicationPacketBase packet);

        bool CacheResult { get; }

        bool IsBroadcastHandler { get; }
    }
}
