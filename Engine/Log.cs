/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011-2012 Benoit Blanchon
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RecursiveCleaner.Engine
{
    public enum LogLevel
    {
        Error,
        Warning,
        Info,
        Debug,
    }

    public static class Log
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

        const string EVENT_SOURCE = "RecursiveCleaner";

        private static void Print(LogLevel level, string format, params object[] args)
        {
            if (level <= Filter)
            {
                var s = string.Format(format, args);

                if (LogToConsole) Console.WriteLine(s);
                if (LogToEventLog) EventLog.WriteEntry(EVENT_SOURCE, s, levelMap[level], 1);
            }
        } 

        static readonly Dictionary<LogLevel,EventLogEntryType> levelMap = 
            new Dictionary<LogLevel,EventLogEntryType>
            {
                { LogLevel.Error,   EventLogEntryType.Error       },
                { LogLevel.Warning, EventLogEntryType.Warning     },
                { LogLevel.Info,    EventLogEntryType.Information },
                { LogLevel.Debug,   EventLogEntryType.Information },
            };

        #endregion
    }
}
