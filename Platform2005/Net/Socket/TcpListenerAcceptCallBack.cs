namespace Platform.Net.Socket
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    public delegate void TcpListenerAcceptCallBack(Socket clientSocket);
}
