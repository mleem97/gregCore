/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     In-game Lua REPL for live debugging.
/// Maintainer:   Opens/closes via keybind (F12). UI Toolkit-based.
///               Evaluates Lua expressions against a persistent script context.
/// </file-summary>

using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using gregCore.UI;

namespace gregCore.Infrastructure.Scripting.Lua.Dev;

public sealed partial class LuaRepl
{
    private bool _visible;
    private string _input = "";
    private readonly List<string> _history = new();
    private readonly List<string> _output = new();
    private int _historyIndex = -1;
    private ScrollView _outputScroll = null!;
    private TextField _inputField = null!;
    private VisualElement _root = null!;

    private readonly int _maxOutputLines = 200;
    private Script? _replScript;

    /// <summary>
    /// Creates the REPL with a fresh MoonSharp script context.
    /// </summary>
    public void Initialize()
    {
        try
        {
            var replScript = new Script(CoreModules.Preset_SoftSandbox);
            var gregTable = new Table(replScript);
            RegisterLoggingApi(gregTable);
            RegisterEconomyApi(gregTable);
            RegisterWorldApi(gregTable);
            replScript.Globals["greg"] = gregTable;
            RegisterPrintHelper(replScript);
            PrintWelcome();
            RegisterHelpCommand(replScript);
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
    /// Toggles visibility (called from OnUpdate on F12).
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
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        try
        {
            if (TryHistoryUp(keyboard)) return;
            if (TryHistoryDown(keyboard)) return;
            TrySubmitEnter(keyboard);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaREPL] Update failed: {ex.Message}");
        }
    }

    private bool TryHistoryUp(Keyboard keyboard)
    {
        try
        {
            if (!keyboard.upArrowKey.wasPressedThisFrame || _history.Count == 0) return false;
            _historyIndex = Math.Max(0, _historyIndex - 1);
            if (_historyIndex < _history.Count)
            {
                _input = _history[_historyIndex];
                _inputField.value = _input;
            }
            return true;
        }
        catch { return false; }
    }

    private bool TryHistoryDown(Keyboard keyboard)
    {
        try
        {
            if (!keyboard.downArrowKey.wasPressedThisFrame || _history.Count == 0) return false;
            _historyIndex = Math.Min(_history.Count, _historyIndex + 1);
            _input = _historyIndex < _history.Count ? _history[_historyIndex] : "";
            _inputField.value = _input;
            return true;
        }
        catch { return false; }
    }

    private void TrySubmitEnter(Keyboard keyboard)
    {
        try
        {
            if (!keyboard.enterKey.wasPressedThisFrame) return;
            SubmitCurrentInput();
        }
        catch { }
    }

    private void SubmitCurrentInput()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_input) || _replScript == null) return;
            Evaluate(_input, _replScript);
            _history.Add(_input);
            _historyIndex = _history.Count;
            _input = "";
            _inputField.value = "";
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaREPL] Submit failed: {ex.Message}");
        }
    }

    private void BuildUI(Script replScript)
    {
        try
        {
            _replScript = replScript;
            _root = CreateRoot();
            _root.Add(CreateHeader());
            _outputScroll = CreateOutputScroll();
            _root.Add(_outputScroll);
            _root.Add(CreateInputRow(replScript));
            GregUIManager.RegisterPanel("LuaREPL", _root);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaREPL] BuildUI failed: {ex.Message}");
        }
    }

    private static VisualElement CreateRoot()
    {
        return new VisualElement
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
    }

    private static Label CreateHeader()
    {
        return new Label("gregCore Lua REPL")
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
    }

    private static ScrollView CreateOutputScroll()
    {
        return new ScrollView(ScrollViewMode.Vertical)
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
    }

    private VisualElement CreateInputRow(Script replScript)
    {
        var inputRow = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center
            }
        };
        inputRow.Add(CreatePrompt());
        _inputField = CreateInputField();
        inputRow.Add(_inputField);
        inputRow.Add(CreateRunButton(replScript));
        return inputRow;
    }

    private static Label CreatePrompt()
    {
        return new Label("›")
        {
            style =
            {
                fontSize = 14,
                color = new Color(0.6f, 0.6f, 0.6f),
                marginRight = 4,
                width = 15
            }
        };
    }

    private TextField CreateInputField()
    {
        var field = new TextField
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
        field.RegisterCallback<ChangeEvent<string>>(new Action<ChangeEvent<string>>(evt => _input = evt.newValue));
        return field;
    }

    private Button CreateRunButton(Script replScript)
    {
        var runButton = new Button
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
        runButton.RegisterCallback<ClickEvent>(new Action<ClickEvent>(evt => SubmitCurrentInput()));
        return runButton;
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
