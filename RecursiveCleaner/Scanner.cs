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
using System.IO;
using System.Linq;
using RecursiveCleaner.Config;
using RecursiveCleaner.Rules;

namespace RecursiveCleaner.Scanner
{
    class Engine
    {
        public bool IsSimulating { get; set; }

        public void ScanFolder(DirectoryInfo folder)
        {
            var parentsRules = new List<IRule>();

            for (var parent = folder.Parent; parent != null; parent = parent.Parent)
            {
                parentsRules.AddRange(ReadFolderLocalRules(parent));
            }

            ScanFolder(folder, parentsRules);
        }

        IEnumerable<IRule> ReadFolderLocalRules(DirectoryInfo folder)
        {
            var configPath = Path.Combine(folder.FullName, ConfigFileReader.Filename);

            if (!File.Exists(configPath))return Enumerable.Empty<IRule>();
            
            Log.Info("Found {0}", configPath);
            try
            {
                return ConfigFileReader.Read(configPath);
            }
            catch (Exception e)
            {
                Log.Error("Failed to read {0} : {1}", configPath, e.Message);
                return Enumerable.Empty<IRule>();
            }            
        }

        public void ScanFolder(DirectoryInfo dir, IEnumerable<IRule> rules)
        {
            Log.Debug("Scanning folder {0}", dir.FullName);

            try
            {
                rules = ReadFolderLocalRules(dir).Concat(rules).ToArray();

                var folderRules = rules.Where(x => x.Target == RuleTarget.Folders || x.Target == RuleTarget.FilesAndFolders);
                var fileRules = rules.Where(x => x.Target == RuleTarget.Files || x.Target == RuleTarget.FilesAndFolders);

                foreach (var subFolder in dir.EnumerateDirectories())
                {
                    var matchingRule = folderRules.FirstOrDefault(x => x.IsMatch(subFolder));

                    if (matchingRule != null)
                    {
                        matchingRule.Apply(subFolder, IsSimulating);
                    }
                    else
                    {
                        ScanFolder(subFolder, rules.Where(x => x.AppliesToSubfolders));
                    }
                }

                if (fileRules.Any())
                {
                    foreach (var file in dir.EnumerateFiles())
                    {
                        var matchingRule = fileRules.FirstOrDefault(x => x.IsMatch(file));

                        if (matchingRule != null)
                        {
                            matchingRule.Apply(file, IsSimulating);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning("Failed to scan folder {0}: {1}", dir.FullName, e.Message);
            }
        }           
    }
}
