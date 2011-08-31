using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Tests.Helpers
{
    class DummyFile : FileSystemInfo
    {
        bool exists = true;
        readonly string name;

        public DummyFile(string name="tmpfile.tmp")
        {
            this.name = name;
        }

        public override void Delete()
        {
            exists = false;
        }

        public override bool Exists
        {
            get { return exists; }
        }

        public override string Name
        {
            get { return name; }
        }
    }
}
