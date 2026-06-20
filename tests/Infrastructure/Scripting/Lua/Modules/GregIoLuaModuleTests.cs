using System;
using System.IO;
using Xunit;
using FluentAssertions;
using MoonSharp.Interpreter;
using gregCore.Infrastructure.Scripting.Lua.Modules;

namespace gregCore.Tests.Infrastructure.Scripting.Lua.Modules;

public class GregIoLuaModuleTests : IDisposable
{
    private readonly string _testBaseDir;
    private readonly string _modDir;
    private readonly string _dataDir;
    private readonly string _dataSecretDir;
    private readonly Script _script;
    private readonly Table _gregTable;

    public GregIoLuaModuleTests()
    {
        _testBaseDir = Path.Combine(Path.GetTempPath(), "GregIoLuaModuleTests_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testBaseDir);

        _modDir = Path.Combine(_testBaseDir, "mods", "modA");
        _dataDir = Path.Combine(_modDir, "data");
        Directory.CreateDirectory(_dataDir);

        // A sibling directory that shares the "data" prefix.
        _dataSecretDir = Path.Combine(_modDir, "data_secret");
        Directory.CreateDirectory(_dataSecretDir);

        File.WriteAllText(Path.Combine(_dataSecretDir, "secret.txt"), "super secret");

        _script = new Script();
        _gregTable = new Table(_script);

        GregIoLuaModule.Register(_gregTable, _script, "modA", _modDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testBaseDir))
        {
            Directory.Delete(_testBaseDir, true);
        }
    }

    [Fact]
    public void ResolveSafe_WhenAccessingSiblingDirWithPrefixMatch_ShouldReturnFalseFromFunction()
    {
        // Act
        var ioTable = _gregTable.Get("io").Table;

        // Execute the function using Script.Call to properly catch errors or using the direct delegates
        // The methods are assigned as Action/Func directly to the table in MoonSharp.
        // We will call the function through Lua scripting to properly use MoonSharp's execution context.
        _script.Globals["greg"] = _gregTable;
        var luaCode = "return greg.io.file_exists('../data_secret/secret.txt')";
        var result = _script.DoString(luaCode);

        // Assert
        result.Boolean.Should().BeFalse();
    }
}
