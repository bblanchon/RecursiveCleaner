using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner
{
    class CommandLine
    {
        public bool SimulationMode { private set; get; }

        public bool ScanAllFixedDrives { private set; get; }

        public LogLevel LogLevel { private set; get;}

        public List<DirectoryInfo> Folders { private set; get; }

        public CommandLine(params string[] args)
        {
            Folders = new List<DirectoryInfo>();
            LogLevel = LogLevel.Info;

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-v":
                        LogLevel = LogLevel.Debug;
                        break;
                    case "-q":
                        LogLevel = LogLevel.Warning;
                        break;
                    case "-s":
                        SimulationMode = true;
                        break;
                    case "-a":
                        ScanAllFixedDrives = true;
                        break;
                    default:
                        Folders.Add(new DirectoryInfo(arg));
                        break;
                }
            }
        }
    }
}
