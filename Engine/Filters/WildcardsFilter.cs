/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011-2012 Benoit Blanchon
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace RecursiveCleaner.Engine.Filters
{
    using Environments;

    class WildcardsFilter : IFilter
    {
        readonly Regex regex;

        public WildcardsFilter(string pattern)
        {
            regex = new Regex(BuildRegexPattern(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(FileSystemInfo fsi, Environment environment)
        {
            return regex.IsMatch(fsi.Name);
        }

        #region Wildcards --> Regex

        static string BuildRegexPattern(string pattern)
        {
            string s = "^" + Regex.Escape(pattern) + "$";

            foreach (var kvp in map)
                s = s.Replace(kvp.Key, kvp.Value);

            return s;
        }

        static Dictionary<string, string> map = new Dictionary<string, string>()
        {
            { @"\*", ".*" },
            { @"\?", "." },
            { @"\|", "$|^" },
        };
        
        #endregion
    }
}
