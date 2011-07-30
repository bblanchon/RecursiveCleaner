using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Rules
{
    class DeleteRule : RuleBase
    {
        public override void Apply(FileSystemInfo fsi, bool simulation)
        {
            if (!simulation)
            {
                try
                {
                    fsi.Delete();
                    Log.Info("Delete {0}... OK", fsi.FullName);
                }
                catch (Exception e)
                {
                    Log.Warning("Delete {0}... {1}", fsi.FullName, e.Message);
                }
            }
            else
            {
                Log.Info("Delete {0}", fsi.FullName);
            }
        }     
    }
}
