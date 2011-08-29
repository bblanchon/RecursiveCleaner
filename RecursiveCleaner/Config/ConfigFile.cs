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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
                        IRule rule = null;
                        try
                        {
                            rule = ReadRule(xml);
                        }
                        catch (Exception e)
                        {
                            Log.Warning("{0} (line {1}): {2}", path, (xml as IXmlLineInfo).LineNumber, e.Message); 
                        }
                        if (rule != null) yield return rule;
                    }
                }             
            }
        }

        #region Rule reading

        private static IRule ReadRule(XmlReader xml)
        {
            IRule rule;

            var elementName = xml.Name;

            switch (elementName)
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
                    xml.Skip();
                    throw new IgnoredElementException(elementName);
            }

            var attributes = new AttributeParser(xml);

            attributes.Get("Target", () => rule.Target);
            attributes.Get("ApplyToSubfolders", () => rule.AppliesToSubfolders);
            attributes.AssertNoUnused();

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
                case "Wildcards":
                    return ReadWildcardsFilter(xml);
                default:
                    Log.Warning("Ignoring element <{0}>", xml.Name);
                    xml.Skip();
                    return null;
            }
        }

        private static IFilter ReadOlderThanFilter(XmlReader xml)
        {
            var attributes = new AttributeParser(xml);

            if (attributes.Count == 0)
                throw new Exception("Attribute missing in <OlderThan>");

            int years=0, months=0, days=0, hours=0, minutes=0, seconds=0;

            attributes.Get("years", () => years);
            attributes.Get("months", () => months);
            attributes.Get("days", () => days);
            attributes.Get("hours", () => hours);
            attributes.Get("minutes", () => minutes);
            attributes.Get("seconds", () => seconds);
            attributes.AssertNoUnused();

            return new OlderThanFilter(years, months, days, hours, minutes, seconds);
        }

        private static IFilter ReadRegexFilter(XmlReader xml)
        {
            var attributes = new AttributeParser(xml);
            string pattern = null;

            attributes.Get("pattern", () => pattern);
            attributes.AssertNoUnused();

            if( string.IsNullOrEmpty(pattern) )
                pattern = xml.ReadElementContentAsString();

            return new RegexFilter(pattern);
        }

        private static IFilter ReadWildcardsFilter(XmlReader xml)
        {
            var attributes = new AttributeParser(xml);
            string pattern = null;

            attributes.Get("pattern", () => pattern);
            attributes.AssertNoUnused();

            if (string.IsNullOrEmpty(pattern))
                pattern = xml.ReadElementContentAsString();

            return new WildcardsFilter(pattern);
        }

        #endregion       
    }
}
