namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerStepThrough, GeneratedCode("System.Web.Services", "2.0.50727.312"), DesignerCategory("code")]
    public class CreateSettingNameCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal CreateSettingNameCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
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
