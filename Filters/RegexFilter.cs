using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RecursiveCleaner.Filters
{
    class RegexFilter : IFilter
    {
        public RegexFilter(string pattern)
        {
            regex = new Regex(pattern, RegexOptions.Compiled);
        }

        readonly Regex regex;

        public bool IsMatch(string name)
        {
            return regex.IsMatch(name);
        }
    }
}
