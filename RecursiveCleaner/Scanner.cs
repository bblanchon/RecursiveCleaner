using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RecursiveCleaner.Filters;
using System.Runtime.InteropServices;
using RecursiveCleaner.Config;
using RecursiveCleaner.Imports;
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
            var configPath = Path.Combine(folder.FullName, ConfigFile.Filename);

            if (!File.Exists(configPath))return Enumerable.Empty<IRule>();
            
            Log.Info("Found {0}", configPath);
            try
            {
                return ConfigFile.Read(configPath);
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

            rules = ReadFolderLocalRules(dir).Concat(rules).ToArray();

            var folderRules = rules.Where(x=>x.Target==RuleTarget.Folders || x.Target==RuleTarget.FilesAndFolders);
            var fileRules = rules.Where(x=>x.Target==RuleTarget.Files || x.Target==RuleTarget.FilesAndFolders);
            
            foreach (var subFolder in dir.EnumerateDirectories())
            {
                var matchingRule = folderRules.FirstOrDefault(x => x.IsMatch(subFolder));

                if( matchingRule != null ) 
                {
                    matchingRule.Apply(subFolder, IsSimulating);
                }
                else
                {
                    ScanFolder(subFolder, rules.Where(x=>x.AppliesToSubfolders));
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
    }
}
