namespace Platform.Utils
{
    using System;

    public sealed class CompareUtility
    {
        public static int Compare(byte[] b1, byte[] b2)
        {
            int length = b1.Length;
            int num2 = b2.Length;
            if (length > num2)
            {
                return 1;
            }
            if (length < num2)
            {
                return -1;
            }
            for (int i = 0; i < length; i++)
            {
                if (b1[i] > b2[i])
                {
                    return 1;
                }
                if (b1[i] < b2[i])
                {
                    return -1;
                }
            }
            return 0;
        }

        public static int Compare(byte[] b1, byte[] b2, int len)
        {
            int length = b1.Length;
            int num2 = b2.Length;
            if ((length < len) || (num2 < len))
            {
                if (length > num2)
                {
                    return 1;
                }
                if (length < num2)
                {
                    return -1;
                }
                len = length;
            }
            for (int i = 0; i < len; i++)
            {
                if (b1[i] > b2[i])
                {
                    return 1;
                }
                if (b1[i] < b2[i])
                {
                    return -1;
                }
            }
            return 0;
        }

        public static int Compare(byte[] b1, int off1, byte[] b2, int off2, int len)
        {
            int length = b1.Length;
            int num2 = b2.Length;
            if (((length - off1) < len) || ((num2 - off2) < len))
            {
                if ((length - off1) > (num2 - off2))
                {
                    return 1;
                }
                if ((length - off1) < (num2 - off2))
                {
                    return -1;
                }
                len = length - off1;
            }
            for (int i = 0; i < len; i++)
            {
                if (b1[off1 + i] > b2[off2 + i])
                {
                    return 1;
                }
                if (b1[off1 + i] < b2[off2 + i])
                {
                    return -1;
                }
            }
            return 0;
        }

        public static bool IsEqual(byte[] b1, byte[] b2)
        {
            int length = b1.Length;
            int num2 = b2.Length;
            if (length != num2)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsEqual(byte[] b1, byte[] b2, int len)
        {
            int length = b1.Length;
            int num2 = b2.Length;
            if ((length < len) || (num2 < len))
            {
                if (length != num2)
                {
                    return false;
                }
                len = length;
            }
            for (int i = 0; i < len; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsEqual(byte[] b1, int off1, byte[] b2, int off2, int len)
        {
            int length = b1.Length;
            int num2 = b2.Length;
            if (((length - off1) < len) || ((num2 - off2) < len))
            {
                return false;
            }
            for (int i = 0; i < len; i++)
            {
                if (b1[off1 + i] != b2[off2 + i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
