using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Rules
{
    class IgnoreRule : RuleBase
    {
        public override void Apply(FileSystemInfo fsi, bool simulation)
        {
            Log.Info("Ignore {0}", fsi.FullName);
        }
    }
}
