namespace Platform.CSS.SerialSink
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class SecuritySerialSinkException : Exception
    {
        public SecuritySerialSinkException()
        {
        }

        public SecuritySerialSinkException(string message) : base(message)
        {
        }

        public SecuritySerialSinkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
