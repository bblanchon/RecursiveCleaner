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
    class SimpleConfigFiles
    {
        [Test]        
        public void RecycleFilesRegex()
        {
            var xml = 
@"<RecursiveCleaner>
    <Recycle Target='Files'>
        <Regex>\.\w+~</Regex>
  </Recycle>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<RecycleRule>(rules[0]);
                Assert.AreEqual(RuleTarget.Files, rules[0].Target);
                Assert.IsInstanceOf<RegexFilter>(rules[0].Filter);
            }
        }

        [Test]
        public void DeleteFoldersWildcards()
        {
            var xml =
@"<RecursiveCleaner>
    <Delete Target='Folders'>
        <Wildcards>\.\w+~</Wildcards>
  </Delete>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<DeleteRule>(rules[0]);
                Assert.AreEqual(RuleTarget.Folders, rules[0].Target);
                Assert.IsInstanceOf<WildcardsFilter>(rules[0].Filter);
            }
        }


        [Test]
        public void IgnoreFilerAndFoldersOlderThan()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore Target='FilesAndFolders'>
        <OlderThan years='1' months='2' days='3' hours='4' minutes='5' seconds='6' />
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.AreEqual(RuleTarget.FilesAndFolders, rules[0].Target);
                Assert.IsInstanceOf<OlderThanFilter>(rules[0].Filter);
                var filter = rules[0].Filter as OlderThanFilter;
                Assert.AreEqual(4, (int)filter.TimeSpan.Hours);
                Assert.AreEqual(5, (int)filter.TimeSpan.Minutes);
                Assert.AreEqual(6, (int)filter.TimeSpan.Seconds);
            }
        }
    }
}
