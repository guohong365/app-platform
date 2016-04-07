namespace Platform.Net
{
    using System;
    using System.Net;

    public sealed class IPUtility
    {
        private static IPAddress[] m_LocalIPs = GetIPAddress(Dns.GetHostName());

        public static IPAddress[] GetIPAddress(string hostName)
        {
            return Dns.GetHostEntry(hostName).AddressList;
        }

        public static bool IsLocalIPAddress(IPAddress ip)
        {
            if (m_LocalIPs != null)
            {
                if (ip == IPAddress.Loopback)
                {
                    return true;
                }
                for (int i = 0; i < m_LocalIPs.Length; i++)
                {
                    if (m_LocalIPs[i] == ip)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
