namespace Platform.Compress
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void ZipEventHandler(string filename, long compressedSize, long size, bool zipFlag);
}
