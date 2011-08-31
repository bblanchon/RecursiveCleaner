using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Engine.Filters
{
    class MatchingAnyFilter : ParentFilter
    {
        public override bool IsMatch(FileSystemInfo fsi)
        {
            return Children.Any(x => x.IsMatch(fsi));
        }
    }
}
