using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecursiveCleaner.Config
{
    class IgnoredElementException : Exception
    {
        public IgnoredElementException(string elementName)
            : base(string.Format("Ignoring element <{0}>", elementName))
        {
        }
    }
}
