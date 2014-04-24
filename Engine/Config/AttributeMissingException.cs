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

namespace RecursiveCleaner.Engine.Config
{
    class AttributeMissingException : Exception
    {
        public AttributeMissingException(string elementName)
            : base(string.Format("Attribute missing in <{0}>", elementName))
        {

        }

        public AttributeMissingException(string elementName, string attributeName)
            : base(string.Format("Attribute {1} missing in <{0}>", elementName, attributeName))
        {

        }
    }
}
