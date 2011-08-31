using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Engine.Filters
{
    class DelegateFilter : IFilter
    {
        readonly Predicate<FileSystemInfo> condition;

        public DelegateFilter(Predicate<FileSystemInfo> condition)
        {
            this.condition = condition;
        }

        public bool IsMatch(FileSystemInfo fsi)
        {
            return condition(fsi);
        }
    }
}
