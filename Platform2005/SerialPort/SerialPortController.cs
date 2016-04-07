namespace Platform.SerialPort
{
    using System;
    using System.Collections;

    public sealed class SerialPortController
    {
        private static Hashtable m_Listeners = new Hashtable();
        private static Hashtable m_Sockets = new Hashtable();

        internal static void AddListener(SerialSocket socket)
        {
            lock (m_Listeners.SyncRoot)
            {
                m_Listeners.Remove(socket.PortNumber + "," + socket.PortNumber);
            }
        }

        internal static void AddSocket(SerialSocket socket)
        {
        }

        internal static int Connect(int portNumber, int port)
        {
            return -1;
        }

        private static void OnReceive(int portNumber, int cmd, byte[] buffer, int offest, int count)
        {
        }

        internal static int Receive(int portNumber, int port, byte[] buffer, int offest, int count)
        {
            return 0;
        }

        internal static void RemoveSocket(SerialSocket socket)
        {
        }

        internal static int Send(int portNumber, int port, byte[] buffer, int offest, int count)
        {
            return 0;
        }
    }
}
