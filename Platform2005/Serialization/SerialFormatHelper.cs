namespace Platform.Serialization
{
    using Platform.IO;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public sealed class SerialFormatHelper
    {
        public static object BinaryDeserial(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        public static object BinaryDeserial(byte[] input)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Platform.IO.MemoryStream serializationStream = new Platform.IO.MemoryStream(input);
            return formatter.Deserialize(serializationStream);
        }

        public static object BinaryDeserial(byte[] input, int index, int count)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Platform.IO.MemoryStream serializationStream = new Platform.IO.MemoryStream(input, index, count);
            return formatter.Deserialize(serializationStream);
        }

        public static byte[] BinarySerial(object input)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Platform.IO.MemoryStream serializationStream = new Platform.IO.MemoryStream();
            formatter.Serialize(serializationStream, input);
            return serializationStream.ToArray();
        }

        public static int BinarySerial(Stream stream, object input)
        {
            long position = stream.Position;
            new BinaryFormatter().Serialize(stream, input);
            return (int) (stream.Position - position);
        }
    }
}
