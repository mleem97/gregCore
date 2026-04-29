/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        In-Game Lua REPL für Live-Debugging.
/// Maintainer:   Öffnet/Schließt via Keybind (F12). UI Toolkit-basiert.
///               Evaluiert Lua-Expressions gegen einen persistenten Script-Context.
/// </file-summary>

using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;
using gregCore.UI;

namespace gregCore.Infrastructure.Scripting.Lua.Dev;

public sealed class LuaRepl
{
    private bool _visible;
    private string _input = "";
    private readonly List<string> _history = new();
    private readonly List<string> _output = new();
    private int _historyIndex = -1;
    private ScrollView _outputScroll;
    private TextField _inputField;
    private VisualElement _root;
    private readonly int _maxOutputLines = 200;
    private Script? _replScript;

    /// <summary>
    /// Erstellt den REPL mit einem frischen MoonSharp-Script-Context.
    /// </summary>
    public void Initialize()
    {
        try
        {
            var replScript = new Script(CoreModules.Preset_SoftSandbox);

            // Register greg API in REPL context
            var gregTable = new Table(replScript);

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

            replScript.Globals["greg"] = gregTable;

            // Helper: print redirects to output
            replScript.Globals["print"] = (Action<DynValue[]>)(args =>
            {
                string line = string.Join("\t", Array.ConvertAll(args, a => a.ToPrintString()));
                AddOutput(line);
            });

            AddOutput("─── gregCore Lua REPL v1.1.0 ───");
            AddOutput("Type Lua expressions. Press Enter to evaluate.");
            AddOutput("Use greg.* for API access. Type 'help()' for commands.");

            replScript.Globals["help"] = (Action)(() =>
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

            replScript.Globals["clear"] = (Action)(() => _output.Clear());

            // Build UI
            BuildUI(replScript);
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
        if (_root != null)
        {
            _root.style.display = _visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public void Update()
    {
        if (_root == null || _root.style.display == DisplayStyle.None) return;
        if (_inputField == null) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) && _history.Count > 0)
        {
            _historyIndex = Math.Max(0, _historyIndex - 1);
            if (_historyIndex < _history.Count)
            {
                _input = _history[_historyIndex];
                _inputField.value = _input;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && _history.Count > 0)
        {
            _historyIndex = Math.Min(_history.Count, _historyIndex + 1);
            _input = _historyIndex < _history.Count ? _history[_historyIndex] : "";
            _inputField.value = _input;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrWhiteSpace(_input) && _replScript != null)
            {
                Evaluate(_input, _replScript);
                _history.Add(_input);
                _historyIndex = _history.Count;
                _input = "";
                _inputField.value = "";
            }
        }
    }

    private void BuildUI(Script replScript)
    {
        _replScript = replScript;
        _root = new VisualElement
        {
            name = "LuaREPL",
            style =
            {
                position = Position.Absolute,
                top = 20,
                right = 20,
                width = 600,
                height = 400,
                backgroundColor = new Color(0.07f, 0.07f, 0.07f, 0.96f),
                borderTopColor = new Color(0.07f, 0.75f, 0.65f),
                borderBottomColor = new Color(0.07f, 0.75f, 0.65f),
                borderLeftColor = new Color(0.07f, 0.75f, 0.65f),
                borderRightColor = new Color(0.07f, 0.75f, 0.65f),
                borderTopWidth = 2,
                borderBottomWidth = 2,
                borderLeftWidth = 2,
                borderRightWidth = 2,
                borderTopLeftRadius = 8,
                borderTopRightRadius = 8,
                borderBottomLeftRadius = 8,
                borderBottomRightRadius = 8,
                flexDirection = FlexDirection.Column,
                paddingTop = 10,
                paddingBottom = 10,
                paddingLeft = 10,
                paddingRight = 10,
                display = DisplayStyle.None
            }
        };

        // Header
        var header = new Label("gregCore Lua REPL")
        {
            style =
            {
                fontSize = 18,
                unityFontStyleAndWeight = FontStyle.Bold,
                color = new Color(0f, 0.75f, 0.65f),
                unityTextAlign = TextAnchor.MiddleLeft,
                marginBottom = 8,
                borderBottomColor = new Color(0.2f, 0.2f, 0.2f),
                borderBottomWidth = 1,
                paddingBottom = 6
            }
        };
        _root.Add(header);

        // Output scroll view
        _outputScroll = new ScrollView(ScrollViewMode.Vertical)
        {
            style =
            {
                flexGrow = 1,
                marginTop = 4,
                marginBottom = 8,
                backgroundColor = new Color(0.05f, 0.05f, 0.05f, 0.8f),
                borderTopColor = new Color(0.15f, 0.15f, 0.15f),
                borderBottomColor = new Color(0.15f, 0.15f, 0.15f),
                borderLeftColor = new Color(0.15f, 0.15f, 0.15f),
                borderRightColor = new Color(0.15f, 0.15f, 0.15f),
                borderTopWidth = 1,
                borderBottomWidth = 1,
                borderLeftWidth = 1,
                borderRightWidth = 1,
                borderTopLeftRadius = 4,
                borderTopRightRadius = 4,
                borderBottomLeftRadius = 4,
                borderBottomRightRadius = 4,
                paddingTop = 6,
                paddingBottom = 6,
                paddingLeft = 6,
                paddingRight = 6
            }
        };
        _root.Add(_outputScroll);

        // Input row
        var inputRow = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center
            }
        };

        var prompt = new Label("›")
        {
            style =
            {
                fontSize = 14,
                color = new Color(0.6f, 0.6f, 0.6f),
                marginRight = 4,
                width = 15
            }
        };
        inputRow.Add(prompt);

        _inputField = new TextField
        {
            value = _input,
            style =
            {
                flexGrow = 1,
                fontSize = 14,
                backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                color = Color.white,
                height = 24
            }
        };
        _inputField.RegisterValueChangedCallback<string>(evt => _input = evt.newValue);
        inputRow.Add(_inputField);

        var runButton = new Button(() =>
        {
            if (!string.IsNullOrWhiteSpace(_input))
            {
                Evaluate(_input, replScript);
                _history.Add(_input);
                _historyIndex = _history.Count;
                _input = "";
                _inputField.value = "";
            }
        })
        {
            text = "Run",
            style =
            {
                width = 50,
                height = 24,
                backgroundColor = new Color(0f, 0.75f, 0.65f),
                color = Color.black,
                unityFontStyleAndWeight = FontStyle.Bold,
                marginLeft = 4
            }
        };
        inputRow.Add(runButton);

        _root.Add(inputRow);

        // Register in UI manager
        GregUIManager.RegisterPanel("LuaREPL", _root);
    }

    private void Evaluate(string code, Script replScript)
    {
        AddOutput($"<color=#888888>› {code}</color>");

        try
        {
            var result = replScript.DoString(code);
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

        if (_outputScroll != null)
        {
            var label = new Label(line)
            {
                style =
                {
                    fontSize = 12,
                    color = GetLineColor(line),
                    whiteSpace = WhiteSpace.Normal,
                    marginBottom = 2
                }
            };
            _outputScroll.Add(label);
            _outputScroll.ScrollTo(label);
        }
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
