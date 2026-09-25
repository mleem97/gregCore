/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Auto-generates Lua bindings from game_hooks.json.
/// Maintainer:   Reads the hook definitions and creates greg.hooks.{group}
///               tables with on_{method_name} subscription functions.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.Infrastructure.Scripting.Lua;

public sealed partial class LuaHookBindingGenerator
{
    private readonly GregEventBus _eventBus;
    private readonly string _hooksFilePath;

    /// <summary>
    /// Loaded hook definitions, grouped by group.
    /// </summary>
    private readonly Dictionary<string, List<HookDefinition>> _hooksByGroup = new();

    public LuaHookBindingGenerator(GregEventBus eventBus, string hooksFilePath)
    {
        _eventBus = eventBus;
        _hooksFilePath = hooksFilePath;
    }

    /// <summary>
    /// Loads game_hooks.json and builds the internal registry.
    /// </summary>
    public void LoadHooks()
    {
        try
        {
            if (!File.Exists(_hooksFilePath))
            {
                MelonLogger.Warning($"[LuaHookGen] Hooks file not found: {_hooksFilePath}");
                return;
            }

            string json = File.ReadAllText(_hooksFilePath);
            // Simple JSON array parsing – MoonSharp/MelonLoader doesn't include Newtonsoft
            // We parse the essential fields with a lightweight approach
            var hooks = ParseHooksJson(json);

            foreach (var hook in hooks)
            {
                if (!_hooksByGroup.TryGetValue(hook.Group, out var list))
                {
                    list = new List<HookDefinition>();
                    _hooksByGroup[hook.Group] = list;
                }
                list.Add(hook);
            }

            int totalHooks = _hooksByGroup.Values.Sum(g => g.Count);
            MelonLogger.Msg($"[LuaHookGen] Loaded {totalHooks} hooks in {_hooksByGroup.Count} groups");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaHookGen] Failed to load hooks: {ex.Message}");
        }
    }

    /// <summary>
    /// Registers greg.hooks.{group} tables in the script.
    /// </summary>
    public void RegisterInScript(Script script, Table greg, string modId)
    {
        try
        {
            var hooksTable = new Table(script);
            RegisterAllGroups(script, hooksTable, modId);
            RegisterGroupList(script, hooksTable);
            greg["hooks"] = hooksTable;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaHookGen] Failed to register hooks in script: {ex.Message}");
        }
    }

    /// <summary>
    /// Returns the number of loaded groups.
    /// </summary>
    public int GroupCount => _hooksByGroup.Count;

    /// <summary>
    /// Returns the total number of all hooks.
    /// </summary>
    public int TotalHookCount => _hooksByGroup.Values.Sum(g => g.Count);

    // ─── Helpers ─────────────────────────────────────────────────────

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c) && i > 0 && !char.IsUpper(input[i - 1]))
            {
                result.Append('_');
            }
            result.Append(char.ToLowerInvariant(c));
        }
        return result.ToString();
    }

    /// <summary>
    /// Lightweight JSON array parser for hook definitions.
    /// Extracts Group, ClassName, MethodName, ReturnType fields.
    /// </summary>
    private static List<HookDefinition> ParseHooksJson(string json)
    {
        var hooks = new List<HookDefinition>();
        try
        {
            int pos = 0;
            HookDefinition? current = null;
            while (TryReadPair(json, ref pos, out string key, out string? value, out bool skipped))
            {
                if (skipped) continue;
                current = ApplyHookField(hooks, current, key, value);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaHookGen] Parse failed: {ex.Message}");
        }
        return hooks;
    }

    private static bool TryReadPair(string json, ref int pos, out string key, out string? value, out bool skipped)
    {
        key = "";
        value = null;
        skipped = false;
        try
        {
            if (!TryReadKey(json, ref pos, out key)) return false;
            if (!SkipColon(json, ref pos)) return false;
            SkipWhitespace(json, ref pos);
            if (pos >= json.Length) return false;
            if (json[pos] == '[')
            {
                SkipArray(json, ref pos);
                skipped = true;
                return true;
            }
            value = ReadScalar(json, ref pos);
            return true;
        }
        catch { return false; }
    }

    private static bool TryReadKey(string json, ref int pos, out string key)
    {
        key = "";
        try
        {
            int keyStart = json.IndexOf('"', pos);
            if (keyStart < 0) return false;
            int keyEnd = json.IndexOf('"', keyStart + 1);
            if (keyEnd < 0) return false;
            key = json.Substring(keyStart + 1, keyEnd - keyStart - 1);
            pos = keyEnd + 1;
            return true;
        }
        catch { return false; }
    }

    private static bool SkipColon(string json, ref int pos)
    {
        try
        {
            int colonPos = json.IndexOf(':', pos);
            if (colonPos < 0) return false;
            pos = colonPos + 1;
            return true;
        }
        catch { return false; }
    }

    private static void SkipWhitespace(string json, ref int pos)
    {
        try
        {
            while (pos < json.Length && char.IsWhiteSpace(json[pos])) pos++;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void SkipArray(string json, ref int pos)
    {
        try
        {
            int depth = 1;
            pos++;
            while (pos < json.Length && depth > 0)
            {
                if (json[pos] == '[') depth++;
                else if (json[pos] == ']') depth--;
                pos++;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static string? ReadScalar(string json, ref int pos)
    {
        try
        {
            if (json[pos] == '"')
            {
                int valStart = pos + 1;
                int valEnd = json.IndexOf('"', valStart);
                if (valEnd < 0) return null;
                string v = json.Substring(valStart, valEnd - valStart);
                pos = valEnd + 1;
                return v;
            }
            int start = pos;
            while (pos < json.Length && json[pos] != ',' && json[pos] != '}') pos++;
            return json.Substring(start, pos - start).Trim();
        }
        catch { return null; }
    }

    private static HookDefinition? ApplyHookField(List<HookDefinition> hooks, HookDefinition? current, string key, string? value)
    {
        try
        {
            if (key == "Group") return NewHook(value);
            ApplyNonGroup(hooks, current, key, value);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return current;
    }

    private static HookDefinition NewHook(string? value)
    {
        try
        {
            string g = value;
            if (string.IsNullOrEmpty(g)) g = "Unknown";
            return new HookDefinition { Group = g };
        }
        catch { return new HookDefinition { Group = "Unknown" }; }
    }

    private static void ApplyNonGroup(List<HookDefinition> hooks, HookDefinition? current, string key, string? value)
    {
        try
        {
            if (key == "ClassName") SetClass(current, value);
            else if (key == "MethodName") SetMethod(current, value);
            else if (key == "ReturnType") SetReturn(current, value);
            else if (key == "IsVoid") CompleteHook(hooks, current, value);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void SetClass(HookDefinition? current, string? value)
    {
        try { if (current != null) current.ClassName = Coalesce(value); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void SetMethod(HookDefinition? current, string? value)
    {
        try { if (current != null) current.MethodName = Coalesce(value); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void SetReturn(HookDefinition? current, string? value)
    {
        try { if (current != null) current.ReturnType = Coalesce(value, "Void"); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static string Coalesce(string? value, string fallback = "")
    {
        try { return string.IsNullOrEmpty(value) ? fallback : value; }
        catch { return fallback; }
    }

    private static void CompleteHook(List<HookDefinition> hooks, HookDefinition? current, string? value)
    {
        try
        {
            if (current == null) return;
            current.IsVoid = value?.Trim().ToLowerInvariant() == "true";
            if (current.Group != null && current.MethodName != null)
            {
                hooks.Add(current);
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    // ─── Inner Types ─────────────────────────────────────────────────

    public class HookDefinition
    {
        public string Group { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string MethodName { get; set; } = "";
        public string ReturnType { get; set; } = "Void";
        public bool IsVoid { get; set; } = true;
    }
}
