using System;
using System.Collections.Generic;
using greg.Core.Hooks;
using greg.Sdk;
using MoonSharp.Interpreter;
using MelonLoader;
using greg.Core;
using greg.Core.Scripting.Lua;

namespace greg.Core.Scripting.Lua;

/// <summary>
/// Connects Lua scripts to the gregCore event/hook system.
/// <list type="bullet">
///   <item><c>greg.on(hookName, fn)</c> — subscribe to <see cref="gregEventDispatcher"/> events</item>
///   <item><c>greg.off(hookName)</c> — unsubscribe all handlers for a hook</item>
///   <item><c>greg.hook.before(hookName, fn)</c> — Harmony prefix via <see cref="HookBinder"/></item>
///   <item><c>greg.hook.after(hookName, fn)</c> — Harmony postfix via <see cref="HookBinder"/></item>
///   <item><c>greg.emit(hookName, payload)</c> — emit custom event</item>
/// </list>
/// </summary>
public sealed class gregHooksLuaModule : iGregLuaModule
{
    private MelonLogger.Instance _logger;

    public gregHooksLuaModule(MelonLogger.Instance logger = null)
    {
        _logger = logger ?? new MelonLogger.Instance("gregCore");
    }

    public void Register(Script vm, Table greg)
    {
        var registeredEventHandlers = new List<(string hookName, Action<object> handler)>();
        var registeredHookHandlers = new List<(string hookName, Action<HookContext> handler, bool isBefore)>();

        // greg.on(hookName, fn)
        greg["on"] = (Action<string, Closure>)((hookName, fn) =>
        {
            if (string.IsNullOrWhiteSpace(hookName) || fn == null) return;
            Action<object> handler = payload =>
            {
                try { vm.Call(fn, PayloadToLua(vm, payload)); }
                catch (Exception ex) { CrashLog.LogException($"lua:greg.on[{hookName}]", ex); }
            };
            registeredEventHandlers.Add((hookName, handler));
            gregEventDispatcher.On(hookName, handler);
        });

        // greg.events sub-table
        var eventsTable = new Table(vm);
        greg["events"] = eventsTable;
        
        var updateHandlers = new List<Closure>();
        var guiHandlers = new List<Closure>();

        eventsTable["on"] = (Action<string, Closure, string>)((hookName, fn, modId) =>
        {
            if (string.IsNullOrWhiteSpace(hookName) || fn == null) return;
            Action<object> handler = payload =>
            {
                try { vm.Call(fn, PayloadToLua(vm, payload)); }
                catch (Exception ex) { CrashLog.LogException($"lua:greg.events.on[{hookName}]", ex); }
            };
            registeredEventHandlers.Add((hookName, handler));
            gregEventDispatcher.On(hookName, handler, modId);
        });

        eventsTable["on_update"] = (Action<Closure>)(fn => { if (fn != null) updateHandlers.Add(fn); });
        eventsTable["on_gui"] = (Action<Closure>)(fn => { if (fn != null) guiHandlers.Add(fn); });
        eventsTable["off"] = greg["off"];

        // Expose handlers to the bridge for execution
        greg["_internal_update"] = (Action<double>)(dt => {
            for (int i = 0; i < updateHandlers.Count; i++) {
                try { vm.Call(updateHandlers[i], dt); } catch { }
            }
        });
        greg["_internal_gui"] = (Action)(() => {
            for (int i = 0; i < guiHandlers.Count; i++) {
                try { vm.Call(guiHandlers[i]); } catch { }
            }
        });

        // greg.registry sub-table
        var registryTable = new Table(vm);
        greg["registry"] = registryTable;
        registryTable["registerMod"] = (Action<Table>)(meta =>
        {
            // Stub for mod registration
            string id = meta["id"]?.ToString() ?? "unknown";
            _logger.Msg($"[lua] Registered mod: {id}");
        });

        // greg.off(hookName) — removes all Lua handlers for this hook
        greg["off"] = (Action<string>)(hookName =>
        {
            if (string.IsNullOrWhiteSpace(hookName)) return;
            for (int i = registeredEventHandlers.Count - 1; i >= 0; i--)
            {
                var (name, handler) = registeredEventHandlers[i];
                if (string.Equals(name, hookName, StringComparison.Ordinal))
                {
                    gregEventDispatcher.Off(hookName, handler);
                    registeredEventHandlers.RemoveAt(i);
                }
            }
        });

        // greg.payload sub-table
        var payloadTable = new Table(vm);
        greg["payload"] = payloadTable;
        payloadTable["get"] = (Func<object, string, object, object>)((p, field, fallback) =>
        {
            return gregPayload.Get<object>(p, field, fallback);
        });

        // greg.framework sub-table
        var frameworkTable = new MoonSharp.Interpreter.Table(vm);
        greg["framework"] = frameworkTable;
        frameworkTable["publish_tick"] = (Action<MoonSharp.Interpreter.Table>)(tick =>
        {
            float dt = tick["deltaTime"] is double d ? (float)d : 0f;
            int frame = tick["frame"] is double f ? (int)f : 0;
            global::greg.Exporter.ModFramework.Events.Publish(new global::greg.Exporter.ModTickEvent(dt, frame));
        });

        // greg.emit(hookName, payload)
        greg["emit"] = (Action<string, DynValue>)((hookName, payload) =>
        {
            if (string.IsNullOrWhiteSpace(hookName)) return;
            object nativePayload = payload?.Type == DataType.Table ? LuaTableToDict(payload.Table) : payload?.ToObject();
            gregEventDispatcher.Emit(hookName, nativePayload);
        });

        // greg.hook sub-table
        var hook = new Table(vm);
        greg["hook"] = hook;

        // greg.hook.before(hookName, fn)
        hook["before"] = (Action<string, Closure>)((hookName, fn) =>
        {
            if (string.IsNullOrWhiteSpace(hookName) || fn == null) return;
            Action<HookContext> handler = ctx =>
            {
                try
                {
                    var ctxTable = HookContextToLua(vm, ctx);
                    vm.Call(fn, ctxTable);
                }
                catch (Exception ex) { CrashLog.LogException($"lua:greg.hook.before[{hookName}]", ex); }
            };
            registeredHookHandlers.Add((hookName, handler, true));
            HookBinder.OnBefore(hookName, handler);
        });

        // greg.hook.after(hookName, fn)
        hook["after"] = (Action<string, Closure>)((hookName, fn) =>
        {
            if (string.IsNullOrWhiteSpace(hookName) || fn == null) return;
            Action<HookContext> handler = ctx =>
            {
                try
                {
                    var ctxTable = HookContextToLua(vm, ctx);
                    vm.Call(fn, ctxTable);
                }
                catch (Exception ex) { CrashLog.LogException($"lua:greg.hook.after[{hookName}]", ex); }
            };
            registeredHookHandlers.Add((hookName, handler, false));
            HookBinder.OnAfter(hookName, handler);
        });

        // greg.hook.off(hookName)
        hook["off"] = (Action<string>)(hookName =>
        {
            if (string.IsNullOrWhiteSpace(hookName)) return;
            HookBinder.Unregister(hookName);
            registeredHookHandlers.RemoveAll(x => string.Equals(x.hookName, hookName, StringComparison.Ordinal));
        });
    }

