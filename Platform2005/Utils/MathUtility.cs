namespace Platform.Utils
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public sealed class MathUtility
    {
        public static bool GetAvList(int min, int max, out long[] compare, out long[] div)
        {
            compare = null;
            div = null;
            if ((max < 1) || (max < 1))
            {
                return false;
            }
            int num = max / min;
            if (num < 1)
            {
                return false;
            }
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            long num2 = 0;
            long num3 = 1;
            while (true)
            {
                list.Insert(0, num2);
                list2.Insert(0, num3);
                long num4 = num3 * max;
                if ((num4 < num2) || (num4 < 0))
                {
                    break;
                }
                num2 = num4;
                num3 = num4 / ((long) num);
            }
            compare = (long[]) list.ToArray(typeof(long));
            div = (long[]) list2.ToArray(typeof(long));
            return true;
        }
    }
}
