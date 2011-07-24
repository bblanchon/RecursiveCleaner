using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RecursiveCleaner.Scanner;
using System.Xml;
using RecursiveCleaner.Filters;

namespace RecursiveCleaner.Config
{
    class ConfigFile
    {
        public static readonly string Filename = "RecursiveCleaner.config"; 

        public static IEnumerable<Rule> Read(string path)
        {
            using (var xml = XmlReader.Create(path))
            {
                if( ! xml.ReadToFollowing("RecursiveCleaner") )
                    throw new FileLoadException("Element <RecursiveCleaner> is missing");

                while (xml.ReadToFollowing("Rule"))
                {
                    var rule = new Rule();

                    if (xml.MoveToAttribute("Action"))
                        rule.Action = ParseEnum<RuleAction> (xml.Value);

                    if (xml.MoveToAttribute("Target"))
                        rule.Target = ParseEnum<RuleTarget>(xml.Value);

                    xml.MoveToContent();

                    while (xml.Read() && xml.NodeType != XmlNodeType.EndElement)
                    {
                        if (xml.NodeType == XmlNodeType.Element)
                        {
                            switch (xml.Name)
                            {
                                case "Regex":
                                    var pattern = xml.ReadElementContentAsString();
                                    rule.Filters.Add(new RegexFilter(pattern));
                                    break;
                                default:
                                    Log.Warning("Ignoring element <{0}>", xml.Name);
                                    xml.Skip();
                                    break;
                            }
                        }         
                    }

                    yield return rule;
                }                
            }
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}
