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

namespace RecursiveCleaner.Engine.Rules
{
    using Environments;

    abstract class MoveRuleBase : RuleBase
    {
        public enum IfExistsMode
        {
            Rename,
            Cancel,
            Recycle,  
            Delete,                      
        }

        public IfExistsMode IfExists { get; set; }

        protected void MoveCore(FileSystemInfo fsi, string destination, Environment environment)
        {
            var source = fsi.FullName;

            if (fsi is FileInfo)
            {
                MoveFile(source, destination, environment);
            }

            if( fsi is DirectoryInfo )
            {
                MoveDirectory(source, destination, environment);
            }
        }

        void MoveDirectory(string source, string destination, Environment environment)
        {
            if (Directory.Exists(destination))
            {
                if (IfExists == IfExistsMode.Cancel)
                {
                    Log.Info("Move {0}... Cancelled because destination directory exists", source);
                    return;
                }

                if (IfExists == IfExistsMode.Recycle)
                {
                    try
                    {
                        if (!environment.IsSimulating)
                        {
                            RecycleRule.Recycle(destination);
                        }
                        Log.Info("Move {0}... existing destination file recycled", source);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Move {0}... failed to recycle existing destination file: {1}", source, e.Message);
                        return;
                    }
                }

                if (IfExists == IfExistsMode.Delete)
                {
                    try
                    {
                        if (!environment.IsSimulating)
                        {
                            Directory.Delete(destination, true);
                        }
                        Log.Info("Move {0}... existing destination file deleted", source);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Move {0}... failed to delete existing destination file: {1}", source, e.Message);
                        return;
                    }
                }

                if (IfExists == IfExistsMode.Rename)
                {
                    var directory = Path.GetDirectoryName(destination);
                    var baseName = Path.GetFileNameWithoutExtension(source);
                    var ext = Path.GetExtension(source);

                    for (var i = 1;; i++)
                    {
                        if (i > 1000)
                        {
                            Log.Warning("Move {0}... failed to find a new name", source);
                            return;
                        }

                        destination = Path.Combine(directory, string.Format("{0} ({1}){2}", baseName, i, ext));

                        if (!Directory.Exists(destination))
                        {
                            break;
                        }
                    }
                }
            }

            try
            {
                if (!environment.IsSimulating)
                {
                    Directory.Move(source, destination);
                }
                Log.Info("Move {0} to {1}... OK", source, destination);
            }
            catch (Exception e)
            {
                Log.Info("Move {0} to {1}... {2}", source, destination, e.Message);
            }
        }

        void MoveFile(string source, string destination, Environment environment)
        {
            if (File.Exists(destination))
            {
                if (IfExists == IfExistsMode.Cancel)
                {
                    Log.Info("Move {0}... Cancelled because destination file exists", source);
                    return;
                }

                if (IfExists == IfExistsMode.Recycle)
                {
                    try
                    {
                        if (!environment.IsSimulating)
                        {
                            RecycleRule.Recycle(destination);
                        }
                        Log.Info("Move {0}... existing destination file recycled", source);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Move {0}... failed to recycle existing destination file: {1}", source, e.Message);
                        return;
                    }
                }

                if (IfExists == IfExistsMode.Delete)
                {
                    try
                    {
                        if (!environment.IsSimulating)
                        {
                            File.Delete(destination);
                        }
                        Log.Info("Move {0}... existing destination file deleted", source);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Move {0}... failed to delete existing destination file: {1}", source, e.Message);
                        return;
                    }
                }

                if (IfExists == IfExistsMode.Rename)
                {
                    var directory = Path.GetDirectoryName(destination);
                    var baseName = Path.GetFileNameWithoutExtension(source);
                    var ext = Path.GetExtension(source);

                    for (var i = 1;; i++)
                    {
                        if (i > 1000)
                        {
                            Log.Warning("Move {0}... failed to find a new name", source);
                            return;
                        }

                        destination = Path.Combine(directory, string.Format("{0} ({1}){2}", baseName, i, ext));

                        if (!File.Exists(destination))
                        {
                            break;
                        }
                    }
                }
            }

            try
            {
                if (!environment.IsSimulating)
                {
                    File.Move(source, destination);
                }
                Log.Info("Move {0} to {1}... OK", source, destination);
            }
            catch (Exception e)
            {
                Log.Info("Move {0} to {1}... {2}", source, destination, e.Message);
            }
        }
    }
}
