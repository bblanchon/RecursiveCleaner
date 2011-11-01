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

using System.IO;
using System.Runtime.InteropServices;
using RecursiveCleaner.Engine.Imports;
using System;

namespace RecursiveCleaner.Engine.Rules
{
    class RecycleRule : RuleBase
    {
        public override void Apply(FileSystemInfo fsi, bool simulation)
        {
            var path = fsi.FullName;

            if (!simulation)
            {
                try
                {
                    Recycle(path);
                    Log.Info("Recycle {0}... OK", path);
                }
                catch( Exception e )
                {
                    Log.Warning("Recycle {0}... {1}", path, e.Message);
                }
            }
            else
            {
                Log.Info("Recycle {0}", path);
            }
        }

        internal static void Recycle(string path)
        {
            var shf = new Shell32.SHFILEOPSTRUCT();
            shf.wFunc = Shell32.FO_Func.FO_DELETE;
            shf.fFlags = Shell32.FOF_ALLOWUNDO | Shell32.FOF_NO_UI;
            shf.pFrom = Marshal.StringToHGlobalUni(path + '\0');

            var ret = Shell32.SHFileOperation(ref shf);
            if (ret != 0) throw new Exception("Error #" + ret);
        }
    }
}
