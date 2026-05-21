using System;
using System.IO;
using System.Reflection;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace gregCore.Tests.Bridge.CSharpScript
{
    public class GregCSharpCompilerTests
    {
        private Type GetCompilerType()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "gregCore");
            if (assembly == null)
            {
                // In some test runners, the assembly might not be loaded yet
                assembly = Assembly.Load("gregCore");
            }

            var type = assembly.GetType("gregCore.Bridge.CSharpScript.GregCSharpCompiler");
            if (type == null)
            {
                throw new Exception("GregCSharpCompiler type not found");
            }

            return type;
        }

        [Fact]
        public void Compile_WhenCodeIsInvalid_ReturnsNull()
        {
            // Arrange
            string modId = "InvalidCodeMod";
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                string sourceFile = Path.Combine(tempDir, "script.cs");
                // Intentionally broken C# code (missing semicolon and closing brace)
                File.WriteAllText(sourceFile, "public class Test { public void Do() { int x = 5 }");

                // Act - using reflection since InternalsVisibleTo is unreliable in some build setups
                Type compilerType = GetCompilerType();
                MethodInfo? compileMethod = compilerType.GetMethod("Compile", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                // Assert
                compileMethod.Should().NotBeNull("Compile method should exist");

                object? result = compileMethod!.Invoke(null, new object[] { modId, new[] { sourceFile } });
                result.Should().BeNull();
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public void Compile_WithEmptySourceFiles_ReturnsNull()
        {
            // Arrange
            string modId = "EmptyMod";

            // Act
            Type compilerType = GetCompilerType();
            MethodInfo? compileMethod = compilerType.GetMethod("Compile", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            object? result = compileMethod!.Invoke(null, new object[] { modId, Array.Empty<string>() });

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Compile_WithNonExistentFiles_ReturnsNull()
        {
            // Arrange
            string modId = "NonExistentMod";
            string nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".cs");

            // Act
            Type compilerType = GetCompilerType();
            MethodInfo? compileMethod = compilerType.GetMethod("Compile", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            object? result = compileMethod!.Invoke(null, new object[] { modId, new[] { nonExistentFile } });

            // Assert
            result.Should().BeNull();
        }
    }
}
