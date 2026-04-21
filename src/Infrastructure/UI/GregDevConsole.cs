using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace gregCore.Infrastructure.UI;

public sealed class GregDevConsole
{
    private static readonly Lazy<GregDevConsole> _instance = new(() => new GregDevConsole());
    public static GregDevConsole Instance => _instance.Value;

    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    private Rect _windowRect = new Rect(20f, 20f, 600f, 400f);
    private string _inputCommand = "";
    private readonly List<LogEntry> _logs = new();
    private Vector2 _scrollPosition;

    public void Toggle() => _isOpen = !_isOpen;

    public void AddLog(string message, LogType type)
    {
        _logs.Add(new LogEntry { Message = message, Type = type, Time = DateTime.Now });
        if (_logs.Count > 100) _logs.RemoveAt(0);
        _scrollPosition.y = float.MaxValue; 
    }

    public void OnGUI()
    {
        if (!_isOpen) return;
        _windowRect = GUI.Window(1337, _windowRect, (UnityEngine.GUI.WindowFunction)DrawWindow, "gregCore DevConsole");
    }

    private void DrawWindow(int windowId)
    {
        GUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        foreach (var log in _logs)
        {
            GUILayout.Label($"[{log.Time:HH:mm:ss}] {log.Message}");
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        _inputCommand = GUILayout.TextField(_inputCommand);
        if (GUILayout.Button("Send", GUILayout.Width(60f)))
        {
            if (!string.IsNullOrWhiteSpace(_inputCommand))
            {
                AddLog($"> {_inputCommand}", LogType.Log);
                _inputCommand = "";
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    private struct LogEntry
    {
        public string Message;
        public LogType Type;
        public DateTime Time;
    }
}
