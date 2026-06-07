using System;
using System.IO;
using FluentAssertions;
using gregCore.Infrastructure.Scripting;
using Xunit;

namespace gregCore.Tests.Infrastructure.Scripting
{
    public class GregSandboxHelperTests
    {
        [Fact]
        public void IsPathInsideDirectory_ExactMatch_ReturnsTrue()
        {
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var fullPath = baseDir;

            var result = GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsPathInsideDirectory_InsideDirectory_ReturnsTrue()
        {
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var fullPath = Path.Combine(baseDir, "test.txt");

            var result = GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsPathInsideDirectory_PrefixMatch_ReturnsFalse()
        {
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "data_evil", "test.txt");

            var result = GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsPathInsideDirectory_PathTraversal_ReturnsFalse()
        {
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var fullPath = Path.GetFullPath(Path.Combine(baseDir, "..", "data_evil", "test.txt"));

            var result = GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsPathInsideDirectory_PathTraversalResolvingInside_ReturnsTrue()
        {
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var fullPath = Path.GetFullPath(Path.Combine(baseDir, "subdir", "..", "test.txt"));

            var result = GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir);

            result.Should().BeTrue();
        }
    }
}
