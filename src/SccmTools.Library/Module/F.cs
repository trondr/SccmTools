using System;
using System.IO;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SccmTools.Library.Module
{
    public static class F
    {
        public static Func<Result<ExistingDirectoryPath>> GetAdminConsoleBinPath => fun(() =>
        {
            //
            //SMS_ADMIN_UI_PATH=C:\Program Files (x86)\Microsoft Endpoint Manager\AdminConsole\bin\i386
            //
            var smsAdminUiPath = ExistingDirectoryPath.Get(Environment.GetEnvironmentVariable("SMS_ADMIN_UI_PATH"));
            return smsAdminUiPath
                .Match(GetParentPath, 
                    exception => new Result<ExistingDirectoryPath>(GetDirectoryNotFoundException(exception)));

        }).Memo();

        private static Exception GetDirectoryNotFoundException(Exception innerException)
        {
            return new DirectoryNotFoundException($"Failed to located the install location of Microsoft System Configuration Manager using the environment variable SMS_ADMIN_UI_PATH due to {innerException.Message} Please verify that Microsoft System Configuration Manager Console is installed on this machine.");
        }

        private static Result<ExistingDirectoryPath> GetParentPath(ExistingDirectoryPath path)
        {
            try
            {
                return ExistingDirectoryPath.Get(new DirectoryInfo(path.Value).Parent?.FullName);
            }
            catch (Exception e)
            {
                return new Result<ExistingDirectoryPath>(e);
            }
        }
    }
}
