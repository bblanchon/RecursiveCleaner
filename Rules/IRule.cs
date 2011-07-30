using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RecursiveCleaner.Filters;

namespace RecursiveCleaner.Rules
{
    interface IRule
    {
        RuleTarget Target { get; set; }

        List<IFilter> Filters { get; }

        bool IsMatch(FileSystemInfo fsi);

        void Apply(FileSystemInfo fsi, bool simulation);
    }
}
