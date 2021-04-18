using System;
using System.IO;
using Common.Logging.Factory;
using SccmTools.Library.Module;
using SmsClientLib;

namespace SccmTools
{
    public static class RunTimeRequirements
    {
        public static void AssertAll()
        {
            AssertAdminConsoleInstalled();
            AssertClientInstalled();
        }
        
        public static void AssertAdminConsoleInstalled()
        {
            F.GetAdminConsoleBinPath()
                .Match(
                    path => path, 
                    ex => throw new DirectoryNotFoundException($"Please install Endpoint Manager Console and try again. {ex.Message}"));
            LoggingFunctions.Logger(typeof(RunTimeRequirements)).Info("Endpoint Manager Console has been verified.");
        }

        public static void AssertClientInstalled()
        {
            try
            {
                var smsClient = new SmsClient();
                smsClient.GetAssignedSite();
                LoggingFunctions.Logger(typeof(RunTimeRequirements)).Info("SCCM Client has been verified.");
            }
            catch (Exception)
            {
                throw new Exception("Please install and SCCM Client. SccmTools uses the sccm client to get the assigned SCCM site.");
            }
        }
    }
}
