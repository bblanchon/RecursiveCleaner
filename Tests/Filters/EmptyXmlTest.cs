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
    class EmptyXmlTest
    {
        [Test]
        public void ValidElement()
        {
            var xml = new XmlSample(@"<Empty />");
            using( var reader = xml.Read() )
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<EmptyFilter>(filter);
            }
        }
    }
}
