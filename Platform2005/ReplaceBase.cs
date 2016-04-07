namespace Platform
{
    using System;
    using System.Data;

    public abstract class ReplaceBase
    {
        protected ReplaceBase()
        {
        }

        public virtual string GetReplaceString(DataRow row)
        {
            return null;
        }

        public abstract void Replace(DataRow row);
    }
}
