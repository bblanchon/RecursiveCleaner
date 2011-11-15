using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
