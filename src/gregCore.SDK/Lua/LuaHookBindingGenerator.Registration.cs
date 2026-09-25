using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua;

public sealed partial class LuaHookBindingGenerator
{
    // Registers one greg.hooks.{group} table per loaded group.
    private void RegisterAllGroups(Script script, Table hooksTable, string modId)
    {
        try
        {
            foreach (var kvp in _hooksByGroup)
            {
                try
                {
                    RegisterSingleGroup(script, hooksTable, modId, kvp.Key, kvp.Value);
                }
                catch { }
            }
        }
        catch { }
    }

    // Builds the table for a single hook group.
    private void RegisterSingleGroup(Script script, Table hooksTable, string modId, string groupKey, List<HookDefinition> hooks)
    {
        try
        {
            string groupName = groupKey.ToLowerInvariant();
            var groupTable = new Table(script);
            RegisterHookSubscriptions(script, groupTable, modId, hooks);
            RegisterHookList(script, groupTable, hooks);
            hooksTable[groupName] = groupTable;
        }
        catch { }
    }

    // Creates on_{method} subscription functions for one group.
    private void RegisterHookSubscriptions(Script script, Table groupTable, string modId, List<HookDefinition> hooks)
    {
        try
        {
            foreach (var hook in hooks)
            {
                try
                {
                    RegisterOneSubscription(script, groupTable, modId, hook);
                }
                catch { }
            }
        }
        catch { }
    }

    // Creates a single on_{method} subscription entry.
    private void RegisterOneSubscription(Script script, Table groupTable, string modId, HookDefinition hook)
    {
        try
        {
            string luaMethodName = "on_" + ToSnakeCase(hook.MethodName);
            string fullHookId = $"greg.{hook.Group}.{hook.MethodName}";
            groupTable[luaMethodName] = (Action<Closure>)(callback =>
            {
                SubscribeHook(script, modId, fullHookId, callback);
            });
        }
        catch { }
    }

    // Subscribes a Lua callback to the event bus with defensive error handling.
    private void SubscribeHook(Script script, string modId, string fullHookId, Closure callback)
    {
        try
        {
            _eventBus.Subscribe(fullHookId, payload =>
            {
                try
                {
                    var luaPayload = Modules.GregEventLuaModule.PayloadToTable(script, payload);
                    callback.Call(luaPayload);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[LuaMod:{modId}] Hook handler error for '{fullHookId}': {ex.Message}");
                }
            });
        }
        catch { }
    }

    // Adds the per-group list() discovery function.
    private static void RegisterHookList(Script script, Table groupTable, List<HookDefinition> hooks)
    {
        try
        {
            groupTable["list"] = (Func<Table>)(() => BuildHookNameList(script, hooks));
        }
        catch { }
    }

    // Builds the table returned by group list().
    private static Table BuildHookNameList(Script script, List<HookDefinition> hooks)
    {
        var list = new Table(script);
        try
        {
            int i = 1;
            foreach (var hook in hooks)
            {
                try
                {
                    list[i++] = "on_" + ToSnakeCase(hook.MethodName);
                }
                catch { }
            }
        }
        catch { }
        return list;
    }

    // Adds the top-level groups() discovery function.
    private void RegisterGroupList(Script script, Table hooksTable)
    {
        try
        {
            hooksTable["groups"] = (Func<Table>)(() => BuildGroupNameList(script));
        }
        catch { }
    }

    // Builds the table returned by groups().
    private Table BuildGroupNameList(Script script)
    {
        var list = new Table(script);
        try
        {
            int i = 1;
            foreach (var group in _hooksByGroup.Keys.OrderBy(k => k))
            {
                try
                {
                    list[i++] = group.ToLowerInvariant();
                }
                catch { }
            }
        }
        catch { }
        return list;
    }
}
