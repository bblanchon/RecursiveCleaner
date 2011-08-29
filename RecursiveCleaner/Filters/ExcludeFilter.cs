using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Filters
{
    class ExcludeFilter : IFilter
    {
        public ExcludeFilter(IEnumerable<IFilter> innerFilters)
        {
            InnerFilters = new List<IFilter>(innerFilters);
        }

        public bool IsMatch(FileSystemInfo fsi)
        {
            return !InnerFilters.Any(x => x.IsMatch(fsi));
        }

        public IEnumerable<IFilter> InnerFilters
        {
            get; private set;
        }
    }
}
