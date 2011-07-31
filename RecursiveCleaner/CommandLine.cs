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
