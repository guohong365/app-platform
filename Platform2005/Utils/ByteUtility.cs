namespace Platform.Utils
{
    using Platform;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;

    public sealed class ByteUtility
    {
        public static void BlockCopy(byte[] source, int soffset, byte[] dest, int doffset, int count)
        {
            if (count < 9)
            {
                while (--count >= 0)
                {
                    dest[doffset + count] = source[soffset + count];
                }
            }
            else
            {
                Buffer.BlockCopy(source, soffset, dest, doffset, count);
            }
        }

        public static bool[] ByteArrayToBoolArray(byte[] array)
        {
            if (array == null)
            {
                return null;
            }
            bool[] flagArray = new bool[array.Length << 3];
            int index = 0;
            int num2 = 0;
            while (index < array.Length)
            {
                int num3 = 0;
                while (num3 < 8)
                {
                    flagArray[num2] = (array[index] & (((int) 1) << num3)) != 0;
                    num3++;
                    num2++;
                }
                index++;
            }
            return flagArray;
        }

        public static byte[][] ByteArrayToBytesArray(byte[] input)
        {
            int length = input.Length;
            if (length < 8)
            {
                return null;
            }
            int startIndex = 0;
            int num3 = BitConverter.ToInt32(input, startIndex);
            if (num3 > length)
            {
                return null;
            }
            startIndex += 4;
            int num4 = BitConverter.ToInt32(input, startIndex);
            startIndex += 4;
            if ((num4 < 0) || (num3 < 8))
            {
                return null;
            }
            ArrayList list = new ArrayList();
            int count = 0;
            for (int i = 0; i < num4; i++)
            {
                if ((startIndex + 4) > length)
                {
                    return null;
                }
                count = BitConverter.ToInt32(input, startIndex);
                startIndex += 4;
                if ((startIndex + count) > length)
                {
                    return null;
                }
                byte[] dst = new byte[count];
                Buffer.BlockCopy(input, startIndex, dst, 0, count);
                startIndex += count;
                list.Add(dst);
            }
            return (list.ToArray(typeof(byte[])) as byte[][]);
        }

        public static int[] ByteArrayToInt32Array(byte[] input)
        {
            int length = input.Length;
            if (length < 4)
            {
                return null;
            }
            int startIndex = 0;
            int num3 = BitConverter.ToInt32(input, startIndex);
            if (((num3 * 4) + 4) > length)
            {
                return null;
            }
            startIndex += 4;
            int[] numArray = new int[num3];
            for (int i = 0; i < num3; i++)
            {
                numArray[i] = BitConverter.ToInt32(input, startIndex);
                startIndex += 4;
            }
            return numArray;
        }

        public static string[] ByteArrayToStringArray(byte[] input)
        {
            int length = input.Length;
            if (length < 8)
            {
                return null;
            }
            int startIndex = 0;
            int num3 = BitConverter.ToInt32(input, startIndex);
            if (num3 > length)
            {
                return null;
            }
            startIndex += 4;
            int num4 = BitConverter.ToInt32(input, startIndex);
            startIndex += 4;
            if ((num4 < 0) || (num3 < 8))
            {
                return null;
            }
            ArrayList list = new ArrayList();
            int count = 0;
            for (int i = 0; i < num4; i++)
            {
                if ((startIndex + 4) > length)
                {
                    return null;
                }
                count = BitConverter.ToInt32(input, startIndex);
                startIndex += 4;
                if ((startIndex + count) > length)
                {
                    return null;
                }
                string text = PlatformConfig.TextEncoding.GetString(input, startIndex, count);
                startIndex += count;
                list.Add(text);
            }
            return (list.ToArray(typeof(string)) as string[]);
        }

        public static uint[] ByteArrayToUInt32Array(byte[] input)
        {
            int length = input.Length;
            if (length < 4)
            {
                return null;
            }
            int startIndex = 0;
            int num3 = BitConverter.ToInt32(input, startIndex);
            if (((num3 * 4) + 4) > length)
            {
                return null;
            }
            startIndex += 4;
            uint[] numArray = new uint[num3];
            for (int i = 0; i < num3; i++)
            {
                numArray[i] = BitConverter.ToUInt32(input, startIndex);
                startIndex += 4;
            }
            return numArray;
        }

        public static byte[] BytesArrayToByteArray(byte[][] input)
        {
            MemoryStream stream = new MemoryStream();
            int length = input.Length;
            byte[] bytes = BitConverter.GetBytes(length);
            stream.Write(bytes, 0, 4);
            stream.Write(bytes, 0, 4);
            int num2 = 8;
            for (int i = 0; i < length; i++)
            {
                byte[] buffer = input[i];
                bytes = BitConverter.GetBytes(buffer.Length);
                stream.Write(bytes, 0, 4);
                stream.Write(buffer, 0, buffer.Length);
                num2 += buffer.Length + 4;
            }
            stream.Seek((long) 0, SeekOrigin.Begin);
            bytes = BitConverter.GetBytes(num2);
            stream.Write(bytes, 0, 4);
            return stream.ToArray();
        }

        public static string BytesToHexString(byte[] input)
        {
            return BytesToHexString(input, "");
        }

        public static string BytesToHexString(byte[] input, bool isUpperCase)
        {
            return BytesToHexString(input, "", isUpperCase);
        }

        public static string BytesToHexString(byte[] input, string front)
        {
            return BytesToHexString(input, front, "");
        }

        public static string BytesToHexString(byte[] input, string front, bool isUpperCase)
        {
            return BytesToHexString(input, 0, input.Length, front, "", isUpperCase);
        }

        public static string BytesToHexString(byte[] input, string front, string after)
        {
            return BytesToHexString(input, 0, input.Length, front, after);
        }

        public static string BytesToHexString(byte[] input, int offset, int len, string front, string after)
        {
            return BytesToHexString(input, 0, input.Length, front, after, true);
        }

        public static string BytesToHexString(byte[] input, int offset, int len, string front, string after, bool isUpperCase)
        {
            string format = isUpperCase ? "X2" : "x2";
            string text2 = "";
            int num = offset + len;
            for (int i = offset; i < num; i++)
            {
                if (front != null)
                {
                    text2 = text2 + front;
                }
                text2 = text2 + input[i].ToString(format);
                if ((after != null) && (i != (num - 1)))
                {
                    text2 = text2 + after;
                }
            }
            return text2;
        }

        public static byte[] CopyByteArray(byte[] array)
        {
            if (array == null)
            {
                return null;
            }
            byte[] dst = new byte[array.Length];
            Buffer.BlockCopy(array, 0, dst, 0, array.Length);
            return dst;
        }

        public static bool GetBitValue(byte[] input, int index)
        {
            int num = index >> 3;
            int num2 = index & 7;
            if (input.Length <= num)
            {
                return false;
            }
            return ((input[num] & (((int) 1) << num2)) != 0);
        }

        public static byte[] HexStringToBytes(string input)
        {
            if ((input.Length % 2) != 0)
            {
                return null;
            }
            try
            {
                byte[] buffer = new byte[input.Length / 2];
                for (int i = 0; i < input.Length; i += 2)
                {
                    string s = input.Substring(i, 2);
                    buffer[i] = byte.Parse(s, NumberStyles.HexNumber);
                }
                return buffer;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] HexStringToBytes(string input, char splitChar)
        {
            string[] textArray = input.ToUpper().Replace("0X", "").Replace(" ", "").Split(new char[] { splitChar });
            byte[] buffer = new byte[textArray.Length];
            for (int i = 0; i < textArray.Length; i++)
            {
                buffer[i] = byte.Parse(textArray[i], NumberStyles.HexNumber);
            }
            return buffer;
        }

        public static byte[] Int32ArrayToByteArray(int[] input)
        {
            int length = input.Length;
            byte[] dst = new byte[(length * 4) + 4];
            byte[] bytes = BitConverter.GetBytes(length);
            int dstOffset = 0;
            Buffer.BlockCopy(bytes, 0, dst, dstOffset, 4);
            dstOffset += 4;
            for (int i = 0; i < length; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, dstOffset, 4);
                dstOffset += 4;
            }
            return dst;
        }

        public static void SetBitValue(byte[] input, int index, bool val)
        {
            int num = index >> 3;
            int num2 = index & 7;
            if (input.Length > num)
            {
                if (val)
                {
                    input[num] = (byte) (input[num] | ((byte) (((int) 1) << num2)));
                }
                else
                {
                    input[num] = (byte) (input[num] & ((byte) ~(((int) 1) << num2)));
                }
            }
        }

        public static byte[] StringArrayToByteArray(string[] input)
        {
            MemoryStream stream = new MemoryStream();
            int length = input.Length;
            byte[] bytes = BitConverter.GetBytes(length);
            stream.Write(bytes, 0, 4);
            stream.Write(bytes, 0, 4);
            int num2 = 8;
            for (int i = 0; i < length; i++)
            {
                byte[] buffer = PlatformConfig.TextEncoding.GetBytes(input[i]);
                bytes = BitConverter.GetBytes(buffer.Length);
                stream.Write(bytes, 0, 4);
                stream.Write(buffer, 0, buffer.Length);
                num2 += buffer.Length + 4;
            }
            stream.Seek((long) 0, SeekOrigin.Begin);
            bytes = BitConverter.GetBytes(num2);
            stream.Write(bytes, 0, 4);
            return stream.ToArray();
        }

        public static byte[] UInt32ArrayToByteArray(uint[] input)
        {
            int length = input.Length;
            byte[] dst = new byte[(length * 4) + 4];
            byte[] bytes = BitConverter.GetBytes(length);
            int dstOffset = 0;
            Buffer.BlockCopy(bytes, 0, dst, dstOffset, 4);
            dstOffset += 4;
            for (int i = 0; i < length; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(input[i]), 0, dst, dstOffset, 4);
                dstOffset += 4;
            }
            return dst;
        }
    }
}
