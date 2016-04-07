namespace Platform.CSS.Communication.Channels.Socket
{
    using Platform.Caching;
    using Platform.CSS;
    using Platform.CSS.Communication.Channels;
    using Platform.IO;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    [Channel("SocketTcp")]
    public sealed class SocketTcpClientChannel : IClientChannel
    {
        private int m_BufferLen;
        private static byte[] m_CheckBuffer = new byte[1];
        private BufferStorage m_ConnectionStorage = new BufferStorage(typeof(Connection));
        private IPEndPoint m_Ipe;

        public bool CheckServerOnLine(string settingName)
        {
            bool flag;
            Connection buffer = this.m_ConnectionStorage.GetBuffer() as Connection;
            try
            {
                flag = buffer.CheckConnection(this, 1);
            }
            catch
            {
                flag = false;
            }
            finally
            {
                this.m_ConnectionStorage.FreeBuffer(buffer);
            }
            return flag;
        }

        public void CloseConnections(string settingName)
        {
            this.m_ConnectionStorage.FinalReset();
        }

        public bool InitializeChannel(string settingName, IPEndPoint[] ipes)
        {
            this.m_ConnectionStorage.FinalReset();
            this.m_Ipe = ipes[0];
            this.m_BufferLen = CSSConfig.CommunicationBufferLength;
            return true;
        }

        public bool Send(string settingName, Guid packetID, Platform.IO.MemoryStream ms, Platform.IO.MemoryStream msrc, ref int breadPoint, int timeOut)
        {
            bool flag3;
            Connection buffer = this.m_ConnectionStorage.GetBuffer() as Connection;
            try
            {
                int num;
                bool flag2 = false;
                for (int i = 0; i < 2; i++)
                {
                    if (!buffer.CheckConnection(this))
                    {
                        return false;
                    }
                    try
                    {
                        ms.Seek((long) 0, SeekOrigin.Begin);
                        flag2 = SocketTcpClientHelper.SendStream(buffer.m_Client, ms, 0, (int) ms.Length, breadPoint, timeOut * 0x3e8);
                        if (flag2)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        buffer.Close();
                    }
                }
                if (!flag2)
                {
                    buffer.Close();
                    return false;
                }
                bool flag = SocketTcpClientHelper.ReceiveStream(buffer.m_Client, msrc, breadPoint, out num, out breadPoint, this.m_BufferLen, timeOut * 0x3e8);
                breadPoint += num;
                if (!flag)
                {
                    buffer.Close();
                    return false;
                }
                flag3 = true;
            }
            catch
            {
                buffer.Close();
                flag3 = false;
            }
            finally
            {
                this.m_ConnectionStorage.FreeBuffer(buffer);
            }
            return flag3;
        }

        public bool IsBroadcastChannel
        {
            get
            {
                return false;
            }
        }

        private sealed class Connection : IBufferItem
        {
            public Socket m_Client;
            private static byte[] m_EmptyBuffer = new byte[0];

            public bool CheckConnection(SocketTcpClientChannel channel)
            {
                return this.CheckConnection(channel, 3);
            }

            public bool CheckConnection(SocketTcpClientChannel channel, int times)
            {
                if (times < 1)
                {
                    return false;
                }
                if ((this.m_Client != null) && this.m_Client.Connected)
                {
                    return true;
                }
                try
                {
                    this.m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.m_Client.Connect(channel.m_Ipe);
                    if (this.m_Client.Connected)
                    {
                        int bufferLen = (channel.m_BufferLen > CSSConfig.CommunicationTcpMaxBufferLimited) ? CSSConfig.CommunicationTcpMaxBufferLimited : channel.m_BufferLen;
                        if (bufferLen < 0)
                        {
                            bufferLen = CSSConfig.CommunicationTcpMaxBufferLimited;
                        }
                        SocketTcpHelper.SetSocketBuffer(this.m_Client, bufferLen);
                        this.m_Client.Send(new byte[] { CSSConfig.SocketTcpCheckValue }, 0, 1, SocketFlags.None);
                        return true;
                    }
                }
                catch
                {
                }
                this.Close();
                times--;
                if (times < 1)
                {
                    return false;
                }
                return this.CheckConnection(channel, times);
            }

            public void Close()
            {
                if (this.m_Client != null)
                {
                    try
                    {
                        this.m_Client.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }
                    try
                    {
                        this.m_Client.Close();
                    }
                    catch
                    {
                    }
                    this.m_Client = null;
                }
            }

            public void FinalRelease()
            {
                this.Close();
            }
        }
    }
}
