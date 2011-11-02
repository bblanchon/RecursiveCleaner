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
    class TemporaryFile : IDisposable
    {        
        public TemporaryFile(string name="testfile.tmp")
        {            
            FileInfo = new FileInfo(name);
            Console.WriteLine("Create file {0}", FileInfo.Name);
            FileInfo.Create().Close();
        }

        public string Contents
        {
            set
            {
                File.WriteAllText(Path, value);                
            }
            get
            {
                return File.ReadAllText(Path);
            }
        }

        public string Path
        {
            get { return FileInfo.FullName; }
        }

        public string Name
        {
            get { return FileInfo.Name; }
        }

        public FileInfo FileInfo
        {
            get;
            private set;
        }

        public void Dispose()
        {
            Console.WriteLine("Delete file {0}", FileInfo.Name);
            FileInfo.Delete();
        }
    }
}
