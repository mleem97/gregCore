/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        In-Game DevConsole Overlay mittels Unity IMGUI.
/// Maintainer:   Läuft im Unity Main Thread. Nutzt UnityEngine.GUI.
/// </file-summary>

using UnityEngine;
using System.Collections.Generic;

namespace gregCore.Infrastructure.Performance; // Integration in den Performance-Gedanken (Overlay-Last)

internal sealed class GregDevConsole
{
    private static GregDevConsole? _instance;
    public static GregDevConsole Instance => _instance ??= new GregDevConsole();

    public bool IsOpen { get; private set; }
    private string _input = "";
    private readonly List<string> _logs = new();
    private readonly Dictionary<string, Action<string[]>> _commands = new();
    private Vector2 _scroll;
    private GUIStyle? _boxStyle;

    private GregDevConsole()
    {
        RegisterCommand("help", _ => Log("Verfügbare Befehle: " + string.Join(", ", _commands.Keys)));
        RegisterCommand("clear", _ => _logs.Clear());
        RegisterCommand("exit", _ => Toggle());
        
        Log("gregCore DevConsole initialisiert. Tippe 'help' für Befehle.");
    }

    public void RegisterCommand(string name, Action<string[]> action)
    {
        _commands[name.ToLower()] = action;
    }

    public void Toggle()
    {
        IsOpen = !IsOpen;
        if (IsOpen)
        {
            _input = "";
            // Cursor-Locking wird über Patches/Input blockiert
        }
    }

    public void Log(string message)
    {
        _logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (_logs.Count > 100) _logs.RemoveAt(0);
        _scroll.y = float.MaxValue; 
    }

    public void OnGUI()
    {
        if (!IsOpen) return;

        // Simple IMGUI Style
        if (_boxStyle == null)
        {
            _boxStyle = new GUIStyle(GUI.skin.box);
            _boxStyle.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.8f));
        }

        float width = Screen.width * 0.8f;
        float height = Screen.height * 0.4f;
        float x = (Screen.width - width) / 2;

        GUI.Box(new Rect(x, 10, width, height), "<b>gregCore DevConsole</b>", _boxStyle);
        
        // Logs
        GUILayout.BeginArea(new Rect(x + 10, 40, width - 20, height - 80));
        _scroll = GUILayout.BeginScrollView(_scroll);
        foreach (var log in _logs)
        {
            GUILayout.Label(log);
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        // Input
        GUI.SetNextControlName("GregConsoleInput");
        _input = GUI.TextField(new Rect(x + 10, height - 30, width - 120, 25), _input);
        
        if (GUI.Button(new Rect(x + width - 100, height - 30, 90, 25), "Run") || 
            (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "GregConsoleInput"))
        {
            if (!string.IsNullOrWhiteSpace(_input))
            {
                Execute(_input);
                _input = "";
            }
            GUI.FocusControl("GregConsoleInput");
        }

        GUI.FocusControl("GregConsoleInput");
    }

    private void Execute(string input)
    {
        Log($"> {input}");
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        var cmd = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (_commands.TryGetValue(cmd, out var action))
        {
            try { action(args); }
            catch (Exception ex) { Log($"<color=red>Fehler: {ex.Message}</color>"); }
        }
        else
        {
            Log($"<color=yellow>Unbekannter Befehl: {cmd}</color>");
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i) pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
