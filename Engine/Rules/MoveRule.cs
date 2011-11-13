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
