using System;
using System.Reflection;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Xunit;
using gregCore.Bridge.LuaFFI;

namespace gregCore.Tests.Bridge;

public class LuaFFIBridgeTests
{
    private void InjectPlugin(LuaPlugin plugin)
    {
        var pluginsField = typeof(LuaFFIBridge).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static);
        var pluginsObj = pluginsField!.GetValue(null)!;

        if (pluginsObj is List<LuaPlugin> list)
        {
            list.Clear();
            list.Add(plugin);
        }
        else if (pluginsObj is System.Collections.IDictionary dict)
        {
            dict.Clear();
            dict.Add(plugin.Id, plugin);
        }

        var initField = typeof(LuaFFIBridge).GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static);
        initField!.SetValue(null, true);
    }

    private void ClearPlugins()
    {
        var pluginsField = typeof(LuaFFIBridge).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static);
        var pluginsObj = pluginsField!.GetValue(null)!;

        if (pluginsObj is List<LuaPlugin> list)
        {
            list.Clear();
        }
        else if (pluginsObj is System.Collections.IDictionary dict)
        {
            dict.Clear();
        }

        var initField = typeof(LuaFFIBridge).GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static);
        initField!.SetValue(null, false);
    }

    [Fact]
    public void OnSceneLoaded_WhenPluginThrows_DoesNotCrash()
    {
        // Arrange
        var script = new Script();
        var closure = script.DoString("return function() error('Test Error') end").Function;

        var plugin = new LuaPlugin
        {
            Id = "test_plugin",
            Script = script,
            OnSceneLoaded = closure
        };

        InjectPlugin(plugin);

        try
        {
            // Act
            var exception = Record.Exception(() => LuaFFIBridge.OnSceneLoaded("TestScene"));

            // Assert
            Assert.Null(exception); // Should not throw
        }
        finally
        {
            ClearPlugins();
        }
    }

    [Fact]
    public void OnSceneLoaded_WhenPluginValid_ExecutesSuccessfully()
    {
        // Arrange
        var script = new Script();
        script.Globals["sceneLoaded"] = false;
        var closure = script.DoString("return function(name) sceneLoaded = name end").Function;

        var plugin = new LuaPlugin
        {
            Id = "test_plugin",
            Script = script,
            OnSceneLoaded = closure
        };

        InjectPlugin(plugin);

        try
        {
            // Act
            var exception = Record.Exception(() => LuaFFIBridge.OnSceneLoaded("TestScene"));

            // Assert
            Assert.Null(exception);
            Assert.Equal("TestScene", script.Globals["sceneLoaded"]);
        }
        finally
        {
            ClearPlugins();
        }
    }

    [Fact]
    public void OnSceneLoaded_WhenUninitialized_DoesNothing()
    {
        // Arrange
        ClearPlugins();

        // Act
        var exception = Record.Exception(() => LuaFFIBridge.OnSceneLoaded("TestScene"));

        // Assert
        Assert.Null(exception); // Should return early and not throw
    }

    [Fact]
    public void Shutdown_WhenPluginThrows_DoesNotCrashAndClearsState()
    {
        // Arrange
        var script = new Script();
        var closure = script.DoString("return function() error('Test Error') end").Function;

        var plugin = new LuaPlugin
        {
            Id = "test_plugin",
            Script = script,
            OnShutdown = closure
        };

        InjectPlugin(plugin);

        try
        {
            // Act
            var exception = Record.Exception(() => LuaFFIBridge.Shutdown());

            // Assert
            Assert.Null(exception); // Should not throw

            var pluginsField = typeof(LuaFFIBridge).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static);
            var pluginsObj = pluginsField!.GetValue(null)!;
            if (pluginsObj is List<LuaPlugin> list) Assert.Empty(list);
            else if (pluginsObj is System.Collections.IDictionary dict) Assert.Empty(dict);

            var initField = typeof(LuaFFIBridge).GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.False((bool)initField!.GetValue(null)!); // Should clear initialized
        }
        finally
        {
            ClearPlugins();
        }
    }
}
