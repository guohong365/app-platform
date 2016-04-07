namespace Platform.Serialization
{
    using System;

    public sealed class SerialDataAttribute : Attribute
    {
        private byte m_SerialNumber;

        public SerialDataAttribute(byte serial)
        {
            this.m_SerialNumber = serial;
        }

        public byte SerialNumber
        {
            get
            {
                return this.m_SerialNumber;
            }
        }
    }
}
