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
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

namespace RecursiveCleaner.Engine.Config
{
    class AttributeParser
    {
        class AttributeInfo
        {
            public string Name;
            public string Value;
            public bool HasBeenUsed;
        }

        readonly Dictionary<string, AttributeInfo> dict;

        public AttributeParser(XmlReader xml)
        {
            var element = "<" + xml.Name + ">";
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

        public int Count
        {
            get
            {
                return dict.Count;
            }
        }

        public bool HasUnused
        {
            get { return dict.Values.Any(x => !x.HasBeenUsed); }
        }

        public IEnumerable<string> Unused
        {
            get
            {
                return dict.Values.Where(x => !x.HasBeenUsed).Select(x=>x.Name);
            }
        }

        public void AssertNoUnused()
        {
            if (HasUnused) throw new UnknownAttributesException(Unused);
        }

        public void Get<T>(string attributeName, Expression<Func<T>> property, bool isRequired = false)
        {
            var attr = GetAttribute(attributeName);

            if (attr == null)
            {
                if (isRequired) throw new Exception("Attribute " + attributeName + " is missing");
                else return;
            }                 
       
            T value;
            if (TryParse(attr.Value, out value))
            {
                var assign = Expression.Assign(property.Body, Expression.Constant(value));
                Expression.Lambda(assign).Compile().DynamicInvoke();
            }
            else
            {
                throw new FormatException(string.Format(
                    "\"{0}\" is not a valid value for attribute {1}",
                    attr.Value, attributeName));
            }

            attr.HasBeenUsed = true;
        }

        #region Private parsing helpers

        private static bool TryParse<T>(string input, out T value)
        {
            try
            {
                if (typeof(T).IsEnum)
                    value = (T)Enum.Parse(typeof(T), input, true);
                else if (typeof(T) == typeof(bool))
                    value = (T)(object)ParseBool(input);
                else
                    value = (T)Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }
        }

        private static bool ParseBool(string input)
        {
            switch (input.Trim().ToLower())
            {
                case "yes":
                case "true":
                case "1":
                    return true;

                case "no":
                case "false":
                case "0":
                    return false;

                default:
                    throw new InvalidCastException(string.Format("\"{0}\" can't be converted to a boolean", input));
            }
        }

        #endregion
    }
}
