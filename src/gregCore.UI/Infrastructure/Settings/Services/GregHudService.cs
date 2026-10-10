using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using gregCore.Core.Abstractions;
using gregCore.UI;

namespace gregCore.Infrastructure.Settings.Services
{
    public class GregHudService
    {
        private readonly IGregLogger _logger;
        private readonly GregKeybindRegistry _keybindRegistry;
        private bool _showHud = false;
        private VisualElement? _hudPanel;

        public GregHudService(IGregLogger logger, GregKeybindRegistry keybindRegistry)
        {
            _logger = logger.ForContext("HudService");
            _keybindRegistry = keybindRegistry;
        }

        public void Toggle() 
        {
            _showHud = !_showHud;
            if (_showHud && _hudPanel == null)
            {
                BuildUI();
            }
            GregUIManager.SetPanelActive("HUD", _showHud);
        }

        private void BuildUI()
        {
            _hudPanel = NewHudPanel();
            _hudPanel.Add(NewTitleLabel());

            var conflicts = _keybindRegistry.GetAll().Where(k => k.HasConflict).ToList();
            foreach (var conflict in conflicts)
                _hudPanel.Add(NewConflictLabel(conflict.DisplayName, conflict.CurrentKey));

            GregUIManager.RegisterPanel("HUD", _hudPanel);
        }

        private static VisualElement NewHudPanel()
        {
            return new VisualElement
            {
                name = "HUD",
                style =
                {
                    position = Position.Absolute,
                    top = 10,
                    left = 10,
                    width = 300,
                    backgroundColor = new Color(0.07f, 0.07f, 0.07f, 0.96f),
                    borderTopColor = new Color(1f, 0.32f, 0.32f),
                    borderBottomColor = new Color(1f, 0.32f, 0.32f),
                    borderLeftColor = new Color(1f, 0.32f, 0.32f),
                    borderRightColor = new Color(1f, 0.32f, 0.32f),
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderTopLeftRadius = 6,
                    borderTopRightRadius = 6,
                    borderBottomLeftRadius = 6,
                    borderBottomRightRadius = 6,
                    paddingTop = 8,
                    paddingBottom = 8,
                    paddingLeft = 10,
                    paddingRight = 10,
                    display = DisplayStyle.None
                }
            };
        }

        private static Label NewTitleLabel()
        {
            return new Label("gregCore: keybind conflicts!")
            {
                style =
                {
                    fontSize = 14,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = new Color(1f, 0.32f, 0.32f),
                    marginBottom = 8
                }
            };
        }

        private static Label NewConflictLabel(string displayName, UnityEngine.KeyCode currentKey)
        {
            return new Label($"{displayName} ({currentKey})")
            {
                style =
                {
                    fontSize = 12,
                    color = new Color(0.88f, 0.88f, 0.88f),
                    marginBottom = 4
                }
            };
        }
    }
}
