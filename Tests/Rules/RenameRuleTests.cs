using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace RecursiveCleaner.Tests.Rules
{    
    using Helpers;
    using Engine.Rules;
    using Engine.Environments;

    [TestFixture]
    class RenameRuleTests
    {
        static Environment environment = new Environment()
        {
            IsSimulating = false
        };

        [Test]
        public void FileNoCollision()
        {
            using( TemporaryFolder folder = new TemporaryFolder() )
            {
                var file = folder.CreateFile();

                var originalName = file.Name;
                const string newName = "test";

                var rule = new RenameRule
                {
                    Name = newName
                };

                rule.Apply(file.FileInfo, environment);

                Assert.IsFalse(folder.Files.Any(x => x.Name == originalName));
                Assert.IsTrue(folder.Files.Any(x => x.Name == newName));
                Assert.AreEqual(1, folder.Files.Count());
            }
        }

        [Test]
        public void FileIfExistsCancel()
        {
            using (TemporaryFolder folder = new TemporaryFolder())
            {
                var file1 = folder.CreateFile();
                var file2 = folder.CreateFile();

                var originalName = file1.Name;
                var newName = file2.Name;

                var rule = new RenameRule
                {
                    Name = newName,
                    IfExists = RenameRule.IfExistsMode.Cancel
                };

                rule.Apply(file1.FileInfo, environment);

                Assert.IsTrue(folder.Files.Any(x => x.Name == originalName));
                Assert.IsTrue(folder.Files.Any(x => x.Name == newName));
                Assert.AreEqual(2, folder.Files.Count());
            }
        }

        [Test]
        public void FileIfExistsDelete()
        {
            using (TemporaryFolder folder = new TemporaryFolder())
            {
                var file1 = folder.CreateFile();
                var file2 = folder.CreateFile();

                var originalName = file1.Name;
                var newName = file2.Name;

                var rule = new RenameRule
                {
                    Name = newName,
                    IfExists = RenameRule.IfExistsMode.Delete
                };

                rule.Apply(file1.FileInfo, environment);

                Assert.IsFalse(folder.Files.Any(x => x.Name == originalName));
                Assert.IsTrue(folder.Files.Any(x => x.Name == newName));
                Assert.AreEqual(1, folder.Files.Count());
            }
        }

        [Test]
        public void FileIfExistsRecycle()
        {
            using (TemporaryFolder folder = new TemporaryFolder())
            {
                var file1 = folder.CreateFile();
                var file2 = folder.CreateFile();

                var originalName = file1.Name;
                var newName = file2.Name;

                var rule = new RenameRule
                {
                    Name = newName,
                    IfExists = RenameRule.IfExistsMode.Recycle
                };

                rule.Apply(file1.FileInfo, environment);

                Assert.IsFalse(folder.Files.Any(x => x.Name == originalName));
                Assert.IsTrue(folder.Files.Any(x => x.Name == newName));
                Assert.AreEqual(1, folder.Files.Count());
            }
        }

        [Test]
        public void FileIfExistsRename()
        {
            using (TemporaryFolder folder = new TemporaryFolder())
            {
                var file1 = folder.CreateFile();
                var file2 = folder.CreateFile();

                var originalName = file1.Name;
                var newName = file2.Name;

                var rule = new RenameRule
                {
                    Name = newName,
                    IfExists = RenameRule.IfExistsMode.Rename
                };

                rule.Apply(file1.FileInfo, environment);

                Assert.IsFalse(folder.Files.Any(x => x.Name == originalName));
                Assert.IsTrue(folder.Files.Any(x => x.Name == newName));
                Assert.AreEqual(2, folder.Files.Count());
            }
        }
    }
}
