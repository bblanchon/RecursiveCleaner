﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RecursiveCleaner.Scanner;
using System.Xml;
using RecursiveCleaner.Filters;
using RecursiveCleaner.Rules;

namespace RecursiveCleaner.Config
{
    class ConfigFile
    {
        public static readonly string Filename = "RecursiveCleaner.config"; 

        public static IEnumerable<IRule> Read(string path)
        {
            using (var xml = XmlReader.Create(path))
            {
                if( ! xml.ReadToFollowing("RecursiveCleaner") )
                    throw new FileLoadException("Element <RecursiveCleaner> is missing");

                while (xml.Read() && xml.NodeType != XmlNodeType.EndElement)
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        var rule = ReadRule(xml);
                        if (rule != null) yield return rule;
                    }
                }             
            }
        }

        #region Rule reading

        private static IRule ReadRule(XmlReader xml)
        {
            IRule rule;

            switch (xml.Name)
            {
                case "Recycle":
                    rule = new RecycleRule();                    
                    break;
                case "Delete":
                    rule = new DeleteRule();
                    break;
                case "Ignore":
                    rule = new IgnoreRule();
                    break;
                default:
                    Log.Warning("Ignoring element <{0}>", xml.Name);
                    xml.Skip();
                    return null;
            }

            if (xml.MoveToAttribute("Target"))
                rule.Target = ParseEnum<RuleTarget>(xml.Value);

            xml.MoveToContent();

            while (xml.Read() && xml.NodeType != XmlNodeType.EndElement)
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    var filter = ReadFilter(xml);
                    if (filter != null) rule.Filters.Add(filter);
                }
            }

            return rule;
        }

        #endregion

        #region Filter reading

        private static IFilter ReadFilter(XmlReader xml)
        {
            switch (xml.Name)
            {
                case "Regex":
                    return ReadRegexFilter(xml);
                case "OlderThan":
                    return ReadOlderThanFilter(xml);
                default:
                    Log.Warning("Ignoring element <{0}>", xml.Name);
                    xml.Skip();
                    return null;
            }
        }

        private static IFilter ReadOlderThanFilter(XmlReader xml)
        {
            var attributes = ReadAttributes(xml, "years", "months", "days", "hours", "minutes", "seconds");

            if (attributes.Count == 0)
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

        #endregion

        #region Helpers

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

        #endregion    
    }
}
