using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SccmTools.Gui
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                var process = new Process();
                var consoleExe = GetConsoleExe();
                var arguments = GetArguments();
                process.StartInfo = new ProcessStartInfo(consoleExe, arguments)
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                process.Start();
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return  1;                
            }
        }

        private static string GetArguments()
        {
            var commandLine = Environment.CommandLine;
            var args = Environment.GetCommandLineArgs();
            //Remove first argument (the exe file path) from start of command line
            var arguments = commandLine.Replace(args[0],"").TrimStart(new []{'"',' '});
            return arguments;
        }

        private static string GetConsoleExe()
        {
            var assembly = Assembly.GetEntryAssembly();
            if(assembly == null)
            {
                throw new FileNotFoundException("Failed to derive console executable from entry assembly. Entry assembly is null.");
            }
            var consoleExe = assembly.Location.Replace(".Gui.exe",".exe");
            if(!File.Exists(consoleExe))
            {
                throw new FileNotFoundException("Failed to derive console executable from entry assembly. Entry assembly is null.");
            }
            return consoleExe;
        }
    }
}
