using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using RecursiveCleaner.Filters;

namespace RecursiveCleaner.Rules
{
    abstract class RuleBase : IRule
    {
        public RuleTarget Target { get; set; }

        public List<IFilter> Filters { get; private set; }

        public RuleBase()
        {
            Filters = new List<IFilter>();
        }

        public bool IsMatch(FileSystemInfo fsi)
        {
            return Filters.All(x => x.IsMatch(fsi));
        }

        public abstract void Apply(FileSystemInfo fsi, bool simulation);
    }
}
