namespace Platform
{
    using System;

    public sealed class SQLUtility
    {
        public static string[] GetTableNames(string sql)
        {
            string text = sql.ToUpper();
            int startIndex = 0;
            int index = 0;
            string text2 = "";
            do
            {
                string text3;
                startIndex = text.IndexOf("FROM ", index);
                if (startIndex < 0)
                {
                    break;
                }
                startIndex += 5;
                index = text.IndexOf("WHERE ", startIndex);
                if (index < 0)
                {
                    text3 = sql.Substring(startIndex);
                }
                else
                {
                    text3 = sql.Substring(startIndex, index - startIndex);
                }
                if (text2.Length > 0)
                {
                    text2 = text2 + ",";
                }
                text2 = text2 + text3;
            }
            while (index >= 0);
            text2 = text2.Replace(" ", "");
            if (text2.Length == 0)
            {
                return null;
            }
            return text2.Split(new char[] { ',' });
        }
    }
}
