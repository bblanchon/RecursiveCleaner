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
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace RecursiveCleaner.Tests.Rules
{
    using Helpers;
    using Engine.Rules;
    using Engine.Config;

    [TestFixture]
    class RenameXmlTests
    {
        [Test]
        public void AttributeMissing()
        {
            var xml = new XmlSample(@"<Rename />");
            using (var reader = xml.Read())
            {
                Assert.Throws<AttributeMissingException>(() =>
                {
                    ConfigFileReader.ReadRule(reader, false);
                });
            }
        }

        [Test]
        public void DestinationAttribute()
        {
            var xml = new XmlSample(@"<Rename Name='newname' />");
            using (var reader = xml.Read())
            {
                var rule = ConfigFileReader.ReadRule(reader, false) as RenameRule ;
                Assert.AreEqual("newname", rule.Name);
            }
        }

        [Test]
        public void IfExistsRename()
        {
            var xml = new XmlSample(@"<Rename Name='newname' IfExists='rename' />");
            using (var reader = xml.Read())
            {
                var rule = ConfigFileReader.ReadRule(reader, false) as RenameRule;
                Assert.AreEqual(RenameRule.IfExistsMode.Rename, rule.IfExists);
            }
        }

        [Test]
        public void IfExistsCancel()
        {
            var xml = new XmlSample(@"<Rename Name='newname' IfExists='cancel' />");
            using (var reader = xml.Read())
            {
                var rule = ConfigFileReader.ReadRule(reader, false) as RenameRule;
                Assert.AreEqual(RenameRule.IfExistsMode.Cancel, rule.IfExists);
            }
        }

        [Test]
        public void IfExistsRecycle()
        {
            var xml = new XmlSample(@"<Rename Name='newname' IfExists='recycle' />");
            using (var reader = xml.Read())
            {
                var rule = ConfigFileReader.ReadRule(reader, false) as RenameRule;
                Assert.AreEqual(RenameRule.IfExistsMode.Recycle, rule.IfExists);
            }
        }

        [Test]
        public void IfExistsDelete()
        {
            var xml = new XmlSample(@"<Rename Name='newname' IfExists='delete' />");
            using (var reader = xml.Read())
            {
                var rule = ConfigFileReader.ReadRule(reader, false) as RenameRule;
                Assert.AreEqual(RenameRule.IfExistsMode.Delete, rule.IfExists);
            }
        }
    }
}
