/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011-2014 Benoit Blanchon
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

namespace RecursiveCleaner.Engine.Config
{
    using System;

    class AttributeInfo
    {
        public string Name;
        public string Value;
        public bool HasBeenUsed;
    }

    static class AttributeInfoExtensions
    {
        public static string AsString(this AttributeInfo attributeInfo)
        {
            return attributeInfo != null ? attributeInfo.Value : null;
        }

        static T? AsEnum<T>(this AttributeInfo attributeInfo) where T : struct
        {
            if (attributeInfo == null) return null;
            return (T)Enum.Parse(typeof(T), attributeInfo.Value, true);
        }

        public static T AsEnum<T>(this AttributeInfo attributeInfo, T defaultValue) where T : struct
        {
            return attributeInfo.AsEnum<T>().GetValueOrDefault(defaultValue);
        }

        static bool? AsBool(this AttributeInfo attributeInfo)
        {
            if (attributeInfo == null) return null;
            return ParseBool(attributeInfo.Value);
        }

        public static bool AsBool(this AttributeInfo attributeInfo, bool defaultValue)
        {
            return attributeInfo.AsBool().GetValueOrDefault(defaultValue);
        }

        static long? AsLong(this AttributeInfo attributeInfo)
        {
            if (attributeInfo == null) return null;
            return long.Parse(attributeInfo.Value);
        }

        public static long AsLong(this AttributeInfo attributeInfo, long defaultValue)
        {
            return attributeInfo.AsLong().GetValueOrDefault(defaultValue);
        }

        static int? AsInt(this AttributeInfo attributeInfo)
        {
            if (attributeInfo == null) return null;
            return int.Parse(attributeInfo.Value);
        }

        public static int AsInt(this AttributeInfo attributeInfo, int defaultValue)
        {
            return attributeInfo.AsInt().GetValueOrDefault(defaultValue);
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
    }
}