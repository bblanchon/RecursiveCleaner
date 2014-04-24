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

using NUnit.Framework;

namespace RecursiveCleaner.Tests.Filters
{
    using Helpers;
    using Engine.Filters;
    using Engine.Environments;

    [TestFixture]
    class RegexFilterTests
    {
        Environment environment;
        IFilter filter;

        [SetUp]
        public void SetUp()
        {
            environment = new Environment();
            filter = new RegexFilter("^Match");
        }

        [Test]
        public void Match ()
        {
            var file = new DummyFile("MatchingFile.txt");
            Assert.IsTrue(filter.IsMatch(file, environment));
        }

        [Test]
        public void NonMatch()
        {
            var file = new DummyFile("NonMatchingFile.txt");
            Assert.IsFalse(filter.IsMatch(file, environment));
        }

        [Test]
        public void NamedGroup()
        {
            var environment = new Environment();
            var filter = new RegexFilter(@"^file(?<number>\d+)\.txt$");
            var file = new DummyFile("file456.txt");
            filter.IsMatch(file, environment);
            Assert.AreEqual("456", environment.Get("number"));
        }

        [Test]
        public void UnnamedGroup()
        {
            var environment = new Environment();
            var filter = new RegexFilter(@"^file(\d+)\.txt$");
            var file = new DummyFile("file456.txt");
            filter.IsMatch(file, environment);
            Assert.AreEqual("456", environment.Get("1"));
        }
    }
}
