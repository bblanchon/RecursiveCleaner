using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RecursiveCleaner.Filters
{
    interface IFilter
    {
        bool IsMatch(FileSystemInfo fsi);
    }
}
