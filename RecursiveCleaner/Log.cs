using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RecursiveCleaner
{
    enum LogLevel
    {
        Error,
        Warning,
        Info,
        Debug,
    }

    static class Log
    {
        public static LogLevel Filter = LogLevel.Debug;
        public static bool LogToConsole = true;
        public static bool LogToEventLog = false;

        public static void Error(string format, params object[] args)
        {
            Print(LogLevel.Error, format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            Print(LogLevel.Warning, format, args);
        }

        public static void Info(string format, params object[] args)
        {
            Print(LogLevel.Info, format, args);
        }

        public static void Debug(string format, params object[] args)
        {
            Print(LogLevel.Debug, format, args);
        }

        #region Private part

        const string EventSource = "RecursiveCleaner";

        private static void Print(LogLevel level, string format, params object[] args)
        {
            if (level <= Filter)
            {
                var s = string.Format(format, args);

                if (LogToConsole) Console.WriteLine(s);
                if (LogToEventLog) EventLog.WriteEntry(EventSource, s, levelMap[level], 1);
            }
        } 

        static Dictionary<LogLevel,EventLogEntryType> levelMap = 
            new Dictionary<LogLevel,EventLogEntryType>
            {
                { LogLevel.Error, EventLogEntryType.Error },
                { LogLevel.Warning, EventLogEntryType.Warning },
                { LogLevel.Info, EventLogEntryType.Information },
                { LogLevel.Debug, EventLogEntryType.Information },
            };

        #endregion
    }
}
