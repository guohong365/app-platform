namespace Platform.CSS.Communication.Packet
{
    using Platform.CSS.SerialSink;
    using System;
    using System.Collections;

    internal sealed class PacketSerialSinkInfo
    {
        public ArrayList DeserialSinkTable = new ArrayList();
        public int HeaderLen = 0x1a;
        public ArrayList SerialSinkTable = new ArrayList();

        public void RegisterSerialSinkType(Type type, int headerLength)
        {
            if (type.GetInterface("Platform.CSS.SerialSink.ISerialSink", false) != null)
            {
                ISerialSink sink = Activator.CreateInstance(type) as ISerialSink;
                SerialSinkInfo info = new SerialSinkInfo(sink, headerLength, this.HeaderLen);
                this.SerialSinkTable.Add(info);
                this.DeserialSinkTable.Insert(0, info);
                this.HeaderLen += headerLength;
            }
        }
    }
}