    private static DynValue PayloadToLua(Script vm, object payload)
    {
        if (payload == null)
            return DynValue.Nil;

        if (payload is string s)
            return DynValue.NewString(s);

        if (payload is int i)
            return DynValue.NewNumber(i);

        if (payload is double d)
            return DynValue.NewNumber(d);

        if (payload is float f)
            return DynValue.NewNumber(f);

        if (payload is bool b)
            return DynValue.NewBoolean(b);

        // Anonymous types / rich payloads: flatten public properties to Lua table
        var t = new Table(vm);
        try
        {
            foreach (var prop in payload.GetType().GetProperties())
            {
                try
                {
                    var val = prop.GetValue(payload);
                    if (val == null) continue;
                    if (val is string sv) t[prop.Name] = sv;
                    else if (val is int iv) t[prop.Name] = iv;
                    else if (val is double dv) t[prop.Name] = dv;
                    else if (val is float fv) t[prop.Name] = (double)fv;
                    else if (val is bool bv) t[prop.Name] = bv;
                    else if (val is uint uv) t[prop.Name] = (double)uv;
                    else t[prop.Name] = val.ToString();
                }
                catch { }
            }
        }
        catch { }

        return DynValue.NewTable(t);
    }

    private static Table HookContextToLua(Script vm, HookContext ctx)
    {
        var t = new Table(vm);
        t["hook_name"] = ctx.HookName;
        t["method_name"] = ctx.Method?.Name ?? "";
        t["type_name"] = ctx.Method?.DeclaringType?.FullName ?? "";
        t["has_instance"] = ctx.Instance != null;
        t["arg_count"] = ctx.Arguments?.Length ?? 0;
        if (ctx.Instance != null)
        {
            // Store a handle for the instance so Lua can use greg.unity on it
            t["instance_handle"] = gregLuaObjectHandleRegistry.Register(ctx.Instance);
        }
        if (ctx.Arguments != null)
        {
            for (int i = 0; i < ctx.Arguments.Length; i++)
            {
                if (ctx.Arguments[i] != null)
                    t[$"arg{i}_handle"] = gregLuaObjectHandleRegistry.Register(ctx.Arguments[i]);
            }
        }
        return t;
    }

    private static Dictionary<string, object> LuaTableToDict(Table table)
    {
        var dict = new Dictionary<string, object>();
        foreach (var pair in table.Pairs)
        {
            string key = pair.Key.Type == DataType.String ? pair.Key.String : pair.Key.ToString();
            dict[key] = pair.Value.ToObject();
        }
        return dict;
    }
}






