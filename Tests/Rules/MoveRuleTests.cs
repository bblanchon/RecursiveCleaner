using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RecursiveCleaner.Engine.Rules;
using RecursiveCleaner.Tests.Helpers;

namespace RecursiveCleaner.Tests.Rules
{
    [TestFixture]
    class MoveRuleTests
    {
        [Test]
        public void FileNoCollision()
        {
            using( TemporaryFolder folder1 = new TemporaryFolder(), folder2 = new TemporaryFolder() )
            {
                var file = folder1.CreateFile();                

                var rule = new MoveRule
                {
                    Destination = folder2.Path
                };

                rule.Apply(file.FileInfo, false);

                Assert.IsFalse(folder1.Files.Any(x => x.Name == file.Name));
                Assert.IsTrue(folder2.Files.Any(x=>x.Name==file.Name));
            }                           
        }

        [Test]
        public void FileIfExistsCancel()
        {
            using (TemporaryFolder folder1 = new TemporaryFolder(), folder2 = new TemporaryFolder())
            {
                var file1 = folder1.CreateFile();
                var file2 = folder2.CreateFile(file1.Name);

                var rule = new MoveRule
                {
                    Destination = folder2.Path,
                    IfExists = MoveRule.IfExistsMode.Cancel
                };

                rule.Apply(file1.FileInfo, false);

                Assert.IsTrue(folder1.Files.Any(x => x.Name == file1.Name));
                Assert.AreEqual(1, folder2.Files.Count());
            }
        }

        [Test]
        public void FileIfExistsDelete()
        {
            using (TemporaryFolder folder1 = new TemporaryFolder(), folder2 = new TemporaryFolder())
            {
                var file1 = folder1.CreateFile();
                var file2 = folder2.CreateFile(file1.Name);

                var rule = new MoveRule
                {
                    Destination = folder2.Path,
                    IfExists = MoveRule.IfExistsMode.Delete
                };

                rule.Apply(file1.FileInfo, false);

                Assert.IsFalse(folder1.Files.Any(x => x.Name == file1.Name));
                Assert.IsTrue(folder2.Files.Any(x => x.Name == file1.Name));
                Assert.AreEqual(1, folder2.Files.Count());
            }
        }

        [Test]
        public void FileIfExistsRecycle()
        {
            using (TemporaryFolder folder1 = new TemporaryFolder(), folder2 = new TemporaryFolder())
            {
                var file1 = folder1.CreateFile();
                var file2 = folder2.CreateFile(file1.Name);

                var rule = new MoveRule
                {
                    Destination = folder2.Path,
                    IfExists = MoveRule.IfExistsMode.Recycle
                };

                rule.Apply(file1.FileInfo, false);

                Assert.IsFalse(folder1.Files.Any(x => x.Name == file1.Name));
                Assert.IsTrue(folder2.Files.Any(x => x.Name == file1.Name));
                Assert.AreEqual(1, folder2.Files.Count());
            }
        }

        [Test]
        public void FileIfExistsRename()
        {
            using (TemporaryFolder folder1 = new TemporaryFolder(), folder2 = new TemporaryFolder())
            {
                var file1 = folder1.CreateFile();
                var file2 = folder2.CreateFile(file1.Name);

                var rule = new MoveRule
                {
                    Destination = folder2.Path,
                    IfExists = MoveRule.IfExistsMode.Rename
                };

                rule.Apply(file1.FileInfo, false);

                Assert.IsFalse(folder1.Files.Any(x => x.Name == file1.Name));
                Assert.IsTrue(folder2.Files.Any(x => x.Name == file1.Name));
                Assert.AreEqual(2, folder2.Files.Count());
            }
        }
    }
}
