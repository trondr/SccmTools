using System;
using NCmdLiner;
using SccmTools.Infrastructure;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Module;

namespace SccmTools
{
    public static class Run
    {
        public static Result<int> WireupAndRun(string[] args, int fatalExitCode)
        {
            using (var bootStrapper = new BootStrapper())
            {
                var applicationInfo = bootStrapper.Container.Resolve<IApplicationInfo>();
                applicationInfo.Authors = @"trondr@outlook.com";
                // ReSharper disable once CoVariantArrayConversion
                object[] commandTargets = bootStrapper.Container.ResolveAll<CommandDefinition>();
                var logger = typeof(Program).Logger();
                logger.Info($"Start: {applicationInfo.Name}.{applicationInfo.Version}. Command line: {Environment.CommandLine}");
                var result = CmdLinery.RunEx(commandTargets, args, applicationInfo, bootStrapper.Container.Resolve<IMessenger>())
                    .OnFailure(exception =>
                    {
                        logger.Error(logger.IsDebugEnabled ? exception.ToString() : exception.Message);
                    });
                logger.Info($"Stop: {applicationInfo.Name}.{applicationInfo.Version}. Return value: {(result.IsSuccess ? result.Value : fatalExitCode)}");
                return result;
            }
        }
    }
}