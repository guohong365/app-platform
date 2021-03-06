namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;

    [DesignerCategory("code"), GeneratedCode("System.Web.Services", "2.0.50727.312"), DebuggerStepThrough]
    public class GetNoteListCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal GetNoteListCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public DataSet Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (DataSet) this.results[0];
            }
        }
    }
}
