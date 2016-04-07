namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;

    [DesignerCategory("code"), GeneratedCode("System.Web.Services", "2.0.50727.312"), DebuggerStepThrough]
    public class GetFileListCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal GetFileListCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
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
