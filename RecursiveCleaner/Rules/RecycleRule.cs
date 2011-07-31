using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RecursiveCleaner.Imports;
using System.Runtime.InteropServices;

namespace RecursiveCleaner.Rules
{
    class RecycleRule : RuleBase
    {
        public override void Apply(FileSystemInfo fsi, bool simulation)
        {
            var path = fsi.FullName;

            if (!simulation)
            {
                var shf = new Shell32.SHFILEOPSTRUCT();
                shf.wFunc = Shell32.FO_Func.FO_DELETE;
                shf.fFlags = Shell32.FOF_ALLOWUNDO | Shell32.FOF_NO_UI;
                shf.pFrom = Marshal.StringToHGlobalUni(path + '\0');
                var ret = Shell32.SHFileOperation(ref shf);

                if (ret == 0)
                    Log.Warning("Recycle {0}... Error #{1}", path, ret);
                else
                    Log.Info("Recycle {0}... OK", path);
            }
            else
            {
                Log.Info("Recycle {0}", path);
            }
        }
    }
}
