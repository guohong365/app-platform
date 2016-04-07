namespace Platform.CSS.Communication.Channels.Socket
{
    using Platform.Caching;
    using Platform.CSS;
    using Platform.CSS.Communication.Channels;
    using Platform.IO;
    using Platform.Net;
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;

    [Channel("BroadcastSocketUdp")]
    public sealed class SocketUdpBroadcastClientChannel : IClientChannel
    {
        private static readonly IPEndPoint IPEAny = new IPEndPoint(IPAddress.Any, 0);
        private static readonly IPEndPoint IPV6EAny = new IPEndPoint(IPAddress.IPv6Any, 0);
        private BufferStorage m_ClientStorage = new BufferStorage(typeof(UdpClient));
        private IPEndPoint[] m_IPEs;

        public bool CheckServerOnLine(string settingName)
        {
            return true;
        }

        public void CloseConnections(string settingName)
        {
        }

        public bool InitializeChannel(string settingName, IPEndPoint[] ipes)
        {
            if (CSSConfig.CommunicationClientBroadcastLocalIPEnable)
            {
                this.m_IPEs = ipes;
            }
            else
            {
                ArrayList list = new ArrayList();
                foreach (IPEndPoint point in ipes)
                {
                    if (!IPUtility.IsLocalIPAddress(point.Address))
                    {
                        list.Add(point);
                    }
                }
                this.m_IPEs = list.ToArray(typeof(IPEndPoint)) as IPEndPoint[];
            }
            return true;
        }

        public bool Send(string settingName, Guid packetID, MemoryStream ms, MemoryStream msrc, ref int breadPoint, int timeOut)
        {
            try
            {
                byte[] buffer = ms.ToArray();
                UdpClient item = this.m_ClientStorage.GetBuffer() as UdpClient;
                item.Send(buffer, 0, buffer.Length, this.m_IPEs);
                this.m_ClientStorage.FreeBuffer(item);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsBroadcastChannel
        {
            get
            {
                return true;
            }
        }

        private class UdpClient : IBufferItem
        {
            private System.Net.Sockets.Socket Socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            public void FinalRelease()
            {
            }

            public void Send(byte[] buffer, int index, int count, IPEndPoint[] ipes)
            {
                foreach (IPEndPoint point in ipes)
                {
                    this.Socket.SendTo(buffer, index, count, SocketFlags.None, point);
                }
            }
        }
    }
}
