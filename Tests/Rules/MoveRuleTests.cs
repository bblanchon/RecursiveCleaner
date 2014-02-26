/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011-2012 Benoit Blanchon
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 */

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
    class MoveRuleTests
    {
        static Environment environment = new Environment()
        {
            IsSimulating = false
        };

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

                rule.Apply(file.FileInfo, environment);

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

                rule.Apply(file1.FileInfo, environment);

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

                rule.Apply(file1.FileInfo, environment);

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

                rule.Apply(file1.FileInfo, environment);

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

                rule.Apply(file1.FileInfo, environment);

                Assert.IsFalse(folder1.Files.Any(x => x.Name == file1.Name));
                Assert.IsTrue(folder2.Files.Any(x => x.Name == file1.Name));
                Assert.AreEqual(2, folder2.Files.Count());
            }
        }
    }
}
