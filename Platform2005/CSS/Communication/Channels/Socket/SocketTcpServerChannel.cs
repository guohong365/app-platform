namespace Platform.CSS.Communication.Channels.Socket
{
    using Platform;
    using Platform.CSS;
    using Platform.CSS.Communication;
    using Platform.CSS.Communication.Channels;
    using Platform.IO;
    using Platform.Net.Socket;
    using Platform.Threading;
    using Platform.Tracing;
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    [Channel("SocketTcp")]
    public sealed class SocketTcpServerChannel : IServerChannel
    {
        private TcpListenerAcceptCallBack m_AcceptCallBack;
        private static ArrayList m_Channels = new ArrayList();
        private bool m_ChannelStoped;
        private ArrayList m_Clients = new ArrayList();
        private ArrayList m_Listener = new ArrayList();
        private WaitCallback m_ReceiveHandler;
        private string m_SettingName;

        static SocketTcpServerChannel()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(SocketTcpServerChannel.OnProcessExit);
        }

        public SocketTcpServerChannel()
        {
            this.m_ReceiveHandler = new WaitCallback(this.ReceiveThread);
            this.m_AcceptCallBack = new TcpListenerAcceptCallBack(this.OnAcceptCallBack);
            m_Channels.Add(this);
        }

        private void CloseAllClients()
        {
            lock (this.m_Clients.SyncRoot)
            {
                while (this.m_Clients.Count > 0)
                {
                    this.CloseSocket(this.m_Clients[0] as Socket);
                }
            }
        }

        private void CloseSocket(Socket socket)
        {
            if (socket != null)
            {
                lock (this.m_Clients.SyncRoot)
                {
                    try
                    {
                       TraceHelper.WriteLine("关闭客户端连接：" + socket.RemoteEndPoint);
                    }
                    catch
                    {
                    }
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }
                    try
                    {
                        socket.Close();
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (this.m_Clients.Contains(socket))
                        {
                            this.m_Clients.Remove(socket);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void CloseSocketUnPrompt(Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                try
                {
                    socket.Close();
                }
                catch
                {
                }
            }
        }

        public bool InitializeChannel(string settingName, IPEndPoint[] ipes)
        {
            this.StopListener();
            this.CloseAllClients();
            this.m_SettingName = settingName;
            this.m_Listener.Clear();
            foreach (IPEndPoint point in ipes)
            {
                this.m_Listener.Add(new Platform.Net.Socket.TcpListener(point, this.m_AcceptCallBack));
            }
            return true;
        }

        private void OnAcceptCallBack(Socket clientSocket)
        {
            if (clientSocket != null)
            {
                try
                {
                    if (!CSSConfig.IsRefusedIP(((IPEndPoint) clientSocket.RemoteEndPoint).Address))
                    {
                        if (!CSSConfig.SocketTcpCheckUseThread)
                        {
                            try
                            {
                                byte[] buffer = new byte[1];
                                SocketTcpHelper.SetSocketTimeout(clientSocket, CSSConfig.SocketTcpCheckTimeout * 0x3e8);
                                if ((clientSocket.Receive(buffer, 0, 1, SocketFlags.None) >= 1) && (buffer[0] == CSSConfig.SocketTcpCheckValue))
                                {
                                    goto Label_0096;
                                }
                                this.CloseSocketUnPrompt(clientSocket);
                            }
                            catch
                            {
                                this.CloseSocketUnPrompt(clientSocket);
                            }
                            return;
                        }
                        goto Label_0096;
                    }
                    try
                    {
                        TraceHelper.WriteLine("拒绝客户端连接：" + clientSocket.RemoteEndPoint);
                    }
                    catch
                    {
                    }
                    this.CloseSocketUnPrompt(clientSocket);
                }
                catch
                {
                }
            }
            return;
        Label_0096:
            Monitor.Enter(this.m_Clients.SyncRoot);
            try
            {
                this.m_Clients.Add(clientSocket);
            }
            finally
            {
                Monitor.Exit(this.m_Clients.SyncRoot);
            }
            if (Platform.Threading.ThreadPool.Run(this.m_ReceiveHandler, clientSocket) == null)
            {
                this.CloseSocket(clientSocket);
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            foreach (SocketTcpServerChannel channel in m_Channels)
            {
                channel.StopListener();
                channel.CloseAllClients();
            }
        }

        private void ReceiveThread(object stateObject)
        {
            Socket socket = stateObject as Socket;
            if (socket != null)
            {
                try
                {
                    IPEndPoint remoteEndPoint;
                    MemoryStream stream;
                    int num3;
                    if (!this.m_ChannelStoped)
                    {
                        int bufferLen = (CSSConfig.CommunicationBufferLength > CSSConfig.CommunicationTcpMaxBufferLimited) ? CSSConfig.CommunicationTcpMaxBufferLimited : CSSConfig.CommunicationBufferLength;
                        if (bufferLen < 0)
                        {
                            bufferLen = CSSConfig.CommunicationTcpMaxBufferLimited;
                        }
                        SocketTcpHelper.SetSocketBuffer(socket, bufferLen);
                        remoteEndPoint = socket.RemoteEndPoint as IPEndPoint;
                        if (!CSSConfig.SocketTcpCheckUseThread)
                        {
                            goto Label_015F;
                        }
                        byte[] buffer = new byte[1];
                        SocketTcpHelper.SetSocketTimeout(socket, CSSConfig.SocketTcpCheckTimeout * 0x3e8);
                        if ((socket.Receive(buffer, 0, 1, SocketFlags.None) >= 1) && (buffer[0] == CSSConfig.SocketTcpCheckValue))
                        {
                            goto Label_015F;
                        }
                    }
                    return;
                Label_0093:
                    stream = new MemoryStream();
                    MemoryStream msrc = null;
                    int breakPoint = 0;
                    if (!SocketTcpServerHelper.ReceiveStream(socket, stream, 0, out num3, out breakPoint, CSSConfig.CommunicationBufferLength, CSSConfig.ServerCommunicationTimeout * 0x3e8))
                    {
                        return;
                    }
                    Platform.Threading.ThreadPool.UpdateActiveTime();
                    long ticks = DateTime.Now.Ticks;
                    try
                    {
                        bool flag;
                        if (!ServerCommunication.HandlePacket(this.m_SettingName, stream, out msrc, out flag, remoteEndPoint) || (msrc == null))
                        {
                            return;
                        }
                        if (!flag)
                        {
                            breakPoint = 0;
                        }
                    }
                    finally
                    {
                        long num6 = DateTime.Now.Ticks;
                        TimeSpan span = new TimeSpan(num6 - ticks);
                        TraceHelper.WriteLine("服务器处理时间（不包含接收发送）：" + span.TotalSeconds);
                    }
                    if (!SocketTcpServerHelper.SendStream(socket, msrc, breakPoint, ((int) msrc.Length) - breakPoint, breakPoint, CSSConfig.ServerCommunicationTimeout * 0x3e8))
                    {
                        return;
                    }
                Label_015F:
                    if (!this.m_ChannelStoped && PlatformConfig.AppRuning)
                    {
                        goto Label_0093;
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    this.CloseSocket(socket);
                }
            }
        }

        public bool StartChannel()
        {
            this.StopChannel();
            this.StopClients();
            return this.StartListener();
        }

        public bool StartListener()
        {
            this.StopListener();
            this.m_ChannelStoped = false;
            lock (this.m_Listener.SyncRoot)
            {
                int num = 0;
                foreach (Platform.Net.Socket.TcpListener listener in this.m_Listener)
                {
                    num += listener.Start(0x400) ? 1 : 0;
                }
                return (num > 0);
            }
        }

        public bool StopChannel()
        {
            this.StopListener();
            return true;
        }

        public bool StopClients()
        {
            this.CloseAllClients();
            return true;
        }

        public void StopListener()
        {
            this.m_ChannelStoped = true;
            lock (this.m_Listener.SyncRoot)
            {
                foreach (Platform.Net.Socket.TcpListener listener in this.m_Listener)
                {
                    listener.Stop();
                }
            }
        }
    }
}
