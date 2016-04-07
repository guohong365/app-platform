namespace Platform.CSS.Packet
{
    using Platform.CSS.Communication.Packet;
    using Platform.IO;
    using System;

    [CommunicationPacket(2, "SocketTcp", "BroadcastSocketUdp")]
    internal sealed class LoginPacket : CommunicationPacketBase
    {
        private bool m_IsLogin = true;
        private bool m_IsReturn;
        private byte[] m_KeyData = new byte[0];
        private int m_LoginCode;
        private int m_ReturnCode;
        private byte[] m_UserData = new byte[0];
        private Guid m_UserID = Guid.Empty;

        public override bool Deserial(MemoryStream stream)
        {
            if (!stream.Read(ref this.m_IsLogin))
            {
                return false;
            }
            if (!stream.Read(ref this.m_IsReturn))
            {
                return false;
            }
            if (this.m_IsReturn)
            {
                if (!stream.Read(ref this.m_UserID))
                {
                    return false;
                }
                if (!stream.Read(ref this.m_ReturnCode))
                {
                    return false;
                }
                if (!stream.Read(ref this.m_LoginCode))
                {
                    return false;
                }
            }
            else if (!this.m_IsLogin && !stream.Read(ref this.m_UserID))
            {
                return false;
            }
            this.m_UserData = stream.ReadByteArray();
            if (this.m_UserData == null)
            {
                return false;
            }
            this.m_KeyData = stream.ReadByteArray();
            if (this.m_KeyData == null)
            {
                return false;
            }
            return true;
        }

        public override void FreeBuffer()
        {
            this.Reset();
            base.FreeBuffer();
        }

        public void Reset()
        {
            this.m_IsLogin = true;
            this.m_IsReturn = false;
            this.m_UserData = new byte[0];
            this.m_KeyData = new byte[0];
            this.m_UserID = Guid.Empty;
            this.m_ReturnCode = 0;
            this.m_LoginCode = 0;
        }

        public override bool Serial(MemoryStream stream)
        {
            stream.Write(this.m_IsLogin);
            stream.Write(this.m_IsReturn);
            if (this.m_IsReturn)
            {
                stream.Write(this.m_UserID);
                stream.Write(this.m_ReturnCode);
                stream.Write(this.m_LoginCode);
            }
            else if (!this.m_IsLogin)
            {
                stream.Write(this.m_UserID);
            }
            stream.WriteByteArray(this.m_UserData);
            stream.WriteByteArray(this.m_KeyData);
            return true;
        }

        public bool IsLogin
        {
            get
            {
                return this.m_IsLogin;
            }
            set
            {
                this.m_IsLogin = value;
            }
        }

        public bool IsReturn
        {
            get
            {
                return this.m_IsReturn;
            }
            set
            {
                this.m_IsReturn = value;
            }
        }

        public byte[] KeyData
        {
            get
            {
                return this.m_KeyData;
            }
            set
            {
                this.m_KeyData = value;
            }
        }

        public int LoginCode
        {
            get
            {
                return this.m_LoginCode;
            }
            set
            {
                this.m_LoginCode = value;
            }
        }

        public int ReturnCode
        {
            get
            {
                return this.m_ReturnCode;
            }
            set
            {
                this.m_ReturnCode = value;
            }
        }

        public byte[] UserData
        {
            get
            {
                return this.m_UserData;
            }
            set
            {
                this.m_UserData = value;
            }
        }

        public Guid UserID
        {
            get
            {
                return this.m_UserID;
            }
            set
            {
                this.m_UserID = value;
            }
        }
    }
}
