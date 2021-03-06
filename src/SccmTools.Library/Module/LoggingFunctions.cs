﻿using System;
using System.IO;
using Common.Logging;
using LanguageExt;
using SccmTools.Library.Infrastructure;

namespace SccmTools.Library.Module
{
    public static class LoggingFunctions
    {
        private static readonly Func<Type, ILog> LogFactory = LogManager.GetLogger;
        private static readonly Func<Type, ILog> CachedLogFactory = LogFactory.Memo();
        public static ILog Logger(this object obj) => CachedLogFactory(obj.GetType());
        public static ILog Logger(Type type) => CachedLogFactory(type);
        public static ILog Logger<T>() => CachedLogFactory(typeof(T));

        public static LogLevel ToLogLevel(string logLevel)
        {
            switch (logLevel)
            {
                case "All":
                    return LogLevel.All;
                case "Trace":
                    return LogLevel.Trace;
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Info;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Fatal":
                    return LogLevel.Fatal;
                case "Off":
                    return LogLevel.Off;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid loglevel '{logLevel}' specified in app config. Valid log level values are: {string.Join("|",EnumUtils.EnumStringValueToList<LogLevel>())}");
            }
        }

        public static void OpenLogFile()
        {
            try
            {
                var loggingConfiguration = new LoggingConfiguration();
                var logFile = Path.Combine(loggingConfiguration.LogDirectoryPath, loggingConfiguration.LogFileName);
                if(File.Exists(logFile))
                    System.Diagnostics.Process.Start(logFile);
            }
            catch (Exception e)
            {
                Logger(typeof(LoggingFunctions)).Error($"Failed to  open log file. {e.Message}");
            }
        }
    }
}
