namespace Platform
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    public class ReplaceLength : ReplaceBase
    {
        private string destColName;
        private int maxLength;

        public ReplaceLength(string destCol, int maxLen)
        {
            this.destColName = destCol;
            this.maxLength = maxLen;
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
                    string text = row[this.destColName].ToString();
                    if ((this.maxLength > 0) && (text.Length > this.maxLength))
                    {
                        return text.Substring(0, this.maxLength);
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
