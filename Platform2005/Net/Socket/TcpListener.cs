namespace Platform.Net.Socket
{
    using Platform;
    using Platform.ExceptionHandling;
    using Platform.Threading;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public sealed class TcpListener
    {
        private TcpListenerAcceptCallBack m_AcceptCallBack;
        private WaitCallback m_AcceptHandler = new WaitCallback(Platform.Net.Socket.TcpListener.AcceptThread);
        private IPEndPoint m_IP;
        private Socket m_Listener;

        public TcpListener(IPEndPoint ipe, TcpListenerAcceptCallBack acceptCallBack)
        {
            this.m_AcceptCallBack = acceptCallBack;
            this.m_IP = ipe;
        }

        private static void AcceptThread(object state)
        {
            Platform.Net.Socket.TcpListener listener = state as Platform.Net.Socket.TcpListener;
            if (listener != null)
            {
                try
                {
                    while (PlatformConfig.AppRuning)
                    {
                        Socket socket = listener.m_Listener.Accept();
                        if (socket != null)
                        {
                            if (listener.m_AcceptCallBack == null)
                            {
                                CloseSocket(socket);
                            }
                            try
                            {
                                listener.m_AcceptCallBack(socket);
                                continue;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private static void CloseSocket(Socket socket)
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

        public bool Start(int count)
        {
            this.Stop();
            try
            {
                this.m_Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.m_Listener.Bind(this.m_IP);
                this.m_Listener.Listen(count);
                Platform.Threading.ThreadPool.Run(this.m_AcceptHandler, this);
                return true;
            }
            catch (Exception exception)
            {
                this.Stop();
                Platform.ExceptionHandling.ExceptionHelper.HandleException(exception);
                return false;
            }
        }

        public void Stop()
        {
            CloseSocket(this.m_Listener);
            this.m_Listener = null;
        }
    }
}
