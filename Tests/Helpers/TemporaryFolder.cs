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
using System.IO;

namespace RecursiveCleaner.Tests.Helpers
{
    class TemporaryFolder : IDisposable
    {
        private static int count = 0;

        public TemporaryFolder()
            : this("tmp"+count++)
        {

        }

        public TemporaryFolder(string name)
        {
            DirectoryInfo = new DirectoryInfo(name);
            Console.WriteLine("Create folder {0}", DirectoryInfo.Name);
            DirectoryInfo.Create();
        }

        public TemporaryFile CreateFile(string name)
        {
            return new TemporaryFile(System.IO.Path.Combine(DirectoryInfo.Name,name));
        }

        public TemporaryFile CreateFile()
        {
            var num = 1 + DirectoryInfo.EnumerateFiles().Count();
            return CreateFile("tmp" + num);
        }

        public DirectoryInfo DirectoryInfo
        {
            private set;
            get;
        }

        public IEnumerable<FileInfo> Files
        {
            get { return DirectoryInfo.EnumerateFiles(); }
        }

        public void Dispose()
        {
            Console.WriteLine("Delete folder {0}", DirectoryInfo.Name);
            DirectoryInfo.Delete(true);
        }

        public string Path 
        { 
            get 
            {
                return DirectoryInfo.FullName;
            } 
        }
    }
}
