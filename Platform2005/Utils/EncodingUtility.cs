namespace Platform.Utils
{
    using System;
    using System.Text;

    public class EncodingUtility
    {
        private static Encoding gbkEncoding = Encoding.GetEncoding(0x3a8);

        public static bool IsInGB2312(char c)
        {
            byte[] bytes = gbkEncoding.GetBytes(new char[] { c });
            if (((bytes.Length == 2) && (0xb0 <= bytes[0])) && ((bytes[0] <= 0xf7) && (160 <= bytes[1])))
            {
                return (bytes[1] <= 0xfe);
            }
            return false;
        }

        public static bool IsInGB2312(string str)
        {
            if ((str == null) || (str == ""))
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (!IsInGB2312(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsInGBK(char c)
        {
            byte[] bytes = gbkEncoding.GetBytes(new char[] { c });
            if (((bytes.Length == 2) && (0x81 <= bytes[0])) && ((bytes[0] <= 0xfe) && (0x40 <= bytes[1])))
            {
                return (bytes[1] <= 0xfe);
            }
            return false;
        }

        public static bool IsInGBK(string str)
        {
            if ((str == null) || (str == ""))
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (!IsInGBK(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static string NarrowToGB2312(string sourceString, char replaceChar)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char ch in sourceString.ToCharArray())
            {
                if (IsInGB2312(ch))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(replaceChar);
                }
            }
            return builder.ToString();
        }

        public static string NarrowToGBK(string sourceString, char replaceChar)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char ch in sourceString.ToCharArray())
            {
                if (IsInGBK(ch))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(replaceChar);
                }
            }
            return builder.ToString();
        }
    }
}
