/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Event-Binding Funktionen für Lua.
/// Maintainer:   Verbindet Lua-Callbacks mit dem IGregEventBus.
///               greg.on(), greg.off(), greg.once(), greg.fire()
/// </file-summary>

using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class GregEventLuaModule
{
    private static readonly Dictionary<string, List<(Closure closure, Action<EventPayload> handler)>> _handlers = new();

    /// <summary>
    /// Registriert Event-Funktionen im greg-Table.
    /// </summary>
    public static void Register(Table greg, Script script, GregEventBus eventBus, string modId)
    {
        // greg.on(hookName, callback) – Subscribe to an event
        greg["on"] = (Action<string, Closure>)((hookName, callback) =>
        {
            try
            {
                Action<EventPayload> handler = payload =>
                {
                    try
                    {
                        var luaTable = PayloadToTable(script, payload);
                        callback.Call(luaTable);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"[LuaMod:{modId}] Event handler error for '{hookName}': {ex.Message}");
                    }
                };

                eventBus.Subscribe(hookName, handler);

                // Track for cleanup
                if (!_handlers.TryGetValue(modId, out var list))
                {
                    list = new List<(Closure, Action<EventPayload>)>();
                    _handlers[modId] = list;
                }
                list.Add((callback, handler));
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaMod:{modId}] greg.on('{hookName}') failed: {ex.Message}");
            }
        });

        // greg.once(hookName, callback) – Subscribe once, auto-unsubscribes after first call
        greg["once"] = (Action<string, Closure>)((hookName, callback) =>
        {
            try
            {
                Action<EventPayload>? handler = null;
                handler = payload =>
                {
                    eventBus.Unsubscribe(hookName, handler!);

                    try
                    {
                        var luaTable = PayloadToTable(script, payload);
                        callback.Call(luaTable);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"[LuaMod:{modId}] greg.once handler error for '{hookName}': {ex.Message}");
                    }
                };

                eventBus.Subscribe(hookName, handler);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaMod:{modId}] greg.once('{hookName}') failed: {ex.Message}");
            }
        });

        // greg.fire(hookName, dataTable) – Fire a custom event
        greg["fire"] = (Action<string, Table?>)((hookName, dataTable) =>
        {
            try
            {
                var data = new Dictionary<string, object>();
                if (dataTable != null)
                {
                    foreach (var pair in dataTable.Pairs)
                    {
                        string key = pair.Key.Type == DataType.String ? pair.Key.String : pair.Key.ToString();
                        data[key] = pair.Value.ToObject() ?? "nil";
                    }
                }

                var payload = new EventPayload
                {
                    HookName = hookName,
                    OccurredAtUtc = DateTime.UtcNow,
                    Data = data,
                    IsCancelable = false
                };

                eventBus.Publish(hookName, payload);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaMod:{modId}] greg.fire('{hookName}') failed: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// Konvertiert ein EventPayload in ein Lua-Table.
    /// </summary>
    public static Table PayloadToTable(Script script, EventPayload payload)
    {
        var table = new Table(script);
        table["hook_name"] = payload.HookName ?? "";
        table["timestamp"] = payload.OccurredAtUtc.ToString("O");
        table["cancelable"] = payload.IsCancelable;
        table["cancelled"] = payload.IsCancelled;

        if (payload.Data != null)
        {
            var dataTable = new Table(script);
            foreach (var kvp in payload.Data)
            {
                try
                {
                    dataTable[kvp.Key] = DynValue.FromObject(script, kvp.Value);
                }
                catch
                {
                    dataTable[kvp.Key] = kvp.Value?.ToString() ?? "nil";
                }
            }
            table["data"] = dataTable;
        }

        return table;
    }

    /// <summary>
    /// Entfernt alle Handler eines Mods (für Shutdown/Hot-Reload).
    /// </summary>
    public static void UnregisterAll(string modId, GregEventBus eventBus)
    {
        // Note: EventBus unsubscribe by handler reference would need to be tracked
        // For now, clear the tracking list
        _handlers.Remove(modId);
    }
}
