using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecursiveCleaner.Filters
{
    interface IFilter
    {
        bool IsMatch(string name);
    }
}
