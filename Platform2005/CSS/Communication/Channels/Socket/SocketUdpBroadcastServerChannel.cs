namespace Platform.CSS.Communication.Channels.Socket
{
    using Platform;
    using Platform.CSS;
    using Platform.CSS.Communication;
    using Platform.CSS.Communication.Channels;
    using Platform.ExceptionHandling;
    using Platform.IO;
    using Platform.Net;
    using Platform.Threading;
    using Platform.Tracing;
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    [Channel("BroadcastSocketUdp")]
    public sealed class SocketUdpBroadcastServerChannel : IServerChannel
    {
        private static readonly IPEndPoint IPEAny = new IPEndPoint(IPAddress.Any, 0);
        private static readonly IPEndPoint IPV6EAny = new IPEndPoint(IPAddress.IPv6Any, 0);
        private ArrayList m_Clients = new ArrayList();
        private WaitCallback m_HandleThread;
        private IPEndPoint[] m_IPEs;
        private WaitCallback m_ReceiveThread;
        private string m_SettingName;

        public SocketUdpBroadcastServerChannel()
        {
            this.m_ReceiveThread = new WaitCallback(this.ReceiveThread);
            this.m_HandleThread = new WaitCallback(this.HandleThread);
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

        private void HandleThread(object state)
        {
            BroadcastPacketParameter parameter = state as BroadcastPacketParameter;
            if (parameter != null)
            {
                MemoryStream msrc = null;
                try
                {
                    bool flag;
                    ServerCommunication.HandlePacket(this.m_SettingName, parameter.MS, out msrc, out flag, parameter.IPE);
                }
                catch (Exception exception)
                {
                    Platform.ExceptionHandling.ExceptionHelper.HandleException(exception);
                }
            }
        }

        public bool InitializeChannel(string settingName, IPEndPoint[] ipes)
        {
            this.m_SettingName = settingName;
            this.m_IPEs = ipes;
            return true;
        }

        public void ReceiveThread(object state)
        {
            Socket socket = state as Socket;
            if (socket != null)
            {
                byte[] buffer = new byte[CSSConfig.CommunicationUdpBufferLength];
                try
                {
                    while (PlatformConfig.AppRuning)
                    {
                        EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        int count = socket.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP);
                        if (count >= 1)
                        {
                            if (CSSConfig.IsBroadcastRefusedIP(((IPEndPoint) remoteEP).Address))
                            {
                                try
                                {
                                    TraceHelper.WriteLine("¾Ü¾ø¹ã²¥£º" + remoteEP);
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                if (!CSSConfig.CommunicationServerBroadcastLocalIPEnable && IPUtility.IsLocalIPAddress(((IPEndPoint) remoteEP).Address))
                                {
                                    try
                                    {
                                        TraceHelper.WriteLine("¾Ü¾ø¹ã²¥£º" + remoteEP);
                                    }
                                    catch
                                    {
                                    }
                                    continue;
                                }
                                MemoryStream ms = new MemoryStream();
                                MemoryStream msrc = null;
                                ms.Write(buffer, 0, count);
                                if (CSSConfig.BroadcastServerMultiThreadHandling)
                                {
                                    BroadcastPacketParameter stateObject = new BroadcastPacketParameter(ms, (IPEndPoint) remoteEP);
                                    Platform.Threading.ThreadPool.Run(CSSConfig.CommunicationBroadcastServerThreadPriority, this.m_HandleThread, stateObject);
                                }
                                else
                                {
                                    try
                                    {
                                        bool flag;
                                        ServerCommunication.HandlePacket(this.m_SettingName, ms, out msrc, out flag, (IPEndPoint) remoteEP);
                                    }
                                    catch (Exception exception)
                                    {
                                        Platform.ExceptionHandling.ExceptionHelper.HandleException(exception);
                                    }
                                    msrc = null;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public bool StartChannel()
        {
            this.StopChannel();
            this.m_Clients.Clear();
            for (int i = 0; i < this.m_IPEs.Length; i++)
            {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.Bind(this.m_IPEs[i]);
                    this.m_Clients.Add(socket);
                }
                catch
                {
                }
            }
            for (int j = 0; j < this.m_Clients.Count; j++)
            {
                Platform.Threading.ThreadPool.Run(CSSConfig.CommunicationBroadcastServerThreadPriority, this.m_ReceiveThread, this.m_Clients[j]);
            }
            return true;
        }

        public bool StopChannel()
        {
            this.CloseAllClients();
            return true;
        }

        public bool StopClients()
        {
            return true;
        }

        private class BroadcastPacketParameter
        {
            public IPEndPoint IPE;
            public MemoryStream MS;

            public BroadcastPacketParameter(MemoryStream ms, IPEndPoint ipe)
            {
                this.MS = ms;
                this.IPE = ipe;
            }
        }
    }
}
