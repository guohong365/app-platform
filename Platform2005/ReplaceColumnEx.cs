namespace Platform
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    public class ReplaceColumnEx : ReplaceColumn
    {
        protected string frontAdd;
        protected string lastAdd;
        protected int Length;
        protected int StartIndex;

        public ReplaceColumnEx(DataSet d, string sourceFilterCol, string sourceCol, string destCol, int startIndex, int length) : this(d, sourceFilterCol, sourceCol, destCol, startIndex, length, "", "")
        {
        }

        public ReplaceColumnEx(DataSet d, string sourceFilterCol, string sourceCol, string destCol, int startIndex, int length, string front, string last) : base(d, sourceFilterCol, sourceCol, destCol)
        {
            this.StartIndex = startIndex;
            this.Length = length;
            this.frontAdd = front;
            this.lastAdd = last;
        }

        public override string GetReplaceString(DataRow row)
        {
            if (row != null)
            {
                if (!row.Table.Columns.Contains(base.destColName) || row.IsNull(base.destColName))
                {
                    return null;
                }
                try
                {
                    if (((base.sourceFilterColName != null) && (base.sourceColName != null)) && ((base.ds != null) && (base.ds.Tables.Count > 0)))
                    {
                        string text = row[base.destColName].ToString();
                        if (text.Length >= (this.StartIndex + this.Length))
                        {
                            text = this.frontAdd + text.Substring(this.StartIndex, this.Length) + this.lastAdd;
                            DataRow[] rowArray = base.ds.Tables[0].Select(base.sourceFilterColName + "='" + text + "'");
                            if ((rowArray != null) && (rowArray.Length > 0))
                            {
                                return rowArray[0][base.sourceColName].ToString();
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(base.destColName + "  " + exception.Message);
                }
            }
            return null;
        }

        public override void Replace(DataRow row)
        {
            string replaceString = this.GetReplaceString(row);
            if (replaceString != null)
            {
                row[base.destColName] = replaceString;
            }
        }
    }
}
