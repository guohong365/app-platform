namespace Platform.Net.Socket
{
    using Platform.Threading;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public sealed class Spy
    {
        private bool m_Binded;
        private Socket m_SpySocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
        private Platform.Threading.ThreadPool m_Thread;
        private WaitCallback m_ThreadHandler;
        private const uint SIO_RCVALL = 0x98000001;

        public Spy()
        {
            int optionValue = 0x1388;
            this.m_SpySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, optionValue);
            byte[] buffer3 = new byte[4];
            buffer3[0] = 1;
            byte[] optionInValue = buffer3;
            byte[] optionOutValue = new byte[4];
            this.m_SpySocket.IOControl(-1744830463, optionInValue, optionOutValue);
            this.m_ThreadHandler = new WaitCallback(this.Run);
        }

        private void Run(object state)
        {
            while (true)
            {
                try
                {
                    if (this.m_SpySocket.Available > 0)
                    {
                        byte[] buffer = new byte[this.m_SpySocket.Available];
                        this.m_SpySocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    }
                }
                catch
                {
                }
            }
        }

        public void SetSpyIP(IPAddress spyIP)
        {
            this.SetSpyIP(new IPEndPoint(spyIP, 0));
        }

        public void SetSpyIP(IPEndPoint spyIPE)
        {
            this.m_SpySocket.Bind(spyIPE);
            this.m_Binded = true;
        }

        public void SetSpyIP(string spyIP)
        {
            this.SetSpyIP(IPAddress.Parse(spyIP));
        }

        public void Start()
        {
            if (this.m_Binded)
            {
                this.m_Thread = Platform.Threading.ThreadPool.Run(this.m_ThreadHandler);
            }
        }
    }
}
