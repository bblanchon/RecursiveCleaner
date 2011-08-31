using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RecursiveCleaner.Engine.Filters;
using RecursiveCleaner.Tests.Helpers;

namespace RecursiveCleaner.Tests.Filters
{
    [TestFixture]
    class MatchingNoneFilterTests
    {
        [Test]
        public void AllTrue()
        {
            var filter = new MatchingNoneFilter
            {
                Children = new []
                {
                    new DelegateFilter((x)=>true),
                    new DelegateFilter((x)=>true),
                }
            };
            Assert.IsFalse(filter.IsMatch(new DummyFile()));
        }

        [Test]
        public void AllFalse()
        {
            var filter = new MatchingNoneFilter
            {
                Children = new[]
                {
                    new DelegateFilter((x)=>false),
                    new DelegateFilter((x)=>false),
                }
            };
            Assert.IsTrue(filter.IsMatch(new DummyFile()));
        }

        [Test]
        public void Mixed()
        {
            var filter = new MatchingNoneFilter
            {
                Children = new[]
                {
                    new DelegateFilter((x)=>true),
                    new DelegateFilter((x)=>false),
                }
            };
            Assert.IsFalse(filter.IsMatch(new DummyFile()));
        }
    }
}
