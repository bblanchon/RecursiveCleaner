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
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace RecursiveCleaner.Engine.Environments
{
    class Environment : IEnumerable
    {
        private static readonly Regex syntaxRegex = new Regex(@"%([\w\.]+?)%");
        private readonly Environment parent;
        private readonly Dictionary<string,object> variables;

        public Environment(Environment parent=null)
        {
            this.parent = parent;
            this.variables = new Dictionary<string, object>();

            if (parent != null)
            {
                IsSimulating = parent.IsSimulating;
                CurrentDirectory = parent.CurrentDirectory;
            }
        }

        public bool IsSimulating { get; set; }

        private string GetKeyFromVariableName(string name)
        {
            return name.ToLowerInvariant();
        }       

        public object Get(string name)
        {
            var key = GetKeyFromVariableName(name);

            if (variables.ContainsKey(key)) return variables[key];
            
            if (parent != null) return parent.Get(name);

            var systemVariable = System.Environment.GetEnvironmentVariable(name);
            if (systemVariable != null) return systemVariable;

            Log.Warning("Unknown environment variable \"{0}\"", name);
            return null;
        }

        public void Add(string name, object value)
        {
            var key = GetKeyFromVariableName(name);

            if (variables.ContainsKey(key)) variables[key] = value;
            else variables.Add(key, value);
        }

        public string ExpandVariables(string s)
        {
            var matches = syntaxRegex.Matches(s);

            foreach (Match m in matches)
            {
                var name = m.Groups[1].Value;
                var value = Get(name);

                if (value != null)                
                    s = s.Replace(m.Value, value.ToString());                
            }

            return s;
        }
                
        public IEnumerator GetEnumerator()
        {
            // not used, just here to use the C# 3.0 collection initializer
            throw new NotImplementedException();
        }

        public DirectoryInfo CurrentDirectory { get; set; }

        public string GetFullPath(string path)
        {
            return 
                Path.IsPathRooted(path) ? path : 
                CurrentDirectory!=null ? Path.Combine(CurrentDirectory.FullName, path) :
                Path.GetFullPath(path);
        }
    }
}
