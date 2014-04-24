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
    class MatchingAllFilterTests
    {
        static Environment environment = new Environment();

        [Test]
        public void AllTrue()
        {
            var filter = new MatchingAllFilter
            {
                Children = new []
                {
                    new DelegateFilter((x)=>true),
                    new DelegateFilter((x)=>true),
                }
            };
            Assert.IsTrue(filter.IsMatch(new DummyFile(), environment));
        }

        [Test]
        public void AllFalse()
        {
            var filter = new MatchingAllFilter
            {
                Children = new []
                {
                    new DelegateFilter((x)=>false),
                    new DelegateFilter((x)=>false),
                }
            };
            Assert.IsFalse(filter.IsMatch(new DummyFile(), environment));
        }

        [Test]
        public void Mixed()
        {
            var filter = new MatchingAllFilter
            {
                Children = new []
                {
                    new DelegateFilter((x)=>true),
                    new DelegateFilter((x)=>false),
                }
            };
            Assert.IsFalse(filter.IsMatch(new DummyFile(), environment));
        }
    }
}
