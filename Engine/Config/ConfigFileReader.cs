﻿/*
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
            rule.Filters.AddRange(ReadFilters(xml));            

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

        private static IFilter ReadFilter(XmlReader xml)
        {
            var elementName = xml.Name;
            var attributes = new AttributeParser(xml);

            IFilter filter = null;

            switch (elementName.ToLower())
            {
                case "regex":
                    filter = ReadRegexFilter(xml, attributes);
                    break;
                case "olderthan":
                    filter = ReadOlderThanFilter(xml, attributes);
                    break;
                case "wildcards":
                    filter = ReadWildcardsFilter(xml, attributes);
                    break;
                case "exclude":
                    filter = ReadExcludeFilter(xml, attributes);
                    break;
                default:                    
                    xml.Skip();
                    throw new IgnoredElementException(elementName);
            }

            attributes.AssertNoUnused();

            return filter;
        }

        private static IFilter ReadOlderThanFilter(XmlReader xml, AttributeParser attributes)
        {
            if (attributes.Count == 0)
                throw new Exception("Attribute missing in <OlderThan>");

            int years=0, months=0, days=0, hours=0, minutes=0, seconds=0;

            attributes.Get("years", () => years);
            attributes.Get("months", () => months);
            attributes.Get("days", () => days);
            attributes.Get("hours", () => hours);
            attributes.Get("minutes", () => minutes);
            attributes.Get("seconds", () => seconds);            

            return new OlderThanFilter(years, months, days, hours, minutes, seconds);
        }

        private static IFilter ReadRegexFilter(XmlReader xml, AttributeParser attributes)
        {
            string pattern = null;

            attributes.Get("pattern", () => pattern);

            if( string.IsNullOrEmpty(pattern) )
                pattern = xml.ReadElementContentAsString();

            return new RegexFilter(pattern);
        }

        private static IFilter ReadWildcardsFilter(XmlReader xml, AttributeParser attributes)
        {
            string pattern = null;

            attributes.Get("pattern", () => pattern);

            if (string.IsNullOrEmpty(pattern))
                pattern = xml.ReadElementContentAsString();

            return new WildcardsFilter(pattern);
        }

        private static IFilter ReadExcludeFilter(XmlReader xml, AttributeParser attributes)
        {
            xml.MoveToContent();
            var innerFilters = ReadFilters(xml);
            return new ExcludeFilter(innerFilters);
        }

        #endregion       
    }
}