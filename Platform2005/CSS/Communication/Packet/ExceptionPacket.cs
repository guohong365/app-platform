namespace Platform.CSS.Communication.Packet
{
    using Platform.IO;
    using Platform.Serialization;
    using System;

    [CommunicationPacket(1, "SocketTcp", "BroadcastSocketUdp")]
    internal sealed class ExceptionPacket : CommunicationPacketBase
    {
        private System.Exception m_Exception;

        public ExceptionPacket()
        {
        }

        public ExceptionPacket(System.Exception exp)
        {
            this.m_Exception = exp;
        }

        public override bool Deserial(MemoryStream stream)
        {
            this.m_Exception = SerialFormatHelper.BinaryDeserial(stream) as System.Exception;
            return true;
        }

        public override bool Serial(MemoryStream stream)
        {
            SerialFormatHelper.BinarySerial(stream, this.m_Exception);
            return true;
        }

        public System.Exception Exception
        {
            get
            {
                return this.m_Exception;
            }
            set
            {
                this.m_Exception = value;
            }
        }
    }
}
