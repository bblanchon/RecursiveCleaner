using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecursiveCleaner.Config
{
    class UnknownAttributesException : Exception
    {
        public UnknownAttributesException(IEnumerable<string> names)
            : base("Unknown attribute(s): "+string.Join(", ", names))
        {

        }
    }
}
