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
    class BiggerThanXmlTest
    {
        [Test]
        public void AttributeMissing()
        {
            var xml = new XmlSample(@"<BiggerThan />");
            using (var reader = xml.Read())
            {
                Assert.Throws<AttributeMissingException>(() =>
                {
                    ConfigFileReader.ReadFilter(reader);
                });            
            }
        }

        [Test]
        public void Bytes()
        {
            var xml = new XmlSample(@"<BiggerThan bytes='10' />");
            using( var reader = xml.Read() )
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10, size);
            }
        }

        [Test]
        public void Kilo()
        {
            var xml = new XmlSample(@"<BiggerThan KB='10' />");
            using (var reader = xml.Read())
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10*1024, size);
            }
        }

        [Test]
        public void Mega()
        {
            var xml = new XmlSample(@"<BiggerThan MB='10' />");
            using (var reader = xml.Read())
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10 * 1024 * 1024, size);
            }
        }

        [Test]
        public void Tera()
        {
            var xml = new XmlSample(@"<BiggerThan TB='10' />");
            using (var reader = xml.Read())
            {
                var filter = ConfigFileReader.ReadFilter(reader);
                Assert.IsInstanceOf<BiggerThanFilter>(filter);

                var size = ((BiggerThanFilter)filter).Size;
                Assert.AreEqual(10L * 1024 * 1024 * 1024, size);
            }
        }
    }
}
