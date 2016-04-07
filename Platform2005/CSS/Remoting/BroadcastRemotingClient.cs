namespace Platform.CSS.Remoting
{
    using Platform.CSS.Communication;
    using System;

    public sealed class BroadcastRemotingClient
    {
        public static void RemoteExecute(string fullName, object[] parameters)
        {
            BroadcastRemotingPacket packet = new BroadcastRemotingPacket();
            packet.FullName = fullName;
            packet.Parameters = parameters;
            ClientCommunication.BroadcastPacket(packet, false);
        }
    }
}
