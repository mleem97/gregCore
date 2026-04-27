/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        UI-Overlay für Lua-Fehler.
/// Maintainer:   Zeigt Fehler mit Stacktrace, Auto-Hide nach 10 Sekunden.
///               IMGUI-basiert, semi-transparent.
/// </file-summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Dev;

public sealed class LuaErrorOverlay
{
    private readonly List<ErrorEntry> _errors = new();
    private readonly int _maxErrors = 5;
    private readonly float _autoHideDuration = 10f;

    /// <summary>
    /// Fügt einen neuen Fehler hinzu.
    /// </summary>
    public void ReportError(string modId, string message, string? stackTrace = null)
    {
        _errors.Add(new ErrorEntry
        {
            ModId = modId,
            Message = message,
            StackTrace = stackTrace ?? "",
            Timestamp = Time.realtimeSinceStartup,
            Dismissed = false
        });

        // Keep only the most recent errors
        while (_errors.Count > _maxErrors)
            _errors.RemoveAt(0);
    }

    /// <summary>
    /// IMGUI-Rendering. Muss aus OnGUI aufgerufen werden.
    /// </summary>
    public void OnGUI()
    {
        float currentTime = Time.realtimeSinceStartup;

        // Remove expired errors
        _errors.RemoveAll(e => e.Dismissed || (currentTime - e.Timestamp) > _autoHideDuration);

        if (_errors.Count == 0) return;

        float y = Screen.height - 20f;

        for (int i = _errors.Count - 1; i >= 0; i--)
        {
            var error = _errors[i];
            float elapsed = currentTime - error.Timestamp;
            float alpha = Mathf.Clamp01(1f - (elapsed / _autoHideDuration) * 0.5f);

            float boxHeight = string.IsNullOrEmpty(error.StackTrace) ? 50f : 80f;
            y -= boxHeight + 5f;

            // Semi-transparent background
            var bgColor = new Color(0.15f, 0.05f, 0.05f, alpha * 0.9f);
            GUI.color = new Color(1f, 1f, 1f, alpha);

            var boxRect = new Rect(20f, y, 500f, boxHeight);
            GUI.Box(boxRect, "");

            // Error text
            var oldColorTitle = GUI.contentColor;
            GUI.contentColor = new Color(1f, 0.32f, 0.32f, alpha);
            GUI.Label(new Rect(25f, y + 2f, 440f, 20f), new GUIContent($"[{error.ModId}] {error.Message}"));
            GUI.contentColor = oldColorTitle;

            if (!string.IsNullOrEmpty(error.StackTrace))
            {
                var oldColorStack = GUI.contentColor;
                GUI.contentColor = new Color(0.7f, 0.7f, 0.7f, alpha);
                GUI.Label(new Rect(25f, y + 22f, 440f, 45f), new GUIContent(error.StackTrace));
                GUI.contentColor = oldColorStack;
            }

            // Dismiss button
            if (GUI.Button(new Rect(470f, y + 5f, 40f, 20f), "✕"))
            {
                error.Dismissed = true;
            }

            // Time remaining indicator
            float remaining = _autoHideDuration - elapsed;
            var oldColorTime = GUI.contentColor;
            GUI.contentColor = new Color(0.5f, 0.5f, 0.5f, alpha);
            GUI.Label(new Rect(25f, y + boxHeight - 15f, 480f, 15f), new GUIContent($"{remaining:F0}s"));
            GUI.contentColor = oldColorTime;
        }

        GUI.color = Color.white; // Reset
    }

    /// <summary>
    /// Entfernt alle sichtbaren Fehler.
    /// </summary>
    public void DismissAll()
    {
        _errors.Clear();
    }

    public int VisibleErrorCount => _errors.Count;

    // ─── Inner Types ─────────────────────────────────────────────────

    private class ErrorEntry
    {
        public string ModId = "";
        public string Message = "";
        public string StackTrace = "";
        public float Timestamp;
        public bool Dismissed;
    }
}
