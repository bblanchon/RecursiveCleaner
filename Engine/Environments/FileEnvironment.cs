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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Engine.Environments
{
    class FileEnvironment : Environment
    {
        public FileEnvironment(FileSystemInfo file, Environment parent)
            : base(parent)
        {
            Add("source.name", file.Name);
            Add("source.date", file.LastWriteTime.Date.ToLongDateString());
            Add("source.time", file.LastWriteTime.Date.ToLongTimeString());
            Add("source.year", file.LastWriteTime.Date.Year);
            Add("source.month", file.LastWriteTime.Date.Month);
            Add("source.day", file.LastWriteTime.Date.Day);
            Add("source.hour", file.LastWriteTime.Date.Hour);
            Add("source.minute", file.LastWriteTime.Date.Minute);
            Add("source.second", file.LastWriteTime.Date.Second);
        }
    }
}
