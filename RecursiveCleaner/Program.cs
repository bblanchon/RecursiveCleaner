﻿/*
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

using System;
using System.IO;
using System.Linq;
using RecursiveCleaner.Scanner;

namespace RecursiveCleaner
{
    class Program
    {
        static void Main(string[] args)
        {            
            var cmdLine = new CommandLine(args);

            Log.Filter = cmdLine.LogLevel;
            Log.LogToEventLog = cmdLine.LogToEventLog;

            var scanner = new Engine();            

#if !DEBUG
            scanner.IsSimulating = cmdLine.SimulationMode;
#else
            scanner.IsSimulating = true;
#endif

            Log.Info("Started with arguments: {0}", string.Join(" ",args));

            if (cmdLine.ScanAllFixedDrives)
            {
                foreach (var drive in DriveInfo.GetDrives())
                    if (drive.DriveType == DriveType.Fixed && drive.IsReady )
                        scanner.ScanFolder(drive.RootDirectory);
            }

            foreach (var folder in cmdLine.Folders)
                scanner.ScanFolder(folder);

            if( !cmdLine.Folders.Any() && !cmdLine.ScanAllFixedDrives )
            {
                Log.Error("No folder specified (you may also try option -a).");      
            }

            Log.Info("Terminated");

#if DEBUG
            Console.WriteLine("-- PRESS ENTER TO CLOSE --");
            Console.ReadLine();
#endif
        }
    }
}
