using System;
using NCmdLiner;
using SccmTools.Library.Module;

namespace SccmTools
{
    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.ResolveHandler;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }
        private static readonly int FatalExitCode = 1;
        [STAThread]
        static int Main(string[] args)
        {
            var exitCode = FatalExitCode;
            try
            {
                Run.WireupAndRun(args, FatalExitCode)
                    .OnSuccess(i => exitCode = i);
            }
            catch (Exception e)
            {
                LogHelper.WriteErrorToEventLog($"Fatal error when wiring up the application.{Environment.NewLine}{e}");
            }
            if (exitCode != 0) LoggingFunctions.OpenLogFile();
            return exitCode;
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            LogHelper.WriteErrorToEventLog($"Fatal error when wiring up the application.{Environment.NewLine}{unhandledExceptionEventArgs.ExceptionObject}");
        }
    }
}
