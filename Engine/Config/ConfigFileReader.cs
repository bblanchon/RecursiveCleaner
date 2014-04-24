/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011-2012 Benoit Blanchon
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
using System.Xml;

namespace RecursiveCleaner.Engine.Config
{
    using Filters;
    using Rules;

    class ConfigFileReader
    {
        public static readonly string Filename = "RecursiveCleaner.config"; 

        public static IEnumerable<IRule> Read(string path)
        {
            var rules = new List<IRule> ();

            using (var xml = XmlReader.Create(path))
            {
                if( ! xml.ReadToFollowing("RecursiveCleaner") )
                    throw new FileLoadException("Element <RecursiveCleaner> is missing");

                while (xml.Read() && xml.NodeType != XmlNodeType.EndElement)
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        try
                        {
                            rules.Add(ReadRule(xml));
                        }
                        catch (Exception e)
                        {
                            Log.Warning("{0} (line {1}): {2}", path, (xml as IXmlLineInfo).LineNumber, e.Message); 
                        }
                    }
                }             
            }

            if (rules.Count == 0) Log.Warning("{0}: file doesn't contain any rule", path);

            return rules;
        }

        #region Rule reading

        internal static IRule ReadRule(XmlReader xml, bool includeFilter=true)
        {
            IRule rule;

            var elementName = xml.Name;

            switch (elementName.ToLower())
            {
                case "recycle":
                    rule = new RecycleRule();                    
                    break;
                case "delete":
                    rule = new DeleteRule();
                    break;
                case "ignore":
                    rule = new IgnoreRule();
                    break;
                case "move":
                    rule = new MoveRule();
                    break;
                case "rename":
                    rule = new RenameRule();
                    break;
                default:                    
                    xml.Skip();
                    throw new IgnoredElementException(elementName);
            }

            var attributes = new AttributeParser(xml);

            rule.Target              = attributes.GetOptional("Target").AsEnum(RuleTarget.FilesAndFolders);
            rule.AppliesToSubfolders = attributes.GetOptional("ApplyToSubfolders").AsBool(true);
           
            if (rule is MoveRuleBase)
            {
                ((MoveRuleBase)rule).IfExists =
                    attributes.GetOptional("ifexists").AsEnum(MoveRuleBase.IfExistsMode.Cancel);
            }

            if( rule is MoveRule)
            {
                ((MoveRule)rule).Destination = attributes.GetMandatory("destination").AsString();
                ((MoveRule)rule).CreateFolder = attributes.GetOptional("createfolder").AsBool(true);
            }

            if (rule is RenameRule)
            {
                ((RenameRule)rule).Name = attributes.GetMandatory("name").AsString();
            }

            attributes.AssertNoUnused();

            if (includeFilter)
            {
                var filters = ReadFilters(xml).ToArray();

                if (filters.Length == 0)
                    throw new Exception("You must specificy a filter for this rule");

                if (filters.Length > 1)
                    throw new Exception("You can only specify one filter at rule's root. Please use <MatchingAll>, <MatchingAny> or <MatchingNone>.");

                rule.Filter = filters.First();
            }

            return rule;
        }

        #endregion

        #region Filter reading

        private static IEnumerable<IFilter> ReadFilters(XmlReader xml)
        {
            while (xml.Read() && xml.NodeType != XmlNodeType.EndElement)
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    yield return ReadFilter(xml);
                }
            }
        }

        internal static IFilter ReadFilter(XmlReader xml)
        {
            var elementName = xml.Name;
            var attributes = new AttributeParser(xml);

            IFilter filter = null;

            switch (elementName.ToLower())
            {
                case "biggerthan":
                    filter = ReadBiggerThanFilter(xml, attributes);
                    break;
                case "regex":
                    filter = ReadRegexFilter(xml, attributes);
                    break;
                case "olderthan":
                    filter = ReadOlderThanFilter(xml, attributes);
                    break;
                case "wildcards":
                    filter = ReadWildcardsFilter(xml, attributes);
                    break;
                case "matchingnone":
                    filter = ReadMatchingNoneFilter(xml, attributes);
                    break;
                case "matchingall":
                    filter = ReadMatchingAllFilter(xml, attributes);
                    break;
                case "matchingany":
                    filter = ReadMatchingAnyFilter(xml, attributes);
                    break;
                case "smallerthan":
                    filter = ReadSmallerThanFilter(xml, attributes);
                    break;
                case "empty":
                    filter = ReadEmptyFilter(xml, attributes);
                    break;
                default:                    
                    xml.Skip();
                    throw new IgnoredElementException(elementName);
            }

            attributes.AssertNoUnused();

            return filter;
        }

        private static IFilter ReadEmptyFilter(XmlReader xml, AttributeParser attributes)
        {
            return new EmptyFilter();
        }

        private static IFilter ReadBiggerThanFilter(XmlReader xml, AttributeParser attributes)
        {
            attributes.AssertNotEmpty();

            var b = attributes.GetOptional("bytes").AsLong(0);
            var kb = attributes.GetOptional("kb").AsLong(0);
            var mb = attributes.GetOptional("mb").AsLong(0);
            var tb = attributes.GetOptional("tb").AsLong(0);

            return new BiggerThanFilter { Size = b + (kb << 10) + (mb << 20) + (tb << 30) };
        }

        private static IFilter ReadSmallerThanFilter(XmlReader xml, AttributeParser attributes)
        {
            attributes.AssertNotEmpty();

            var b  = attributes.GetOptional("bytes").AsLong(0);
            var kb = attributes.GetOptional("kb").AsLong(0);
            var mb = attributes.GetOptional("mb").AsLong(0);
            var tb = attributes.GetOptional("tb").AsLong(0);

            return new SmallerThanFilter { Size = b + (kb << 10) + (mb << 20) + (tb << 30) };
        }

        private static IFilter ReadOlderThanFilter(XmlReader xml, AttributeParser attributes)
        {
            attributes.AssertNotEmpty();

            var years   = attributes.GetOptional("years").AsInt(0);
            var months  = attributes.GetOptional("months").AsInt(0);
            var weeks   = attributes.GetOptional("weeks").AsInt(0);
            var days    = attributes.GetOptional("days").AsInt(0);
            var hours   = attributes.GetOptional("hours").AsInt(0);
            var minutes = attributes.GetOptional("minutes").AsInt(0);
            var seconds = attributes.GetOptional("seconds").AsInt(0);      

            return new OlderThanFilter(years, months, days+7*weeks, hours, minutes, seconds);
        }

        private static IFilter ReadRegexFilter(XmlReader xml, AttributeParser attributes)
        {
            var pattern = attributes.GetOptional("pattern").AsString();

            if (string.IsNullOrEmpty(pattern))
                pattern = xml.ReadElementContentAsString();

            return new RegexFilter(pattern);
        }

        private static IFilter ReadWildcardsFilter(XmlReader xml, AttributeParser attributes)
        {
            var pattern = attributes.GetOptional("pattern").AsString();

            if (string.IsNullOrEmpty(pattern))
                pattern = xml.ReadElementContentAsString();

            return new WildcardsFilter(pattern);
        }

        private static IFilter ReadMatchingNoneFilter(XmlReader xml, AttributeParser attributes)
        {
            return new MatchingNoneFilter { Children = ReadFilters(xml) };
        }

        private static IFilter ReadMatchingAllFilter(XmlReader xml, AttributeParser attributes)
        {
            return new MatchingAllFilter { Children = ReadFilters(xml) };
        }

        private static IFilter ReadMatchingAnyFilter(XmlReader xml, AttributeParser attributes)
        {
            return new MatchingAnyFilter { Children = ReadFilters(xml) };
        }

        #endregion       
    }
}
