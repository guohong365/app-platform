namespace Platform.Exchange
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate object ConvertHandler(string settingName, string sectionName, object obj);
}
