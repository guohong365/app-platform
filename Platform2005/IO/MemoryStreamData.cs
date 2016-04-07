namespace Platform.IO
{
    using System;
    using System.Collections;

    [Serializable]
    public class MemoryStreamData
    {
        private int m_BufferLength;
        private ArrayList m_Buffers;
        private int m_CurrentBufferIndex;
        private int m_CurrentBufferOffset;
        private long m_Length;
        private long m_Position;

        public MemoryStreamData(MemoryStream stream)
        {
            this.m_Position = stream.Position;
            this.m_Length = stream.Length;
            this.m_Buffers = stream.Buffers;
            this.m_BufferLength = stream.BufferLength;
            this.m_CurrentBufferIndex = stream.BufferIndex;
            this.m_CurrentBufferOffset = stream.BufferOffset;
        }

        internal int BufferIndex
        {
            get
            {
                return this.m_CurrentBufferIndex;
            }
        }

        internal int BufferLength
        {
            get
            {
                return this.m_BufferLength;
            }
        }

        internal int BufferOffset
        {
            get
            {
                return this.m_CurrentBufferOffset;
            }
        }

        internal ArrayList Buffers
        {
            get
            {
                return this.m_Buffers;
            }
        }

        internal long Length
        {
            get
            {
                return this.m_Length;
            }
        }

        internal long Position
        {
            get
            {
                return this.m_Position;
            }
        }
    }
}
