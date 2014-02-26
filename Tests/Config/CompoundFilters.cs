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
    class CompoundFilters
    {
        [Test]
        public void IgnoreMatchingAll()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore>
        <MatchingAll>            
            <OlderThan seconds='1' />
            <Regex>.*</Regex>
            <Wildcards>*.*</Wildcards>
        </MatchingAll>       
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.IsInstanceOf<MatchingAllFilter>(rules[0].Filter);
                var filter = rules[0].Filter as ParentFilter;
                var children = filter.Children.ToArray();
                Assert.AreEqual(3, children.Length);
                Assert.IsInstanceOf<OlderThanFilter>(children[0]);
                Assert.IsInstanceOf<RegexFilter>(children[1]);
                Assert.IsInstanceOf<WildcardsFilter>(children[2]);
            }
        }    

        [Test]
        public void IgnoreMatchingAny()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore>
        <MatchingAny>            
            <OlderThan seconds='1' />
            <Regex>.*</Regex>
            <Wildcards>*.*</Wildcards>
        </MatchingAny>       
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.IsInstanceOf<MatchingAnyFilter>(rules[0].Filter);
                var filter = rules[0].Filter as ParentFilter;
                var children = filter.Children.ToArray();
                Assert.AreEqual(3, children.Length);
                Assert.IsInstanceOf<OlderThanFilter>(children[0]);
                Assert.IsInstanceOf<RegexFilter>(children[1]);
                Assert.IsInstanceOf<WildcardsFilter>(children[2]);
            }
        }

        [Test]
        public void IgnoreMatchingNone()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore>
        <MatchingNone>            
            <OlderThan seconds='1' />
            <Regex>.*</Regex>
            <Wildcards>*.*</Wildcards>
        </MatchingNone>       
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.IsInstanceOf<MatchingNoneFilter>(rules[0].Filter);
                var filter = rules[0].Filter as ParentFilter;
                var children = filter.Children.ToArray();
                Assert.AreEqual(3, children.Length);
                Assert.IsInstanceOf<OlderThanFilter>(children[0]);
                Assert.IsInstanceOf<RegexFilter>(children[1]);
                Assert.IsInstanceOf<WildcardsFilter>(children[2]);
            }
        }

        [Test]
        public void FourLevel()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore>
        <MatchingNone>      
            <MatchingAll>
                <MatchingAny>
                    <Wildcards>*.*</Wildcards>
                </MatchingAny>
            </MatchingAll>      
        </MatchingNone>       
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);

                Assert.IsInstanceOf<MatchingNoneFilter>(rules[0].Filter);
                var filter1 = rules[0].Filter as ParentFilter;
                var children1 = filter1.Children.ToArray();
                Assert.AreEqual(1, children1.Length);

                Assert.IsInstanceOf<MatchingAllFilter>(children1[0]);
                var filter2 = children1[0] as ParentFilter;
                var children2 = filter2.Children.ToArray();
                Assert.AreEqual(1, children2.Length);

                Assert.IsInstanceOf<MatchingAnyFilter>(children2[0]);
                var filter3 = children2[0] as ParentFilter;
                var children3 = filter3.Children.ToArray();
                Assert.AreEqual(1, children3.Length);

                Assert.IsInstanceOf<WildcardsFilter>(children3[0]);
            }
        }
    }
}
