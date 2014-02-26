/*
 * RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
 * Copyright (C) 2011-2014 Benoit Blanchon
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
using NUnit.Framework;

namespace RecursiveCleaner.Tests.Filters
{
    using Helpers;
    using Engine.Filters;
    using Engine.Environments;

    [TestFixture]
    class OlderThanFilterTests
    {
        IFilter filter;
        Environment environment;

        [SetUp]
        public void SetUp()
        {
            filter = new OlderThanFilter(0, 0, 1, 0, 0, 0);
            environment = new Environment();
        }

        [Test]
        public void OldFile_IsMatch_ReturnsTrue()
        {
            using (var file = new TemporaryFile())
            {
                file.FileInfo.LastWriteTime = DateTime.Now.Subtract(TimeSpan.FromDays(2));
                Assert.IsTrue(filter.IsMatch(file.FileInfo, environment));
            }
        }

        [Test]
        public void NewFile_IsMatch_ReturnsFalse()
        {
            using (var file = new TemporaryFile())
            {
                file.FileInfo.LastWriteTime = DateTime.Now;
                Assert.IsFalse(filter.IsMatch(file.FileInfo, environment));
            }
        }

        [Test]
        public void OldFolder_IsMatch_ReturnsTrue()
        {
            using (var folder = new TemporaryFolder())
            {
                folder.DirectoryInfo.LastWriteTime = DateTime.Now.Subtract(TimeSpan.FromDays(2));
                Assert.IsTrue(filter.IsMatch(folder.DirectoryInfo, environment));
            }
        }

        [Test]
        public void NewFolder_IsMatch_ReturnsFalse()
        {
            using (var folder = new TemporaryFolder())
            {
                folder.DirectoryInfo.LastWriteTime = DateTime.Now;
                Assert.IsFalse(filter.IsMatch(folder.DirectoryInfo, environment));
            }
        }
    }
}
