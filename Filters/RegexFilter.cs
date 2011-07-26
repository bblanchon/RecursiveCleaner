using System;
using System.Collections.Generic;
using System.IO;
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

        public bool IsMatch(FileSystemInfo fsi)
        {
            return regex.IsMatch(fsi.Name);
        }
    }
}
