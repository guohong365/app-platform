namespace Platform
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    public class ReplaceColumn : ReplaceBase
    {
        protected string destColName;
        protected DataSet ds;
        protected string sourceColName;
        protected string sourceFilterColName;

        public ReplaceColumn(DataSet d, string sourceFilterCol, string sourceCol, string destCol)
        {
            this.ds = d;
            this.sourceColName = sourceCol;
            this.destColName = destCol;
            this.sourceFilterColName = sourceFilterCol;
        }

        public override string GetReplaceString(DataRow row)
        {
            if (row != null)
            {
                if (!row.Table.Columns.Contains(this.destColName) || row.IsNull(this.destColName))
                {
                    return null;
                }
                try
                {
                    if (((this.sourceFilterColName != null) && (this.sourceColName != null)) && ((this.ds != null) && (this.ds.Tables.Count > 0)))
                    {
                        string text = row[this.destColName].ToString();
                        DataRow[] rowArray = this.ds.Tables[0].Select(this.sourceFilterColName + "='" + text + "'");
                        if ((rowArray != null) && (rowArray.Length > 0))
                        {
                            return rowArray[0][this.sourceColName].ToString();
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(this.destColName + "  " + exception.Message);
                }
            }
            return null;
        }

        public override void Replace(DataRow row)
        {
            string replaceString = this.GetReplaceString(row);
            if (replaceString != null)
            {
                row[this.destColName] = replaceString;
            }
        }
    }
}
