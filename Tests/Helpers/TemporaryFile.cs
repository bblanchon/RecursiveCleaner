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
