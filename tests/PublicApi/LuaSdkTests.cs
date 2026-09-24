/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Greg.LUA SDK ohne Spiel: Modul-Registrierung, Safe-Defaults
///               ohne Live-Singletons, Config/Save-Persistenz, JSON-Roundtrip.
/// Maintainer:   Alles hier muss 100 % Zeilen-Coverage halten.
/// </file-summary>

using System;
using System.IO;
using FluentAssertions;
using Xunit;
using MoonSharp.Interpreter;
using gregCore.Infrastructure.Scripting.Lua.Modules;

namespace gregCore.Tests.PublicApi;

public class LuaSdkTests
{
    private static Script NewScript() => new Script();

    private static Table NewGreg(Script script)
    {
        var greg = new Table(script);
        script.Globals["greg"] = greg;
        return greg;
    }

    private static string NewTempDir()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gregLuaTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(dir, "data"));
        return dir;
    }

    private static void DeleteTempDir(string dir)
    {
        try { if (Directory.Exists(dir)) Directory.Delete(dir, true); } catch { }
    }

    private static DynValue Call(Table table, string func, params object[] args)
    {
        var fn = table.Get(func);
        (fn.Type == DataType.Function || fn.Type == DataType.ClrFunction)
            .Should().BeTrue(func + " must be registered");
        return table.OwnerScript.Call(fn, args);
    }

    [Fact]
    public void Switch_Register_Exposes_All_Functions()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaSwitchModule.Register(greg, script, "test");
        var sw = greg.Get("switch").Table;
        foreach (var fn in new[] { "get_all", "get_list", "count", "broken_count", "find_by_id", "repair", "repair_all" })
            sw.Get(fn).Type.Should().BeOneOf(DataType.Function, DataType.ClrFunction);
    }

    [Fact]
    public void Switch_Count_Zero_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaSwitchModule.Register(greg, script, "test");
        var sw = greg.Get("switch").Table;
        Call(sw, "count").Number.Should().Be(0);
        Call(sw, "get_all").Table.Length.Should().Be(0);
        Call(sw, "find_by_id", "nope").IsNil().Should().BeTrue();
        Call(sw, "repair", "nope").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Switch_EndToEnd_LuaSnippet()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaSwitchModule.Register(greg, script, "test");
        script.DoString("return greg.switch.count()").Number.Should().Be(0);
    }

    [Fact]
    public void Tech_Counts_Zero_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaTechModule.Register(greg, script, "test");
        var tech = greg.Get("tech").Table;
        Call(tech, "free_count").Number.Should().Be(0);
        Call(tech, "total_count").Number.Should().Be(0);
        Call(tech, "dispatch_server").Number.Should().Be(0);
        Call(tech, "dispatch_switch").Number.Should().Be(0);
    }

    [Fact]
    public void Server_Extended_Functions_Exist_And_Default()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaServerModule.Register(greg, script, "test");
        var server = greg.Get("server").Table;
        foreach (var fn in new[] { "get_all", "get_list", "count", "broken_count", "find_by_id",
                     "find_by_ip", "repair", "repair_all", "power_on", "power_off", "set_ip", "set_customer" })
            server.Get(fn).Type.Should().BeOneOf(DataType.Function, DataType.ClrFunction);
        Call(server, "count").Number.Should().Be(0);
        Call(server, "find_by_id", "nope").IsNil().Should().BeTrue();
        Call(server, "power_on", "nope").Boolean.Should().BeFalse();
        Call(server, "set_ip", "nope", "10.0.0.1").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Config_Roundtrip_Persists_To_File()
    {
        string dir = NewTempDir();
        try
        {
            var script = NewScript();
            var greg = NewGreg(script);
            LuaConfigModule.Register(greg, script, "test", dir);
            var cfg = greg.Get("config").Table;
            Call(cfg, "get", "k").IsNil().Should().BeTrue();
            Call(cfg, "set", "k", "v");
            Call(cfg, "get", "k").String.Should().Be("v");
            Call(cfg, "get_or", "missing", "d").String.Should().Be("d");
            Call(cfg, "has", "k").Boolean.Should().BeTrue();
            Call(cfg, "delete", "k").Boolean.Should().BeTrue();
            Call(cfg, "has", "k").Boolean.Should().BeFalse();
            File.Exists(Path.Combine(dir, "data", "config.json")).Should().BeTrue();

            // Reload from disk (new Script simulates a fresh session).
            Call(cfg, "set", "keep", "yes");
            var script2 = NewScript();
            var greg2 = NewGreg(script2);
            LuaConfigModule.Register(greg2, script2, "test", dir);
            Call(greg2.Get("config").Table, "get", "keep").String.Should().Be("yes");
        }
        finally { DeleteTempDir(dir); }
    }

    [Fact]
    public void Save_Roundtrip_And_SaveNow()
    {
        string dir = NewTempDir();
        try
        {
            var script = NewScript();
            var greg = NewGreg(script);
            LuaSaveModule.Register(greg, script, "test", dir);
            var save = greg.Get("save").Table;
            Call(save, "set", "runs", "3");
            Call(save, "save_now").Boolean.Should().BeTrue();
            File.Exists(Path.Combine(dir, "data", "save.json")).Should().BeTrue();
            Call(save, "get", "runs").String.Should().Be("3");

            var script2 = NewScript();
            var greg2 = NewGreg(script2);
            LuaSaveModule.Register(greg2, script2, "test", dir);
            Call(greg2.Get("save").Table, "get", "runs").String.Should().Be("3");
        }
        finally { DeleteTempDir(dir); }
    }

    [Fact]
    public void Json_Parse_Stringify_Roundtrip()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaJsonModule.Register(greg, script, "test");
        var json = greg.Get("json").Table;
        var parsed = Call(json, "parse", "{\"a\":1,\"b\":[true,false],\"c\":\"x\",\"d\":null}");
        parsed.Type.Should().Be(DataType.Table);
        parsed.Table.Get("a").Number.Should().Be(1);
        parsed.Table.Get("c").String.Should().Be("x");
        string text = Call(json, "stringify", parsed).String;
        text.Should().Contain("\"a\"").And.Contain("1");
        Call(json, "parse", "not json{{").IsNil().Should().BeTrue();
    }

    [Fact]
    public void Io_Aliases_And_Json_File_Helpers()
    {
        string dir = NewTempDir();
        try
        {
            var script = NewScript();
            var greg = NewGreg(script);
            GregIoLuaModule.Register(greg, script, "test", dir);
            var io = greg.Get("io").Table;
            foreach (var fn in new[] { "read_file", "read_text", "write_file", "write_text",
                         "append_file", "file_exists", "list_files", "delete_file", "read_json", "write_json" })
                io.Get(fn).Type.Should().BeOneOf(DataType.Function, DataType.ClrFunction);
            Call(io, "write_text", "a.txt", "hi").Should().NotBeNull();
            Call(io, "read_text", "a.txt").String.Should().Be("hi");
            Call(io, "write_json", "d.json", script.DoString("return {n=7}")).Boolean.Should().BeTrue();
            Call(io, "read_json", "d.json").Table.Get("n").Number.Should().Be(7);
        }
        finally { DeleteTempDir(dir); }
    }
}
