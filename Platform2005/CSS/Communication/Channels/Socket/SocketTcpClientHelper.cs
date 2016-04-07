namespace Platform.CSS.Communication.Channels.Socket
{
    using Platform;
    using Platform.IO;
    using Platform.Tracing;
    using System;
    using System.Collections;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;

    internal sealed class SocketTcpClientHelper : SocketTcpHelper
    {
        public static bool ReceiveStream(Socket socket, Platform.IO.MemoryStream msrc, int offset, out int readed, out int breakPoint, int maxCount, int timeout)
        {
            int num;
            SocketTcpHelper.SetSocketTimeout(socket, timeout);
            readed = 0;
            breakPoint = 0;
            byte[] buffer = new byte[8];
            if (!SocketTcpHelper.ReceiveData(socket, buffer, 0, 8, out num, timeout) || (num != 8))
            {
                return false;
            }
            num = BitConverter.ToInt32(buffer, 0);
            breakPoint = BitConverter.ToInt32(buffer, 4);
            if (((num <= 0) || ((maxCount > 0) && (num > maxCount))) || ((breakPoint < 0) || (breakPoint > offset)))
            {
                return false;
            }
            msrc.Seek((long) breakPoint, SeekOrigin.Begin);
            int bufferLength = msrc.BufferLength;
            int bufferIndex = msrc.BufferIndex;
            int bufferOffset = msrc.BufferOffset;
            ArrayList buffers = msrc.Buffers;
            byte[] buffer2 = (byte[]) buffers[bufferIndex];
        Label_0096:
            if (bufferOffset >= bufferLength)
            {
                bufferOffset -= bufferLength;
                bufferIndex++;
                if (bufferIndex >= buffers.Count)
                {
                    buffer2 = new byte[bufferLength];
                    buffers.Add(buffer2);
                }
                else
                {
                    buffer2 = (byte[]) buffers[bufferIndex];
                }
            }
            int count = bufferLength - bufferOffset;
            if (count > num)
            {
                count = num;
            }
            if (SocketTcpHelper.ReceiveData(socket, buffer2, bufferOffset, count, out count, timeout) && (count > 0))
            {
                bufferOffset += count;
                readed += count;
                num -= count;
                if ((num > 0) && PlatformConfig.AppRuning)
                {
                    goto Label_0096;
                }
            }
            msrc.SetLength((long) (offset + readed));
            if (num > 0)
            {
                return false;
            }
            return true;
        }

        public static bool SendStream(Socket socket, Platform.IO.MemoryStream ms, int offset, int count, int breakPoint, int timeout)
        {
            if (count < (ms.Length - offset))
            {
                return false;
            }
            SocketTcpHelper.SetSocketTimeout(socket, timeout);
            TraceHelper.WriteLine("发送数据：" + (count + 8) + " 字节");
            byte[] dst = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(count), 0, dst, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(breakPoint), 0, dst, 4, 4);
            if (SocketTcpHelper.SendData(socket, dst, 0, dst.Length, SocketFlags.None) != 8)
            {
                return false;
            }
            ArrayList buffers = ms.Buffers;
            int bufferLength = ms.BufferLength;
            ms.Seek((long) offset, SeekOrigin.Begin);
            int num3 = (int) (ms.Position / ((long) bufferLength));
            int num4 = (int) (ms.Position % ((long) bufferLength));
            int num5 = (int) ((ms.Position + count) / ((long) bufferLength));
            int num6 = (int) ((ms.Position + count) % ((long) bufferLength));
            if (num3 == num5)
            {
                if ((num6 > num4) && (SocketTcpHelper.SendData(socket, (byte[]) buffers[num5], num4, num6 - num4, SocketFlags.None) != (num6 - num4)))
                {
                    return false;
                }
                return true;
            }
            if (SocketTcpHelper.SendData(socket, (byte[]) buffers[num3], num4, bufferLength - num4, SocketFlags.None) != (bufferLength - num4))
            {
                return false;
            }
            for (int i = num3 + 1; i < num5; i++)
            {
                if (SocketTcpHelper.SendData(socket, (byte[]) buffers[i], 0, bufferLength, SocketFlags.None) != bufferLength)
                {
                    return false;
                }
            }
            if ((num6 > 0) && (SocketTcpHelper.SendData(socket, (byte[]) buffers[num5], 0, num6, SocketFlags.None) != num6))
            {
                return false;
            }
            return true;
        }
    }
}
