namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;

    [GeneratedCode("System.Web.Services", "2.0.50727.312"), DebuggerStepThrough, DesignerCategory("code")]
    public class RemoveFileByIdCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal RemoveFileByIdCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
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
