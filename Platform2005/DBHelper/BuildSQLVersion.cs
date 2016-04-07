namespace Platform.DBHelper
{
    using System;

    public enum BuildSQLVersion
    {
        ORACLE = 0x20000,
        ORACLE0805 = 0x20001,
        ORACLE0817 = 0x20002,
        ORACLE0900 = 0x20003,
        ORACLE1000 = 0x20004,
        SQLSERVER = 0x10000,
        SQLSERVER2000 = 0x10001,
        SQLSERVER2005 = 0x10002
    }
}
