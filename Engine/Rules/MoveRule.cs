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

namespace RecursiveCleaner.Engine.Rules
{
    using Environments;

    class MoveRule : MoveRuleBase
    {
        public MoveRule()
        {
            CreateFolder = true;
        }

        public string Destination { get; set; }
        public bool CreateFolder { get; set; }

        public override void Apply(FileSystemInfo fsi, Environment environment)
        {
            var destinationFolder = environment.ExpandVariables(environment.GetFullPath(Destination));

            if (!Directory.Exists(destinationFolder))
            {
                if (CreateFolder)
                {
                    Log.Info("Create folder {0}", destinationFolder);
                    if (!environment.IsSimulating) Directory.CreateDirectory(destinationFolder);
                }
                else
                {
                    Log.Warning("Move {0}... Destination folder ({1}) doesn't exists",
                        fsi.FullName, destinationFolder);
                    return;
                }
            }

            MoveCore(fsi, Path.Combine(destinationFolder, fsi.Name), environment);
        }
    }
}
