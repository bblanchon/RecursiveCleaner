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
using RecursiveCleaner.Tests.Helpers;
using RecursiveCleaner.Engine.Config;
using RecursiveCleaner.Engine.Rules;
using RecursiveCleaner.Engine.Filters;

namespace RecursiveCleaner.Tests.Config
{
    [TestFixture]
    class SimpleConfigFiles
    {
        [Test]        
        public void RecycleFilesRegex()
        {
            var xml = 
@"<RecursiveCleaner>
    <Recycle Target='Files'>
        <Regex>\.\w+~</Regex>
  </Recycle>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<RecycleRule>(rules[0]);
                Assert.AreEqual(RuleTarget.Files, rules[0].Target);
                Assert.IsInstanceOf<RegexFilter>(rules[0].Filter);
            }
        }

        [Test]
        public void DeleteFoldersWildcards()
        {
            var xml =
@"<RecursiveCleaner>
    <Delete Target='Folders'>
        <Wildcards>\.\w+~</Wildcards>
  </Delete>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<DeleteRule>(rules[0]);
                Assert.AreEqual(RuleTarget.Folders, rules[0].Target);
                Assert.IsInstanceOf<WildcardsFilter>(rules[0].Filter);
            }
        }


        [Test]
        public void IgnoreFilerAndFoldersOlderThan()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore Target='FilesAndFolders'>
        <OlderThan years='1' months='2' days='3' hours='4' minutes='5' seconds='6' />
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.AreEqual(RuleTarget.FilesAndFolders, rules[0].Target);
                Assert.IsInstanceOf<OlderThanFilter>(rules[0].Filter);
                var filter = rules[0].Filter as OlderThanFilter;
                Assert.AreEqual(4, (int)filter.TimeSpan.Hours);
                Assert.AreEqual(5, (int)filter.TimeSpan.Minutes);
                Assert.AreEqual(6, (int)filter.TimeSpan.Seconds);
            }
        }
    }
}
