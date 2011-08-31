using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecursiveCleaner.Engine.Filters
{
    abstract class ParentFilter : IFilter
    {
        private IEnumerable<IFilter> children;

        public IEnumerable<IFilter> Children
        {
            get { return children; }
            set { children = value.ToList(); }
        }

        public abstract bool IsMatch(System.IO.FileSystemInfo fsi);
    }    
}
