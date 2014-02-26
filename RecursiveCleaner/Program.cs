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
using System.IO;
using System.Linq;
using RecursiveCleaner.Engine;

namespace RecursiveCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdLine = new CommandLine(args);

#if DEBUG
            MainCore(cmdLine);
#else
            try
            {
                MainCore(cmdLine);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
#endif

            if (cmdLine.InteractiveMode)
            {
                Console.WriteLine("-- PRESS ENTER TO CLOSE --");
                Console.ReadLine();
            }
        }

        static void MainCore(CommandLine cmdLine)
        {
            if (cmdLine.ShowHelp)
            {
                Console.WriteLine("RecursiveCleaner - Scan and clean folders\r\n");
                Console.WriteLine("To use this software properly, you must create a file RecursiveCleaner.config");
                Console.WriteLine("in each folder you wan't to clean. This XML file contains the settings for the");
                Console.WriteLine("directory and all its subdirectories.");
                Console.WriteLine("See documentation for syntax.\r\n");
                Console.WriteLine(CommandLine.Documentation);
                return;
            }

            Log.Filter = cmdLine.LogLevel;
            Log.LogToEventLog = cmdLine.LogToEventLog;

            var scanner = new Scanner();

            scanner.IsSimulating = cmdLine.SimulationMode;

            Log.Info("Started with arguments: {0}", string.Join(" ", cmdLine.Arguments));

            if (cmdLine.ScanAllFixedDrives)
            {
                foreach (var drive in DriveInfo.GetDrives())
                    if (drive.DriveType == DriveType.Fixed && drive.IsReady)
                        scanner.ScanFolder(drive.RootDirectory);
            }

            foreach (var folder in cmdLine.Folders)
                scanner.ScanFolder(folder);

            if (!cmdLine.Folders.Any() && !cmdLine.ScanAllFixedDrives)
            {
                Log.Error("No folder specified (you may also try option -a).");
            }

            Log.Info("Terminated");
        }
    }
}
