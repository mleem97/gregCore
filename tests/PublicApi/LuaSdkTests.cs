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

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "S1313:Hardcoded IP addresses should not be used", Justification = "Test fixtures only: RFC1918 documentation address (10.0.0.1) and netmask literal (255.255.255.0) as Lua assert inputs; never bound or connected.")]
public partial class LuaSdkTests
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
        try { if (Directory.Exists(dir)) Directory.Delete(dir, true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
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
        Call(tech, "list").Table.Length.Should().Be(0);
        Call(tech, "send_to_server", 1, "nope").Boolean.Should().BeFalse();
        Call(tech, "hire", 0).Boolean.Should().BeFalse();
        Call(tech, "fire", 1).Boolean.Should().BeFalse();
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

    [Fact]
    public void Patch_Register_And_Defaults()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaPatchModule.Register(greg, script, "test");
        var patch = greg.Get("patch").Table;
        foreach (var fn in new[] { "get_all", "get_list", "count", "find_by_id", "has_cable" })
            patch.Get(fn).Type.Should().BeOneOf(DataType.Function, DataType.ClrFunction);
        Call(patch, "count").Number.Should().Be(0);
        Call(patch, "find_by_id", "nope").IsNil().Should().BeTrue();
        Call(patch, "has_cable", "nope").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Customer_Bases_Empty_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaCustomerModule.Register(greg, script, "test");
        var customer = greg.Get("customer").Table;
        Call(customer, "bases").Table.Length.Should().Be(0);
        Call(customer, "find_base", 1).IsNil().Should().BeTrue();
        Call(customer, "is_ip_present", 1, "10.0.0.1").Boolean.Should().BeFalse();
        Call(customer, "app_id_for_ip", 1, "10.0.0.1").Number.Should().Be(-1);
        Call(customer, "register_subnet", 1, 10, "k", script.DoString("return {}")).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Economy_Nil_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaEconomyModule.Register(greg, script, "test");
        var economy = greg.Get("economy").Table;
        Call(economy, "sheet").IsNil().Should().BeTrue();
        Call(economy, "history").Table.Length.Should().Be(0);
    }

    [Fact]
    public void Shop_Empty_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaShopModule.Register(greg, script, "test");
        var shop = greg.Get("shop").Table;
        foreach (var fn in new[] { "items", "unlock", "buy", "cart", "cart_add", "cart_remove" })
            shop.Get(fn).Type.Should().BeOneOf(DataType.Function, DataType.ClrFunction);
        Call(shop, "items").Table.Length.Should().Be(0);
        Call(shop, "buy", 1).Boolean.Should().BeFalse();
        Call(shop, "cart").Table.Length.Should().Be(0);
    }

    [Fact]
    public void Net_Empty_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaNetModule.Register(greg, script, "test");
        var net = greg.Get("net").Table;
        foreach (var fn in new[] { "routers", "firewalls", "sfps", "lacps", "cables" })
            Call(net, fn).Table.Length.Should().Be(0);
    }

    [Fact]
    public void Mods_Registry_And_Deps()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaModsModule.Register(greg, script, "test");
        var mods = greg.Get("mods").Table;
        Call(mods, "is_loaded", "nope").Boolean.Should().BeFalse();
        Call(mods, "version", "nope").String.Should().Be("");
        Call(mods, "declare", script.DoString("return {{mod='x', min_version='1.0'}}")).Boolean.Should().BeTrue();
        var check = Call(mods, "check");
        check.Type.Should().Be(DataType.Table);
        var ok = Call(mods, "ensure", script.DoString("return {mod='x'}"));
        ok.Type.Should().Be(DataType.Tuple);
    }

    [Fact]
    public void Tablet_Open_Fails_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaTabletModule.Register(greg, script, "test");
        Call(greg, "tablet_open", "T").String.Should().Be("");
        Call(greg, "widget_open", "W", 1.0, 2.0).String.Should().Be("");
        Call(greg, "panel_visible", "nope").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Subnet_Pure_Math_Works_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaSubnetModule.Register(greg, script, "test");
        var subnet = greg.Get("subnet").Table;
        Call(subnet, "mask_from_cidr", 24).String.Should().Be("255.255.255.0");
        // Live-Game nötig: headless kommen leere Defaults zurück.
        Call(subnet, "first_usable", "192.168.1.0/24").String.Should().Be("");
        Call(subnet, "usable_ips", "192.168.1.0/30").Table.Length.Should().Be(0);
    }

    [Fact]
    public void Requests_Empty_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaRequestsModule.Register(greg, script, "test");
        var req = greg.Get("requests").Table;
        Call(req, "list").Table.Length.Should().Be(0);
        Call(req, "current_number").Number.Should().Be(0);
        // requests.add() deliberately untested headless: constructing a live
        // Il2Cpp.ServiceRequest outside the game crashes its finalizer on GC
        // (game-only path, verified by review + build).
    }

}
