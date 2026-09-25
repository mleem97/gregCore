using System;
using System.IO;
using FluentAssertions;
using Xunit;
using MoonSharp.Interpreter;
using gregCore.Infrastructure.Scripting.Lua.Modules;

namespace gregCore.Tests.PublicApi;

public partial class LuaSdkTests
{
    [Fact]
    public void Items_Register_Fails_Headless_Without_Game()
    {
        string dir = NewTempDir();
        try
        {
            var script = NewScript();
            var greg = NewGreg(script);
            LuaItemsModule.Register(greg, script, "test", dir);
            var items = greg.Get("items").Table;
            var spec = script.DoString("return {name='T', price=100}");
            Call(items, "register_shop_item", " meshes", spec).Boolean.Should().BeFalse();
            Call(items, "register_static_item", "meshes", spec).Boolean.Should().BeFalse();
        }
        finally { DeleteTempDir(dir); }
    }

    [Fact]
    public void Tech_Extended_List_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaTechModule.Register(greg, script, "test");
        // base functions from the first batch still behave
        Call(greg.Get("tech").Table, "free_count").Number.Should().Be(0);
    }

    [Fact]
    public void World_Open_All_Walls_Registered()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaWorldModule.Register(greg, script, "test");
        greg.Get("world").Table.Get("open_all_walls").Type
            .Should().BeOneOf(DataType.Function, DataType.ClrFunction);
    }

    [Fact]
    public void Server_Round3_Ops_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaServerModule.Register(greg, script, "test");
        var server = greg.Get("server").Table;
        Call(server, "set_app", "nope", 1).Boolean.Should().BeFalse();
        Call(server, "clear_warning", "nope").Boolean.Should().BeFalse();
        Call(server, "has_cable", "nope").Boolean.Should().BeFalse();
        Call(server, "valid_position", "nope").Boolean.Should().BeFalse();
        Call(server, "capture", "nope").IsNil().Should().BeTrue();
        Call(server, "insert_into_rack", "nope",
            script.DoString("return {rack_uid=1}")).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Patch_Round3_Ops_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaPatchModule.Register(greg, script, "test");
        var patch = greg.Get("patch").Table;
        Call(patch, "valid_position", "nope").Boolean.Should().BeFalse();
        Call(patch, "capture", "nope").IsNil().Should().BeTrue();
        Call(patch, "insert_into_rack", "nope",
            script.DoString("return {rack_uid=1}")).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Rack_Round3_Ops_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaRackModule.Register(greg, script, "test");
        var rack = greg.Get("rack").Table;
        Call(rack, "position_count", 12345).Number.Should().Be(-1);
        Call(rack, "position_usage", 12345).Table.Length.Should().Be(0);
        Call(rack, "unmount", 12345).Boolean.Should().BeFalse();
        Call(rack, "position_set_used", 99, true).Boolean.Should().BeFalse();
        Call(rack, "position_allowed", 99).Boolean.Should().BeFalse();
        Call(rack, "position_begin_insert", 99).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Cable_Links_And_Ops_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaCableModule.Register(greg, script, "test");
        var cable = greg.Get("cable").Table;
        Call(cable, "links").Table.Length.Should().Be(0);
        Call(cable, "set_speed", 1, 5.0).Boolean.Should().BeFalse();
        Call(cable, "remove_sfp", 1).Boolean.Should().BeFalse();
        Call(cable, "second_action", 1).Boolean.Should().BeFalse();
        Call(cable, "label_action", 1).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Shop_Round3_Ops_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaShopModule.Register(greg, script, "test");
        var shop = greg.Get("shop").Table;
        Call(shop, "mod_items").Table.Length.Should().Be(0);
        Call(shop, "buy_mod_item", 1).Boolean.Should().BeFalse();
        Call(shop, "picker_is_open").Boolean.Should().BeFalse();
        Call(shop, "picker_open").Boolean.Should().BeFalse();
        Call(shop, "picker_cancel").Boolean.Should().BeFalse();
        Call(shop, "picker_color").IsNil().Should().BeTrue();
        Call(shop, "picker_set_color", 1.0, 0.0, 0.0, 1.0).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Tech_Request_Next_Job_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaTechModule.Register(greg, script, "test");
        Call(greg.Get("tech").Table, "request_next_job", 1).Boolean.Should().BeFalse();
    }

    [Fact]
    public void ModSave_List_Upsert_Remove_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaModSaveModule.Register(greg, script, "test");
        var modsave = greg.Get("modsave").Table;
        Call(modsave, "list", "test").Table.Length.Should().Be(0);
        Call(modsave, "upsert", "test",
            script.DoString("return {position={x=1,y=2,z=3}}")).Boolean.Should().BeFalse();
        Call(modsave, "remove", "test").Boolean.Should().BeFalse();
    }

    [Fact]
    public void SetIp_Show_Cancel_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaSubnetModule.Register(greg, script, "test");
        var setip = greg.Get("setip").Table;
        Call(setip, "show_for", "nope").Boolean.Should().BeFalse();
        Call(setip, "cancel").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Customer_Register_Subnet_New_Form()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaCustomerModule.Register(greg, script, "test");
        Call(greg.Get("customer").Table, "register_subnet",
            1, 10, "k", script.DoString("return {}")).Boolean.Should().BeFalse();
        Call(greg.Get("customer").Table, "apply_save",
            1, script.DoString("return {difficulty=2}")).Boolean.Should().BeFalse();
    }

    [Fact]
    public void All_Modules_Register_Together_Like_Bridge()
    {
        string dir = NewTempDir();
        try
        {
            var script = NewScript();
            var greg = NewGreg(script);
            RegisterAllBridgeModules(greg, script, dir);
            AssertAllBridgeTables(greg, script);
        }
        finally { DeleteTempDir(dir); }
    }

    private static void RegisterAllBridgeModules(Table greg, Script script, string dir)
    {
        RegisterBridgeBatchA(greg, script, dir);
        RegisterBridgeBatchB(greg, script, dir);
    }

    private static void RegisterBridgeBatchA(Table greg, Script script, string dir)
    {
        LuaPlayerModule.Register(greg, script, "test");
        LuaWorldModule.Register(greg, script, "test");
        LuaRackModule.Register(greg, script, "test");
        LuaServerModule.Register(greg, script, "test");
        LuaSwitchModule.Register(greg, script, "test");
        LuaPatchModule.Register(greg, script, "test");
        LuaTechModule.Register(greg, script, "test");
        LuaCableModule.Register(greg, script, "test");
        LuaNetModule.Register(greg, script, "test");
        LuaCustomerModule.Register(greg, script, "test");
        LuaEconomyModule.Register(greg, script, "test");
        LuaShopModule.Register(greg, script, "test");
        LuaRequestsModule.Register(greg, script, "test");
        LuaSubnetModule.Register(greg, script, "test");
        LuaUiModule.Register(greg, script, "test");
    }

    private static void RegisterBridgeBatchB(Table greg, Script script, string dir)
    {
        LuaTabletModule.Register(greg, script, "test");
        LuaModsModule.Register(greg, script, "test");
        LuaModSaveModule.Register(greg, script, "test");
        LuaItemsModule.Register(greg, script, "test", dir);
        LuaJsonModule.Register(greg, script, "test");
        LuaConfigModule.Register(greg, script, "test", dir);
        LuaSaveModule.Register(greg, script, "test", dir);
        LuaInternetModule.Register(greg, script, "test");
        LuaSettingsModule.Register(greg, script, "test");
        LuaObjectivesModule.Register(greg, script, "test");
        LuaTooltipModule.Register(greg, script, "test");
        LuaCoopModule.Register(greg, script, "test");
        LuaMiscModule.Register(greg, script, "test");
    }

    private static void AssertAllBridgeTables(Table greg, Script script)
    {
        foreach (var name in new[] { "player", "world", "rack", "server", "switch",
                         "patch", "tech", "cable", "net", "customer", "economy", "shop",
                         "requests", "subnet", "setip", "ui", "mods",
                         "modsave", "items", "json", "config", "save", "internet",
                         "settings", "objectives", "tooltip", "coop", "steam", "locale",
                         "numpad", "pause" })
        {
            var v = greg.Get(name);
            (v.Type == DataType.Table).Should().BeTrue(name + " table missing");
        }
        // Spot-check cross-module calls through one shared table.
        script.DoString("return greg.switch.count() + greg.server.count()").Number.Should().Be(0);
    }

    [Fact]
    public void Internet_Endpoints_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaInternetModule.Register(greg, script, "test");
        var net = greg.Get("internet").Table;
        Call(net, "endpoints").Table.Length.Should().Be(0);
        Call(net, "command_center_level").Number.Should().Be(0);
        Call(net, "auto_repair_mode").Number.Should().Be(-1);
        Call(net, "set_auto_repair_mode", 1).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Settings_Volumes_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaSettingsModule.Register(greg, script, "test");
        var settings = greg.Get("settings").Table;
        Call(settings, "set_master_volume", 0.5).Boolean.Should().BeFalse();
        Call(settings, "set_music_volume", 0.5).Boolean.Should().BeFalse();
        Call(settings, "set_effect_volume", 0.5).Boolean.Should().BeFalse();
        Call(settings, "set_racks_volume", 0.5).Boolean.Should().BeFalse();
        Call(settings, "reload").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Objectives_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaObjectivesModule.Register(greg, script, "test");
        var obj = greg.Get("objectives").Table;
        Call(obj, "show", 1).Boolean.Should().BeFalse();
        Call(obj, "stop").Boolean.Should().BeFalse();
        Call(obj, "skip").Boolean.Should().BeFalse();
        Call(obj, "active").Table.Length.Should().Be(0);
        Call(obj, "tutorial_in_progress").Boolean.Should().BeFalse();
        Call(obj, "create", script.DoString("return {loc=1, uid=2}")).Boolean.Should().BeFalse();
        Call(obj, "start", 1, 0.0, 0.0, 0.0).Boolean.Should().BeFalse();
        Call(obj, "clear").Boolean.Should().BeFalse();
        Call(obj, "play_video", 1, false).Boolean.Should().BeFalse();
        Call(obj, "show_in_pause", 1).Boolean.Should().BeFalse();
        Call(obj, "stop_video_in_pause").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Tooltip_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaTooltipModule.Register(greg, script, "test");
        var tip = greg.Get("tooltip").Table;
        Call(tip, "overlay", "hi", 0.0, 0.0, 0.0, 5).Boolean.Should().BeFalse();
        Call(tip, "hide").Boolean.Should().BeFalse();
        Call(tip, "interact", "hi").Boolean.Should().BeFalse();
        Call(tip, "hide_interact").Boolean.Should().BeFalse();
    }

    [Fact]
    public void Coop_Default_Headless()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaCoopModule.Register(greg, script, "test");
        var coop = greg.Get("coop").Table;
        Call(coop, "peers").Table.Length.Should().Be(0);
        Call(coop, "peer_timeout").Number.Should().Be(-1);
        Call(coop, "remove_avatar", 123.0).Boolean.Should().BeFalse();
    }

    [Fact]
    public void Misc_Steam_Locale_Numpad_Pause()
    {
        var script = NewScript();
        var greg = NewGreg(script);
        LuaMiscModule.Register(greg, script, "test");
        Call(greg.Get("steam").Table, "parse_lobby", "x").Number.Should().Be(0);
        var locale = greg.Get("locale").Table;
        Call(locale, "text", 1, "fb").String.Should().Be("fb");
        Call(locale, "change", 1).Boolean.Should().BeFalse();
        var numpad = greg.Get("numpad").Table;
        Call(numpad, "is_active").Boolean.Should().BeFalse();
        Call(numpad, "press", "1").Boolean.Should().BeFalse();
        Call(greg.Get("pause").Table, "is_paused").Boolean.Should().BeFalse();
    }
}
