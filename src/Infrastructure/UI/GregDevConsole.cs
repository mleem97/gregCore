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
    private GameObject _uiPanel;

    public void Toggle() 
    {
        _isOpen = !_isOpen;
        if (_isOpen && _uiPanel == null)
        {
            BuildUI();
        }
        gregCore.UI.GregUIManager.SetPanelActive("DevConsole", _isOpen);
    }

    private void BuildUI()
    {
        var builder = gregCore.UI.GregUIBuilder.Create("DevConsole")
            .SetSize(600, 400);

        _uiPanel = builder.Build();
        // Note: Full log list and scrolling would need more UGUI components like ScrollRect
        // For now, we initialize the panel and we can add labels dynamically
        RefreshLogs();
    }

    private void RefreshLogs()
    {
        if (_uiPanel == null) return;
        // In a real UGUI implementation, we'd update a Text component or instantiate labels in a content container
    }

    public void AddLog(string message, LogType type)
    {
        _logs.Add(new LogEntry { Message = message, Type = type, Time = DateTime.Now });
        if (_logs.Count > 100) _logs.RemoveAt(0);
        _scrollPosition.y = float.MaxValue; 
        RefreshLogs();
    }

    public void OnGUI()
    {
        // IMGUI OnGUI is now disabled to prevent stripping crashes.
        // The UI is handled via BuildUI and UGUI.
    }

    private void DrawWindow(int windowId)
    {
        // Legacy IMGUI method - no longer called
    }

    private struct LogEntry
    {
        public string Message;
        public LogType Type;
        public DateTime Time;
    }
}
