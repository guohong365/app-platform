namespace Platform.Exchange
{
    using Platform.Configuration;
    using System;
    using System.Collections;

    public sealed class ExchangeErrorHelper
    {
        [ConfigCollectionItem("/ExchangeErrorCode", null, null, typeof(string))]
        private static Hashtable m_ExchangeErrorCode = null;

        public static void SetError(int code, ref int errorCode, ref string errorString)
        {
            errorCode |= code;
            if (errorString == null)
            {
                errorString = "";
            }
            if (errorString != "")
            {
                errorString = errorString + "\r\n";
            }
            if (m_ExchangeErrorCode == null)
            {
                object obj2 = errorString;
                errorString = string.Concat(new object[] { obj2, "Î´Öª´íÎó£¡£¨´íÎó´úÂë£º", code, "£©" });
            }
            else
            {
                string text = m_ExchangeErrorCode[code.ToString()] as string;
                if (text == null)
                {
                    object obj3 = errorString;
                    errorString = string.Concat(new object[] { obj3, "Î´Öª´íÎó£¡£¨´íÎó´úÂë£º", code, "£©" });
                }
                else
                {
                    object obj4 = errorString;
                    errorString = string.Concat(new object[] { obj4, text, "£¨´íÎó´úÂë£º", code, "£©" });
                }
            }
        }

        public static void SetError(int code, ref int errorCode, ref string errorString, string msg)
        {
            errorCode |= code;
            if (errorString == null)
            {
                errorString = "";
            }
            if (errorString != "")
            {
                errorString = errorString + "\r\n";
            }
            if (m_ExchangeErrorCode == null)
            {
                object obj2 = errorString;
                errorString = string.Concat(new object[] { obj2, "Î´Öª´íÎó£¡£¨´íÎó´úÂë£º", code, "£©" });
            }
            else
            {
                string text = m_ExchangeErrorCode[code.ToString()] as string;
                if (text == null)
                {
                    object obj3 = errorString;
                    errorString = string.Concat(new object[] { obj3, "Î´Öª´íÎó£¡£¨´íÎó´úÂë£º", code, "£©" });
                }
                object obj4 = errorString;
                errorString = string.Concat(new object[] { obj4, text, "£¨´íÎó´úÂë£º", code, "£©" });
            }
            errorString = errorString + msg;
        }
    }
}
