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

using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RecursiveCleaner.Engine.Config
{
    class AttributeParser
    {
        readonly Dictionary<string, AttributeInfo> dict;
        readonly string elementName;

        public AttributeParser(XmlReader xml)
        {
            elementName = xml.Name;
            dict = new Dictionary<string, AttributeInfo>(xml.AttributeCount);

            if (xml.MoveToFirstAttribute())
            {
                do
                {
                    dict.Add(xml.Name.ToLower(), new AttributeInfo
                    {
                        Name = xml.Name,
                        Value = xml.Value
                    });

                } while (xml.MoveToNextAttribute());
            }
        }

        private AttributeInfo GetAttribute(string name)
        {
            var key = name.ToLower();
            return dict.ContainsKey(key) ? dict[key] : null;
        }
        
        public AttributeInfo GetMandatory(string attributeName)
        {
            var attr = GetAttribute(attributeName);

            if (attr == null)
                throw new AttributeMissingException(elementName, attributeName);

            attr.HasBeenUsed = true;

            return attr;
        }

        public AttributeInfo GetOptional(string attributeName)
        {
            var attr = GetAttribute(attributeName);

            if (attr != null)
                attr.HasBeenUsed = true;

            return attr;
        }

        public int Count
        {
            get { return dict.Count; }
        }

        public void AssertNoUnused()
        {
            var unusedAttributes = dict.Values.Where(x => !x.HasBeenUsed).Select(x => x.Name);

            // ReSharper disable PossibleMultipleEnumeration
            if (unusedAttributes.Any())
                throw new UnknownAttributesException(unusedAttributes);
            // ReSharper restore PossibleMultipleEnumeration
        }
    }
}
