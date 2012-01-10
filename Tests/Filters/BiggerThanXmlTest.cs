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
using System.Linq;
using System.Text;
using NUnit.Framework;
using RecursiveCleaner.Engine.Config;
using RecursiveCleaner.Tests.Helpers;
using RecursiveCleaner.Engine.Filters;

namespace RecursiveCleaner.Tests.Filters
{
    [TestFixture]
    class BiggerThanXmlTest
    {
        [Test]
        public void AttributeMissing()
        {
            var xml = new XmlSample(@"<BiggerThan />");
            using (var reader = xml.Read())
            {
                Assert.Throws<AttributeMissingException>(() =>
                {
                    ConfigFileReader.ReadFilter(reader);
                });            
            }
        }

        [Test]
        public void Bytes()
        {
            var xml = new XmlSample(@"<BiggerThan bytes='10' />");
            using( var reader = xml.Read() )
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10, size);
            }
        }

        [Test]
        public void Kilo()
        {
            var xml = new XmlSample(@"<BiggerThan KB='10' />");
            using (var reader = xml.Read())
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10*1024, size);
            }
        }

        [Test]
        public void Mega()
        {
            var xml = new XmlSample(@"<BiggerThan MB='10' />");
            using (var reader = xml.Read())
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10 * 1024 * 1024, size);
            }
        }

        [Test]
        public void Tera()
        {
            var xml = new XmlSample(@"<BiggerThan TB='10' />");
            using (var reader = xml.Read())
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10L * 1024 * 1024 * 1024, size);
            }
        }
    }
}
