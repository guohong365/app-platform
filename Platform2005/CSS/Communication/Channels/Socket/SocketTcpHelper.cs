namespace Platform.CSS.Communication.Channels.Socket
{
    using Platform;
    using Platform.CSS;
    using Platform.Utils;
    using System;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class SocketTcpHelper
    {
        private static byte[] m_CheckBuffer = new byte[1];

        [DllImport("ws2_32.dll")]
        internal static extern int ioctlsocket(IntPtr socketHandle, uint cmd, ref long argp);
        internal static unsafe bool ReceiveData(Socket socket, byte[] buffer, int index, int count, out int readed, int timeout)
        {
            readed = 0;
            int num = 0;
            if (CSSConfig.SocketTcpUseAPI)
            {
                try
                {
                    if (timeout < 1)
                    {
                        do
                        {
                            if (Select(socket, SelectMode.SelectRead, (long) 0))
                            {
                                fixed (byte* numRef = &(buffer[index + readed]))
                                {
                                    num = recv(socket.Handle, numRef, count - readed, SocketFlags.None);
                                }
                                if (num <= 0)
                                {
                                    break;
                                }
                                readed += num;
                            }
                            else
                            {
                                Thread.Sleep(1);
                            }
                        }
                        while ((readed < count) && PlatformConfig.AppRuning);
                    }
                    else
                    {
                        PlatformTimeout timeout2 = new PlatformTimeout(timeout * 0x2710);
                        do
                        {
                            if (Select(socket, SelectMode.SelectRead, (long) 0))
                            {
                                fixed (byte* numRef2 = &(buffer[index + readed]))
                                {
                                    num = recv(socket.Handle, numRef2, count - readed, SocketFlags.None);
                                }
                                if (num <= 0)
                                {
                                    break;
                                }
                                timeout2.Reset();
                                readed += num;
                            }
                            else
                            {
                                if (timeout2.IsTimeout)
                                {
                                    break;
                                }
                                Thread.Sleep(1);
                            }
                        }
                        while ((readed < count) && PlatformConfig.AppRuning);
                    }
                    if (readed < count)
                    {
                        return false;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            try
            {
                if (timeout < 1)
                {
                    do
                    {
                        if (socket.Poll(0, SelectMode.SelectRead))
                        {
                            num = socket.Receive(buffer, index + readed, count - readed, SocketFlags.None);
                            if (num <= 0)
                            {
                                break;
                            }
                            readed += num;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    while ((readed < count) && PlatformConfig.AppRuning);
                }
                else
                {
                    PlatformTimeout timeout3 = new PlatformTimeout(timeout * 0x2710);
                    do
                    {
                        if (socket.Poll(0, SelectMode.SelectRead))
                        {
                            num = socket.Receive(buffer, index + readed, count - readed, SocketFlags.None);
                            if (num <= 0)
                            {
                                break;
                            }
                            timeout3.Reset();
                            readed += num;
                        }
                        else
                        {
                            if (timeout3.IsTimeout)
                            {
                                break;
                            }
                            Thread.Sleep(1);
                        }
                    }
                    while ((readed < count) && PlatformConfig.AppRuning);
                }
                if (readed < count)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("ws2_32.dll")]
        internal static extern unsafe int recv(IntPtr socketHandle, byte* buffer, int len, SocketFlags socketFlags);
        [DllImport("ws2_32.dll")]
        internal static extern int select([In] int nfds, [In, Out] ref FileDescriptorSet readfds, [In] IntPtr writefds, [In] IntPtr exceptfds, [In] IntPtr timeout);
        [DllImport("ws2_32.dll")]
        internal static extern int select([In] int nfds, [In] IntPtr readfds, [In, Out] ref FileDescriptorSet writefds, [In] IntPtr exceptfds, [In] ref TimeValue timeout);
        [DllImport("ws2_32.dll")]
        internal static extern int select([In] int nfds, [In] IntPtr readfds, [In, Out] ref FileDescriptorSet writefds, [In] IntPtr exceptfds, [In] IntPtr timeout);
        [DllImport("ws2_32.dll")]
        internal static extern int select([In] int nfds, [In] IntPtr readfds, [In] IntPtr writefds, [In, Out] ref FileDescriptorSet exceptfds, [In] ref TimeValue timeout);
        [DllImport("ws2_32.dll")]
        internal static extern int select([In] int nfds, [In] IntPtr readfds, [In] IntPtr writefds, [In, Out] ref FileDescriptorSet exceptfds, [In] IntPtr timeout);
        [DllImport("ws2_32.dll")]
        internal static extern int select([In] int nfds, [In, Out] ref FileDescriptorSet readfds, [In] IntPtr writefds, [In] IntPtr exceptfds, [In] ref TimeValue timeout);
        internal static bool Select(Socket socket, SelectMode mode, long microSeconds)
        {
            FileDescriptorSet readfds = new FileDescriptorSet(1);
            readfds.Array[0] = socket.Handle;
            TimeValue timeout = new TimeValue(microSeconds);
            int num = -1;
            switch (mode)
            {
                case SelectMode.SelectRead:
                    num = (microSeconds >= 0) ? select(0, ref readfds, IntPtr.Zero, IntPtr.Zero, ref timeout) : select(0, ref readfds, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    break;

                case SelectMode.SelectWrite:
                    num = (microSeconds >= 0) ? select(0, IntPtr.Zero, ref readfds, IntPtr.Zero, ref timeout) : select(0, IntPtr.Zero, ref readfds, IntPtr.Zero, IntPtr.Zero);
                    break;

                case SelectMode.SelectError:
                    num = (microSeconds >= 0) ? select(0, IntPtr.Zero, IntPtr.Zero, ref readfds, ref timeout) : select(0, IntPtr.Zero, IntPtr.Zero, ref readfds, IntPtr.Zero);
                    break;
            }
            return (num > 0);
        }

        [DllImport("ws2_32.dll")]
        internal static extern unsafe int send(IntPtr socketHandle, byte* buffer, int len, SocketFlags socketFlags);
        internal static unsafe int SendData(Socket socket, byte[] buffer, int offset, int count, SocketFlags flag)
        {
            if (CSSConfig.SocketTcpUseAPI)
            {
                fixed (byte* numRef = &(buffer[offset]))
                {
                    return send(socket.Handle, numRef, count, flag);
                }
            }
            return socket.Send(buffer, offset, count, flag);
        }

        internal static void SetSocketBlocking(IntPtr handle, bool flag)
        {
            long argp = flag ? ((long) 0) : ((long) (-1));
            ioctlsocket(handle, 0x8004667e, ref argp);
        }

        internal static void SetSocketBuffer(Socket socket, int bufferLen)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, bufferLen);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, bufferLen);
        }

        internal static void SetSocketTimeout(Socket socket, int timeout)
        {
            if (timeout < 1)
            {
                timeout = 0;
            }
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FileDescriptorSet
        {
            public const int MaxCount = 0x40;
            public int Count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x40)]
            public IntPtr[] Array;
            public static readonly int Size;
            public static readonly SocketTcpHelper.FileDescriptorSet Empty;
            public FileDescriptorSet(int count)
            {
                this.Count = count;
                this.Array = (count == 0) ? null : new IntPtr[0x40];
            }

            static FileDescriptorSet()
            {
                Size = Marshal.SizeOf(typeof(SocketTcpHelper.FileDescriptorSet));
                Empty = new SocketTcpHelper.FileDescriptorSet(0);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TimeValue
        {
            public int Seconds;
            public int Microseconds;
            public TimeValue(long microSeconds)
            {
                this.Seconds = (int) (microSeconds / ((long) 0xf4240));
                this.Microseconds = (int) (microSeconds % ((long) 0xf4240));
            }
        }
    }
}
