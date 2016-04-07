namespace Platform.Exchange
{
    using Platform.DBHelper;
    using Platform.Exchange.Channels;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Data;
    using System.Xml;

    public sealed class ExchangeManager
    {
        private static Hashtable m_Converters = new Hashtable();
        private static Hashtable m_ConvertSections = new Hashtable();
        private static Hashtable m_ReaderChannels = new Hashtable();
        private static Hashtable m_WriterChannels = new Hashtable();

        private static Section GetConvertSection(string settingName, string sectionName)
        {
            Hashtable hashtable = m_ConvertSections[settingName] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            return (hashtable[sectionName] as Section);
        }

        public static int GetCount(string settingName, string channelSectionName, ref int errors, ref string errorString, params string[] filters)
        {
            IExchangeReaderChannel readerChannel = GetReaderChannel(settingName);
            if (readerChannel == null)
            {
                ExchangeErrorHelper.SetError(1, ref errors, ref errorString);
                return -1;
            }
            return readerChannel.GetCount(settingName, channelSectionName, ref errors, ref errorString, filters);
        }

        public static DataSet GetData(string settingName, string convertSectionName, string channelSectionName, ref int errors, ref string errorString, params string[] filters)
        {
            IExchangeReaderChannel readerChannel = GetReaderChannel(settingName);
            if (readerChannel == null)
            {
                ExchangeErrorHelper.SetError(1, ref errors, ref errorString);
                return null;
            }
            DataSet ds = readerChannel.GetData(settingName, channelSectionName, ref errors, ref errorString, filters);
            if (ds == null)
            {
                return null;
            }
            if (convertSectionName == null)
            {
                return ds;
            }
            Section convertSection = GetConvertSection(settingName, convertSectionName);
            if (convertSection == null)
            {
                ExchangeErrorHelper.SetError(2, ref errors, ref errorString);
                return ds;
            }
            return convertSection.Convert(settingName, convertSectionName, ds, ref errorString);
        }

        public static IExchangeReaderChannel GetReaderChannel(string settingName)
        {
            return (m_ReaderChannels[settingName] as IExchangeReaderChannel);
        }

        public static IExchangeWriterChannel GetWriterChannel(string settingName)
        {
            if (!m_WriterChannels.ContainsKey(settingName))
            {
                return null;
            }
            return (m_WriterChannels[settingName] as IExchangeWriterChannel);
        }

        public static bool InitializeExchangeConvertSetting(string settingName, string xmlFileName, bool clear)
        {
            if (clear)
            {
                m_ConvertSections.Clear();
            }
            Hashtable hashtable = m_ConvertSections[settingName] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                m_ConvertSections[settingName] = hashtable;
            }
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(xmlFileName);
                XmlElement documentElement = document.DocumentElement;
                if (documentElement.Name != "EXCHANGE")
                {
                    return false;
                }
                foreach (XmlNode node in documentElement.ChildNodes)
                {
                    if (node.Name == "SECTION")
                    {
                        Section section = new Section(node);
                        hashtable[section.SNAME] = section;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool InitializeExchangeConvertSetting(string settingName, DataRow sectionRow, string sindex, DataTable resultsTable, string relation, string sort)
        {
            string text = sectionRow[sindex].ToString();
            DataRow[] resultsRows = resultsTable.Select(string.Format(relation, text), sort);
            Hashtable hashtable = m_ConvertSections[settingName] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                m_ConvertSections[settingName] = hashtable;
            }
            try
            {
                Section section = new Section(sectionRow, resultsRows);
                hashtable[section.SNAME] = section;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool InitializeExchangeConvertSetting(string settingName, DataTable sectionTable, string sindex, DataTable resultsTable, string relation, string sort, bool clear)
        {
            if (sectionTable == null)
            {
                return false;
            }
            if (resultsTable == null)
            {
                return false;
            }
            if (clear)
            {
                m_ConvertSections.Clear();
            }
            foreach (DataRow row in sectionTable.Rows)
            {
                InitializeExchangeConvertSetting(settingName, row, sindex, resultsTable, relation, sort);
            }
            return true;
        }

        public static bool InitializeExchangeReaderChannelSetting(string settingName, string xmlFileName, bool clear)
        {
            IExchangeReaderChannel channel = m_ReaderChannels[settingName] as IExchangeReaderChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.InitializeSetting(settingName, xmlFileName, clear);
        }

        public static bool InitializeExchangeReaderChannelSetting(string settingName, DataRow sectionRow, DataTable resultsTable, DataTable conditionTable, DataTable relationTable)
        {
            IExchangeReaderChannel channel = m_ReaderChannels[settingName] as IExchangeReaderChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.InitializeSetting(settingName, sectionRow, resultsTable, conditionTable, relationTable);
        }

        public static bool InitializeExchangeReaderChannelSetting(string settingName, DataTable sectionTable, DataTable resultsTable, DataTable conditionTable, DataTable relationTable, bool clear)
        {
            IExchangeReaderChannel channel = m_ReaderChannels[settingName] as IExchangeReaderChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.InitializeSetting(settingName, sectionTable, resultsTable, conditionTable, relationTable, clear);
        }

        public static bool InitializeExchangeWriterChannelSetting(string settingName, string xmlFileName, bool clear)
        {
            if (!m_WriterChannels.ContainsKey(settingName))
            {
                return false;
            }
            IExchangeWriterChannel channel = m_WriterChannels[settingName] as IExchangeWriterChannel;
            return channel.InitializeSetting(settingName, xmlFileName, clear);
        }

        public static bool InitializeExchangeWriterChannelSetting(string settingName, DataRow sectionRow, DataTable resultsTable, DataTable conditionTable)
        {
            if (!m_WriterChannels.ContainsKey(settingName))
            {
                return false;
            }
            IExchangeWriterChannel channel = m_WriterChannels[settingName] as IExchangeWriterChannel;
            return channel.InitializeSetting(settingName, sectionRow, resultsTable, conditionTable);
        }

        public static bool InitializeExchangeWriterChannelSetting(string settingName, DataTable sectionTable, DataTable resultsTable, DataTable conditionTable, bool clear)
        {
            if (!m_WriterChannels.ContainsKey(settingName))
            {
                return false;
            }
            IExchangeWriterChannel channel = m_WriterChannels[settingName] as IExchangeWriterChannel;
            return channel.InitializeSetting(settingName, sectionTable, resultsTable, conditionTable, clear);
        }

        public static bool RegisterExchangeReaderChannel(string settingName, Type channelType)
        {
            if (channelType.GetInterface("Platform.Exchange.Channels.IExchangeReaderChannel", false) == null)
            {
                return false;
            }
            ExchangeChannelAttribute[] customAttributes = channelType.GetCustomAttributes(typeof(ExchangeChannelAttribute), false) as ExchangeChannelAttribute[];
            if (customAttributes.Length < 1)
            {
                return false;
            }
            try
            {
                IExchangeReaderChannel channel = Activator.CreateInstance(channelType) as IExchangeReaderChannel;
                m_ReaderChannels[settingName] = channel;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RegisterExchangeWriterChannel(string settingName, Type channelType)
        {
            if (channelType.GetInterface("Platform.Exchange.Channels.IExchangeWriterChannel", false) == null)
            {
                return false;
            }
            ExchangeChannelAttribute[] customAttributes = channelType.GetCustomAttributes(typeof(ExchangeChannelAttribute), false) as ExchangeChannelAttribute[];
            if (customAttributes.Length < 1)
            {
                return false;
            }
            try
            {
                IExchangeWriterChannel channel = Activator.CreateInstance(channelType) as IExchangeWriterChannel;
                m_WriterChannels[settingName] = channel;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ResetChannel(string settingName)
        {
            IExchangeReaderChannel channel = m_ReaderChannels[settingName] as IExchangeReaderChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.ResetChannel(settingName);
        }

        public static bool ResetChannel(string settingName, string sectionName)
        {
            IExchangeReaderChannel channel = m_ReaderChannels[settingName] as IExchangeReaderChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.ResetChannel(settingName, sectionName);
        }

        public static bool SendData(string settingName, string convertSectionName, string channelSectionName, ref int errors, ref string errorString, DataSet ds)
        {
            IExchangeWriterChannel writerChannel = GetWriterChannel(settingName);
            if (writerChannel == null)
            {
                ExchangeErrorHelper.SetError(1, ref errors, ref errorString);
                return false;
            }
            if (convertSectionName != null)
            {
                Section convertSection = GetConvertSection(settingName, convertSectionName);
                if (convertSection != null)
                {
                    ExchangeErrorHelper.SetError(2, ref errors, ref errorString);
                    ds = convertSection.Convert(settingName, convertSectionName, ds, ref errorString);
                }
            }
            if (ds == null)
            {
                ExchangeErrorHelper.SetError(4, ref errors, ref errorString);
                return false;
            }
            return writerChannel.SendData(settingName, channelSectionName, ref errors, ref errorString, ds);
        }

        private class Result
        {
            [FillField]
            public string CAPTION = null;
            [FillField]
            public string DFIELD = null;
            public int DispSort;
            [FillField]
            public string DISPSORT = null;
            public int Length;
            [FillField]
            public string LENGTH = null;
            private ArrayList m_SFields;
            [FillField]
            public string SFIELD = null;
            public int Start;
            [FillField]
            public string TYPE = null;

            public Result(DataRow row)
            {
                this.DispSort = -1;
                this.Start = -1;
                this.Length = -1;
                this.m_SFields = new ArrayList();
                FillUtility.FillFields(this, row);
                if ((this.LENGTH != null) && (this.LENGTH != ""))
                {
                    string[] textArray = this.LENGTH.Split(new char[] { ',' });
                    if (textArray.Length == 2)
                    {
                        this.Start = int.Parse(textArray[0]);
                        this.Length = int.Parse(textArray[1]);
                    }
                }
                if ((this.DISPSORT != null) && (this.DISPSORT != ""))
                {
                    this.DispSort = int.Parse(this.DISPSORT);
                }
                this.AddSField(row);
            }

            public Result(XmlNode node)
            {
                this.DispSort = -1;
                this.Start = -1;
                this.Length = -1;
                this.m_SFields = new ArrayList();
                FillUtility.FillFields(this, node);
                if ((this.LENGTH != null) && (this.LENGTH != ""))
                {
                    string[] textArray = this.LENGTH.Split(new char[] { ',' });
                    if (textArray.Length == 2)
                    {
                        this.Start = int.Parse(textArray[0]);
                        this.Length = int.Parse(textArray[1]);
                    }
                }
                if ((this.DISPSORT != null) && (this.DISPSORT != ""))
                {
                    this.DispSort = int.Parse(this.DISPSORT);
                }
                this.AddSField(node);
            }

            public void AddSField(DataRow row)
            {
                if (!row.IsNull("SFIELD") && (row["SFIELD"].ToString().Trim() != ""))
                {
                    ExchangeManager.SField field = new ExchangeManager.SField(row);
                    this.m_SFields.Add(field);
                }
            }

            public void AddSField(XmlNode node)
            {
                if ((node.Attributes["SFIELD"] != null) && (node.Attributes["SFIELD"].Value.Trim() != ""))
                {
                    ExchangeManager.SField field = new ExchangeManager.SField(node);
                    this.m_SFields.Add(field);
                }
            }

            public void Convert(string settingName, string sectionName, DataRow sourceRow, DataRow destRow, ref string errorString)
            {
                if (this.m_SFields.Count >= 1)
                {
                    if (this.TYPE == "B")
                    {
                        byte[] buffer = ((ExchangeManager.SField)this.m_SFields[0]).GetBinary(settingName, sectionName, sourceRow, ref errorString);
                        if (buffer == null)
                        {
                            destRow[this.DFIELD] = DBNull.Value;
                        }
                        else
                        {
                            destRow[this.DFIELD] = buffer;
                        }
                    }
                    else
                    {
                        string s = "";
                        foreach (ExchangeManager.SField field in this.m_SFields)
                        {
                            s = s + field.GetString(settingName, sectionName, sourceRow, ref errorString);
                        }
                        s = StringUtility.GetSubString(s, this.Start, this.Length);
                        if (s == null)
                        {
                            destRow[this.DFIELD] = DBNull.Value;
                        }
                        else
                        {
                            destRow[this.DFIELD] = s;
                        }
                    }
                }
            }
        }

        private class Section
        {
            [FillField]
            private string DSNAME = null;
            private Hashtable m_Results;
            private ArrayList m_ResultsList;
            [FillField]
            public string SNAME = null;
            [FillField]
            private string TBNAME = null;

            public Section(XmlNode node)
            {
                this.m_Results = new Hashtable();
                this.m_ResultsList = new ArrayList();
                FillUtility.FillFields(this, node);
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    if ((node2.Name != "RESULT") || (node2.Attributes["DFIELD"] == null))
                    {
                        continue;
                    }
                    string key = node2.Attributes["DFIELD"].Value.Trim();
                    if (key != "")
                    {
                        if (this.m_Results.ContainsKey(key))
                        {
                            (this.m_Results[key] as ExchangeManager.Result).AddSField(node2);
                            continue;
                        }
                        ExchangeManager.Result result2 = new ExchangeManager.Result(node2);
                        this.m_Results[key] = result2;
                        bool flag = false;
                        if (result2.DispSort >= 0)
                        {
                            for (int i = 0; i < this.m_ResultsList.Count; i++)
                            {
                                int dispSort = ((ExchangeManager.Result)this.m_ResultsList[i]).DispSort;
                                if ((dispSort < 0) || (dispSort > result2.DispSort))
                                {
                                    this.m_ResultsList.Insert(i, result2);
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            this.m_ResultsList.Add(result2);
                        }
                    }
                }
            }

            public Section(DataRow row, DataRow[] resultsRows)
            {
                this.m_Results = new Hashtable();
                this.m_ResultsList = new ArrayList();
                FillUtility.FillFields(this, row);
                foreach (DataRow row2 in resultsRows)
                {
                    if (!row2.IsNull("DFIELD"))
                    {
                        string key = row2["DFIELD"].ToString().Trim();
                        if (key != "")
                        {
                            if (this.m_Results.ContainsKey(key))
                            {
                                (this.m_Results[key] as ExchangeManager.Result).AddSField(row2);
                            }
                            else
                            {
                                ExchangeManager.Result result2 = new ExchangeManager.Result(row2);
                                this.m_Results[key] = result2;
                                bool flag = false;
                                if (result2.DispSort >= 0)
                                {
                                    for (int i = 0; i < this.m_ResultsList.Count; i++)
                                    {
                                        int dispSort = ((ExchangeManager.Result)this.m_ResultsList[i]).DispSort;
                                        if ((dispSort < 0) || (dispSort > result2.DispSort))
                                        {
                                            this.m_ResultsList.Insert(i, result2);
                                            flag = true;
                                            break;
                                        }
                                    }
                                }
                                if (!flag)
                                {
                                    this.m_ResultsList.Add(result2);
                                }
                            }
                        }
                    }
                }
            }

            public DataSet Convert(string settingName, string sectionName, DataSet ds, ref string errorString)
            {
                if (ds == null)
                {
                    return null;
                }
                if (ds.Tables.Count < 1)
                {
                    return null;
                }
                DataSet set = this.MakeDataSet();
                DataTable table = set.Tables[0];
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DataRow destRow = table.NewRow();
                    foreach (ExchangeManager.Result result in this.m_Results.Values)
                    {
                        result.Convert(settingName, sectionName, row, destRow, ref errorString);
                    }
                    table.Rows.Add(destRow);
                }
                set.AcceptChanges();
                return set;
            }

            private DataSet MakeDataSet()
            {
                DataSet set = new DataSet(this.DSNAME);
                DataTable table = set.Tables.Add(this.TBNAME);
                foreach (ExchangeManager.Result result in this.m_ResultsList)
                {
                    DataColumn column;
                    if (result.TYPE == "A")
                    {
                        column = new DataColumn(result.DFIELD, typeof(string), null, MappingType.Element);
                    }
                    else
                    {
                        column = new DataColumn(result.DFIELD, typeof(byte[]), null, MappingType.Element);
                    }
                    if ((result.CAPTION != null) && (result.CAPTION != ""))
                    {
                        column.Caption = result.CAPTION;
                    }
                    else
                    {
                        column.Caption = result.DFIELD;
                    }
                    table.Columns.Add(column);
                }
                return set;
            }
        }

        private class SField
        {
            [FillField]
            public string CONVERTER = null;
            public string DMSql;
            [FillField]
            public string SDMDFIELD = null;
            [FillField]
            public string SDMSFIELD = null;
            [FillField]
            public string SDMTABLE = null;
            [FillField]
            public string SFIELD = null;
            public int SLength;
            [FillField]
            public string SLENGTH = null;
            public int SStart;

            public SField(DataRow row)
            {
                this.SStart = -1;
                this.SLength = -1;
                FillUtility.FillFields(this, row);
                if ((this.SLENGTH != null) && (this.SLENGTH != ""))
                {
                    string[] textArray = this.SLENGTH.Split(new char[] { ',' });
                    if (textArray.Length == 2)
                    {
                        this.SStart = int.Parse(textArray[0]);
                        this.SLength = int.Parse(textArray[1]);
                    }
                }
                if ((((this.SDMTABLE != null) && (this.SDMTABLE != "")) && ((this.SDMSFIELD != null) && (this.SDMSFIELD != ""))) && ((this.SDMDFIELD != null) && (this.SDMDFIELD != "")))
                {
                    this.DMSql = "SELECT " + this.SDMDFIELD + " FROM " + this.SDMTABLE + " WHERE " + this.SDMSFIELD + "='{0}'";
                }
            }

            public SField(XmlNode node)
            {
                this.SStart = -1;
                this.SLength = -1;
                FillUtility.FillFields(this, node);
                if ((this.SLENGTH != null) && (this.SLENGTH != ""))
                {
                    string[] textArray = this.SLENGTH.Split(new char[] { ',' });
                    if (textArray.Length == 2)
                    {
                        this.SStart = int.Parse(textArray[0]);
                        this.SLength = int.Parse(textArray[1]);
                    }
                }
                if ((((this.SDMTABLE != null) && (this.SDMTABLE != "")) && ((this.SDMSFIELD != null) && (this.SDMSFIELD != ""))) && ((this.SDMDFIELD != null) && (this.SDMDFIELD != "")))
                {
                    this.DMSql = "SELECT " + this.SDMDFIELD + " FROM " + this.SDMTABLE + " WHERE " + this.SDMSFIELD + "='{0}'";
                }
            }

            public byte[] GetBinary(string settingName, string sectionName, DataRow row, ref string errorString)
            {
                if (!row.Table.Columns.Contains(this.SFIELD))
                {
                    return null;
                }
                if (row.IsNull(this.SFIELD))
                {
                    return null;
                }
                if ((this.CONVERTER != null) && ExchangeManager.m_Converters.ContainsKey(this.CONVERTER))
                {
                    return (((ConvertHandler)ExchangeManager.m_Converters[this.CONVERTER])(settingName, sectionName, row[this.SFIELD]) as byte[]);
                }
                object obj2 = row[this.SFIELD];
                if (obj2.GetType() == typeof(string))
                {
                    try
                    {
                        return Convert.FromBase64String(obj2 as string);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return (row[this.SFIELD] as byte[]);
            }

            public string GetString(string settingName, string sectionName, DataRow row, ref string errorString)
            {
                string text;
                if (!row.Table.Columns.Contains(this.SFIELD))
                {
                    return null;
                }
                if (row.IsNull(this.SFIELD))
                {
                    return null;
                }
                if ((this.CONVERTER == null) || !ExchangeManager.m_Converters.ContainsKey(this.CONVERTER))
                {
                    object obj2 = row[this.SFIELD];
                    if (obj2.GetType() == typeof(DateTime))
                    {
                        text = ((DateTime)obj2).ToString("yyyyMMddHHmmss");
                    }
                    else
                    {
                        text = obj2.ToString();
                    }
                }
                else
                {
                    text = ((ConvertHandler)ExchangeManager.m_Converters[this.CONVERTER])(settingName, sectionName, row[this.SFIELD]) as string;
                }
                if ((text != null) && (this.DMSql != null))
                {
                    object obj3 = DBHandler.ExecuteScalarOnce(string.Format(this.DMSql, text));
                    if (obj3 != null)
                    {
                        if (obj3.GetType() == typeof(DateTime))
                        {
                            text = ((DateTime)obj3).ToString("yyyyMMddHHmmss");
                        }
                        else
                        {
                            text = obj3.ToString();
                        }
                    }
                }
                return StringUtility.GetSubString(text, this.SStart, this.SLength);
            }
        }
    }
}
