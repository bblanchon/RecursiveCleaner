using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Engine.Environments
{
    class FileEnvironment : Environment
    {
        public FileEnvironment(FileSystemInfo file, Environment parent)
            : base(parent)
        {
            Add("source.name", file.Name);
            Add("source.date", file.LastWriteTime.Date.ToLongDateString());
            Add("source.time", file.LastWriteTime.Date.ToLongTimeString());
            Add("source.year", file.LastWriteTime.Date.Year);
            Add("source.month", file.LastWriteTime.Date.Month);
            Add("source.day", file.LastWriteTime.Date.Day);
            Add("source.hour", file.LastWriteTime.Date.Hour);
            Add("source.minute", file.LastWriteTime.Date.Minute);
            Add("source.second", file.LastWriteTime.Date.Second);
        }
    }
}
