namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerStepThrough, GeneratedCode("System.Web.Services", "2.0.50727.312"), DesignerCategory("code")]
    public class GetFileCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal GetFileCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public byte[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (byte[]) this.results[0];
            }
        }
    }
}
