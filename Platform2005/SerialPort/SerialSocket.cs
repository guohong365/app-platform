namespace Platform.SerialPort
{
    using System;
    using System.Collections;

    public class SerialSocket
    {
        private bool m_IsListener;
        private int m_Port;
        private static int m_PortMaker = 0;
        private int m_PortNumber;
        private static ArrayList m_Ports = new ArrayList();

        public SerialSocket() : this(-1, -1)
        {
        }

        public SerialSocket(int portNumber) : this(portNumber, -1)
        {
        }

        public SerialSocket(int portNumber, int port)
        {
            this.m_IsListener = false;
            this.m_PortNumber = portNumber;
            if (port >= 0)
            {
                lock (m_Ports.SyncRoot)
                {
                    if (m_Ports.Contains(port))
                    {
                       throw new Exception("端口已被使用！");
                    }
                    m_Ports.Add(port);
                    this.m_Port = port;
                    return;
                }
            }
            this.m_Port = this.GetPort();
        }

        public void Close()
        {
            lock (m_Ports.SyncRoot)
            {
                SerialPortController.RemoveSocket(this);
                m_Ports.Remove(this.m_Port);
            }
        }

        public void Connect(int portNumber, int port)
        {
        }

        private int GetPort()
        {
            lock (m_Ports.SyncRoot)
            {
                int portMaker = m_PortMaker;
                while (m_Ports.Contains(m_PortMaker))
                {
                    if (m_PortMaker == 0x7fffffff)
                    {
                        m_PortMaker = 0;
                    }
                    else
                    {
                        m_PortMaker++;
                    }
                    if (m_PortMaker == portMaker)
                    {
                        throw new Exception("无可用的端口！");
                    }
                }
                m_Ports.Add(m_PortMaker);
                return m_PortMaker;
            }
        }

        public void Listen()
        {
            SerialPortController.AddListener(this);
            this.m_IsListener = true;
        }

        public int Receive(byte[] buffer, int offset, int count)
        {
            if (!this.m_IsListener)
            {
                return -1;
            }
            return SerialPortController.Receive(this.m_PortNumber, this.m_Port, buffer, offset, count);
        }

        public int Send(byte[] buffer, int offset, int count)
        {
            if (!this.m_IsListener)
            {
                return -1;
            }
            return SerialPortController.Send(this.m_PortNumber, this.m_Port, buffer, offset, count);
        }

        public int Port
        {
            get
            {
                return this.m_Port;
            }
        }

        public int PortNumber
        {
            get
            {
                return this.m_PortNumber;
            }
        }
    }
}
