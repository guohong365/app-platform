namespace Platform
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Windows.Forms;

    public class BinderItem
    {
        private string[] Column;
        private System.Windows.Forms.Control Control;
        private object DefaultValue;
        private System.Reflection.PropertyInfo PropertyInfo;

        public BinderItem(System.Windows.Forms.Control ctrl, string prop, string col, object def) : this(ctrl, prop, new string[] { col }, def)
        {
        }

        public BinderItem(System.Windows.Forms.Control ctrl, string prop, string[] col, object def)
        {
            System.Type type = ctrl.GetType();
            this.Control = ctrl;
            this.PropertyInfo = type.GetProperty(prop);
            this.Column = col;
            this.DefaultValue = def;
        }

        public void Bind(DataRow row)
        {
            if (row != null)
            {
                this.Bind(row.Table.Columns, row);
            }
        }

        public void Bind(DataColumnCollection cls, DataRow row)
        {
            if ((cls != null) && (row != null))
            {
                bool flag = false;
                string text = "";
                foreach (string text2 in this.Column)
                {
                    if (cls.Contains(text2))
                    {
                        flag = true;
                        text = text + row[text2].ToString();
                    }
                }
                if (flag)
                {
                    this.PropertyInfo.SetValue(this.Control, text, null);
                }
                else
                {
                    this.PropertyInfo.SetValue(this.Control, this.DefaultValue, null);
                }
            }
        }

        public void Clear()
        {
            this.PropertyInfo.SetValue(this.Control, this.DefaultValue, null);
        }
    }
}
