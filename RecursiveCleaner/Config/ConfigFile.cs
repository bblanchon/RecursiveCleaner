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

            var attributes = ReadAttributes(xml, "Target", "ApplyToSubfolders");

            ParseAttribute(attributes, "Target", () => rule.Target);
            ParseAttribute(attributes, "ApplyToSubfolders", () => rule.AppliesToSubfolders);

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
            var attributes = ReadAttributes(xml, "years", "months", "days", "hours", "minutes", "seconds");

            if (attributes.Count == 0)
                throw new Exception("Attribute missing in <OlderThan>");

            int years=0, months=0, days=0, hours=0, minutes=0, seconds=0;

            ParseAttribute(attributes, "years", () => years);
            ParseAttribute(attributes, "months", () => months);
            ParseAttribute(attributes, "days", () => days);
            ParseAttribute(attributes, "hours", () => hours);
            ParseAttribute(attributes, "minutes", () => minutes);
            ParseAttribute(attributes, "seconds", () => seconds);

            return new OlderThanFilter(years, months, days, hours, minutes, seconds);
        }

        private static IFilter ReadRegexFilter(XmlReader xml)
        {
            var attributes = ReadAttributes(xml, "pattern");
            string pattern = null;

            ParseAttribute(attributes, "pattern", () => pattern);

            if( string.IsNullOrEmpty(pattern) )
                pattern = xml.ReadElementContentAsString();

            return new RegexFilter(pattern);
        }

        private static IFilter ReadWildcardsFilter(XmlReader xml)
        {
            var attributes = ReadAttributes(xml, "pattern");
            string pattern = null;

            ParseAttribute(attributes, "pattern", () => pattern);

            if (string.IsNullOrEmpty(pattern))
                pattern = xml.ReadElementContentAsString();

            return new WildcardsFilter(pattern);
        }

        #endregion

        #region Helpers

        private static void ParseAttribute<T> (IDictionary<string,string> attributes,
            string attributeName, Expression<Func<T>> property, bool isRequired=false)
        {
            if (!attributes.ContainsKey(attributeName))
            {
                if (isRequired) throw new Exception("Attribute " + attributeName + " is missing");
                else return;
            }

            T value;
            if (TryParse(attributes[attributeName], out value))
            {
                var assign = Expression.Assign(property.Body, Expression.Constant(value));
                Expression.Lambda(assign).Compile().DynamicInvoke();               
            }
            else 
            {
                throw new FormatException(string.Format(
                    "\"{0}\" is not a valid value for attribute {1}",
                    attributes[attributeName], attributeName));
            }
        }

        private static bool TryParse<T>(string input, out T value)
        {           
            try {
                if (typeof(T).IsEnum)
                    value = (T)Enum.Parse(typeof(T), input, true);
                else
                    value = (T)Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch( Exception ) {
                value = default(T);
                return false;
            }
        }

        private static bool TryParseBool(string input, out bool value)
        {
            switch (input.Trim().ToLower())
            {
                case "yes":
                case "true":
                case "1":
                    value = true;
                    return true;

                case "no":
                case "false":
                case "0":
                    value = false;
                    return true;

                default:
                    value = default(bool);
                    return false;
            }
        }

        private static IDictionary<string, string> ReadAttributes(XmlReader xml, params string[] expectedAttributes)
        {
            var element = "<" + xml.Name + ">";
            var results = new Dictionary<string,string> (expectedAttributes.Length);

            if (xml.MoveToFirstAttribute())
            {

                do
                {
                    var matchingName = expectedAttributes.FirstOrDefault(
                        x => x.Equals(xml.Name, StringComparison.InvariantCultureIgnoreCase));

                    if (matchingName == null)
                        throw new Exception("Unexpected attribute " + xml.Name + " in " + element);

                    if (results.ContainsKey(matchingName))
                        throw new Exception("Attribute " + matchingName + " set more than once");

                    results.Add(matchingName, xml.Value);

                } while (xml.MoveToNextAttribute());
            }

            return results;
        }

        #endregion    
    }
}
