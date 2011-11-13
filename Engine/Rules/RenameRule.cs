using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Engine.Rules
{
    using Environments;

    class RenameRule : MoveRuleBase
    {
        public string Name { get; set; }

        public override void Apply(FileSystemInfo fsi, Environment environment)
        {
            var directory = Path.GetDirectoryName(fsi.FullName);
            var name = environment.ExpandVariables(Name);

            MoveCore(fsi, Path.Combine(directory, name), environment);
        }
    }
}
