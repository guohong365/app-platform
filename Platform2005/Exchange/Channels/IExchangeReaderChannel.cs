namespace Platform.Exchange.Channels
{
    using System;
    using System.Data;

    public interface IExchangeReaderChannel
    {
        int GetCount(string settingName, string sectionName, ref int errors, ref string errorString, params string[] filters);
        DataSet GetData(string settingName, string sectionName, ref int errors, ref string errorString, params string[] filters);
        bool InitializeSetting(string settingName, string xmlFileName, bool clear);
        bool InitializeSetting(string settingName, DataRow sectionRow, DataTable resultsTable, DataTable conditionTable, DataTable relationTable);
        bool InitializeSetting(string settingName, DataTable sectionTable, DataTable resultsTable, DataTable conditionTable, DataTable relationTable, bool clear);
        bool ResetChannel(string settingName);
        bool ResetChannel(string settingName, string sectionName);
    }
}
