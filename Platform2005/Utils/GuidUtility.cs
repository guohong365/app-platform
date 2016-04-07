namespace Platform.Utils
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class GuidUtility
    {
        public static Guid GetGuid(params byte[] input)
        {
            byte[] dst = new byte[0x10];
            int count = (input.Length > 0x10) ? 0x10 : input.Length;
            Buffer.BlockCopy(input, 0, dst, 0, count);
            return new Guid(dst);
        }

        public static Guid GetGuid(params short[] input)
        {
            byte[] dst = new byte[0x10];
            for (int i = 0; (i < input.Length) && (i < 8); i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, i * 2, 2);
            }
            return new Guid(dst);
        }

        public static Guid GetGuid(params int[] input)
        {
            byte[] dst = new byte[0x10];
            for (int i = 0; (i < input.Length) && (i < 4); i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, i * 4, 4);
            }
            return new Guid(dst);
        }

        public static Guid GetGuid(params long[] input)
        {
            byte[] dst = new byte[0x10];
            for (int i = 0; (i < input.Length) && (i < 2); i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, i * 8, 8);
            }
            return new Guid(dst);
        }

        public static Guid GetGuid(params sbyte[] input)
        {
            byte[] b = new byte[0x10];
            for (int i = 0; (i < input.Length) && (i < 0x10); i++)
            {
                b[i] = (byte) input[i];
            }
            return new Guid(b);
        }

        public static Guid GetGuid(params ushort[] input)
        {
            byte[] dst = new byte[0x10];
            for (int i = 0; (i < input.Length) && (i < 8); i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, i * 2, 2);
            }
            return new Guid(dst);
        }

        public static Guid GetGuid(params uint[] input)
        {
            byte[] dst = new byte[0x10];
            for (int i = 0; (i < input.Length) && (i < 4); i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, i * 4, 4);
            }
            return new Guid(dst);
        }

        public static Guid GetGuid(params ulong[] input)
        {
            byte[] dst = new byte[0x10];
            for (int i = 0; (i < input.Length) && (i < 2); i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, i * 8, 8);
            }
            return new Guid(dst);
        }

        public static byte[] ToByteArray(Guid guid)
        {
            return guid.ToByteArray();
        }

        public static short[] ToInt16Array(Guid guid)
        {
            short[] numArray = new short[8];
            byte[] buffer = guid.ToByteArray();
            for (int i = 0; i < 8; i++)
            {
                numArray[i] = BitConverter.ToInt16(buffer, i * 2);
            }
            return numArray;
        }

        public static int[] ToInt32Array(Guid guid)
        {
            int[] numArray = new int[4];
            byte[] buffer = guid.ToByteArray();
            for (int i = 0; i < 4; i++)
            {
                numArray[i] = BitConverter.ToInt32(buffer, i * 4);
            }
            return numArray;
        }

        public static long[] ToInt64Array(Guid guid)
        {
            long[] numArray = new long[2];
            byte[] buffer = guid.ToByteArray();
            for (int i = 0; i < 2; i++)
            {
                numArray[i] = BitConverter.ToInt64(buffer, i * 8);
            }
            return numArray;
        }

        public static sbyte[] ToSByteArray(Guid guid)
        {
            sbyte[] numArray = new sbyte[0x10];
            byte[] buffer = guid.ToByteArray();
            for (int i = 0; i < 0x10; i++)
            {
                numArray[i] = (sbyte) buffer[i];
            }
            return numArray;
        }

        public static ushort[] ToUInt16Array(Guid guid)
        {
            ushort[] numArray = new ushort[8];
            byte[] buffer = guid.ToByteArray();
            for (int i = 0; i < 8; i++)
            {
                numArray[i] = BitConverter.ToUInt16(buffer, i * 2);
            }
            return numArray;
        }

        public static uint[] ToUInt32Array(Guid guid)
        {
            uint[] numArray = new uint[4];
            byte[] buffer = guid.ToByteArray();
            for (int i = 0; i < 4; i++)
            {
                numArray[i] = BitConverter.ToUInt32(buffer, i * 4);
            }
            return numArray;
        }

        public static void ToUInt32Array(Guid guid, out uint p1, out uint p2)
        {
            byte[] buffer = guid.ToByteArray();
            p1 = BitConverter.ToUInt32(buffer, 0);
            p2 = BitConverter.ToUInt32(buffer, 4);
        }

        public static void ToUInt32Array(Guid guid, out uint p1, out uint p2, out uint p3, out uint p4)
        {
            byte[] buffer = guid.ToByteArray();
            p1 = BitConverter.ToUInt32(buffer, 0);
            p2 = BitConverter.ToUInt32(buffer, 4);
            p3 = BitConverter.ToUInt32(buffer, 8);
            p4 = BitConverter.ToUInt32(buffer, 12);
        }

        public static ulong[] ToUInt64Array(Guid guid)
        {
            ulong[] numArray = new ulong[2];
            byte[] buffer = guid.ToByteArray();
            for (int i = 0; i < 2; i++)
            {
                numArray[i] = BitConverter.ToUInt64(buffer, i * 8);
            }
            return numArray;
        }

        public static void ToUInt64Array(Guid guid, out ulong p1)
        {
            byte[] buffer = guid.ToByteArray();
            p1 = BitConverter.ToUInt64(buffer, 0);
        }

        public static void ToUInt64Array(Guid guid, out ulong p1, out ulong p2)
        {
            byte[] buffer = guid.ToByteArray();
            p1 = BitConverter.ToUInt64(buffer, 0);
            p2 = BitConverter.ToUInt64(buffer, 8);
        }
    }
}
