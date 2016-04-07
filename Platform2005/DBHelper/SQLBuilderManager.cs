namespace Platform.DBHelper
{
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Data;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class SQLBuilderManager
    {
        private Type m_ConditionType = typeof(SQLCondition);
        private Hashtable m_MapSQLCondition = new Hashtable();
        private Hashtable m_MapSQLRelation = new Hashtable();
        private Hashtable m_MapSQLResult = new Hashtable();
        private Hashtable m_MapSQLResultItem = new Hashtable();
        private Hashtable m_MapSQLSection = new Hashtable();
        private Type m_RelationType = typeof(SQLRelation);
        private Type m_ResultItemType = typeof(SQLResultItem);
        private Type m_ResultType = typeof(SQLResult);
        internal Hashtable m_Sections = new Hashtable();
        private Type m_SectionType = typeof(SQLSection);
        private static string[] MapSQLCondition;
        private static string[] MapSQLRelation;
        private static string[] MapSQLResult;
        private static string[] MapSQLResultItem;

        static SQLBuilderManager()
        {
            FieldInfo[] fields = typeof(SQLResult).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ArrayList list = new ArrayList();
            foreach (FieldInfo info in fields)
            {
                if (info.GetCustomAttributes(typeof(FillFieldAttribute), false).Length > 0)
                {
                    list.Add(info.Name);
                }
            }
            MapSQLResult = list.ToArray(typeof(string)) as string[];
            list.Clear();
            fields = typeof(SQLResultItem).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            list = new ArrayList();
            foreach (FieldInfo info2 in fields)
            {
                if (info2.GetCustomAttributes(typeof(FillFieldAttribute), false).Length > 0)
                {
                    list.Add(info2.Name);
                }
            }
            MapSQLResultItem = list.ToArray(typeof(string)) as string[];
            list.Clear();
            fields = typeof(SQLRelation).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            list = new ArrayList();
            foreach (FieldInfo info3 in fields)
            {
                if (info3.GetCustomAttributes(typeof(FillFieldAttribute), false).Length > 0)
                {
                    list.Add(info3.Name);
                }
            }
            MapSQLRelation = list.ToArray(typeof(string)) as string[];
            list.Clear();
            fields = typeof(SQLCondition).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            list = new ArrayList();
            foreach (FieldInfo info4 in fields)
            {
                if (info4.GetCustomAttributes(typeof(FillFieldAttribute), false).Length > 0)
                {
                    list.Add(info4.Name);
                }
            }
            MapSQLCondition = list.ToArray(typeof(string)) as string[];
        }

        public SQLSection GetSection(string sectionName)
        {
            return (this.m_Sections[sectionName] as SQLSection);
        }

        public string GetSql(string sectionName, string[] fields, bool[] dmFields, string conditionName, string[] parameters)
        {
            Hashtable hashtable;
            string text = this.MakeSQLTemplate(sectionName, fields, dmFields, conditionName, false, out hashtable);
            if (text == null)
            {
                return null;
            }
            string text2 = this.MakeConditionTemplate(sectionName, conditionName, parameters);
            if (text2 == null)
            {
                return null;
            }
            SQLSection section = this.m_Sections[sectionName] as SQLSection;
            if (section == null)
            {
                return null;
            }
            if ((section.ADDEND != null) && (section.ADDEND != ""))
            {
                return (text + text2 + " " + section.ADDEND);
            }
            return (text + text2);
        }

        public SQLCondition GetSql(string sectionName, string[] fields, bool[] dmFields, string conditionName, string[] parameters, out string sql)
        {
            Hashtable hashtable;
            string text2;
            sql = null;
            string text = this.MakeSQLTemplate(sectionName, fields, dmFields, conditionName, false, out hashtable);
            if (text == null)
            {
                return null;
            }
            SQLCondition condition = this.MakeConditionTemplate(sectionName, conditionName, parameters, out text2);
            if (condition == null)
            {
                return null;
            }
            SQLSection section = this.m_Sections[sectionName] as SQLSection;
            if (section == null)
            {
                return null;
            }
            if ((section.ADDEND != null) && (section.ADDEND != ""))
            {
                sql = text + text2 + " " + section.ADDEND;
                return condition;
            }
            sql = text + text2;
            return condition;
        }

        public string GetSql(string sectionName, string[] fields, bool[] dmFields, string conditionName, string[] parameters, bool makeBinSql, out Hashtable binSqlTable)
        {
            string text = this.MakeSQLTemplate(sectionName, fields, dmFields, conditionName, makeBinSql, out binSqlTable);
            if (text == null)
            {
                return null;
            }
            string text2 = this.MakeConditionTemplate(sectionName, conditionName, parameters);
            if (text2 == null)
            {
                return null;
            }
            SQLSection section = this.m_Sections[sectionName] as SQLSection;
            if (section == null)
            {
                return null;
            }
            if ((section.ADDEND != null) && (section.ADDEND != ""))
            {
                return (text + text2 + " " + section.ADDEND);
            }
            return (text + text2);
        }

        public SQLCondition GetSql(string sectionName, string[] fields, bool[] dmFields, string conditionName, string[] parameters, out string sql, bool makeBinSql, out Hashtable binSqlTable)
        {
            string text2;
            sql = null;
            string text = this.MakeSQLTemplate(sectionName, fields, dmFields, conditionName, makeBinSql, out binSqlTable);
            if (text == null)
            {
                return null;
            }
            SQLCondition condition = this.MakeConditionTemplate(sectionName, conditionName, parameters, out text2);
            if (condition == null)
            {
                return null;
            }
            SQLSection section = this.m_Sections[sectionName] as SQLSection;
            if (section == null)
            {
                return null;
            }
            if ((section.ADDEND != null) && (section.ADDEND != ""))
            {
                sql = text + text2 + " " + section.ADDEND;
                return condition;
            }
            sql = text + text2;
            return condition;
        }

        public bool InitizlizeBuilderInfo(DataRow sectionRow, string[] relations)
        {
            SQLSection section = Activator.CreateInstance(this.m_SectionType) as SQLSection;
            if (section == null)
            {
                return false;
            }
            FillUtility.FillFields(section, sectionRow, false, this.m_MapSQLSection);
            if ((section.SNAME == null) || (section.SNAME == ""))
            {
                return false;
            }
            section.Regular();
            section.OnBeforeInitialize();
            foreach (DataRow row in sectionRow.GetChildRows(relations[0]))
            {
                SQLResult result = Activator.CreateInstance(this.m_ResultType) as SQLResult;
                result.m_Section = section;
                if (result != null)
                {
                    FillUtility.FillFields(result, row, false, this.m_MapSQLResult);
                    if ((result.DFIELD != null) && (result.DFIELD != ""))
                    {
                        section.m_Results[result.DFIELD] = result;
                        section.m_ResultsList.Add(result);
                        foreach (DataRow row2 in row.GetChildRows(relations[3]))
                        {
                            SQLResultItem item = Activator.CreateInstance(this.m_ResultItemType) as SQLResultItem;
                            if (item != null)
                            {
                                FillUtility.FillFields(item, row2, false, this.m_MapSQLResultItem);
                                if (((item.STABLE != null) && (item.STABLE != "")) && ((item.SFIELD != null) && (item.SFIELD != "")))
                                {
                                    result.m_Results.Add(item);
                                }
                            }
                        }
                        result.MakeSQLTemplate();
                    }
                }
            }
            foreach (DataRow row3 in sectionRow.GetChildRows(relations[1]))
            {
                SQLRelation relation = Activator.CreateInstance(this.m_RelationType) as SQLRelation;
                if (relation != null)
                {
                    FillUtility.FillFields(relation, row3, false, this.m_MapSQLRelation);
                    if (((relation.TABLES != null) && (relation.TABLES != "")) && ((relation.RELATION != null) && (relation.RELATION != "")))
                    {
                        relation.Regular();
                        section.m_Relations.Add(relation);
                    }
                }
            }
            foreach (DataRow row4 in sectionRow.GetChildRows(relations[2]))
            {
                SQLCondition condition = Activator.CreateInstance(this.m_ConditionType) as SQLCondition;
                if (condition != null)
                {
                    FillUtility.FillFields(condition, row4, false, this.m_MapSQLCondition);
                    if (((condition.CNAME != null) && (condition.CNAME != "")) && ((condition.CONDITIONSTRING != null) && (condition.CONDITIONSTRING != "")))
                    {
                        condition.Regular();
                        section.m_Conditions[condition.CNAME] = condition;
                    }
                }
            }
            section.OnAfterInitialize();
            this.m_Sections[section.SNAME] = section;
            return true;
        }

        public bool InitizlizeBuilderInfo(DataSet ds, string mainTable, string[] relations)
        {
            if (!ds.Tables.Contains(mainTable))
            {
                return false;
            }
            foreach (DataRow row in ds.Tables[mainTable].Rows)
            {
                this.InitizlizeBuilderInfo(row, relations);
            }
            return true;
        }

        public bool InitizlizeBuilderInfo(DataTable sectionTable, string sectionIndex, DataTable resultTable, string toResultCondition, string resultIndex, DataTable resultItemTable, string toResultItemCondition, DataTable relationTable, string toRelationCondition, DataTable conditionTable, string toConditionCondition)
        {
            return this.InitizlizeBuilderInfo(sectionTable, sectionIndex, resultTable, toResultCondition, resultIndex, null, resultItemTable, toResultItemCondition, null, relationTable, toRelationCondition, conditionTable, toConditionCondition);
        }

        public bool InitizlizeBuilderInfo(DataRow sectionRow, string sectionIndex, DataTable resultTable, string toResultCondition, string resultIndex, DataTable resultItemTable, string toResultItemCondition, string resultItemOrder, DataTable relationTable, string toRelationCondition, DataTable conditionTable, string toConditionCondition)
        {
            return this.InitizlizeBuilderInfo(sectionRow, sectionIndex, resultTable, toResultCondition, resultIndex, null, resultItemTable, toResultItemCondition, resultItemOrder, relationTable, toRelationCondition, conditionTable, toConditionCondition);
        }

        public bool InitizlizeBuilderInfo(DataTable sectionTable, string sectionIndex, DataTable resultTable, string toResultCondition, string resultIndex, DataTable resultItemTable, string toResultItemCondition, string resultItemOrder, DataTable relationTable, string toRelationCondition, DataTable conditionTable, string toConditionCondition)
        {
            return this.InitizlizeBuilderInfo(sectionTable, sectionIndex, resultTable, toResultCondition, resultIndex, null, resultItemTable, toResultItemCondition, resultItemOrder, relationTable, toRelationCondition, conditionTable, toConditionCondition);
        }

        public bool InitizlizeBuilderInfo(DataRow sectionRow, string sectionIndex, DataTable resultTable, string toResultCondition, string resultIndex, string resultOrder, DataTable resultItemTable, string toResultItemCondition, string resultItemOrder, DataTable relationTable, string toRelationCondition, DataTable conditionTable, string toConditionCondition)
        {
            SQLSection section = Activator.CreateInstance(this.m_SectionType) as SQLSection;
            if (section == null)
            {
                return false;
            }
            FillUtility.FillFields(section, sectionRow, false, this.m_MapSQLSection);
            if ((section.SNAME == null) || (section.SNAME == ""))
            {
                return false;
            }
            section.Regular();
            section.OnBeforeInitialize();
            string text = sectionRow[sectionIndex].ToString();
            foreach (DataRow row in resultTable.Select(string.Format(toResultCondition, text), resultOrder))
            {
                SQLResult result = Activator.CreateInstance(this.m_ResultType) as SQLResult;
                result.m_Section = section;
                if (result != null)
                {
                    FillUtility.FillFields(result, row, false, this.m_MapSQLResult);
                    if ((result.DFIELD != null) && (result.DFIELD != ""))
                    {
                        section.m_Results[result.DFIELD] = result;
                        section.m_ResultsList.Add(result);
                        string text2 = row[resultIndex].ToString();
                        foreach (DataRow row2 in resultItemTable.Select(string.Format(toResultItemCondition, text, text2), resultItemOrder))
                        {
                            SQLResultItem item = Activator.CreateInstance(this.m_ResultItemType) as SQLResultItem;
                            if (item != null)
                            {
                                FillUtility.FillFields(item, row2, false, this.m_MapSQLResultItem);
                                if (((item.STABLE != null) && (item.STABLE != "")) && ((item.SFIELD != null) && (item.SFIELD != "")))
                                {
                                    result.m_Results.Add(item);
                                }
                            }
                        }
                        result.MakeSQLTemplate();
                    }
                }
            }
            foreach (DataRow row3 in relationTable.Select(string.Format(toRelationCondition, text)))
            {
                SQLRelation relation = Activator.CreateInstance(this.m_RelationType) as SQLRelation;
                if (relation != null)
                {
                    FillUtility.FillFields(relation, row3, false, this.m_MapSQLRelation);
                    if (((relation.TABLES != null) && (relation.TABLES != "")) && ((relation.RELATION != null) && (relation.RELATION != "")))
                    {
                        relation.Regular();
                        section.m_Relations.Add(relation);
                    }
                }
            }
            foreach (DataRow row4 in conditionTable.Select(string.Format(toConditionCondition, text)))
            {
                SQLCondition condition = Activator.CreateInstance(this.m_ConditionType) as SQLCondition;
                if (condition != null)
                {
                    FillUtility.FillFields(condition, row4, false, this.m_MapSQLCondition);
                    if (((condition.CNAME != null) && (condition.CNAME != "")) && ((condition.CONDITIONSTRING != null) && (condition.CONDITIONSTRING != "")))
                    {
                        condition.Regular();
                        section.m_Conditions[condition.CNAME] = condition;
                    }
                }
            }
            section.OnAfterInitialize();
            this.m_Sections[section.SNAME] = section;
            return true;
        }

        public bool InitizlizeBuilderInfo(DataTable sectionTable, string sectionIndex, DataTable resultTable, string toResultCondition, string resultIndex, string resultOrder, DataTable resultItemTable, string toResultItemCondition, string resultItemOrder, DataTable relationTable, string toRelationCondition, DataTable conditionTable, string toConditionCondition)
        {
            foreach (DataRow row in sectionTable.Rows)
            {
                this.InitizlizeBuilderInfo(row, sectionIndex, resultTable, toResultCondition, resultIndex, resultOrder, resultItemTable, toResultItemCondition, resultItemOrder, relationTable, toRelationCondition, conditionTable, toConditionCondition);
            }
            return true;
        }

        public string MakeConditionTemplate(string sectionName, string conditionName, string[] parameters)
        {
            if ((conditionName != null) && (conditionName != ""))
            {
                SQLSection section = this.m_Sections[sectionName] as SQLSection;
                if (section != null)
                {
                    SQLCondition condition = section.m_Conditions[conditionName] as SQLCondition;
                    if (condition == null)
                    {
                        return null;
                    }
                    if ((condition.CONDITIONSTRING != null) && (condition.CONDITIONSTRING != ""))
                    {
                        return string.Format(condition.CONDITIONSTRING, (object[])parameters);
                    }
                }
            }
            return null;
        }

        public SQLCondition MakeConditionTemplate(string sectionName, string conditionName, string[] parameters, out string conditionResult)
        {
            conditionResult = null;
            if ((conditionName == null) || (conditionName == ""))
            {
                return null;
            }
            SQLSection section = this.m_Sections[sectionName] as SQLSection;
            if (section == null)
            {
                return null;
            }
            SQLCondition condition = section.m_Conditions[conditionName] as SQLCondition;
            if (condition == null)
            {
                return null;
            }
            if ((condition.CONDITIONSTRING == null) || (condition.CONDITIONSTRING == ""))
            {
                return null;
            }
            conditionResult = string.Format(condition.CONDITIONSTRING, (object[])parameters);
            return condition;
        }

        public string MakeSQLTemplate(string sectionName, string[] fields, bool[] dmFields, string conditionName, bool makeExtSql, out Hashtable extSqls)
        {
            extSqls = null;
            if (((fields != null) && (dmFields == null)) || ((fields == null) && (dmFields != null)))
            {
                return null;
            }
            SQLSection section = this.m_Sections[sectionName] as SQLSection;
            if (section == null)
            {
                return null;
            }
            ulong hashCode = (ulong)0;
            if (fields != null)
            {
                hashCode = (ulong)(fields.GetHashCode().ToString() + dmFields.GetHashCode().ToString() + makeExtSql.GetHashCode()).GetHashCode();
            }
            if (!string.IsNullOrEmpty( conditionName ))
            {
                hashCode = hashCode | (ulong)conditionName.GetHashCode();
            }
            string text = section.Sqls[hashCode] as string;
            if (makeExtSql)
            {
                extSqls = section.ExtSqls[hashCode] as Hashtable;
            }
            if (text == null)
            {
                StringBuilder builder = new StringBuilder("SELECT ");
                if (!string.IsNullOrEmpty( section.RULE ))
                {
                    builder.Append(section.RULE);
                    builder.Append(" ");
                }
                StringBuilder builder2 = new StringBuilder(" WHERE ");
                Hashtable hashtable = new Hashtable();
                if (makeExtSql)
                {
                    extSqls = new Hashtable();
                }
                bool flag = false;
                if ((fields != null) && (fields.Length != 0))
                {
                    for (int i = 0; i < fields.Length; i++)
                    {
                        SQLResult result2 = section.m_Results[fields[i]] as SQLResult;
                        if (result2 != null)
                        {
                            flag = true;
                            if (makeExtSql && (result2.M_SEPRESULTSQL != ""))
                            {
                                extSqls[result2.DFIELD] = result2.M_SEPRESULTSQL;
                                builder.Append(result2.M_RESULTSQL);
                                builder2.Append(result2.M_CONDITION);
                            }
                            else if ((result2.M_DMTABLE != "") && !dmFields[i])
                            {
                                builder.Append(result2.M_DMRESULTSQL);
                                if (!hashtable.ContainsKey(result2.M_DMTABLE))
                                {
                                    hashtable[result2.M_DMTABLE] = "";
                                }
                                builder2.Append(result2.M_DMCONDITION);
                            }
                            else
                            {
                                builder.Append(result2.M_RESULTSQL);
                                builder2.Append(result2.M_CONDITION);
                            }
                            builder.Append(',');
                            foreach (DictionaryEntry entry2 in result2.M_DMFORMSQL)
                            {
                                if (!hashtable.ContainsKey(entry2.Key))
                                {
                                    hashtable[entry2.Key] = entry2.Value;
                                    continue;
                                }
                                hashtable[entry2.Key] = (hashtable[entry2.Key] as string) + entry2.Value;
                            }
                        }
                    }
                }
                else
                {
                    foreach (SQLResult result in section.m_ResultsList)
                    {
                        flag = true;
                        if (makeExtSql && (result.M_SEPRESULTSQL != ""))
                        {
                            extSqls[result.DFIELD] = result.M_SEPRESULTSQL;
                            builder.Append(result.M_RESULTSQL);
                            builder2.Append(result.M_CONDITION);
                        }
                        else if (result.M_DMTABLE != "")
                        {
                            builder.Append(result.M_DMRESULTSQL);
                            if (!hashtable.ContainsKey(result.M_DMTABLE))
                            {
                                hashtable[result.M_DMTABLE] = "";
                            }
                            builder2.Append(result.M_DMCONDITION);
                        }
                        else
                        {
                            builder.Append(result.M_RESULTSQL);
                            builder2.Append(result.M_CONDITION);
                        }
                        builder.Append(',');
                        foreach (DictionaryEntry entry in result.M_DMFORMSQL)
                        {
                            if (!hashtable.ContainsKey(entry.Key))
                            {
                                hashtable[entry.Key] = entry.Value;
                                continue;
                            }
                            hashtable[entry.Key] = (hashtable[entry.Key] as string) + entry.Value;
                        }
                    }
                }
                if (flag)
                {
                    builder.Length--;
                }
                builder.Append(" FROM ");
                if ((conditionName != null) && (conditionName != ""))
                {
                    SQLCondition condition = section.Conditions[conditionName] as SQLCondition;
                    if (condition != null)
                    {
                        foreach (string text2 in condition.Tables)
                        {
                            if (!hashtable.ContainsKey(text2))
                            {
                                hashtable[text2] = "";
                            }
                        }
                    }
                }
                flag = false;
                foreach (DictionaryEntry entry3 in hashtable)
                {
                    builder.Append(entry3.Key as string);
                    builder.Append(entry3.Value as string);
                    builder.Append(',');
                    flag = true;
                }
                if (flag)
                {
                    builder.Length--;
                }
                builder.Append(builder2.ToString());
                foreach (SQLRelation relation in section.m_Relations)
                {
                    bool flag2 = true;
                    foreach (string text3 in relation.Tables)
                    {
                        if (!hashtable.ContainsKey(text3))
                        {
                            flag2 = false;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        builder.Append(relation.RELATION);
                        builder.Append(" AND ");
                    }
                }
                text = builder.ToString();
                section.Sqls[hashCode] = text;
                if (makeExtSql)
                {
                    section.ExtSqls[hashCode] = extSqls;
                }
            }
            return text;
        }

        public bool SetupFieldMap(SQLResultMap resultFields, SQLResultItemMap resultItemFields, SQLRelationMap relationFields, SQLConditionMap conditionFields)
        {
            this.m_MapSQLResult.Clear();
            this.m_MapSQLResultItem.Clear();
            this.m_MapSQLRelation.Clear();
            this.m_MapSQLCondition.Clear();
            for (int i = 0; i < MapSQLResult.Length; i++)
            {
                this.m_MapSQLResult[MapSQLResult[i]] = typeof(SQLResultMap).GetField(MapSQLResult[i]).GetValue(resultFields);
            }
            for (int j = 0; j < MapSQLResultItem.Length; j++)
            {
                this.m_MapSQLResultItem[MapSQLResultItem[j]] = typeof(SQLResultItemMap).GetField(MapSQLResultItem[j]).GetValue(resultItemFields);
            }
            for (int k = 0; k < MapSQLRelation.Length; k++)
            {
                this.m_MapSQLRelation[MapSQLRelation[k]] = typeof(SQLRelationMap).GetField(MapSQLRelation[k]).GetValue(relationFields);
            }
            for (int m = 0; m < MapSQLCondition.Length; m++)
            {
                this.m_MapSQLCondition[MapSQLCondition[m]] = typeof(SQLConditionMap).GetField(MapSQLCondition[m]).GetValue(conditionFields);
            }
            return true;
        }

        public bool SetupFieldMap(string[] resultFields, string[] resultItemFields, string[] relationFields, string[] conditionFields)
        {
            if (resultFields.Length != MapSQLResult.Length)
            {
                return false;
            }
            if (resultItemFields.Length != MapSQLResultItem.Length)
            {
                return false;
            }
            if (relationFields.Length != MapSQLRelation.Length)
            {
                return false;
            }
            if (conditionFields.Length != MapSQLCondition.Length)
            {
                return false;
            }
            this.m_MapSQLResult.Clear();
            this.m_MapSQLResultItem.Clear();
            this.m_MapSQLRelation.Clear();
            this.m_MapSQLCondition.Clear();
            for (int i = 0; i < MapSQLResult.Length; i++)
            {
                this.m_MapSQLResult[MapSQLResult[i]] = resultFields[i];
            }
            for (int j = 0; j < MapSQLResultItem.Length; j++)
            {
                this.m_MapSQLResultItem[MapSQLResultItem[j]] = resultItemFields[j];
            }
            for (int k = 0; k < MapSQLRelation.Length; k++)
            {
                this.m_MapSQLRelation[MapSQLRelation[k]] = relationFields[k];
            }
            for (int m = 0; m < MapSQLCondition.Length; m++)
            {
                this.m_MapSQLCondition[MapSQLCondition[m]] = conditionFields[m];
            }
            return true;
        }

        public Type ConditionType
        {
            get
            {
                return this.m_ConditionType;
            }
            set
            {
                if (value == null)
                {
                    this.m_ConditionType = typeof(SQLCondition);
                }
                else if (value.IsSubclassOf(typeof(SQLCondition)) || (value == typeof(SQLCondition)))
                {
                    this.m_ConditionType = value;
                }
            }
        }

        public Type RelationType
        {
            get
            {
                return this.m_RelationType;
            }
            set
            {
                if (value == null)
                {
                    this.m_RelationType = typeof(SQLRelation);
                }
                else if (!value.IsSubclassOf(typeof(SQLRelation)) && (value != typeof(SQLRelation)))
                {
                    this.m_RelationType = value;
                }
            }
        }

        public Type ResultItemType
        {
            get
            {
                return this.m_ResultItemType;
            }
            set
            {
                if (value == null)
                {
                    this.m_ResultItemType = typeof(SQLResultItem);
                }
                else if (value.IsSubclassOf(typeof(SQLResultItem)) || (value == typeof(SQLResultItem)))
                {
                    this.m_ResultItemType = value;
                }
            }
        }

        public Type ResultType
        {
            get
            {
                return this.m_ResultType;
            }
            set
            {
                if (value == null)
                {
                    this.m_ResultType = typeof(SQLResult);
                }
                else if (value.IsSubclassOf(typeof(SQLResult)) || (value == typeof(SQLResult)))
                {
                    this.m_ResultType = value;
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
                    this.m_SectionType = typeof(SQLSection);
                }
                else if (value.IsSubclassOf(typeof(SQLSection)) || (value == typeof(SQLSection)))
                {
                    this.m_SectionType = value;
                }
            }
        }

        public class SQLCondition
        {
            [FillField]
            public string CNAME;
            [FillField]
            public string CONDITIONSTRING;
            internal string[] Tables = new string[0];
            [FillField]
            public string TABLES;

            internal void Regular()
            {
                if ((this.TABLES != null) && (this.TABLES != ""))
                {
                    this.Tables = this.TABLES.Split(new char[] { ',' });
                    for (int i = 0; i < this.Tables.Length; i++)
                    {
                        this.Tables[i] = this.Tables[i].Trim();
                    }
                }
            }

            public string Name
            {
                get
                {
                    return this.CNAME;
                }
            }
        }

        public class SQLConditionMap
        {
            [FillField]
            public string CNAME;
            [FillField]
            public string CONDITIONSTRING;
            [FillField]
            public string TABLES;
        }

        public class SQLRelation
        {
            [FillField]
            public string RELATION;
            internal string[] Tables = new string[0];
            [FillField]
            public string TABLES;

            internal void Regular()
            {
                if ((this.TABLES != null) && (this.TABLES != ""))
                {
                    this.Tables = this.TABLES.Split(new char[] { ',' });
                    for (int i = 0; i < this.Tables.Length; i++)
                    {
                        this.Tables[i] = this.Tables[i].Trim();
                    }
                }
            }
        }

        public class SQLRelationMap
        {
            [FillField]
            public string RELATION;
            [FillField]
            public string TABLES;
        }

        public class SQLResult
        {
            [FillField]
            public DataType DATATYPE;
            [FillField]
            public string DDMFIELDCUSTOM;
            [FillField]
            public int DDMFIELDLENGTH = -1;
            [FillField]
            public int DDMFIELDSTART = -1;
            [FillField]
            public string DFIELD;
            [FillField]
            public string DFIELDCUSTOM;
            [FillField]
            public int DFIELDLENGTH = -1;
            [FillField]
            public int DFIELDSTART = -1;
            [FillField]
            public string DMDFIELD;
            [FillField]
            public string DMDFIELDCUSTOM;
            [FillField]
            public int DMDFIELDLENGTH = -1;
            [FillField]
            public int DMDFIELDSTART = -1;
            [FillField]
            public string DMSFIELD;
            [FillField]
            public string DMSFIELDCUSTOM;
            [FillField]
            public int DMSFIELDLENGTH = -1;
            [FillField]
            public int DMSFIELDSTART = -1;
            [FillField]
            public string DMTABLE;
            internal string M_CONDITION = "";
            internal string M_DMCONDITION = "";
            internal Hashtable M_DMFORMSQL = new Hashtable();
            internal string M_DMRESULTSQL = "";
            internal string M_DMTABLE = "";
            internal bool m_IsBinary = false;
            internal bool m_IsDM;
            internal ArrayList m_Results = new ArrayList();
            internal string M_RESULTSQL = "";
            internal SQLBuilderManager.SQLSection m_Section;
            internal string M_SEPRESULTSQL = "";

            internal void Add(SQLBuilderManager.SQLResultItem item)
            {
                this.m_Results.Add(item);
            }

            internal void MakeSQLTemplate()
            {
                if ((this.DFIELD != null) && (this.DFIELD != ""))
                {
                    string format = "{0}";
                    if ((this.DFIELDCUSTOM != null) && (this.DFIELDCUSTOM != ""))
                    {
                        format = this.DFIELDCUSTOM;
                    }
                    else if ((this.DFIELDLENGTH < 0) && (this.DFIELDSTART < 0))
                    {
                        format = "{0}";
                    }
                    else
                    {
                        format = this.m_Section.m_SUBSTSTRING + "({0}";
                        if (this.DFIELDSTART < 0)
                        {
                            format = format + ",1";
                        }
                        else
                        {
                            format = format + "," + this.DFIELDSTART;
                        }
                        if (this.DFIELDLENGTH < 0)
                        {
                            format = format + ")";
                        }
                        else
                        {
                            object obj2 = format;
                            format = string.Concat(new object[] { obj2, ",", this.DFIELDLENGTH, ")" });
                        }
                    }
                    string dMSFIELDCUSTOM = "";
                    string dMDFIELDCUSTOM = "{0}";
                    string dMTABLE = "";
                    string dDMFIELDCUSTOM = "{0}";
                    bool flag = false;
                    if ((((this.DMTABLE != null) && (this.DMTABLE != "")) && ((this.DMSFIELD != null) && (this.DMSFIELD != ""))) && ((this.DMDFIELD != null) && (this.DMDFIELD != "")))
                    {
                        flag = true;
                        if ((this.DDMFIELDCUSTOM != null) && (this.DDMFIELDCUSTOM != ""))
                        {
                            dDMFIELDCUSTOM = this.DDMFIELDCUSTOM;
                        }
                        else if ((this.DDMFIELDSTART < 0) && (this.DDMFIELDLENGTH < 0))
                        {
                            dDMFIELDCUSTOM = "{0}";
                        }
                        else
                        {
                            dDMFIELDCUSTOM = this.m_Section.m_SUBSTSTRING + "({0}";
                            if (this.DDMFIELDSTART < 0)
                            {
                                dDMFIELDCUSTOM = dDMFIELDCUSTOM + ",1";
                            }
                            else
                            {
                                dDMFIELDCUSTOM = dDMFIELDCUSTOM + "," + this.DDMFIELDSTART;
                            }
                            if (this.DDMFIELDLENGTH < 0)
                            {
                                dDMFIELDCUSTOM = dDMFIELDCUSTOM + ")";
                            }
                            else
                            {
                                object obj3 = dDMFIELDCUSTOM;
                                dDMFIELDCUSTOM = string.Concat(new object[] { obj3, ",", this.DDMFIELDLENGTH, ")" });
                            }
                        }
                        int index = this.DMTABLE.IndexOf(' ');
                        if (index < 0)
                        {
                            dMTABLE = this.DMTABLE;
                        }
                        else
                        {
                            dMTABLE = this.DMTABLE.Substring(index + 1).Trim();
                        }
                        if ((this.DMDFIELDCUSTOM != null) && (this.DMDFIELDCUSTOM != ""))
                        {
                            dMDFIELDCUSTOM = this.DMDFIELDCUSTOM;
                        }
                        else if ((this.DMDFIELDSTART < 0) && (this.DMDFIELDLENGTH < 0))
                        {
                            dMDFIELDCUSTOM = "{0}";
                        }
                        else
                        {
                            dMDFIELDCUSTOM = this.m_Section.m_SUBSTSTRING + "({0}";
                            if (this.DMDFIELDSTART < 0)
                            {
                                dMDFIELDCUSTOM = dMDFIELDCUSTOM + ",1";
                            }
                            else
                            {
                                dMDFIELDCUSTOM = dMDFIELDCUSTOM + "," + this.DMDFIELDSTART;
                            }
                            if (this.DMDFIELDLENGTH < 0)
                            {
                                dMDFIELDCUSTOM = dMDFIELDCUSTOM + ")";
                            }
                            else
                            {
                                object obj4 = dMDFIELDCUSTOM;
                                dMDFIELDCUSTOM = string.Concat(new object[] { obj4, ",", this.DMDFIELDLENGTH, ")" });
                            }
                        }
                        if ((this.DMSFIELDCUSTOM != null) && (this.DMSFIELDCUSTOM != ""))
                        {
                            dMSFIELDCUSTOM = this.DMSFIELDCUSTOM;
                        }
                        else if ((this.DMSFIELDSTART < 0) && (this.DMSFIELDLENGTH < 0))
                        {
                            dMSFIELDCUSTOM = "{0}";
                        }
                        else
                        {
                            dMSFIELDCUSTOM = this.m_Section.m_SUBSTSTRING + "({0}";
                            if (this.DMSFIELDSTART < 0)
                            {
                                dMSFIELDCUSTOM = dMSFIELDCUSTOM + ",1";
                            }
                            else
                            {
                                dMSFIELDCUSTOM = dMSFIELDCUSTOM + "," + this.DMSFIELDSTART;
                            }
                            if (this.DMSFIELDLENGTH < 0)
                            {
                                dMSFIELDCUSTOM = dMSFIELDCUSTOM + ")";
                            }
                            else
                            {
                                object obj5 = dMSFIELDCUSTOM;
                                dMSFIELDCUSTOM = string.Concat(new object[] { obj5, ",", this.DMSFIELDLENGTH, ")" });
                            }
                        }
                        dMDFIELDCUSTOM = string.Format(dMDFIELDCUSTOM, dMTABLE + "." + this.DMDFIELD);
                        this.M_DMTABLE = this.DMTABLE;
                        this.M_SEPRESULTSQL = "SELECT " + string.Format(dDMFIELDCUSTOM, dMDFIELDCUSTOM) + " AS " + this.DFIELD + " FROM " + this.DMTABLE + " WHERE " + string.Format(dMSFIELDCUSTOM, dMTABLE + "." + this.DMSFIELD) + "= '{0}'";
                    }
                    string text6 = "";
                    int num2 = 0;
                    foreach (SQLBuilderManager.SQLResultItem item in this.m_Results)
                    {
                        string sTABLE;
                        if (((item.STABLE == null) || (item.STABLE == "")) || ((item.SFIELD == null) || (item.SFIELD == "")))
                        {
                            continue;
                        }
                        num2++;
                        int num3 = item.STABLE.IndexOf(' ');
                        if (num3 < 0)
                        {
                            sTABLE = item.STABLE;
                        }
                        else
                        {
                            sTABLE = item.STABLE.Substring(num3 + 1).Trim();
                        }
                        string sFIELDCUSTOM = "";
                        if ((item.SFIELDCUSTOM != null) && (item.SFIELDCUSTOM != ""))
                        {
                            sFIELDCUSTOM = item.SFIELDCUSTOM;
                        }
                        else if ((item.SFIELDSTART < 0) && (item.SFIELDLENGTH < 0))
                        {
                            sFIELDCUSTOM = "{0}";
                        }
                        else
                        {
                            sFIELDCUSTOM = this.m_Section.m_SUBSTSTRING + "({0}";
                            if (item.SFIELDSTART < 0)
                            {
                                sFIELDCUSTOM = sFIELDCUSTOM + ",1";
                            }
                            else
                            {
                                sFIELDCUSTOM = sFIELDCUSTOM + "," + item.SFIELDSTART;
                            }
                            if (item.SFIELDLENGTH < 0)
                            {
                                sFIELDCUSTOM = sFIELDCUSTOM + ")";
                            }
                            else
                            {
                                object obj6 = sFIELDCUSTOM;
                                sFIELDCUSTOM = string.Concat(new object[] { obj6, ",", item.SFIELDLENGTH, ")" });
                            }
                        }
                        if ((((item.DMTABLE != null) && (item.DMTABLE != "")) && ((item.DMSFIELD != null) && (item.DMSFIELD != ""))) && ((item.DMDFIELD != null) && (item.DMDFIELD != "")))
                        {
                            string text9;
                            int num4 = item.DMTABLE.IndexOf(' ');
                            if (num4 < 0)
                            {
                                text9 = item.DMTABLE;
                            }
                            else
                            {
                                text9 = item.DMTABLE.Substring(num4 + 1).Trim();
                            }
                            string text10 = "";
                            if ((item.DMDFIELDCUSTOM != null) && (item.DMDFIELDCUSTOM != ""))
                            {
                                text10 = item.DMDFIELDCUSTOM;
                            }
                            else if ((item.DMDFIELDSTART < 0) && (item.DMDFIELDLENGTH < 0))
                            {
                                text10 = "{0}";
                            }
                            else
                            {
                                text10 = this.m_Section.m_SUBSTSTRING + "({0}";
                                if (item.DMDFIELDSTART < 0)
                                {
                                    text10 = text10 + ",1";
                                }
                                else
                                {
                                    text10 = text10 + "," + item.DMDFIELDSTART;
                                }
                                if (item.DMDFIELDLENGTH < 0)
                                {
                                    text10 = text10 + ")";
                                }
                                else
                                {
                                    object obj7 = text10;
                                    text10 = string.Concat(new object[] { obj7, ",", item.DMDFIELDLENGTH, ")" });
                                }
                            }
                            string text11 = "";
                            if ((item.DMSFIELDCUSTOM != null) && (item.DMSFIELDCUSTOM != ""))
                            {
                                text11 = item.DMSFIELDCUSTOM;
                            }
                            else if ((item.DMSFIELDSTART < 0) && (item.DMSFIELDLENGTH < 0))
                            {
                                text11 = "{0}";
                            }
                            else
                            {
                                text11 = this.m_Section.m_SUBSTSTRING + "({0}";
                                if (item.DMSFIELDSTART < 0)
                                {
                                    text11 = text11 + ",1";
                                }
                                else
                                {
                                    text11 = text11 + "," + item.DMSFIELDSTART;
                                }
                                if (item.DMSFIELDLENGTH < 0)
                                {
                                    text11 = text11 + ")";
                                }
                                else
                                {
                                    object obj8 = text11;
                                    text11 = string.Concat(new object[] { obj8, ",", item.DMSFIELDLENGTH, ")" });
                                }
                            }
                            sFIELDCUSTOM = string.Format(sFIELDCUSTOM, sTABLE + "." + item.SFIELD);
                            text11 = string.Format(text11, text9 + "." + item.DMSFIELD);
                            text10 = string.Format(text10, text9 + "." + item.DMDFIELD);
                            string text12 = "";
                            if ((this.m_Section.VERSION & BuildSQLVersion.SQLSERVER) == BuildSQLVersion.SQLSERVER)
                            {
                                text12 = " LEFT OUTER JOIN " + item.DMTABLE + " ON (" + text11 + "=" + sFIELDCUSTOM + ")";
                                if (this.M_DMFORMSQL.ContainsKey(item.STABLE))
                                {
                                    this.M_DMFORMSQL[item.STABLE] = (this.M_DMFORMSQL[item.STABLE] as string) + text12;
                                }
                                else
                                {
                                    this.M_DMFORMSQL[item.STABLE] = text12;
                                }
                                text6 = text6 + text10 + "+";
                            }
                            else if ((this.m_Section.VERSION & BuildSQLVersion.ORACLE) == BuildSQLVersion.ORACLE)
                            {
                                string text13 = this.M_CONDITION;
                                this.M_CONDITION = text13 + sFIELDCUSTOM + "=" + text11 + "(+) AND ";
                                if (!this.M_DMFORMSQL.ContainsKey(item.STABLE))
                                {
                                    this.M_DMFORMSQL[item.STABLE] = "";
                                }
                                if (!this.M_DMFORMSQL.ContainsKey(item.DMTABLE))
                                {
                                    this.M_DMFORMSQL[item.DMTABLE] = "";
                                }
                                text6 = text6 + text10 + "||";
                            }
                            else
                            {
                                text6 = text6 + text10 + "+";
                            }
                            continue;
                        }
                        if (!this.M_DMFORMSQL.ContainsKey(item.STABLE))
                        {
                            this.M_DMFORMSQL[item.STABLE] = "";
                        }
                        if ((this.m_Section.VERSION & BuildSQLVersion.SQLSERVER) == BuildSQLVersion.SQLSERVER)
                        {
                            text6 = text6 + string.Format(sFIELDCUSTOM, sTABLE + "." + item.SFIELD) + "+";
                            continue;
                        }
                        if ((this.m_Section.VERSION & BuildSQLVersion.ORACLE) == BuildSQLVersion.ORACLE)
                        {
                            text6 = text6 + string.Format(sFIELDCUSTOM, sTABLE + "." + item.SFIELD) + "||";
                            continue;
                        }
                        text6 = text6 + string.Format(sFIELDCUSTOM, sTABLE + "." + item.SFIELD) + "+";
                    }
                    if (num2 == 0)
                    {
                        if (this.DATATYPE == DataType.B)
                        {
                            this.M_RESULTSQL = string.Format(format, "NULL");
                        }
                        else
                        {
                            this.M_RESULTSQL = string.Format(format, "''");
                        }
                    }
                    else if (num2 == 1)
                    {
                        this.M_RESULTSQL = string.Format(format, text6.TrimEnd(new char[] { '+', '|' }));
                    }
                    else
                    {
                        this.M_RESULTSQL = string.Format(format, "(" + text6.TrimEnd(new char[] { '+', '|' }) + ")");
                    }
                    this.M_RESULTSQL = this.M_RESULTSQL + " AS " + this.DFIELD;
                    if (flag)
                    {
                        switch (num2)
                        {
                            case 0:
                                this.M_DMRESULTSQL = "";
                                this.M_SEPRESULTSQL = "";
                                this.M_DMCONDITION = "";
                                this.M_DMTABLE = "";
                                return;

                            case 1:
                                this.m_IsDM = true;
                                this.M_DMRESULTSQL = string.Format(dDMFIELDCUSTOM, dMDFIELDCUSTOM) + " AS " + this.DFIELD;
                                this.M_DMCONDITION = this.M_CONDITION + string.Format(dMSFIELDCUSTOM, dMTABLE + "." + this.DMSFIELD) + "=" + text6.TrimEnd(new char[] { '+', '|' }) + " AND ";
                                return;
                        }
                        this.m_IsDM = true;
                        this.M_DMRESULTSQL = string.Format(dDMFIELDCUSTOM, dMDFIELDCUSTOM) + " AS " + this.DFIELD;
                        this.M_DMCONDITION = this.M_CONDITION + string.Format(dMSFIELDCUSTOM, dMTABLE + "." + this.DMSFIELD) + "=(" + text6.TrimEnd(new char[] { '+', '|' }) + ") AND ";
                    }
                }
            }

            public bool IsBinary
            {
                get
                {
                    if (this.DATATYPE != DataType.B)
                    {
                        return (this.DATATYPE == DataType.B);
                    }
                    return true;
                }
            }

            public bool IsDM
            {
                get
                {
                    return this.m_IsDM;
                }
            }

            public ArrayList Results
            {
                get
                {
                    return this.m_Results;
                }
            }
        }

        public class SQLResultItem
        {
            [FillField]
            public string DMDFIELD;
            [FillField]
            public string DMDFIELDCUSTOM;
            [FillField]
            public int DMDFIELDLENGTH = -1;
            [FillField]
            public int DMDFIELDSTART = -1;
            [FillField]
            public string DMSFIELD;
            [FillField]
            public string DMSFIELDCUSTOM;
            [FillField]
            public int DMSFIELDLENGTH = -1;
            [FillField]
            public int DMSFIELDSTART = -1;
            [FillField]
            public string DMTABLE;
            [FillField]
            public string SFIELD;
            [FillField]
            public string SFIELDCUSTOM;
            [FillField]
            public int SFIELDLENGTH = -1;
            [FillField]
            public int SFIELDSTART = -1;
            [FillField]
            public string STABLE;
        }

        public class SQLResultItemMap
        {
            [FillField]
            public string DMDFIELD;
            [FillField]
            public string DMDFIELDCUSTOM;
            [FillField]
            public string DMDFIELDLENGTH;
            [FillField]
            public string DMDFIELDSTART;
            [FillField]
            public string DMSFIELD;
            [FillField]
            public string DMSFIELDCUSTOM;
            [FillField]
            public string DMSFIELDLENGTH;
            [FillField]
            public string DMSFIELDSTART;
            [FillField]
            public string DMTABLE;
            [FillField]
            public string SFIELD;
            [FillField]
            public string SFIELDCUSTOM;
            [FillField]
            public string SFIELDLENGTH;
            [FillField]
            public string SFIELDSTART;
            [FillField]
            public string STABLE;
        }

        public class SQLResultMap
        {
            [FillField]
            public string DATATYPE;
            [FillField]
            public string DDMFIELDCUSTOM;
            [FillField]
            public string DDMFIELDLENGTH;
            [FillField]
            public string DDMFIELDSTART;
            [FillField]
            public string DFIELD;
            [FillField]
            public string DFIELDCUSTOM;
            [FillField]
            public string DFIELDLENGTH;
            [FillField]
            public string DFIELDSTART;
            [FillField]
            public string DMDFIELD;
            [FillField]
            public string DMDFIELDCUSTOM;
            [FillField]
            public string DMDFIELDLENGTH;
            [FillField]
            public string DMDFIELDSTART;
            [FillField]
            public string DMSFIELD;
            [FillField]
            public string DMSFIELDCUSTOM;
            [FillField]
            public string DMSFIELDLENGTH;
            [FillField]
            public string DMSFIELDSTART;
            [FillField]
            public string DMTABLE;
        }

        public class SQLSection
        {
            [FillField]
            public string ADDEND;
            internal Hashtable ExtSqls = new Hashtable();
            internal Hashtable m_Conditions = new Hashtable();
            internal ArrayList m_Relations = new ArrayList();
            internal Hashtable m_Results = new Hashtable();
            internal ArrayList m_ResultsList = new ArrayList();
            internal string m_SUBSTSTRING = "SUBSTRING";
            [FillField]
            public string RULE;
            [FillField]
            public string SNAME;
            internal Hashtable Sqls = new Hashtable();
            [FillField]
            public BuildSQLVersion VERSION = BuildSQLVersion.SQLSERVER2000;

            public virtual void OnAfterInitialize()
            {
            }

            public virtual void OnBeforeInitialize()
            {
            }

            public void Regular()
            {
                if ((this.VERSION & BuildSQLVersion.SQLSERVER) == BuildSQLVersion.SQLSERVER)
                {
                    this.m_SUBSTSTRING = "SUBSTRING";
                }
                else if ((this.VERSION & BuildSQLVersion.ORACLE) == BuildSQLVersion.ORACLE)
                {
                    this.m_SUBSTSTRING = "SUBSTR";
                }
            }

            public Hashtable Conditions
            {
                get
                {
                    return this.m_Conditions;
                }
            }

            public string Name
            {
                get
                {
                    return this.SNAME;
                }
            }

            public ArrayList Relations
            {
                get
                {
                    return this.m_Relations;
                }
            }

            public Hashtable Results
            {
                get
                {
                    return this.m_Results;
                }
            }

            public ArrayList ResultsList
            {
                get
                {
                    return this.m_ResultsList;
                }
            }
        }

        public class SQLSectionMap
        {
            public string ADDEND;
            public string RULE;
            public string SNAME;
            public string VERSION;
        }
    }
}
