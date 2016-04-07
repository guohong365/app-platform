namespace Platform.CSS.Remoting
{
    using Platform.CSS.Communication.Packet;
    using Platform.IO;
    using Platform.Serialization;
    using System;

    [CommunicationPacket(6, "SocketTcp", "BroadcastSocketUdp")]
    public sealed class BroadcastRemotingPacket : CommunicationPacketBase
    {
        private CallParams m_CallParams = new CallParams();

        public override bool Deserial(MemoryStream stream)
        {
            CallParams m_params = SerialFormatHelper.BinaryDeserial(stream) as CallParams;
            this.m_CallParams.FullName = m_params.FullName;
            this.m_CallParams.Parameters = m_params.Parameters;
            return true;
        }

        public override void FreeBuffer()
        {
            this.m_CallParams.FreeBuffer();
            base.FreeBuffer();
        }

        public override bool Serial(MemoryStream stream)
        {
            SerialFormatHelper.BinarySerial(stream, this.m_CallParams);
            return true;
        }

        public string FullName
        {
            get
            {
                return this.m_CallParams.FullName;
            }
            set
            {
                this.m_CallParams.FullName = value;
            }
        }

        public object[] Parameters
        {
            get
            {
                return this.m_CallParams.Parameters;
            }
            set
            {
                this.m_CallParams.Parameters = value;
            }
        }

        [Serializable]
        private sealed class CallParams
        {
            public string FullName;
            public object[] Parameters;

            public void FreeBuffer()
            {
                this.FullName = null;
                this.Parameters = null;
            }
        }
    }
}
