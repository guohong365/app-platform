namespace Platform.LiveUpdate
{
    using Platform.Net;
    using System;
    using System.Runtime.CompilerServices;

    public delegate bool LiveUpdateDownloadHandler(int step, HttpInformation information);
}
