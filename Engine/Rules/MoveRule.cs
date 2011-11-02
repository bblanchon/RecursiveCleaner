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

namespace RecursiveCleaner.Engine.Rules
{
    class MoveRule : RuleBase
    {
        public enum IfExistsMode
        {
            Rename,
            Cancel,
            Recycle,  
            Delete,                      
        }

        public IfExistsMode IfExists { get; set; }
        public string Destination { get; set; }

        public override void Apply(FileSystemInfo fsi, bool simulation)
        {
            Destination = Path.GetFullPath(Destination);
            if( !Directory.Exists(Destination) ) 
            {
                Log.Warning("Move {0}... Destination folder ({1}) doesn't exists",
                    fsi.FullName, Destination);
                return;
            }

            var destination = Path.Combine(Destination, fsi.Name);

            if (fsi is FileInfo)
            {
                var file = fsi as FileInfo;

                if (File.Exists(destination))
                {
                    if (IfExists == IfExistsMode.Cancel)
                    {
                        Log.Info("Move {0}... Cancelled because destination file exists", fsi.FullName);
                        return;
                    }

                    if (IfExists == IfExistsMode.Recycle)
                    {
                        try
                        {
                            if (!simulation) RecycleRule.Recycle(destination);
                            Log.Info("Move {0}... existing destination file recycled", fsi.FullName);
                        }
                        catch (Exception e)
                        {
                            Log.Warning("Move {0}... failed to recycle existing destination file: {1}", fsi.FullName, e.Message);
                            return;
                        }
                    }

                    if (IfExists == IfExistsMode.Delete)
                    {
                        try
                        {
                            if (!simulation) File.Delete(destination);
                            Log.Info("Move {0}... existing destination file deleted", fsi.FullName);
                        }
                        catch (Exception e)
                        {
                            Log.Warning("Move {0}... failed to delete existing destination file: {1}", fsi.FullName, e.Message);
                            return;
                        }
                    }

                    if (IfExists == IfExistsMode.Rename)
                    {
                        var baseName = Path.GetFileNameWithoutExtension(fsi.Name);
                        var ext = Path.GetExtension(fsi.Name);

                        for (var i = 1; ; i++)
                        {
                            if (i > 1000)
                            {
                                Log.Warning("Move {0}... failed to find a new name", fsi.FullName);
                                return;
                            }

                            destination = Path.Combine(Destination, string.Format("{0} ({1}){2}", baseName, i, ext));

                            if (!File.Exists(destination)) break;
                        }
                    }
                }

                try
                {
                    if (!simulation) file.MoveTo(destination);
                    Log.Info("Move {0} to {1}... OK", fsi.FullName, destination);
                }
                catch (Exception e)
                {
                    Log.Info("Move {0} to {1}... {2}", fsi.FullName, destination, e.Message);
                }                
            }
            
            if( fsi is DirectoryInfo )
            {
                var folder = fsi as DirectoryInfo;

                if (Directory.Exists(destination))
                {
                    if (IfExists == IfExistsMode.Cancel)
                    {
                        Log.Info("Move {0}... Cancelled because destination directory exists", fsi.FullName);
                        return;
                    }

                    if (IfExists == IfExistsMode.Recycle)
                    {
                        try
                        {
                            if (!simulation) RecycleRule.Recycle(destination);
                            Log.Info("Move {0}... existing destination file recycled", fsi.FullName);
                        }
                        catch (Exception e)
                        {
                            Log.Warning("Move {0}... failed to recycle existing destination file: {1}", fsi.FullName, e.Message);
                            return;
                        }
                    }

                    if (IfExists == IfExistsMode.Delete)
                    {
                        try
                        {
                            if (!simulation) Directory.Delete(destination, true);
                            Log.Info("Move {0}... existing destination file deleted", fsi.FullName);
                        }
                        catch (Exception e)
                        {
                            Log.Warning("Move {0}... failed to delete existing destination file: {1}", fsi.FullName, e.Message);
                            return;
                        }
                    }

                    if (IfExists == IfExistsMode.Rename)
                    {
                        var baseName = Path.GetFileNameWithoutExtension(fsi.Name);
                        var ext = Path.GetExtension(fsi.Name);

                        for (var i = 1; ; i++)
                        {
                            if (i > 1000)
                            {
                                Log.Warning("Move {0}... failed to find a new name", fsi.FullName);
                                return;
                            }

                            destination = Path.Combine(Destination, string.Format("{0} ({1}){2}", baseName, i, ext));

                            if (!Directory.Exists(destination)) break;
                        }
                    }

                    try
                    {
                        if (!simulation) folder.MoveTo(destination);
                        Log.Info("Move {0} to {1}... OK", fsi.FullName, destination);
                    }
                    catch (Exception e)
                    {
                        Log.Info("Move {0} to {1}... {2}", fsi.FullName, destination, e.Message);
                    } 
                }
            }            
        }
    }
}
