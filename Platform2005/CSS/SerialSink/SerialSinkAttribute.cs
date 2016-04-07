namespace Platform.CSS.SerialSink
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class SerialSinkAttribute : Attribute
    {
        private int m_HeaderLength;

        public SerialSinkAttribute(int headerLength)
        {
            this.m_HeaderLength = headerLength;
        }

        public int HeaderLength
        {
            get
            {
                return this.m_HeaderLength;
            }
        }
    }
}
