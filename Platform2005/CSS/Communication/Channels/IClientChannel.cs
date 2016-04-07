namespace Platform.CSS.Communication.Channels
{
    using Platform.IO;
    using System;
    using System.Net;

    public interface IClientChannel
    {
        bool CheckServerOnLine(string settingName);
        void CloseConnections(string settingName);
        bool InitializeChannel(string settingName, IPEndPoint[] ipes);
        bool Send(string settingName, Guid packetID, MemoryStream ms, MemoryStream msrc, ref int breadPoint, int timeOut);

        bool IsBroadcastChannel { get; }
    }
}
