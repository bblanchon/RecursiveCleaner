using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private static void Print(LogLevel level, string format, params object[] args)
        {
            if (level <= Filter) 
                Console.WriteLine(level+": "+format, args);
        }        

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
    }
}
