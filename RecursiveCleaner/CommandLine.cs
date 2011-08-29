/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011 Benoit Blanchon
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

using System.Collections.Generic;
using System.IO;
using RecursiveCleaner.Engine;

namespace RecursiveCleaner
{
    class CommandLine
    {
        public bool SimulationMode { private set; get; }

        public bool ScanAllFixedDrives { private set; get; }

        public LogLevel LogLevel { get; private set; }

        public bool LogToEventLog { get; private set; }

        public List<DirectoryInfo> Folders { private set; get; }

        public CommandLine(params string[] args)
        {
            Folders = new List<DirectoryInfo>();
            LogLevel = LogLevel.Info;

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-a":
                        ScanAllFixedDrives = true;
                        break;
                    case "-e":
                        LogToEventLog = true;
                        break;
                    case "-q":
                        LogLevel = LogLevel.Warning;
                        break;
                    case "-s":
                        SimulationMode = true;
                        break;
                    case "-v":
                        LogLevel = LogLevel.Debug;
                        break;
                    default:
                        Folders.Add(new DirectoryInfo(arg));
                        break;
                }
            }
        }
    }
}
