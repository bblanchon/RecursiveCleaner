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

namespace RecursiveCleaner.Engine
{
    using Config;
    using Rules;
    using Environments;    
    
    public class Scanner
    {
        public bool IsSimulating { get; set; }

        public bool IncludeSystemFolders { get; set; }

        public bool IncludeSystemFiles { get; set; }

        public void ScanFolder(DirectoryInfo folder)
        {
            var environment = new Environment
            {
                { "date",   DateTime.Now.ToLongDateString() },
                { "time",   DateTime.Now.ToLongTimeString() },
                { "year",   DateTime.Now.Year },
                { "month",  DateTime.Now.Month },
                { "day",    DateTime.Now.Day },
                { "hour",   DateTime.Now.Hour },
                { "minute", DateTime.Now.Minute }
            };

            environment.IsSimulating = IsSimulating;

            var parentsRules = new List<IRule>();

            for (var parent = folder.Parent; parent != null; parent = parent.Parent)
            {
                parentsRules.AddRange(ReadFolderLocalRules(parent));
            }

            ScanFolder(folder, parentsRules, environment);
        }

        IEnumerable<IRule> ReadFolderLocalRules(DirectoryInfo folder)
        {
            var configPath = Path.Combine(folder.FullName, ConfigFileReader.Filename);

            if (!File.Exists(configPath)) return Enumerable.Empty<IRule>();
            
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

        void ScanFolder(DirectoryInfo dir, IEnumerable<IRule> parentRules, Environment parentEnvironment)
        {
            Log.Debug("Scanning folder {0}", dir.FullName);

            var environment = new Environment(parentEnvironment)
            {
                CurrentDirectory = dir                       
            };
            
            try
            {
                //
                // 1. Read local config file
                //
                var rules = ReadFolderLocalRules(dir).Concat(parentRules).ToArray();

                var folderRules = rules.Where(x => x.Target == RuleTarget.Folders || x.Target == RuleTarget.FilesAndFolders);
                var fileRules = rules.Where(x => x.Target == RuleTarget.Files || x.Target == RuleTarget.FilesAndFolders);
                
                //
                // 2. Scan folders
                //
                foreach (var subFolder in dir.EnumerateDirectories())                
                {
                    var folderEnvironment = new FileEnvironment(subFolder, environment);

                    // skip system folders
                    if ((subFolder.Attributes & FileAttributes.System) != 0 && !IncludeSystemFolders)
                        continue;

                    // find first matching rule
                    var matchingRule = folderRules.FirstOrDefault(x => x.IsMatch(subFolder, folderEnvironment));
                                        
                    if (matchingRule != null)
                    {
                        // apply rule to the folder
                        matchingRule.Apply(subFolder, folderEnvironment);
                    }
                    else
                    {
                        // scan folder recursively, using appropriate rules
                        ScanFolder(subFolder, rules.Where(x => x.AppliesToSubfolders), folderEnvironment);
                    }
                }
                
                //
                // 3. Scan files
                //
                if (fileRules.Any())
                {
                    foreach (var file in dir.EnumerateFiles())
                    {                      
                        var fileEnvironment = new FileEnvironment(file, environment);

                        // skip system files
                        if ((file.Attributes & FileAttributes.System) != 0 && !IncludeSystemFiles)
                            continue;

                        // skip file RecursiveCleaner.config
                        if (file.Name.Equals(ConfigFileReader.Filename, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        // get first matching rule
                        var matchingRule = fileRules.FirstOrDefault(x => x.IsMatch(file, fileEnvironment));

                        if (matchingRule != null)
                        {
                            Log.Debug("File \"{0}\" matches {1}", file.Name, matchingRule);
                            // apply the rule to the file
                            matchingRule.Apply(file, fileEnvironment);
                        }
                        else
                        {
                            Log.Debug("No match for file \"{0}\"", file.Name);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Log.Debug("Skip folder {0}: {1}", dir.FullName, e.Message);
            }
            catch (Exception e)
            {
                Log.Warning("Failed to scan folder {0}: {1}", dir.FullName, e.Message);
            }
        }           
    }
}
