using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RecursiveCleaner.Engine.Filters;
using System.IO;

namespace RecursiveCleaner.Tests.Filters
{
    class LambdaFilter : IFilter
    {
        readonly Predicate<FileSystemInfo> func;

        public LambdaFilter(Predicate<FileSystemInfo> func)
        {
            this.func = func;
        }
        
        public bool IsMatch(FileSystemInfo fsi)
        {
            return func(fsi);
        }
    }
}
