namespace Platform.Exchange
{
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class ConvertManager
    {
        private Type m_DFieldType = typeof(DField);
        private Type m_DTableType = typeof(DTable);
        private Hashtable m_Sections = new Hashtable();
        private Type m_SectionType = typeof(Section);
        private Type m_SFieldType = typeof(SField);

        public void Convert(string sectionName, DataSet destds, Hashtable rows, bool acceptLast)
        {
            Section section = this.m_Sections[sectionName] as Section;
            foreach (DTable table in section.DTables)
            {
                DataTable table2 = destds.Tables[table.TNAME];
                if (table2 == null)
                {
                    table2 = destds.Tables.Add(table.TNAME);
                }
                foreach (DField field in table.DFields)
                {
                    if (!table2.Columns.Contains(field.DFNAME))
                    {
                        table2.Columns.Add(field.DFNAME);
                    }
                }
            }
            destds.AcceptChanges();
            foreach (DTable table3 in section.DTables)
            {
                DataTable table4 = destds.Tables[table3.TNAME];
                DataRow row = table4.NewRow();
                foreach (DField field2 in table3.DFields)
                {
                    object obj2;
                    string text = "";
                    DataColumn column = table4.Columns[field2.DFNAME];
                    foreach (SField field3 in field2.SFields)
                    {
                        DataRow row2 = rows[field3.SFTNAME] as DataRow;
                        string text2 = "";
                        if (!row2.IsNull(field3.SFNAME))
                        {
                            text2 = row2[field3.SFNAME].ToString();
                            if (field3.SFSTART > 0)
                            {
                                if (field3.SFSTART >= text2.Length)
                                {
                                    text2 = "";
                                }
                                else if ((field3.SFLENGTH < 0) || ((field3.SFSTART + field3.SFLENGTH) >= text2.Length))
                                {
                                    text2 = text2.Substring(field3.SFSTART);
                                }
                                else
                                {
                                    text2 = text2.Substring(field3.SFSTART, field3.SFLENGTH);
                                }
                            }
                            else if ((field3.SFLENGTH > 0) && (field3.SFLENGTH < text2.Length))
                            {
                                text2 = text2.Substring(0, field3.SFLENGTH);
                            }
                            if ((field3.SFCUSTOM == null) || (field3.SFCUSTOM == ""))
                            {
                                text = text + text2;
                                continue;
                            }
                            text = text + string.Format(field3.SFCUSTOM, text2);
                        }
                    }
                    if ((field2.DFCUSTOM != null) && (field2.DFCUSTOM != ""))
                    {
                        text = string.Format(field2.DFCUSTOM, text);
                    }
                    if (column.DataType != typeof(string))
                    {
                        obj2 = TextConverter.Convert(column.DataType, text, null);
                    }
                    else
                    {
                        obj2 = text;
                    }
                    row[column] = (obj2 == null) ? DBNull.Value : obj2;
                }
                table4.Rows.Add(row);
            }
            if (acceptLast)
            {
                destds.AcceptChanges();
            }
        }

        public bool InitializeConvertInfo(DataRow sectionRow, string[] relations)
        {
            Section section = Activator.CreateInstance(this.m_SectionType) as Section;
            if (section == null)
            {
                return false;
            }
            FillUtility.FillFields(section, sectionRow);
            foreach (DataRow row in sectionRow.GetChildRows(relations[0]))
            {
                DTable table = Activator.CreateInstance(this.m_DTableType) as DTable;
                if (table == null)
                {
                    return false;
                }
                FillUtility.FillFields(table, row);
                foreach (DataRow row2 in row.GetChildRows(relations[1]))
                {
                    DField field = Activator.CreateInstance(this.m_DFieldType) as DField;
                    if (field == null)
                    {
                        return false;
                    }
                    FillUtility.FillFields(field, row2);
                    foreach (DataRow row3 in row2.GetChildRows(relations[2]))
                    {
                        SField field2 = Activator.CreateInstance(this.m_SFieldType) as SField;
                        if (field2 == null)
                        {
                            return false;
                        }
                        FillUtility.FillFields(field2, row3);
                        field.SFields.Add(field2);
                    }
                    table.DFields.Add(field);
                }
                section.DTables.Add(table);
            }
            this.m_Sections[section.SNAME] = section;
            return true;
        }

        public bool InitializeConvertInfo(DataSet ds, string mainTable, string[] relations)
        {
            if (!ds.Tables.Contains(mainTable))
            {
                return false;
            }
            foreach (DataRow row in ds.Tables[mainTable].Rows)
            {
                this.InitializeConvertInfo(row, relations);
            }
            return true;
        }

        public Type DFieldType
        {
            get
            {
                return this.m_DFieldType;
            }
            set
            {
                if (value == null)
                {
                    this.m_DFieldType = typeof(DField);
                }
                else if (!value.IsSubclassOf(typeof(DField)) && (value != typeof(DField)))
                {
                    this.m_DFieldType = value;
                }
            }
        }

        public Type DTableType
        {
            get
            {
                return this.m_DTableType;
            }
            set
            {
                if (value == null)
                {
                    this.m_DTableType = typeof(DTable);
                }
                else if (!value.IsSubclassOf(typeof(DTable)) && (value != typeof(DTable)))
                {
                    this.m_DTableType = value;
                }
            }
        }

        public Hashtable Sections
        {
            get
            {
                return this.m_Sections;
            }
        }

        public Type SectionType
        {
            get
            {
                return this.m_SectionType;
            }
            set
            {
                if (value == null)
                {
                    this.m_SectionType = typeof(Section);
                }
                else if (!value.IsSubclassOf(typeof(Section)) && (value != typeof(Section)))
                {
                    this.m_SectionType = value;
                }
            }
        }

        public Type SFieldType
        {
            get
            {
                return this.m_SFieldType;
            }
            set
            {
                if (value == null)
                {
                    this.m_SFieldType = typeof(SField);
                }
                else if (!value.IsSubclassOf(typeof(SField)) && (value != typeof(SField)))
                {
                    this.m_SFieldType = value;
                }
            }
        }

        public class DField
        {
            [FillField]
            public string DFCUSTOM;
            [FillField]
            public int DFLENGTH = -1;
            [FillField]
            public string DFNAME;
            [FillField]
            public int DFSTART = -1;
            public ArrayList SFields = new ArrayList();
        }

        public class DTable
        {
            public ArrayList DFields = new ArrayList();
            [FillField]
            public string TNAME;
        }

        public class Section
        {
            public ArrayList DTables = new ArrayList();
            [FillField]
            public string SNAME;
        }

        public class SField
        {
            [FillField]
            public string SFCUSTOM;
            [FillField]
            public int SFLENGTH = -1;
            [FillField]
            public string SFNAME;
            [FillField]
            public int SFSTART = -1;
            [FillField]
            public string SFTNAME;
        }
    }
}
