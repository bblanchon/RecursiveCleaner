using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RecursiveCleaner.Tests.Helpers;
using RecursiveCleaner.Engine.Config;

namespace RecursiveCleaner.Tests.Config
{
    [TestFixture]
    class InvalidFiles
    {
        [Test]
        [ExpectedException]
        public void EmptyFile()
        {
            using (var file = new TemporaryFile())
            {
                ConfigFileReader.Read(file.Path);
            }
        }

        [Test]
        [ExpectedException]
        public void InvalidRoot()
        {
            var xml = @"<Recursive></Recursive>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                ConfigFileReader.Read(file.Path);
            }
        }
    }
}
