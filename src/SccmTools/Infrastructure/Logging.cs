using System;
using Common.Logging;
using SccmTools.Library.Module;
using Serilog.Events;

namespace SccmTools.Infrastructure
{
    public static class Logging
    {
        public static LogEventLevel ToLogEventLevel(this Common.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Info:
                    return LogEventLevel.Information;
                case LogLevel.Warn:
                    return LogEventLevel.Warning;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Fatal:
                    return LogEventLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, $"Common logging LogLevel '{logLevel}' is not supported by Serilog LogEventLevel. Supported serilog log event levels: { string.Join("|", EnumUtils.EnumStringValueToList<LogEventLevel>())}");
            }
        }
    }
}
