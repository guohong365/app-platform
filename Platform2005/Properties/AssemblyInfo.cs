// Assembly Platform2005, Version 1.0.0.1

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using Platform;
using Platform.Configuration;
using Platform.CSS;
using Platform.CSS.Communication;
using Platform.CSS.Remoting;
using Platform.CSS.SerialSink;
using Platform.CSS.Server;
using Platform.DBHelper;
using Platform.Exchange;
using Platform.IO;
using Platform.Log;
using Platform.Module;
using Platform.Security;
using Platform.Task;
using Platform.Threading;

[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyConfigType(typeof(TaskConfig))]
[assembly: AssemblyConfigType(typeof(ModuleAssemblies))]
[assembly: AssemblyConfigResource("Threading.Threading.config")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfigType(typeof(ThreadingConfig))]
[assembly: AssemblyConfigResource("Task.Task.config")]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyConfigType(typeof(ModuleConfig))]
[assembly: AssemblyConfigResource("Security.Security.config")]

[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTitle("")]
[assembly: AssemblyConfigType(typeof(DBHandler))]
[assembly: RemoteClassAssembly(typeof(RemotingServerHandler))]
[assembly: AssemblyConfigType(typeof(MemoryStream))]
[assembly: AssemblyConfigResource("IO.IO.config")]
[assembly: AssemblyConfigType(typeof(ExchangeErrorHelper))]
[assembly: AssemblyConfigResource("Exchange.ExchangeErrorCode.config")]
[assembly: AssemblyConfigType(typeof(LoginHandlersConfig))]
[assembly: AssemblyConfigResource("CSS.Server.LoginHandlers.config")]
[assembly: AssemblyConfigType(typeof(SerialSinkConfig))]
[assembly: AssemblyConfigResource("CSS.SerialSink.SerialSink.config")]
[assembly: AssemblyConfigType(typeof(CommunicationErrorCode))]
[assembly: AssemblyConfigResource("CSS.Communication.CommunicationErrorCode.config")]
[assembly: AssemblyConfigType(typeof(LogConfig))]
[assembly: AssemblyConfigResource("Log.Log.config")]
[assembly: AssemblyConfigType(typeof(CSSConfig))]
[assembly: AssemblyConfigResource("CSS.CSS.config")]
[assembly: AssemblyConfigType(typeof(PlatformConfig))]
[assembly: AssemblyConfigResource("Platform.config")]
[assembly: AssemblyConfigType(typeof(SecurityConfig))]
[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

