namespace Platform
{
    using System;
    using System.Collections;
    using System.Data;

    public class DataRowBinder
    {
        private ArrayList ar = new ArrayList();

        public void Add(BinderItem item)
        {
            this.ar.Add(item);
        }

        public void AddRange(BinderItem[] items)
        {
            this.ar.AddRange(items);
        }

        public void Bind(DataRow row)
        {
            if (row != null)
            {
                DataColumnCollection cls = row.Table.Columns;
                foreach (BinderItem item in this.ar)
                {
                    item.Bind(cls, row);
                }
            }
        }

        public void Clear()
        {
            foreach (BinderItem item in this.ar)
            {
                item.Clear();
            }
        }
    }
}
