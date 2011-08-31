using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Tests.Helpers
{
    class TemporaryFile : IDisposable
    {        
        public TemporaryFile(string name="testfile.tmp")
        {
            FileInfo = new FileInfo(name);
            FileInfo.Create().Close();
        }

        public string Contents
        {
            set
            {
                File.WriteAllText(Path, value);
            }
            get
            {
                return File.ReadAllText(Path);
            }
        }

        public string Path
        {
            get { return FileInfo.FullName; }
        }

        public FileInfo FileInfo
        {
            get;
            private set;
        }

        public void Dispose()
        {
            FileInfo.Delete();
        }
    }
}
