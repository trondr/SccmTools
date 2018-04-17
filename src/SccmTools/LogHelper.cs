using System.Diagnostics;

namespace SccmTools
{
    public static class LogHelper
    {
        public static void WriteErrorToEventLog(string message)
        {
            // ReSharper disable once RedundantNameQualifier
            System.Console.WriteLine(message);
            const string logName = "Application";
            using (var eventLog = new EventLog(logName))
            {
                eventLog.Source = logName;
                eventLog.WriteEntry(message, EventLogEntryType.Error, 101, 1);
            }
        }
    }
}