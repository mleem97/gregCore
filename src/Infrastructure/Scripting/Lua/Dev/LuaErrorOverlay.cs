/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        UI-Overlay für Lua-Fehler.
/// Maintainer:   Zeigt Fehler mit Stacktrace, Auto-Hide nach 10 Sekunden.
///               UI Toolkit-basiert, semi-transparent.
/// </file-summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;
using gregCore.UI;

namespace gregCore.Infrastructure.Scripting.Lua.Dev;

public sealed class LuaErrorOverlay
{
    private readonly List<ErrorEntry> _errors = new();
    private readonly int _maxErrors = 5;
    private readonly float _autoHideDuration = 10f;
    private VisualElement? _root;

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

        BuildOrUpdateUI();
    }

    private void BuildOrUpdateUI()
    {
        if (_root == null)
        {
            _root = new VisualElement
            {
                name = "ErrorOverlay",
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    right = 0,
                    width = 520,
                    bottom = 0,
                    flexDirection = FlexDirection.Column,
                    justifyContent = Justify.FlexEnd,
                    alignItems = Align.FlexEnd,
                    paddingBottom = 20,
                    paddingRight = 20,
                    backgroundColor = Color.clear
                }
            };

            GregUIManager.RegisterPanel("ErrorOverlay", _root);
        }

        _root.Clear();

        float currentTime = Time.realtimeSinceStartup;
        _errors.RemoveAll(e => e.Dismissed || (currentTime - e.Timestamp) > _autoHideDuration);

        if (_errors.Count == 0) return;

        float yOffset = 0;
        for (int i = _errors.Count - 1; i >= 0; i--)
        {
            var error = _errors[i];
            float elapsed = currentTime - error.Timestamp;
            float alpha = Mathf.Clamp01(1f - (elapsed / _autoHideDuration) * 0.5f);
            float boxHeight = string.IsNullOrEmpty(error.StackTrace) ? 50f : 80f;

            var errorBox = new VisualElement
            {
                style =
                {
                    width = 500,
                    height = boxHeight,
                    backgroundColor = new Color(0.15f, 0.05f, 0.05f, alpha * 0.9f),
                    borderTopColor = new Color(1f, 0.32f, 0.32f, alpha),
                    borderBottomColor = new Color(1f, 0.32f, 0.32f, alpha),
                    borderLeftColor = new Color(1f, 0.32f, 0.32f, alpha),
                    borderRightColor = new Color(1f, 0.32f, 0.32f, alpha),
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderTopLeftRadius = 6,
                    borderTopRightRadius = 6,
                    borderBottomLeftRadius = 6,
                    borderBottomRightRadius = 6,
                    marginBottom = 5,
                    paddingTop = 8,
                    paddingBottom = 8,
                    paddingLeft = 10,
                    paddingRight = 50,
                    position = Position.Relative
                }
            };

            var titleLabel = new Label($"[{error.ModId}] {error.Message}")
            {
                style =
                {
                    fontSize = 14,
                    color = new Color(1f, 0.32f, 0.32f, alpha),
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 4
                }
            };
            errorBox.Add(titleLabel);

            if (!string.IsNullOrEmpty(error.StackTrace))
            {
                var stackLabel = new Label(error.StackTrace)
                {
                    style =
                    {
                        fontSize = 11,
                        color = new Color(0.7f, 0.7f, 0.7f, alpha),
                        whiteSpace = WhiteSpace.Normal
                    }
                };
                errorBox.Add(stackLabel);
            }

            // Dismiss button
            var dismissBtn = new Button(() =>
            {
                error.Dismissed = true;
                BuildOrUpdateUI();
            })
            {
                text = "✕",
                style =
                {
                    position = Position.Absolute,
                    top = 5,
                    right = 5,
                    width = 20,
                    height = 20,
                    fontSize = 12,
                    backgroundColor = Color.clear,
                    color = new Color(0.7f, 0.7f, 0.7f, alpha),
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0
                }
            };
            errorBox.Add(dismissBtn);

            // Timer indicator
            var timerLabel = new Label($"{_autoHideDuration - elapsed:F0}s")
            {
                style =
                {
                    fontSize = 10,
                    color = new Color(0.5f, 0.5f, 0.5f, alpha),
                    position = Position.Absolute,
                    bottom = 2,
                    left = 10
                }
            };
            errorBox.Add(timerLabel);

            _root.Add(errorBox);
        }
    }

    /// <summary>
    /// Entfernt alle sichtbaren Fehler.
    /// </summary>
    public void DismissAll()
    {
        _errors.Clear();
        _root?.Clear();
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
