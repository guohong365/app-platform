namespace Platform
{
    using System;
    using System.Collections;
    using System.Data;

    public class ReplaceHelper
    {
        private ArrayList ar = new ArrayList();

        public void Add(ReplaceBase replace)
        {
            this.ar.Add(replace);
        }

        public void AddRange(ReplaceBase[] replaces)
        {
            this.ar.AddRange(replaces);
        }

        public void Replace(DataRow row)
        {
            if (row != null)
            {
                foreach (ReplaceBase base2 in this.ar)
                {
                    base2.Replace(row);
                }
            }
        }

        public void Replace(DataTable table)
        {
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    foreach (ReplaceBase base2 in this.ar)
                    {
                        base2.Replace(row);
                    }
                }
            }
        }
    }
}
