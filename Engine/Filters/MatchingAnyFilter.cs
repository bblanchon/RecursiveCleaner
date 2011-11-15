using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Engine.Filters
{
    using Environments;

    class MatchingAnyFilter : ParentFilter
    {
        public override bool IsMatch(FileSystemInfo fsi, Environment environment)
        {
            return Children.Any(x => x.IsMatch(fsi, environment));
        }
    }
}
