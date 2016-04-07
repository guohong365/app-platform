namespace Platform.CSS.Communication.Channels
{
    using System;
    using System.Net;

    public interface IServerChannel
    {
        bool InitializeChannel(string settingName, IPEndPoint[] ipes);
        bool StartChannel();
        bool StopChannel();
        bool StopClients();
    }
}
