using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RecursiveCleaner.Filters;

namespace RecursiveCleaner.Scanner
{
    class Rule
    {
        public RuleAction Action { get; set; }

        public RuleTarget Target { get; set; }

        public List<IFilter> Filters { get; private set; }

        public Rule()
        {
            Filters = new List<IFilter>();
        }

        public bool IsMatch(string name)
        {
            return Filters.All(x => x.IsMatch(name));
        }
    }
}
