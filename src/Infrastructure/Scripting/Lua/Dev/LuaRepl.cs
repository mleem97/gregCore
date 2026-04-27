/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        In-Game Lua REPL für Live-Debugging.
/// Maintainer:   Öffnet/Schließt via Keybind (F12). IMGUI-basiert.
///               Evaluiert Lua-Expressions gegen einen persistenten Script-Context.
/// </file-summary>

using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MelonLoader;
using UnityEngine;

namespace gregCore.Infrastructure.Scripting.Lua.Dev;

public sealed class LuaRepl
{
    private bool _visible;
    private string _input = "";
    private readonly List<string> _history = new();
    private readonly List<string> _output = new();
    private int _historyIndex = -1;
    private Vector2 _scrollPos;
    private Script? _replScript;
    private readonly int _maxOutputLines = 200;

    /// <summary>
    /// Erstellt den REPL mit einem frischen MoonSharp-Script-Context.
    /// </summary>
    public void Initialize()
    {
        try
        {
            _replScript = new Script(CoreModules.Preset_SoftSandbox);

            // Register greg API in REPL context
            var gregTable = new Table(_replScript);

            // Basic logging
            gregTable["log_info"] = (Action<string>)(msg => AddOutput($"[INFO] {msg}"));
            gregTable["log_warning"] = (Action<string>)(msg => AddOutput($"[WARN] {msg}"));
            gregTable["log_error"] = (Action<string>)(msg => AddOutput($"[ERROR] {msg}"));

            // Economy
            gregTable["get_money"] = (Func<double>)(() => API.GregAPI.GetPlayerMoney());
            gregTable["set_money"] = (Action<double>)(v => API.GregAPI.SetPlayerMoney(v));
            gregTable["get_xp"] = (Func<double>)(() => API.GregAPI.GetPlayerXp());
            gregTable["get_reputation"] = (Func<double>)(() => API.GregAPI.GetPlayerReputation());

            // World
            gregTable["server_count"] = (Func<int>)(() => (int)API.GregAPI.GetServerCount());
            gregTable["rack_count"] = (Func<int>)(() => (int)API.GregAPI.GetRackCount());
            gregTable["time_of_day"] = (Func<double>)(() => API.GregAPI.GetTimeOfDay());
            gregTable["day"] = (Func<int>)(() => (int)API.GregAPI.GetDay());
            gregTable["scene"] = (Func<string>)(() => API.GregAPI.GetCurrentScene());
            gregTable["is_paused"] = (Func<bool>)(() => API.GregAPI.IsGamePaused());
            gregTable["pause"] = (Action)(() => API.GregAPI.SetGamePaused(true));
            gregTable["resume"] = (Action)(() => API.GregAPI.SetGamePaused(false));

            _replScript.Globals["greg"] = gregTable;

            // Helper: print redirects to output
            _replScript.Globals["print"] = (Action<DynValue[]>)(args =>
            {
                string line = string.Join("\t", Array.ConvertAll(args, a => a.ToPrintString()));
                AddOutput(line);
            });

            AddOutput("─── gregCore Lua REPL v1.1.0 ───");
            AddOutput("Type Lua expressions. Press Enter to evaluate.");
            AddOutput("Use greg.* for API access. Type 'help()' for commands.");

            _replScript.Globals["help"] = (Action)(() =>
            {
                AddOutput("── Available commands ──");
                AddOutput("  greg.get_money()       → Player money");
                AddOutput("  greg.set_money(1000)   → Set money");
                AddOutput("  greg.server_count()    → Active servers");
                AddOutput("  greg.rack_count()      → Active racks");
                AddOutput("  greg.time_of_day()     → Current time");
                AddOutput("  greg.day()             → Current day");
                AddOutput("  greg.scene()           → Active scene");
                AddOutput("  greg.pause() / resume()");
                AddOutput("  clear()                → Clear output");
            });

            _replScript.Globals["clear"] = (Action)(() => _output.Clear());

            MelonLogger.Msg("[LuaREPL] Initialized. Press F12 to toggle.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaREPL] Init failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Toggle Sichtbarkeit (aufgerufen aus OnUpdate bei F12).
    /// </summary>
    public void Toggle()
    {
        _visible = !_visible;
    }

    public bool IsVisible => _visible;

    /// <summary>
    /// Muss aus OnUpdate aufgerufen werden für Keybind-Check.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Toggle();
        }
    }

    /// <summary>
    /// IMGUI-Rendering. Muss aus OnGUI aufgerufen werden.
    /// </summary>
    public void OnGUI()
    {
        if (!_visible || _replScript == null) return;

        float width = 600f;
        float height = 400f;
        float x = Screen.width - width - 20f;
        float y = 20f;

        GUI.Box(new Rect(x - 5, y - 5, width + 10, height + 10), "");
        GUILayout.BeginArea(new Rect(x, y, width, height));

        var oldColorHeader = GUI.contentColor;
        GUI.contentColor = new Color(0f, 0.75f, 0.65f);
        GUILayout.Label(new GUIContent("<b>gregCore Lua REPL</b>"));
        GUI.contentColor = oldColorHeader;

        // Output scroll view
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(height - 80));
        foreach (var line in _output)
        {
            var oldColorLine = GUI.contentColor;
            GUI.contentColor = GetLineColor(line);
            GUILayout.Label(new GUIContent(line));
            GUI.contentColor = oldColorLine;
        }
        GUILayout.EndScrollView();

        // Input field
        GUILayout.BeginHorizontal();
        GUILayout.Label("›", GUILayout.Width(15));

        // Handle history navigation
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.UpArrow && _history.Count > 0)
            {
                _historyIndex = Math.Max(0, _historyIndex - 1);
                if (_historyIndex < _history.Count)
                    _input = _history[_historyIndex];
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.DownArrow && _history.Count > 0)
            {
                _historyIndex = Math.Min(_history.Count, _historyIndex + 1);
                _input = _historyIndex < _history.Count ? _history[_historyIndex] : "";
                Event.current.Use();
            }
        }

        GUI.SetNextControlName("LuaREPLInput");
        _input = GUILayout.TextField(_input);

        if ((Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            || GUILayout.Button("Run", GUILayout.Width(50)))
        {
            if (!string.IsNullOrWhiteSpace(_input))
            {
                Evaluate(_input);
                _history.Add(_input);
                _historyIndex = _history.Count;
                _input = "";
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // Auto-focus input
        GUI.FocusControl("LuaREPLInput");
    }

    private void Evaluate(string code)
    {
        AddOutput($"<color=#888888>› {code}</color>");

        try
        {
            var result = _replScript!.DoString(code);
            if (result != null && result.Type != DataType.Void && result.Type != DataType.Nil)
            {
                AddOutput($"<color=#00BFA5>= {result.ToPrintString()}</color>");
            }
        }
        catch (SyntaxErrorException sex)
        {
            AddOutput($"<color=#FF5252>Syntax Error: {sex.Message}</color>");
        }
        catch (ScriptRuntimeException rex)
        {
            AddOutput($"<color=#FF5252>Runtime Error: {rex.Message}</color>");
        }
        catch (Exception ex)
        {
            AddOutput($"<color=#FF5252>Error: {ex.Message}</color>");
        }
    }

    private void AddOutput(string line)
    {
        _output.Add(line);
        while (_output.Count > _maxOutputLines)
            _output.RemoveAt(0);

        // Auto-scroll
        _scrollPos = new Vector2(0, float.MaxValue);
    }

    private static Color GetLineColor(string line)
    {
        if (line.Contains("[ERROR]") || line.Contains("Error:")) return new Color(1f, 0.32f, 0.32f);
        if (line.Contains("[WARN]")) return new Color(1f, 0.76f, 0.03f);
        if (line.StartsWith("›") || line.Contains("color=#888888")) return new Color(0.6f, 0.6f, 0.6f);
        if (line.StartsWith("=") || line.Contains("color=#00BFA5")) return new Color(0f, 0.75f, 0.65f);
        return new Color(0.85f, 0.85f, 0.85f);
    }
}
