namespace Platform
{
    using System;
    using System.Data;
    using System.Runtime.CompilerServices;

    public class ReplaceCustom : ReplaceBase
    {
        private string destColName;
        private ReplaceCustomHandler Handler;

        public ReplaceCustom(string destCol, ReplaceCustomHandler handler)
        {
            this.destColName = destCol;
            this.Handler = handler;
        }

        public override void Replace(DataRow row)
        {
            if ((row.Table.Columns.Contains(this.destColName) && !row.IsNull(this.destColName)) && (this.Handler != null))
            {
                this.Handler(row);
            }
        }

        public delegate void ReplaceCustomHandler(DataRow row);
    }
}
