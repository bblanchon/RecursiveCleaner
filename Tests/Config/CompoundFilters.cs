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
    class CompoundFilters
    {
        [Test]
        public void IgnoreMatchingAll()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore>
        <MatchingAll>            
            <OlderThan seconds='1' />
            <Regex>.*</Regex>
            <Wildcards>*.*</Wildcards>
        </MatchingAll>       
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.IsInstanceOf<MatchingAllFilter>(rules[0].Filter);
                var filter = rules[0].Filter as ParentFilter;
                var children = filter.Children.ToArray();
                Assert.AreEqual(3, children.Length);
                Assert.IsInstanceOf<OlderThanFilter>(children[0]);
                Assert.IsInstanceOf<RegexFilter>(children[1]);
                Assert.IsInstanceOf<WildcardsFilter>(children[2]);
            }
        }    

        [Test]
        public void IgnoreMatchingAny()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore>
        <MatchingAny>            
            <OlderThan seconds='1' />
            <Regex>.*</Regex>
            <Wildcards>*.*</Wildcards>
        </MatchingAny>       
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.IsInstanceOf<MatchingAnyFilter>(rules[0].Filter);
                var filter = rules[0].Filter as ParentFilter;
                var children = filter.Children.ToArray();
                Assert.AreEqual(3, children.Length);
                Assert.IsInstanceOf<OlderThanFilter>(children[0]);
                Assert.IsInstanceOf<RegexFilter>(children[1]);
                Assert.IsInstanceOf<WildcardsFilter>(children[2]);
            }
        }

        [Test]
        public void IgnoreMatchingNone()
        {
            var xml =
@"<RecursiveCleaner>
    <Ignore>
        <MatchingNone>            
            <OlderThan seconds='1' />
            <Regex>.*</Regex>
            <Wildcards>*.*</Wildcards>
        </MatchingNone>       
    </Ignore>
</RecursiveCleaner>";
            using (var file = new TemporaryFile { Contents = xml })
            {
                var rules = ConfigFileReader.Read(file.Path).ToArray();
                Assert.AreEqual(1, rules.Length);
                Assert.IsInstanceOf<IgnoreRule>(rules[0]);
                Assert.IsInstanceOf<MatchingNoneFilter>(rules[0].Filter);
                var filter = rules[0].Filter as ParentFilter;
                var children = filter.Children.ToArray();
                Assert.AreEqual(3, children.Length);
                Assert.IsInstanceOf<OlderThanFilter>(children[0]);
                Assert.IsInstanceOf<RegexFilter>(children[1]);
                Assert.IsInstanceOf<WildcardsFilter>(children[2]);
            }
        }
    }
}
