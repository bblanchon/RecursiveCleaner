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

            foreach( var folder in cmdLine.Folders )
                scanner.ScanFolder(folder);

#if DEBUG
            Console.WriteLine("-- PRESS ENTER TO CLOSE --");
            Console.ReadLine();
#endif
        }
    }
}
