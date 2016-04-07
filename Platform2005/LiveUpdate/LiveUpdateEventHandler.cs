namespace Platform.LiveUpdate
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void LiveUpdateEventHandler(int step, int errorCode, int count, int position, string message);
}
