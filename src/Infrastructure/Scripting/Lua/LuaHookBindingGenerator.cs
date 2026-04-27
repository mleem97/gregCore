/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Auto-generiert Lua-Bindings aus game_hooks.json.
/// Maintainer:   Liest die Hook-Definitionen und erstellt greg.hooks.{group}
///               Tables mit on_{method_name} Subscription-Funktionen.
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

public sealed class LuaHookBindingGenerator
{
    private readonly GregEventBus _eventBus;
    private readonly string _hooksFilePath;

    /// <summary>
    /// Geladene Hook-Definitionen, gruppiert nach Group.
    /// </summary>
    private readonly Dictionary<string, List<HookDefinition>> _hooksByGroup = new();

    public LuaHookBindingGenerator(GregEventBus eventBus, string hooksFilePath)
    {
        _eventBus = eventBus;
        _hooksFilePath = hooksFilePath;
    }

    /// <summary>
    /// Lädt game_hooks.json und baut die interne Registry.
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
    /// Registriert greg.hooks.{group} Tables im Script.
    /// </summary>
    public void RegisterInScript(Script script, Table greg, string modId)
    {
        try
        {
            var hooksTable = new Table(script);

            foreach (var kvp in _hooksByGroup)
            {
                string groupName = kvp.Key.ToLowerInvariant();
                var groupTable = new Table(script);

                foreach (var hook in kvp.Value)
                {
                    string luaMethodName = "on_" + ToSnakeCase(hook.MethodName);
                    string fullHookId = $"greg.{hook.Group}.{hook.MethodName}";

                    // Create subscription function
                    groupTable[luaMethodName] = (Action<Closure>)(callback =>
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
                    });
                }

                // Also add a list function to discover available hooks
                groupTable["list"] = (Func<Table>)(() =>
                {
                    var list = new Table(script);
                    int i = 1;
                    foreach (var hook in kvp.Value)
                    {
                        list[i++] = "on_" + ToSnakeCase(hook.MethodName);
                    }
                    return list;
                });

                hooksTable[groupName] = groupTable;
            }

            // Add top-level list function
            hooksTable["groups"] = (Func<Table>)(() =>
            {
                var list = new Table(script);
                int i = 1;
                foreach (var group in _hooksByGroup.Keys.OrderBy(k => k))
                {
                    list[i++] = group.ToLowerInvariant();
                }
                return list;
            });

            greg["hooks"] = hooksTable;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaHookGen] Failed to register hooks in script: {ex.Message}");
        }
    }

    /// <summary>
    /// Gibt die Anzahl der geladenen Gruppen zurück.
    /// </summary>
    public int GroupCount => _hooksByGroup.Count;

    /// <summary>
    /// Gibt die Gesamtzahl aller Hooks zurück.
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

        // Remove whitespace and newlines for easier parsing
        int pos = 0;
        HookDefinition? current = null;

        while (pos < json.Length)
        {
            // Find next key-value pair
            int keyStart = json.IndexOf('"', pos);
            if (keyStart < 0) break;

            int keyEnd = json.IndexOf('"', keyStart + 1);
            if (keyEnd < 0) break;

            string key = json.Substring(keyStart + 1, keyEnd - keyStart - 1);
            pos = keyEnd + 1;

            // Skip to value
            int colonPos = json.IndexOf(':', pos);
            if (colonPos < 0) break;
            pos = colonPos + 1;

            // Skip whitespace
            while (pos < json.Length && char.IsWhiteSpace(json[pos])) pos++;

            if (pos >= json.Length) break;

            string? value = null;
            if (json[pos] == '"')
            {
                int valStart = pos + 1;
                int valEnd = json.IndexOf('"', valStart);
                if (valEnd < 0) break;
                value = json.Substring(valStart, valEnd - valStart);
                pos = valEnd + 1;
            }
            else if (json[pos] == '[')
            {
                // Skip array (Parameters)
                int depth = 1;
                pos++;
                while (pos < json.Length && depth > 0)
                {
                    if (json[pos] == '[') depth++;
                    else if (json[pos] == ']') depth--;
                    pos++;
                }
                continue;
            }
            else
            {
                // Boolean or number
                int valStart = pos;
                while (pos < json.Length && json[pos] != ',' && json[pos] != '}') pos++;
                value = json.Substring(valStart, pos - valStart).Trim();
            }

            switch (key)
            {
                case "Group":
                    current = new HookDefinition { Group = value ?? "Unknown" };
                    break;
                case "ClassName":
                    if (current != null) current.ClassName = value ?? "";
                    break;
                case "MethodName":
                    if (current != null) current.MethodName = value ?? "";
                    break;
                case "ReturnType":
                    if (current != null) current.ReturnType = value ?? "Void";
                    break;
                case "IsVoid":
                    if (current != null)
                    {
                        current.IsVoid = value?.Trim().ToLower() == "true";
                        // IsVoid is typically the last relevant field before Parameters
                        if (current.Group != null && current.MethodName != null)
                        {
                            hooks.Add(current);
                        }
                    }
                    break;
            }
        }

        return hooks;
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
