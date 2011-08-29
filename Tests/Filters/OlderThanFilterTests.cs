using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RecursiveCleaner.Engine.Filters;
using RecursiveCleaner.Tests.Helpers;

namespace RecursiveCleaner.Tests.Filters
{
    class OlderThanFilterTests
    {
        IFilter filter;

        [SetUp]
        public void SetUp()
        {
            filter = new OlderThanFilter(0, 0, 1, 0, 0, 0);
        }

        [Test]
        public void TestMatch ()
        {
            using (var file = new TemporaryFile())
            {
                file.FileInfo.LastWriteTime = DateTime.Now.Subtract(TimeSpan.FromDays(2));
                Assert.IsTrue(filter.IsMatch(file.FileInfo));
            }
        }

        [Test]
        public void TestNonMatch()
        {
            using (var file = new TemporaryFile())
            {
                file.FileInfo.LastWriteTime = DateTime.Now;
                Assert.IsFalse(filter.IsMatch(file.FileInfo));
            }
        }
    }
}
