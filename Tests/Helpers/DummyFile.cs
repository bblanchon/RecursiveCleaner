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

namespace RecursiveCleaner.Tests.Helpers
{
    class DummyFile : FileSystemInfo
    {
        bool exists = true;
        readonly string name;

        public DummyFile(string name="tmpfile.tmp")
        {
            this.name = name;
        }

        public override void Delete()
        {
            exists = false;
        }

        public override bool Exists
        {
            get { return exists; }
        }

        public override string Name
        {
            get { return name; }
        }         
    }
}
