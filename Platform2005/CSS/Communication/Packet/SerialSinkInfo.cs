namespace Platform.CSS.Communication.Packet
{
    using Platform.CSS.SerialSink;
    using System;

    internal sealed class SerialSinkInfo
    {
        public int HeaderLength;
        public int HeaderOffset;
        public ISerialSink Sink;

        public SerialSinkInfo(ISerialSink sink, int headerLength, int currentHeaderOffset)
        {
            this.Sink = sink;
            this.HeaderLength = headerLength;
            this.HeaderOffset = currentHeaderOffset;
        }
    }
}
