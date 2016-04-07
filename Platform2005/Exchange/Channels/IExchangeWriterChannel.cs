namespace Platform.Exchange.Channels
{
    using System;
    using System.Data;

    public interface IExchangeWriterChannel
    {
        bool InitializeSetting(string settingName, string xmlFileName, bool clear);
        bool InitializeSetting(string settingName, DataRow sectionRow, DataTable resultsTable, DataTable conditionTable);
        bool InitializeSetting(string settingName, DataTable sectionTable, DataTable resultsTable, DataTable conditionTable, bool clear);
        bool SendData(string settingName, string sectionName, ref int errors, ref string errorString, DataSet ds);
    }
}
