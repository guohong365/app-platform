namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;

    [DesignerCategory("code"), GeneratedCode("System.Web.Services", "2.0.50727.312"), DebuggerStepThrough]
    public class GetNoteCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal GetNoteCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public string note
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string) this.results[2];
            }
        }

        public bool Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (bool) this.results[0];
            }
        }

        public string title
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string) this.results[1];
            }
        }
    }
}
