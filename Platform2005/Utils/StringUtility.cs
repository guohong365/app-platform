namespace Platform.Utils
{
    using System;

    public sealed class StringUtility
    {
        public static string GetSubString(string s, int start, int length)
        {
            if ((start < 0) && (length < 0))
            {
                return s;
            }
            if (s == null)
            {
                return null;
            }
            if (start < 0)
            {
                start = 0;
            }
            if (s.Length < start)
            {
                return "";
            }
            if (length < 0)
            {
                length = s.Length - start;
            }
            if (s.Length < (start + length))
            {
                length = s.Length - start;
            }
            return s.Substring(start, length);
        }
    }
}
