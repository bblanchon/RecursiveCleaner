using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RecursiveCleaner.Filters;
using System.Runtime.InteropServices;
using RecursiveCleaner.Config;
using RecursiveCleaner.Imports;

namespace RecursiveCleaner.Scanner
{
    class Engine
    {
        public bool IsSimulating { get; set; }

        public void ScanFolder(DirectoryInfo folder)
        {
            var parentsRules = new List<Rule>();

            for (var parent = folder.Parent; parent != null; parent = parent.Parent)
            {
                parentsRules.AddRange(ReadFolderLocalRules(parent));
            }

            ScanFolder(folder, parentsRules);
        }

        IEnumerable<Rule> ReadFolderLocalRules(DirectoryInfo folder)
        {
            var configPath = Path.Combine(folder.FullName, ConfigFile.Filename);

            if (!File.Exists(configPath))return Enumerable.Empty<Rule>();
            
            Log.Info("Found {0}", configPath);
            try
            {
                return ConfigFile.Read(configPath);
            }
            catch (Exception e)
            {
                Log.Error("Failed to read {0} : {1}", configPath, e.Message);
                return Enumerable.Empty<Rule>();
            }            
        }

        public void ScanFolder(DirectoryInfo dir, IEnumerable<Rule> rules)
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
                    ApplyAction(matchingRule.Action, subFolder);              
                }
                else
                {
                    ScanFolder(subFolder, rules);
                }
            }

            if (fileRules.Any())
            {
                foreach (var file in dir.EnumerateFiles())
                {
                    var matchingRule = fileRules.FirstOrDefault(x => x.IsMatch(file));

                    if (matchingRule != null)
                    {
                        ApplyAction(matchingRule.Action, file);
                    }
                }
            }
        }

        void ApplyAction(RuleAction action, FileSystemInfo fsi)
        {
            switch (action)
            {
                case RuleAction.Recycle:
                    Recycle(fsi);
                    break;

                case RuleAction.Delete:
                    Delete(fsi);
                    break;

                case RuleAction.Ignore:
                    break;

                default:
                    throw new NotSupportedException();
            }      
        }

        void Recycle(FileSystemInfo fsi)
        {
            Recycle(fsi.FullName);
        }

        void Recycle(string path)
        {
            if (!IsSimulating)
            {
                var shf = new Shell32.SHFILEOPSTRUCT();
                shf.wFunc = Shell32.FO_Func.FO_DELETE;
                shf.fFlags = Shell32.FOF_ALLOWUNDO | Shell32.FOF_NO_UI;
                shf.pFrom = Marshal.StringToHGlobalUni(path + '\0');
                var ret = Shell32.SHFileOperation(ref shf);

                if( ret == 0 )
                    Log.Warning("Recycle {0}... Error #{1}", path, ret);
                else
                    Log.Info("Recycle {0}... OK", path);
            }
            else
            {
                Log.Info("Recycle {0}", path);
            }
        }

        void Delete(FileSystemInfo fsi)
        {
            if (!IsSimulating)
            {
                try
                {
                    fsi.Delete();
                    Log.Info("Delete {0}... OK", fsi.FullName);
                }
                catch (Exception e)
                {
                    Log.Warning("Delete {0}... OK", fsi.FullName, e.Message);
                }
            }
            else
            {
                Log.Info("Delete {0}", fsi.FullName);
            }
        }           
    }
}
