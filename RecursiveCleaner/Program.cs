using System;
using System.IO;
using System.Linq;
using RecursiveCleaner.Config;
using RecursiveCleaner.Scanner;

namespace RecursiveCleaner
{
    class Program
    {
        static void Main(string[] args)
        {            
            var cmdLine = new CommandLine(args);

            Log.Filter = cmdLine.LogLevel;

            var scanner = new Engine();            

#if !DEBUG
            scanner.IsSimulating = cmdLine.SimulationMode;
#else
            scanner.IsSimulating = true;
#endif

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

#if DEBUG
            Console.WriteLine("-- PRESS ENTER TO CLOSE --");
            Console.ReadLine();
#endif
        }
    }
}
