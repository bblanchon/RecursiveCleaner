/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011 Benoit Blanchon
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RecursiveCleaner.Engine.Filters
{
    class WildcardsFilter : RegexFilter
    {
        public WildcardsFilter(string pattern)
            : base(BuildRegexPattern(pattern))
        {
        }

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
    }
}
