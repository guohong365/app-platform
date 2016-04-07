namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;

    [GeneratedCode("System.Web.Services", "2.0.50727.312"), DebuggerStepThrough, DesignerCategory("code")]
    public class GetFileListTypedCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal GetFileListTypedCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public DataSet dsList
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (DataSet) this.results[1];
            }
        }

        public int Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (int) this.results[0];
            }
        }
    }
}
