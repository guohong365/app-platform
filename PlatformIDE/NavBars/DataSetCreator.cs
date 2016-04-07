namespace PlatformIDE.NavBars
{
    using ADODB;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Odbc;
    using System.Data.OleDb;
    using System.Data.OracleClient;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public class DataSetCreator : UserControl
    {
        private Button button_CreateDataSets;
        private Button button_GetConnectionString;
        private Button button_GetSaveFolder;
        private Button button_GetTableList;
        private Button button_SelectAll;
        private Button button_UnSelectAll;
        private CheckBox checkBox_CreateClass;
        private CheckBox checkBox_CreateXsd;
        private CheckBox checkBox_SaveToFile;
        private CheckBox checkBox_WriteOutput;
        private ColumnHeader columnHeader1;
        private Container components = null;
        private Label label1;
        private Label label3;
        private ListView listView_TableList;
        private Hashtable m_AdapterMap = new Hashtable();
        private DBTypes m_OleType = new DBTypes(typeof(OleDbConnection), typeof(OleDbDataAdapter));
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Panel panel7;
        private Panel panel8;
        private ProgressBar progressBar1;
        private TextBox textBox_ClassNamespace;
        private TextBox textBox_DBConnection;
        private TextBox textBox_Namespace;
        private TextBox textBox_SavePath;

        public DataSetCreator()
        {
            this.InitializeComponent();
            this.m_AdapterMap["Provider=SQLOLEDB.1;"] = new DBTypes(typeof(SqlConnection), typeof(SqlDataAdapter));
            this.m_AdapterMap["Provider=MSDAORA.1;"] = new DBTypes(typeof(OracleConnection), typeof(OracleDataAdapter));
            this.m_AdapterMap["Provider=OraOLEDB.Oracle.1;"] = new DBTypes(typeof(OracleConnection), typeof(OracleDataAdapter));
            this.m_AdapterMap["Provider=MSDASQL;"] = new DBTypes(typeof(OdbcConnection), typeof(OdbcDataAdapter));
        }

        private void button_CreateDataSets_Click(object sender, EventArgs e)
        {
            if (this.listView_TableList.CheckedItems.Count >= 1)
            {
                int count = this.listView_TableList.CheckedItems.Count;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Maximum = this.listView_TableList.CheckedItems.Count;
                this.progressBar1.Value = 0;
                this.progressBar1.Step = 1;
                string text = this.textBox_DBConnection.Text;
                DBTypes oleType = this.m_OleType;
                foreach (DictionaryEntry entry in this.m_AdapterMap)
                {
                    if (text.IndexOf((string)entry.Key) < 0)
                    {
                        continue;
                    }
                    oleType = entry.Value as DBTypes;
                    text = text.Replace(((string)entry.Key) + ";", "");
                    text = text.Replace((string)entry.Key, "");
                    break;
                }
                IDbConnection connection = Activator.CreateInstance(oleType.ConnectionType) as IDbConnection;
                string text2 = this.textBox_Namespace.Text.Trim().TrimEnd(new char[] { '/' });
                string fullPath = Path.GetFullPath(this.textBox_SavePath.Text);
                bool flag = this.checkBox_CreateXsd.Checked;
                bool flag2 = this.checkBox_CreateClass.Checked;
                string classNamespace = this.textBox_ClassNamespace.Text.Trim();
                bool flag3 = this.checkBox_SaveToFile.Checked;
                bool flag4 = this.checkBox_WriteOutput.Checked;
                Directory.CreateDirectory(fullPath);
                int num2 = 0;
                try
                {
                    connection.ConnectionString = text;
                    connection.Open();
                    foreach (ListViewItem item in this.listView_TableList.CheckedItems)
                    {
                        try
                        {
                            string name = item.Text;
                            DbDataAdapter adapter = Activator.CreateInstance(oleType.AdapterType, new object[] { "SELECT * FROM " + name, connection }) as DbDataAdapter;
                            DataTable dataTable = new DataTable();
                            adapter.FillSchema(dataTable, SchemaType.Mapped);
                            DataSet ds = new DataSet("Dataset_" + name);
                            ds.Namespace = text2 + "/Dataset_" + name + ".xsd";
                            DataTable table2 = ds.Tables.Add(name);
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                DataColumn column2 = table2.Columns.Add(column.ColumnName);
                                column2.DataType = column.DataType;
                                column2.AllowDBNull = column.AllowDBNull;
                                column2.AutoIncrement = column.AutoIncrement;
                                column2.AutoIncrementSeed = column.AutoIncrementSeed;
                                column2.AutoIncrementStep = column.AutoIncrementStep;
                                column2.DefaultValue = column.DefaultValue;
                                column2.Unique = column.Unique;
                                column2.ReadOnly = column.ReadOnly;
                            }
                            ArrayList list = new ArrayList();
                            foreach (DataColumn column3 in dataTable.PrimaryKey)
                            {
                                list.Add(table2.Columns[column3.ColumnName]);
                            }
                            table2.PrimaryKey = list.ToArray(typeof(DataColumn)) as DataColumn[];
                            for (int i = 0; i < table2.Constraints.Count; i++)
                            {
                                table2.Constraints[i].ConstraintName = name + "Key" + (i + 1);
                            }
                            if (flag)
                            {
                                if (flag3)
                                {
                                    ds.WriteXmlSchema(fullPath + @"\Dataset_" + name + ".xsd");
                                }
                                if (flag4)
                                {
                                    Console.WriteLine(ds.GetXmlSchema());
                                    Console.WriteLine("");
                                }
                            }
                            if (flag2)
                            {
                                string dataSetClass = this.GetDataSetClass(ds, classNamespace);
                                if (flag3)
                                {
                                    FileStream stream = new FileStream(fullPath + @"\Dataset_" + name + ".cs", FileMode.Create, FileAccess.Write);
                                    byte[] bytes = Encoding.Default.GetBytes(dataSetClass);
                                    stream.Write(bytes, 0, bytes.Length);
                                    stream.Flush();
                                    stream.Close();
                                }
                                if (flag4)
                                {
                                    Console.WriteLine(dataSetClass);
                                    Console.WriteLine("");
                                }
                            }
                            num2++;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("生成错误，表名：" + item.Text);
                            Console.WriteLine(exception);
                        }
                        this.progressBar1.PerformStep();
                    }
                }
                finally
                {
                    Console.WriteLine(string.Concat(new object[] { "生成结束！！共选择 ", count, " 个表生成了 ", num2, "个表" }));
                    connection.Close();
                }
            }
        }

        private void button_GetConnectionString_Click(object sender, EventArgs e)
        {
            try
            {
                MSDASC.DataLinksClass class2 = new MSDASC.DataLinksClass();
                class2.hWnd = (int)Process.GetCurrentProcess().MainWindowHandle;
                Connection connection = class2.PromptNew() as Connection;
                if (connection != null)
                {
                    this.textBox_DBConnection.Text = connection.ConnectionString;
                }
            }
            catch
            {
            }
        }

        private void button_GetSaveFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = this.textBox_SavePath.Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox_SavePath.Text = dialog.SelectedPath;
            }
        }

        private void button_GetTableList_Click(object sender, EventArgs e)
        {
            string connectionString = this.textBox_DBConnection.Text;
            OleDbConnection connection = new OleDbConnection(connectionString);
            try
            {
                this.listView_TableList.Items.Clear();
                connection.Open();
                string text2 = null;
                int index = connectionString.ToUpper().IndexOf("USER ID");
                if (index >= 0)
                {
                    int num2 = connectionString.ToUpper().IndexOf(";", index);
                    if (num2 > 0)
                    {
                        text2 = connectionString.Substring(index + 7, (num2 - index) - 7);
                    }
                    else
                    {
                        text2 = connectionString.Substring(index + 7);
                    }
                    text2 = text2.TrimStart(new char[] { '=' }).Trim().ToUpper();
                }
                object[] restrictions = new object[4];
                restrictions[3] = "TABLE";
                DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);
                foreach (DataRow row in oleDbSchemaTable.Rows)
                {
                    if (text2 != null)
                    {
                        string text3 = (string)row["TABLE_SCHEMA"];
                        if ((text3 != null) && (text3.ToUpper() != text2))
                        {
                            continue;
                        }
                    }
                    this.listView_TableList.Items.Add((string)row["TABLE_NAME"]);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        private void button_SelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView_TableList.Items)
            {
                item.Checked = true;
            }
        }

        private void button_UnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView_TableList.Items)
            {
                item.Checked = false;
            }
        }

        private void DataSetCreator_SizeChanged(object sender, EventArgs e)
        {
            this.columnHeader1.Width = this.listView_TableList.Width - 0x19;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GetDataRowClass(DataTable dt)
        {
            string text = "";
            string text2 = "";
            foreach (DataColumn column in dt.Columns)
            {
                string text5 = text;
                text = text5 + "\r\n\t\t\tpublic " + TypeUtility.GetSimpleDataTypeString(column.DataType) + " " + column.ColumnName + "\r\n\t\t\t{\r\n\t\t\t\tget\r\n\t\t\t\t{";
                if (column.AllowDBNull)
                {
                    text5 = text;
                    text = text5 + "\r\n\t\t\t\t\ttry\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\treturn ((" + TypeUtility.GetSimpleDataTypeString(column.DataType) + ")(this[this.table" + dt.TableName + "." + column.ColumnName + "Column]));\r\n\t\t\t\t\t}\r\n\t\t\t\t\tcatch (InvalidCastException e)\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tthrow new StrongTypingException(\"无法获取值，因为它是 DBNull。\", e);\r\n\t\t\t\t\t}\r\n\t\t\t\t}";
                }
                else
                {
                    text5 = text;
                    text = text5 + "\r\n\t\t\t\t\treturn ((" + TypeUtility.GetSimpleDataTypeString(column.DataType) + ")(this[this.table" + dt.TableName + "." + column.ColumnName + "Column]));\r\n\t\t\t\t}";
                }
                text5 = text;
                text = text5 + "\r\n\t\t\t\tset\r\n\t\t\t\t{\r\n\t\t\t\t\tthis[this.table" + dt.TableName + "." + column.ColumnName + "Column] = value;\r\n\t\t\t\t}\r\n\t\t\t}";
                if (column.AllowDBNull)
                {
                    text5 = text2;
                    text2 = text5 + "\r\n\t\t\tpublic bool Is" + column.ColumnName + "Null()\r\n\t\t\t{\r\n\t\t\t\treturn this.IsNull(this.table" + dt.TableName + "." + column.ColumnName + "Column);\r\n\t\t\t}";
                }
            }
            return ((("\r\n\t\t[System.Diagnostics.DebuggerStepThrough()]\r\n\t\tpublic class " + dt.TableName + "Row : DataRow\r\n\t\t{\r\n\t\t\tprivate " + dt.TableName + "DataTable table" + dt.TableName + ";\r\n\t\t\tinternal " + dt.TableName + "Row(DataRowBuilder rb) : base(rb)\r\n\t\t\t{\r\n\t\t\t\tthis.table" + dt.TableName + " = ((" + dt.TableName + "DataTable)(this.Table));\r\n\t\t\t}") + text) + text2 + "\r\n\t\t}");
        }

        private string GetDataSetClass(DataSet ds, string classNamespace)
        {
            string text = "\r\n\t\tprivate %TBNAME%DataTable table%TBNAME%;";
            string text2 = "\r\n\t\t\t\tif ((ds.Tables[\"%TBNAME%\"] != null))\r\n\t\t\t\t{\r\n\t\t\t\t\tthis.Tables.Add(new %TBNAME%DataTable(ds.Tables[\"%TBNAME%\"]));\r\n\t\t\t\t}";
            string text3 = "\r\n\t\t[System.ComponentModel.Browsable(false)]\r\n\t\t[System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]\r\n\t\tpublic %TBNAME%DataTable %TBNAME%\r\n\t\t{\r\n\t\t\tget\r\n\t\t\t{\r\n\t\t\t\treturn this.table%TBNAME%;\r\n\t\t\t}\r\n\t\t}";
            string text4 = "\r\n\t\t\tif ((ds.Tables[\"%TBNAME%\"] != null))\r\n\t\t\t{\r\n\t\t\t\tthis.Tables.Add(new %TBNAME%DataTable(ds.Tables[\"%TBNAME%\"]));\r\n\t\t\t}";
            string text5 = "\r\n\t\t\tthis.table%TBNAME% = ((%TBNAME%DataTable)(this.Tables[\"%TBNAME%\"]));\r\n\t\t\tif ((this.table%TBNAME% != null))\r\n\t\t\t{\r\n\t\t\t\tthis.table%TBNAME%.InitVars();\r\n\t\t\t}";
            string text6 = "\r\n\t\t\tthis.table%TBNAME% = new %TBNAME%DataTable();\r\n\t\t\tthis.Tables.Add(this.table%TBNAME%);";
            string text7 = "\r\n\t\tprivate bool ShouldSerialize%TBNAME%()\r\n\t\t{\r\n\t\t\treturn false;\r\n\t\t}";
            string text8 = "\r\n\t\tpublic delegate void %TBNAME%RowChangeEventHandler(object sender, %TBNAME%RowChangeEvent e);";
            string text9 = "\r\n\t\t[System.Diagnostics.DebuggerStepThrough()]\r\n\t\tpublic class %TBNAME%RowChangeEvent : System.EventArgs\r\n\t\t{\r\n\t\t\tprivate %TBNAME%Row eventRow;\r\n\t\t\tprivate DataRowAction eventAction;\r\n\t\t\tpublic %TBNAME%RowChangeEvent(%TBNAME%Row row, DataRowAction action)\r\n\t\t\t{\r\n\t\t\t\tthis.eventRow = row;\r\n\t\t\t\tthis.eventAction = action;\r\n\t\t\t}\r\n\t\t\tpublic %TBNAME%Row Row\r\n\t\t\t{\r\n\t\t\t\tget\r\n\t\t\t\t{\r\n\t\t\t\t\treturn this.eventRow;\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t\tpublic DataRowAction Action\r\n\t\t\t{\r\n\t\t\t\tget\r\n\t\t\t\t{\r\n\t\t\t\t\treturn this.eventAction;\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t}";
            string text10 = "";
            string text11 = "";
            string text12 = "";
            string text13 = "";
            string text14 = "";
            string text15 = "";
            string text16 = "";
            string text17 = "";
            string text18 = "";
            string text19 = "";
            string text20 = "";
            foreach (DataTable table in ds.Tables)
            {
                text10 = text10 + text.Replace("%TBNAME%", table.TableName);
                text11 = text11 + text2.Replace("%TBNAME%", table.TableName);
                text12 = text12 + text3.Replace("%TBNAME%", table.TableName);
                text13 = text13 + text4.Replace("%TBNAME%", table.TableName);
                text14 = text14 + text5.Replace("%TBNAME%", table.TableName);
                text15 = text15 + text6.Replace("%TBNAME%", table.TableName);
                text16 = text16 + text7.Replace("%TBNAME%", table.TableName);
                text17 = text17 + text8.Replace("%TBNAME%", table.TableName);
                text18 = text18 + text9.Replace("%TBNAME%", table.TableName);
                text19 = text19 + this.GetDataTableClass(table);
                text20 = text20 + this.GetDataRowClass(table);
            }
            string text23 = ((((("using System;\r\nusing System.Data;\r\nusing System.Xml;\r\nusing System.Runtime.Serialization;\r\nnamespace " + classNamespace + "\r\n{\r\n\t[Serializable()]\r\n\t[System.ComponentModel.DesignerCategoryAttribute(\"code\")]\r\n\t[System.Diagnostics.DebuggerStepThrough()]\r\n\t[System.ComponentModel.ToolboxItem(true)]\r\n\tpublic class " + ds.DataSetName + " : System.Data.DataSet\r\n\t{") + text10) + "\r\n\t\tpublic " + ds.DataSetName + "()\r\n\t\t{\r\n\t\t\tthis.InitClass();\r\n\t\t\tSystem.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);\r\n\t\t\tthis.Tables.CollectionChanged += schemaChangedHandler;\r\n\t\t\tthis.Relations.CollectionChanged += schemaChangedHandler;\r\n\t\t}") + "\r\n\t\tprotected " + ds.DataSetName + "(SerializationInfo info, StreamingContext context)\r\n\t\t{\r\n\t\t\tstring strSchema = ((string)(info.GetValue(\"XmlSchema\", typeof(string))));\r\n\t\t\tif ((strSchema != null))\r\n\t\t\t{\r\n\t\t\t\tDataSet ds = new DataSet();\r\n\t\t\t\tds.ReadXmlSchema(new XmlTextReader(new System.IO.StringReader(strSchema)));") + text11) + "\r\n\t\t\t\tthis.DataSetName = ds.DataSetName;\r\n\t\t\t\tthis.Prefix = ds.Prefix;\r\n\t\t\t\tthis.Namespace = ds.Namespace;\r\n\t\t\t\tthis.Locale = ds.Locale;\r\n\t\t\t\tthis.CaseSensitive = ds.CaseSensitive;\r\n\t\t\t\tthis.EnforceConstraints = ds.EnforceConstraints;\r\n\t\t\t\tthis.Merge(ds, false, System.Data.MissingSchemaAction.Add);\r\n\t\t\t\tthis.InitVars();\r\n\t\t\t}\r\n\t\t\telse\r\n\t\t\t{\r\n\t\t\t\tthis.InitClass();\r\n\t\t\t}\r\n\t\t\tthis.GetSerializationData(info, context);\r\n\t\t\tSystem.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);\r\n\t\t\tthis.Tables.CollectionChanged += schemaChangedHandler;\r\n\t\t\tthis.Relations.CollectionChanged += schemaChangedHandler;\r\n\t\t}" + text12;
            text23 = (((((text23 + "\r\n\t\tpublic override DataSet Clone()\r\n\t\t{\r\n\t\t\t" + ds.DataSetName + " cln = ((" + ds.DataSetName + ")(base.Clone()));\r\n\t\t\tcln.InitVars();\r\n\t\t\treturn cln;\r\n\t\t}") + "\r\n\t\tprotected override bool ShouldSerializeTables()\r\n\t\t{\r\n\t\t\treturn false;\r\n\t\t}") + "\r\n\t\tprotected override bool ShouldSerializeRelations()\r\n\t\t{\r\n\t\t\treturn false;\r\n\t\t}" + "\r\n\t\tprotected override void ReadXmlSerializable(XmlReader reader)\r\n\t\t{\r\n\t\t\tthis.Reset();\r\n\t\t\tDataSet ds = new DataSet();\r\n\t\t\tds.ReadXml(reader);") + text13 + "\r\n\t\t\tthis.DataSetName = ds.DataSetName;\r\n\t\t\tthis.Prefix = ds.Prefix;\r\n\t\t\tthis.Namespace = ds.Namespace;\r\n\t\t\tthis.Locale = ds.Locale;\r\n\t\t\tthis.CaseSensitive = ds.CaseSensitive;\r\n\t\t\tthis.EnforceConstraints = ds.EnforceConstraints;\r\n\t\t\tthis.Merge(ds, false, System.Data.MissingSchemaAction.Add);\r\n\t\t\tthis.InitVars();\r\n\t\t}") + "\r\n\t\tprotected override System.Xml.Schema.XmlSchema GetSchemaSerializable()\r\n\t\t{\r\n\t\t\tSystem.IO.MemoryStream stream = new System.IO.MemoryStream();\r\n\t\t\tthis.WriteXmlSchema(new XmlTextWriter(stream, null));\r\n\t\t\tstream.Position = 0;\r\n\t\t\treturn System.Xml.Schema.XmlSchema.Read(new XmlTextReader(stream), null);\r\n\t\t}" + "\r\n\t\tinternal void InitVars()\r\n\t\t{") + text14 + "\r\n\t\t}";
            return ((((((text23 + "\r\n\t\tprivate void InitClass()\r\n\t\t{\r\n\t\t\tthis.DataSetName = \"" + ds.DataSetName + "\";\r\n\t\t\tthis.Prefix = \"\";\r\n\t\t\tthis.Namespace = \"" + ds.Namespace + "\";\r\n\t\t\tthis.Locale = new System.Globalization.CultureInfo(\"" + ds.Locale.Name + "\");\r\n\t\t\tthis.CaseSensitive = false;\r\n\t\t\tthis.EnforceConstraints = true;") + text15) + "\r\n\t\t}" + text16) + "\r\n\t\tprivate void SchemaChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)\r\n\t\t{\r\n\t\t\tif ((e.Action == System.ComponentModel.CollectionChangeAction.Remove))\r\n\t\t\t{\r\n\t\t\t\tthis.InitVars();\r\n\t\t\t}\r\n\t\t}" + text17) + text19 + text20) + text18 + "\r\n\t}\r\n}\r\n");
        }

        private string GetDataTableClass(DataTable dt)
        {
            string text16;
            string text = "\r\n\t\t\tprivate DataColumn column%CLNAME%;";
            string text2 = "\r\n\t\t\tinternal DataColumn %CLNAME%Column\r\n\t\t\t{\r\n\t\t\t\tget\r\n\t\t\t\t{\r\n\t\t\t\t\treturn this.column%CLNAME%;\r\n\t\t\t\t}\r\n\t\t\t}";
            string text3 = "\r\n\t\t\t\tthis.column%CLNAME% = this.Columns[\"%CLNAME%\"];";
            string text4 = "\r\n\t\t\t\tthis.column%CLNAME% = new DataColumn(\"%CLNAME%\", typeof(%CLTYPE%), null, System.Data.MappingType.Element);\r\n\t\t\t\tthis.Columns.Add(this.column%CLNAME%);";
            string text5 = "";
            string text6 = "";
            string text7 = "";
            string text8 = "";
            string text9 = "";
            string text10 = "";
            string text11 = "";
            string text12 = "";
            string text13 = "";
            foreach (DataColumn column in dt.Columns)
            {
                text5 = text5 + text.Replace("%CLNAME%", column.ColumnName);
                text6 = text6 + text2.Replace("%CLNAME%", column.ColumnName);
                text7 = text7 + text3.Replace("%CLNAME%", column.ColumnName);
                text8 = text8 + text4.Replace("%CLNAME%", column.ColumnName).Replace("%CLTYPE%", TypeUtility.GetSimpleDataTypeString(column.DataType));
                if (!column.AllowDBNull)
                {
                    text10 = text10 + "\r\n\t\t\t\tthis.column" + column.ColumnName + ".AllowDBNull = false;";
                }
                if (column.AutoIncrement)
                {
                    text10 = text10 + "\r\n\t\t\t\tthis.column" + column.ColumnName + ".AutoIncrement = true;";
                }
                if (column.ReadOnly)
                {
                    text10 = text10 + "\r\n\t\t\t\tthis.column" + column.ColumnName + ".ReadOnly = true;";
                }
                if (column.Unique)
                {
                    text10 = text10 + "\r\n\t\t\t\tthis.column" + column.ColumnName + ".Unique = true;";
                }
                text16 = text12;
                text12 = text16 + TypeUtility.GetSimpleDataTypeString(column.DataType) + " " + column.ColumnName + ",";
                if (column.AutoIncrement)
                {
                    text13 = text13 + "\r\n\t\t\t\t\t\t\t\t\t\tnull,";
                }
                else
                {
                    text13 = text13 + "\r\n\t\t\t\t\t\t\t\t\t\t" + column.ColumnName + ",";
                }
            }
            text12 = text12.TrimEnd(new char[] { ',' });
            text13 = text13.TrimEnd(new char[] { ',' });
            for (int i = 0; i < dt.PrimaryKey.Length; i++)
            {
                DataColumn column = dt.PrimaryKey[i];
                object obj2 = text9;
                text9 = string.Concat(new object[] { obj2, "\r\n\t\t\t\tthis.Constraints.Add(new UniqueConstraint(\"", dt.DataSet.DataSetName, "Key", i + 1, "\", new DataColumn[]{ this.column", column.ColumnName, "}, true));" });
                text16 = text11;
                text11 = text16 + "\r\n\t\t\tpublic " + dt.TableName + "Row FindBy" + column.ColumnName + "(string " + column.ColumnName + ")\r\n\t\t\t{\r\n\t\t\t\treturn ((" + dt.TableName + "Row)(this.Rows.Find(new object[] { " + column.ColumnName + "})));\r\n\t\t\t}";
            }
            text16 = ("\r\n\t\t[System.Diagnostics.DebuggerStepThrough()]\r\n\t\tpublic class " + dt.TableName + "DataTable : DataTable, System.Collections.IEnumerable\r\n\t\t{") + text5;
            text16 = ((text16 + "\r\n\t\t\tinternal " + dt.TableName + "DataTable() : base(\"" + dt.TableName + "\")\r\n\t\t\t{\r\n\t\t\t\tthis.InitClass();\r\n\t\t\t}") + "\r\n\t\t\tinternal " + dt.TableName + "DataTable(DataTable table) : base(table.TableName)\r\n\t\t\t{\r\n\t\t\t\tif ((table.CaseSensitive != table.DataSet.CaseSensitive))\r\n\t\t\t\t{\r\n\t\t\t\t\tthis.CaseSensitive = table.CaseSensitive;\r\n\t\t\t\t}\r\n\t\t\t\tif ((table.Locale.ToString() != table.DataSet.Locale.ToString())) \r\n\t\t\t\t{\r\n\t\t\t\t\tthis.Locale = table.Locale;\r\n\t\t\t\t}\r\n\t\t\t\tif ((table.Namespace != table.DataSet.Namespace))\r\n\t\t\t\t{\r\n\t\t\t\t\tthis.Namespace = table.Namespace;\r\n\t\t\t\t}\r\n\t\t\t\tthis.Prefix = table.Prefix;\r\n\t\t\t\tthis.MinimumCapacity = table.MinimumCapacity;\r\n\t\t\t\tthis.DisplayExpression = table.DisplayExpression;\r\n\t\t\t}") + "\r\n\t\t\t[System.ComponentModel.Browsable(false)]\r\n\t\t\tpublic int Count\r\n\t\t\t{\r\n\t\t\t\tget\r\n\t\t\t\t{\r\n\t\t\t\t\treturn this.Rows.Count;\r\n\t\t\t\t}\r\n\t\t\t}" + text6;
            text16 = text16 + "\r\n\t\t\tpublic " + dt.TableName + "Row this[int index]\r\n\t\t\t{\r\n\t\t\t\tget\r\n\t\t\t\t{\r\n\t\t\t\t\treturn ((" + dt.TableName + "Row)(this.Rows[index]));\r\n\t\t\t\t}\r\n\t\t\t}";
            text16 = text16 + "\r\n\t\t\tpublic event " + dt.TableName + "RowChangeEventHandler " + dt.TableName + "RowChanged;";
            text16 = text16 + "\r\n\t\t\tpublic event " + dt.TableName + "RowChangeEventHandler " + dt.TableName + "RowChanging;";
            text16 = text16 + "\r\n\t\t\tpublic event " + dt.TableName + "RowChangeEventHandler " + dt.TableName + "RowDeleted;";
            text16 = text16 + "\r\n\t\t\tpublic event " + dt.TableName + "RowChangeEventHandler " + dt.TableName + "RowDeleting;";
            text16 = text16 + "\r\n\t\t\tpublic void Add" + dt.TableName + "Row(" + dt.TableName + "Row row)\r\n\t\t\t{\r\n\t\t\t\tthis.Rows.Add(row);\r\n\t\t\t}";
            text16 = (text16 + "\r\n\t\t\tpublic " + dt.TableName + "Row Add" + dt.TableName + "Row(" + text12 + ")\r\n\t\t\t{\r\n\t\t\t\t" + dt.TableName + "Row row" + dt.TableName + "Row = ((" + dt.TableName + "Row)(this.NewRow()));\r\n\t\t\t\trow" + dt.TableName + "Row.ItemArray = new object[] {") + text13;
            text16 = (text16 + "};\r\n\t\t\t\tthis.Rows.Add(row" + dt.TableName + "Row);\r\n\t\t\t\treturn row" + dt.TableName + "Row;\r\n\t\t\t}") + text11 + "\r\n\t\t\tpublic System.Collections.IEnumerator GetEnumerator()\r\n\t\t\t{\r\n\t\t\t\treturn this.Rows.GetEnumerator();\r\n\t\t\t}";
            text16 = (((((text16 + "\r\n\t\t\tpublic override DataTable Clone()\r\n\t\t\t{\r\n\t\t\t\t" + dt.TableName + "DataTable cln = ((" + dt.TableName + "DataTable)(base.Clone()));\r\n\t\t\t\tcln.InitVars();\r\n\t\t\t\treturn cln;\r\n\t\t\t}") + "\r\n\t\t\tprotected override DataTable CreateInstance() \r\n\t\t\t{\r\n\t\t\t\treturn new " + dt.TableName + "DataTable();\r\n\t\t\t\t}") + "\r\n\t\t\tinternal void InitVars()\r\n\t\t\t{" + text7) + "\r\n\t\t\t}" + "\r\n\t\t\tprivate void InitClass()\r\n\t\t\t{") + text8 + text9) + text10 + "\r\n\t\t\t}";
            text16 = ((text16 + "\r\n\t\t\tpublic " + dt.TableName + "Row New" + dt.TableName + "Row()\r\n\t\t\t{\r\n\t\t\t\treturn ((" + dt.TableName + "Row)(this.NewRow()));\r\n\t\t\t}") + "\r\n\t\t\tprotected override DataRow NewRowFromBuilder(DataRowBuilder builder)\r\n\t\t\t{\r\n\t\t\t\treturn new " + dt.TableName + "Row(builder);\r\n\t\t\t}") + "\r\n\t\t\tprotected override System.Type GetRowType()\r\n\t\t\t{\r\n\t\t\t\treturn typeof(" + dt.TableName + "Row);\r\n\t\t\t}";
            text16 = text16 + "\r\n\t\t\tprotected override void OnRowChanged(DataRowChangeEventArgs e)\r\n\t\t\t{\r\n\t\t\t\tbase.OnRowChanged(e);\r\n\t\t\t\tif ((this." + dt.TableName + "RowChanged != null))\r\n\t\t\t\t{\r\n\t\t\t\t\tthis." + dt.TableName + "RowChanged(this, new " + dt.TableName + "RowChangeEvent(((" + dt.TableName + "Row)(e.Row)), e.Action));\r\n\t\t\t\t}\r\n\t\t\t}";
            text16 = text16 + "\r\n\t\t\tprotected override void OnRowChanging(DataRowChangeEventArgs e)\r\n\t\t\t{\r\n\t\t\t\tbase.OnRowChanging(e);\r\n\t\t\t\tif ((this." + dt.TableName + "RowChanging != null))\r\n\t\t\t\t{\r\n\t\t\t\t\tthis." + dt.TableName + "RowChanging(this, new " + dt.TableName + "RowChangeEvent(((" + dt.TableName + "Row)(e.Row)), e.Action));\r\n\t\t\t\t}\r\n\t\t\t}";
            text16 = text16 + "\r\n\t\t\tprotected override void OnRowDeleted(DataRowChangeEventArgs e)\r\n\t\t\t{\r\n\t\t\t\tbase.OnRowDeleted(e);\r\n\t\t\t\tif ((this." + dt.TableName + "RowDeleted != null))\r\n\t\t\t\t{\r\n\t\t\t\t\tthis." + dt.TableName + "RowDeleted(this, new " + dt.TableName + "RowChangeEvent(((" + dt.TableName + "Row)(e.Row)), e.Action));\r\n\t\t\t\t}\r\n\t\t\t}";
            text16 = text16 + "\r\n\t\t\tprotected override void OnRowDeleting(DataRowChangeEventArgs e)\r\n\t\t\t{\r\n\t\t\t\tbase.OnRowDeleting(e);\r\n\t\t\t\tif ((this." + dt.TableName + "RowDeleting != null))\r\n\t\t\t\t{\r\n\t\t\t\t\tthis." + dt.TableName + "RowDeleting(this, new " + dt.TableName + "RowChangeEvent(((" + dt.TableName + "Row)(e.Row)), e.Action));\r\n\t\t\t\t}\r\n\t\t\t}";
            return ((text16 + "\r\n\t\t\tpublic void Remove" + dt.TableName + "Row(" + dt.TableName + "Row row)\r\n\t\t\t{\r\n\t\t\t\tthis.Rows.Remove(row);\r\n\t\t\t}") + "\r\n\t\t}");
        }

        private void InitializeComponent()
        {
            this.panel1 = new Panel();
            this.panel8 = new Panel();
            this.progressBar1 = new ProgressBar();
            this.panel3 = new Panel();
            this.button_CreateDataSets = new Button();
            this.button_SelectAll = new Button();
            this.button_UnSelectAll = new Button();
            this.button_GetTableList = new Button();
            this.panel7 = new Panel();
            this.checkBox_WriteOutput = new CheckBox();
            this.checkBox_SaveToFile = new CheckBox();
            this.panel6 = new Panel();
            this.textBox_ClassNamespace = new TextBox();
            this.checkBox_CreateClass = new CheckBox();
            this.panel5 = new Panel();
            this.textBox_SavePath = new TextBox();
            this.button_GetSaveFolder = new Button();
            this.checkBox_CreateXsd = new CheckBox();
            this.panel4 = new Panel();
            this.textBox_Namespace = new TextBox();
            this.label3 = new Label();
            this.panel2 = new Panel();
            this.textBox_DBConnection = new TextBox();
            this.label1 = new Label();
            this.button_GetConnectionString = new Button();
            this.listView_TableList = new ListView();
            this.columnHeader1 = new ColumnHeader();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0xe8, 0xa8);
            this.panel1.TabIndex = 1;
            this.panel8.Controls.Add(this.progressBar1);
            this.panel8.Dock = DockStyle.Top;
            this.panel8.Location = new Point(0, 0x84);
            this.panel8.Name = "panel8";
            this.panel8.Size = new Size(0xe8, 0x18);
            this.panel8.TabIndex = 9;
            this.progressBar1.Dock = DockStyle.Fill;
            this.progressBar1.Location = new Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(0xe8, 0x18);
            this.progressBar1.TabIndex = 0;
            this.panel3.Controls.Add(this.button_CreateDataSets);
            this.panel3.Controls.Add(this.button_SelectAll);
            this.panel3.Controls.Add(this.button_UnSelectAll);
            this.panel3.Controls.Add(this.button_GetTableList);
            this.panel3.Dock = DockStyle.Top;
            this.panel3.Location = new Point(0, 0x6c);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(0xe8, 0x18);
            this.panel3.TabIndex = 4;
            this.button_CreateDataSets.Dock = DockStyle.Left;
            this.button_CreateDataSets.FlatStyle = FlatStyle.Popup;
            this.button_CreateDataSets.Location = new Point(40, 0);
            this.button_CreateDataSets.Name = "button_CreateDataSets";
            this.button_CreateDataSets.Size = new Size(40, 0x18);
            this.button_CreateDataSets.TabIndex = 4;
            this.button_CreateDataSets.Text = "生成";
            this.button_CreateDataSets.Click += new EventHandler(this.button_CreateDataSets_Click);
            this.button_SelectAll.Dock = DockStyle.Right;
            this.button_SelectAll.FlatStyle = FlatStyle.Popup;
            this.button_SelectAll.Location = new Point(0x98, 0);
            this.button_SelectAll.Name = "button_SelectAll";
            this.button_SelectAll.Size = new Size(40, 0x18);
            this.button_SelectAll.TabIndex = 3;
            this.button_SelectAll.Text = "全选";
            this.button_SelectAll.Click += new EventHandler(this.button_SelectAll_Click);
            this.button_UnSelectAll.Dock = DockStyle.Right;
            this.button_UnSelectAll.FlatStyle = FlatStyle.Popup;
            this.button_UnSelectAll.Location = new Point(0xc0, 0);
            this.button_UnSelectAll.Name = "button_UnSelectAll";
            this.button_UnSelectAll.Size = new Size(40, 0x18);
            this.button_UnSelectAll.TabIndex = 2;
            this.button_UnSelectAll.Text = "清除";
            this.button_UnSelectAll.Click += new EventHandler(this.button_UnSelectAll_Click);
            this.button_GetTableList.Dock = DockStyle.Left;
            this.button_GetTableList.FlatStyle = FlatStyle.Popup;
            this.button_GetTableList.Location = new Point(0, 0);
            this.button_GetTableList.Name = "button_GetTableList";
            this.button_GetTableList.Size = new Size(40, 0x18);
            this.button_GetTableList.TabIndex = 1;
            this.button_GetTableList.Text = "列表";
            this.button_GetTableList.Click += new EventHandler(this.button_GetTableList_Click);
            this.panel7.Controls.Add(this.checkBox_WriteOutput);
            this.panel7.Controls.Add(this.checkBox_SaveToFile);
            this.panel7.Dock = DockStyle.Top;
            this.panel7.Location = new Point(0, 0x57);
            this.panel7.Name = "panel7";
            this.panel7.Size = new Size(0xe8, 0x15);
            this.panel7.TabIndex = 8;
            this.checkBox_WriteOutput.Checked = true;
            this.checkBox_WriteOutput.CheckState = CheckState.Checked;
            this.checkBox_WriteOutput.Dock = DockStyle.Right;
            this.checkBox_WriteOutput.Location = new Point(0x90, 0);
            this.checkBox_WriteOutput.Name = "checkBox_WriteOutput";
            this.checkBox_WriteOutput.Size = new Size(0x58, 0x15);
            this.checkBox_WriteOutput.TabIndex = 1;
            this.checkBox_WriteOutput.Text = "显示输出";
            this.checkBox_SaveToFile.Checked = true;
            this.checkBox_SaveToFile.CheckState = CheckState.Checked;
            this.checkBox_SaveToFile.Dock = DockStyle.Left;
            this.checkBox_SaveToFile.Location = new Point(0, 0);
            this.checkBox_SaveToFile.Name = "checkBox_SaveToFile";
            this.checkBox_SaveToFile.Size = new Size(0x58, 0x15);
            this.checkBox_SaveToFile.TabIndex = 0;
            this.checkBox_SaveToFile.Text = "保存成文件";
            this.panel6.Controls.Add(this.textBox_ClassNamespace);
            this.panel6.Controls.Add(this.checkBox_CreateClass);
            this.panel6.Dock = DockStyle.Top;
            this.panel6.Location = new Point(0, 0x42);
            this.panel6.Name = "panel6";
            this.panel6.Size = new Size(0xe8, 0x15);
            this.panel6.TabIndex = 7;
            this.textBox_ClassNamespace.Dock = DockStyle.Fill;
            this.textBox_ClassNamespace.Location = new Point(0x48, 0);
            this.textBox_ClassNamespace.Name = "textBox_ClassNamespace";
            this.textBox_ClassNamespace.Size = new Size(160, 0x15);
            this.textBox_ClassNamespace.TabIndex = 5;
            this.textBox_ClassNamespace.Text = "Example.Common.DataSets";
            this.checkBox_CreateClass.Dock = DockStyle.Left;
            this.checkBox_CreateClass.Location = new Point(0, 0);
            this.checkBox_CreateClass.Name = "checkBox_CreateClass";
            this.checkBox_CreateClass.Size = new Size(0x48, 0x15);
            this.checkBox_CreateClass.TabIndex = 0;
            this.checkBox_CreateClass.Text = "生成类";
            this.panel5.Controls.Add(this.textBox_SavePath);
            this.panel5.Controls.Add(this.button_GetSaveFolder);
            this.panel5.Controls.Add(this.checkBox_CreateXsd);
            this.panel5.Dock = DockStyle.Top;
            this.panel5.Location = new Point(0, 0x2d);
            this.panel5.Name = "panel5";
            this.panel5.Size = new Size(0xe8, 0x15);
            this.panel5.TabIndex = 6;
            this.textBox_SavePath.Dock = DockStyle.Fill;
            this.textBox_SavePath.Location = new Point(0x48, 0);
            this.textBox_SavePath.Name = "textBox_SavePath";
            this.textBox_SavePath.Size = new Size(0x88, 0x15);
            this.textBox_SavePath.TabIndex = 2;
            this.textBox_SavePath.Text = @"C:\DS";
            this.button_GetSaveFolder.Dock = DockStyle.Right;
            this.button_GetSaveFolder.FlatStyle = FlatStyle.Popup;
            this.button_GetSaveFolder.Location = new Point(0xd0, 0);
            this.button_GetSaveFolder.Name = "button_GetSaveFolder";
            this.button_GetSaveFolder.Size = new Size(0x18, 0x15);
            this.button_GetSaveFolder.TabIndex = 6;
            this.button_GetSaveFolder.Text = "…";
            this.button_GetSaveFolder.Click += new EventHandler(this.button_GetSaveFolder_Click);
            this.checkBox_CreateXsd.Checked = true;
            this.checkBox_CreateXsd.CheckState = CheckState.Checked;
            this.checkBox_CreateXsd.Dock = DockStyle.Left;
            this.checkBox_CreateXsd.Location = new Point(0, 0);
            this.checkBox_CreateXsd.Name = "checkBox_CreateXsd";
            this.checkBox_CreateXsd.Size = new Size(0x48, 0x15);
            this.checkBox_CreateXsd.TabIndex = 4;
            this.checkBox_CreateXsd.Text = "生成Xsd";
            this.panel4.Controls.Add(this.textBox_Namespace);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Dock = DockStyle.Top;
            this.panel4.Location = new Point(0, 0x15);
            this.panel4.Name = "panel4";
            this.panel4.Size = new Size(0xe8, 0x18);
            this.panel4.TabIndex = 5;
            this.textBox_Namespace.Dock = DockStyle.Fill;
            this.textBox_Namespace.Location = new Point(0x48, 0);
            this.textBox_Namespace.Name = "textBox_Namespace";
            this.textBox_Namespace.Size = new Size(160, 0x15);
            this.textBox_Namespace.TabIndex = 4;
            this.textBox_Namespace.Text = "http://www.taji.com.cn/tjzf/crj/";
            this.label3.Dock = DockStyle.Left;
            this.label3.Location = new Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x48, 0x18);
            this.label3.TabIndex = 5;
            this.label3.Text = "命名空间：";
            this.label3.TextAlign = ContentAlignment.MiddleRight;
            this.panel2.Controls.Add(this.textBox_DBConnection);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.button_GetConnectionString);
            this.panel2.Dock = DockStyle.Top;
            this.panel2.Location = new Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0xe8, 0x15);
            this.panel2.TabIndex = 3;
            this.textBox_DBConnection.Dock = DockStyle.Fill;
            this.textBox_DBConnection.Location = new Point(0x48, 0);
            this.textBox_DBConnection.Name = "textBox_DBConnection";
            this.textBox_DBConnection.ReadOnly = true;
            this.textBox_DBConnection.Size = new Size(0x88, 0x15);
            this.textBox_DBConnection.TabIndex = 2;
            this.textBox_DBConnection.Text = "Provider=MSDAORA.1;Password=gacrj;User ID=gacrj;Data Source=gacrj;Persist Security Info=True";
            this.label1.Dock = DockStyle.Left;
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x48, 0x15);
            this.label1.TabIndex = 3;
            this.label1.Text = "连接：";
            this.label1.TextAlign = ContentAlignment.MiddleRight;
            this.button_GetConnectionString.Dock = DockStyle.Right;
            this.button_GetConnectionString.FlatStyle = FlatStyle.Popup;
            this.button_GetConnectionString.Location = new Point(0xd0, 0);
            this.button_GetConnectionString.Name = "button_GetConnectionString";
            this.button_GetConnectionString.Size = new Size(0x18, 0x15);
            this.button_GetConnectionString.TabIndex = 5;
            this.button_GetConnectionString.Text = "…";
            this.button_GetConnectionString.Click += new EventHandler(this.button_GetConnectionString_Click);
            this.listView_TableList.BorderStyle = BorderStyle.FixedSingle;
            this.listView_TableList.CheckBoxes = true;
            this.listView_TableList.Columns.AddRange(new ColumnHeader[] { this.columnHeader1 });
            this.listView_TableList.Dock = DockStyle.Fill;
            this.listView_TableList.FullRowSelect = true;
            this.listView_TableList.GridLines = true;
            this.listView_TableList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.listView_TableList.Location = new Point(0, 0xa8);
            this.listView_TableList.MultiSelect = false;
            this.listView_TableList.Name = "listView_TableList";
            this.listView_TableList.Size = new Size(0xe8, 0x150);
            this.listView_TableList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_TableList.TabIndex = 2;
            this.listView_TableList.View = View.Details;
            this.columnHeader1.Text = "表名";
            this.columnHeader1.Width = 0xcc;
            base.Controls.Add(this.listView_TableList);
            base.Controls.Add(this.panel1);
            base.Name = "DataSetCreator";
            base.Size = new Size(0xe8, 0x1f8);
            base.SizeChanged += new EventHandler(this.DataSetCreator_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private class DBTypes
        {
            public System.Type AdapterType;
            public System.Type ConnectionType;

            public DBTypes(System.Type connectionType, System.Type adapterType)
            {
                this.ConnectionType = connectionType;
                this.AdapterType = adapterType;
            }
        }
    }
}
