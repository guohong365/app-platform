namespace Platform.CSS.Remoting
{
    using Platform.CSS.Communication.Packet;
    using Platform.IO;
    using Platform.Serialization;
    using System;

    [CommunicationPacket(5, "SocketTcp", "BroadcastSocketUdp")]
    public sealed class RemotingPacket2nd : CommunicationPacketBase
    {
        private CallParams m_CallParams = new CallParams();

        public override bool Deserial(MemoryStream stream)
        {
            CallParams m_params = SerialFormatHelper.BinaryDeserial(stream) as CallParams;
            this.m_CallParams.FullMethodName = m_params.FullMethodName;
            this.m_CallParams.Parameters = m_params.Parameters;
            this.m_CallParams.ParametersDirect = m_params.ParametersDirect;
            this.m_CallParams.ReturnResult = m_params.ReturnResult;
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

        public string FullMethodName
        {
            get
            {
                return this.m_CallParams.FullMethodName;
            }
            set
            {
                this.m_CallParams.FullMethodName = value;
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

        public byte[] ParametersDirect
        {
            get
            {
                return this.m_CallParams.ParametersDirect;
            }
            set
            {
                this.m_CallParams.ParametersDirect = value;
            }
        }

        public object ReturnResult
        {
            get
            {
                return this.m_CallParams.ReturnResult;
            }
            set
            {
                this.m_CallParams.ReturnResult = value;
            }
        }

        [Serializable]
        private sealed class CallParams
        {
            public string FullMethodName;
            public object[] Parameters;
            public byte[] ParametersDirect;
            public object ReturnResult;

            public void FreeBuffer()
            {
                this.FullMethodName = "";
                this.Parameters = null;
                this.ParametersDirect = null;
                this.ReturnResult = null;
            }
        }
    }
}
