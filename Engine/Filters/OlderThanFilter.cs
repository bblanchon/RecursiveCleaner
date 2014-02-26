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

using System;
using System.IO;
using System.Linq;

namespace RecursiveCleaner.Engine.Filters
{
    using Environments;

    class OlderThanFilter : IFilter
    {
        public OlderThanFilter(int years, int months, int days, int hours, int minutes, int seconds)
        {
            TimeSpan = new TimeSpan(years * 365 + months * 30 + days, hours, minutes, seconds);
        }

        public TimeSpan TimeSpan
        {
            get;
            private set;
        }

        public bool IsMatch(FileSystemInfo fsi, Environment environment)
        {
            if (fsi is DirectoryInfo)
            {
                var folder = fsi as DirectoryInfo;

                if (folder.EnumerateFileSystemInfos().Any())
                {
                    var newest = folder.EnumerateFiles("*", SearchOption.AllDirectories).Max(x => x.LastWriteTime);
                    return newest + TimeSpan < DateTime.Now;
                }
            }

            return fsi.LastWriteTime + TimeSpan < DateTime.Now;
        }
    }
}
