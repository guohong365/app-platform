namespace Platform.CSS.PacketHandling
{
    using Platform.CSS.Communication.Packet;
    using Platform.CSS.Communication.PacketHandler;
    using System;

    internal sealed class ExceptionPacketHandler : ICommunicationPacketHandler
    {
        public CommunicationPacketBase HandlePacket(Guid packetID, CommunicationPacketBase packet)
        {
            return packet;
        }

        public bool CacheResult
        {
            get
            {
                return true;
            }
        }

        public bool IsBroadcastHandler
        {
            get
            {
                return false;
            }
        }
    }
}
