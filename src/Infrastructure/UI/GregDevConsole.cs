/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        In-Game DevConsole Overlay mittels Unity IMGUI.
/// Maintainer:   Läuft im Unity Main Thread. Nutzt UnityEngine.GUI & GUILayout.
/// </file-summary>

using UnityEngine;
using System.Collections.Generic;

namespace gregCore.Infrastructure.UI;

internal sealed class GregDevConsole
{
    private static GregDevConsole? _instance;
    public static GregDevConsole Instance => _instance ??= new GregDevConsole();

    public bool IsOpen { get; private set; }
    private string _input = "";
    private readonly List<string> _logs = new();
    private readonly Dictionary<string, Action<string[]>> _commands = new();
    private Vector2 _scroll;

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
        
        var mgm = global::Il2Cpp.MainGameManager.instance;

        if (IsOpen)
        {
            _input = "";
            if (mgm != null) mgm.isPauseMenuDisallowed = true;
            
            global::UnityEngine.Cursor.visible = true;
            global::UnityEngine.Cursor.lockState = global::UnityEngine.CursorLockMode.None;
        }
        else
        {
            if (mgm != null) mgm.isPauseMenuDisallowed = false;
            
            global::UnityEngine.Cursor.visible = false;
            global::UnityEngine.Cursor.lockState = global::UnityEngine.CursorLockMode.Locked;
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

        float width = Screen.width * 0.8f;
        float height = Screen.height * 0.4f;
        float x = (Screen.width - width) / 2;

        GUILayout.BeginArea(new Rect(x, 10, width, height), GUI.skin.box);
        
        GUILayout.Label("<b>gregCore DevConsole</b>");
        
        _scroll = GUILayout.BeginScrollView(_scroll);
        foreach (var log in _logs)
        {
            GUILayout.Label(log);
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("GregConsoleInput");
        Rect inputRect = GUILayoutUtility.GetRect(200, 20, GUILayout.ExpandWidth(true));
        _input = GUI.TextField(inputRect, _input);
        
        if (GUILayout.Button("Run", GUILayout.Width(80)) || 
            (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "GregConsoleInput"))
        {
            if (!string.IsNullOrWhiteSpace(_input))
            {
                Execute(_input);
                _input = "";
            }
            GUI.FocusControl("GregConsoleInput");
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        if (GUI.GetNameOfFocusedControl() != "GregConsoleInput")
        {
            GUI.FocusControl("GregConsoleInput");
        }
    }

    private void Execute(string input)
    {
        Log($"> {input}");
        var parts = input.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        var cmd = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1..] : System.Array.Empty<string>();

        if (_commands.TryGetValue(cmd, out var action))
        {
            try { action(args); }
            catch (System.Exception ex) { Log($"<color=red>Fehler: {ex.Message}</color>"); }
        }
        else
        {
            Log($"<color=yellow>Unbekannter Befehl: {cmd}</color>");
        }
    }
}
