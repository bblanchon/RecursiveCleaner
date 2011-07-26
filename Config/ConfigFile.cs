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
                                    rule.Filters.Add(ReadRegexFilter(xml));
                                    break;      
                                case "OlderThan":
                                    rule.Filters.Add(ReadOlderThanFilter(xml));
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

        private static IDictionary<string, string> ReadAttributes(XmlReader xml, params string[] expectedAttributes)
        {
            var element = "<" + xml.Name + ">";
            var results = new Dictionary<string,string> (expectedAttributes.Length);

            if (xml.MoveToFirstAttribute())
            {

                do
                {
                    var name = xml.Name.ToLower();

                    if (!expectedAttributes.Contains(name))
                        throw new Exception("Unexpected attribute " + name + " in " + element);

                    if (results.ContainsKey(name))
                        throw new Exception("Attribute " + name + " set more than once");

                    results.Add(name, xml.Value);

                } while (xml.MoveToNextAttribute());
            }

            return results;
        }

        private static IFilter ReadOlderThanFilter(XmlReader xml)
        {
            var attributes = ReadAttributes(xml, "years", "months", "days", "hours", "minutes", "seconds");

            if( attributes.Count == 0 )
                throw new Exception("Attribute missing in <OlderThan>");

            var years = attributes.ContainsKey("years") ? int.Parse(attributes["year"]) : 0;
            var months = attributes.ContainsKey("months") ? int.Parse(attributes["months"]) : 0;
            var days = attributes.ContainsKey("days") ? int.Parse(attributes["days"]) : 0;
            var hours = attributes.ContainsKey("hours") ? int.Parse(attributes["hours"]) : 0;
            var minutes = attributes.ContainsKey("minutes") ? int.Parse(attributes["minutes"]) : 0;
            var seconds = attributes.ContainsKey("seconds") ? int.Parse(attributes["seconds"]) : 0;
            
            return new OlderThanFilter(years, months, days, hours, minutes, seconds);
        }

        private static IFilter ReadRegexFilter(XmlReader xml)
        {
            var attributes = ReadAttributes(xml, "pattern");

            var pattern = attributes.ContainsKey("pattern") ? attributes["pattern"] : xml.ReadElementContentAsString();

            return new RegexFilter(pattern);
        }
    }
}
