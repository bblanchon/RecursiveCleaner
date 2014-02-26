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

using System.IO;
using System.Text.RegularExpressions;

namespace RecursiveCleaner.Engine.Filters
{
    using Environments;

    class RegexFilter : IFilter
    {
        public RegexFilter(string pattern)
        {
            regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        readonly Regex regex;

        public bool IsMatch(FileSystemInfo fsi, Environment environment)
        {
            var m = regex.Match(fsi.Name);

            if (!m.Success) return false;

            foreach( var groupName in regex.GetGroupNames() )
            {   
                var group = m.Groups[groupName];

                Log.Debug("Groups[\"{0}\"] = {1}", groupName, group);

                if (group.Success)
                {
                    environment.Add(groupName, group.Value);
                }
            }

            return true;
        }
    }
}
