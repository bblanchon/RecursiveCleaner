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

namespace RecursiveCleaner.Tests.Filters
{
    using Helpers;
    using Engine.Filters;
    using Engine.Environments;

    [TestFixture]
    public class BiggerThanFilterTests
    {
        IFilter filter;
        Environment environment;

        [SetUp]
        public void SetUp()
        {
            environment = new Environment();
            filter = new BiggerThanFilter
            {
                Size = 10
            };
        }

        [Test]
        public void MatchingFile()
        {
            using (var file = new TemporaryFile())
            {
                file.Contents = "0123456789";
                Assert.IsTrue(filter.IsMatch(file.FileInfo, environment));
            }
        }

        [Test]
        public void NonMatchingFile()
        {
            using (var file = new TemporaryFile())
            {
                file.Contents = "01234";
                Assert.IsFalse(filter.IsMatch(file.FileInfo, environment));
            }
        }

        [Test]
        public void MatchingFolder()
        {
            using (var folder = new TemporaryFolder())
            {
                using (TemporaryFile file1 = folder.CreateFile(), file2 = folder.CreateFile())
                {
                    file1.Contents = "01234";
                    file2.Contents = "56789";
                    Assert.IsTrue(filter.IsMatch(folder.DirectoryInfo, environment));
                }
            }
        }

        [Test]
        public void NonMatchingFolder()
        {
            using (var folder = new TemporaryFolder())
            {
                using (TemporaryFile file1 = folder.CreateFile(), file2 = folder.CreateFile())
                {
                    file1.Contents = "0123";
                    file2.Contents = "5678";
                    Assert.IsFalse(filter.IsMatch(folder.DirectoryInfo, environment));
                }
            }
        }
    }
}
