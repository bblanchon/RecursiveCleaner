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
using System.Reflection;
using System.IO;

namespace RecursiveCleaner.Tests.Filters
{
    using Helpers;
    using Engine.Filters;
    using Engine.Environments;

    [TestFixture]
    class WildcardsFilterTests
    {
        Environment environment;
        IFilter filter;

        [SetUp]
        public void SetUp()
        {
            environment = new Environment();
            filter = new WildcardsFilter("Match*");
        }

        [Test]
        public void TestMatch ()
        {
            var file = new DummyFile("MatchingFile.txt");
            Assert.IsTrue(filter.IsMatch(file, environment));
        }

        [Test]
        public void TestNonMatch()
        {
            var file = new DummyFile("NonMatchingFile.txt");
            Assert.IsFalse(filter.IsMatch(file, environment));
        }
    }
}
