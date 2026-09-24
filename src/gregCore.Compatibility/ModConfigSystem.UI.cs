using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataCenterModLoader
{
    public static partial class ModConfigSystem
    {
        // UI Toolkit implementation - replaces IMGUI DrawSettingsChoice(), ShowPanel(), DrawPanel(), InitStyles()
        
        public static void ShowPanel()
        {
            _showPanel = true;
            _scrollOffset = 0f;

            // Disable EventSystem
            try
            {
                var es = UnityEngine.EventSystems.EventSystem.current;
                if (es != null)
                {
                    _disabledEventSystem = es;
                    es.enabled = false;
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            BuildConfigUI();
        }

        private static void BuildConfigUI()
        {
            if (_panelRoot == null)
            {
                _panelRoot = new VisualElement
                {
                    name = "ModConfigPanel",
                    style =
                    {
                        position = Position.Absolute,
                        width = 800,
                        height = 600,
                        backgroundColor = new Color(0.07f, 0.07f, 0.07f, 0.98f),
                        borderTopColor = new Color(0.5f, 0.5f, 0.5f),
                        borderBottomColor = new Color(0.5f, 0.5f, 0.5f),
                        borderLeftColor = new Color(0.5f, 0.5f, 0.5f),
                        borderRightColor = new Color(0.5f, 0.5f, 0.5f),
                        borderTopWidth = 1,
                        borderBottomWidth = 1,
                        borderLeftWidth = 1,
                        borderRightWidth = 1,
                        borderTopLeftRadius = 8,
                        borderTopRightRadius = 8,
                        borderBottomLeftRadius = 8,
                        borderBottomRightRadius = 8,
                        flexDirection = FlexDirection.Row,
                        paddingTop = 16,
                        paddingBottom = 16,
                        paddingLeft = 16,
                        paddingRight = 16
                    }
                };
                
                // Center on screen
                _panelRoot.style.left = (Screen.width - 800) / 2;
                _panelRoot.style.top = (Screen.height - 600) / 2;
                
                BuildSidebar();
                BuildContentArea();
                
                GregUIManager.RegisterPanel("ModConfigPanel", _panelRoot);
            }
            
            _panelRoot.style.display = DisplayStyle.Flex;
        }

        private static void BuildSidebar()
        {
            var sidebar = MakeSidebarRoot();
            if (_modOrder.Count == 0)
                AddNoModsLabel(sidebar);
            else
                AddModButtons(sidebar);
            _panelRoot?.Add(sidebar);
        }

        private static VisualElement MakeSidebarRoot()
        {
            return new VisualElement
            {
                style =
                {
                    width = 150,
                    backgroundColor = new Color(0.12f, 0.12f, 0.12f),
                    borderRightColor = new Color(0.3f, 0.3f, 0.3f),
                    borderRightWidth = 1,
                    marginRight = 8,
                    flexDirection = FlexDirection.Column
                }
            };
        }

        private static void AddNoModsLabel(VisualElement sidebar)
        {
            var noMods = new Label("No mods")
            {
                style =
                {
                    fontSize = 14,
                    color = new Color(0.7f, 0.7f, 0.7f),
                    marginTop = 4,
                    marginLeft = 4
                }
            };
            sidebar.Add(noMods);
        }

        private static void AddModButtons(VisualElement sidebar)
        {
            foreach (var modId in _modOrder)
                sidebar.Add(MakeModButton(modId));
        }

        private static UnityEngine.UIElements.Button MakeModButton(string modId)
        {
            var btn = new UnityEngine.UIElements.Button();
            btn.text = modId.Length > 16 ? modId.Substring(0, 14) + ".." : modId;
            btn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ =>
            {
                _selectedModId = modId;
                BuildContentArea();
            }));
            btn.style.backgroundColor = modId == _selectedModId ?
                new Color(0.2f, 0.2f, 0.2f) : Color.clear;
            btn.style.color = Color.white;
            btn.style.unityFontStyleAndWeight = FontStyle.Bold;
            btn.style.height = 32;
            btn.style.marginTop = 3;
            btn.style.borderTopWidth = 0;
            btn.style.borderBottomWidth = 0;
            btn.style.borderLeftWidth = 0;
            btn.style.borderRightWidth = 0;
            btn.style.unityTextAlign = TextAnchor.MiddleLeft;
            return btn;
        }

        private static void BuildContentArea()
        {
            // Clear existing content area (keep sidebar)
            if (_panelRoot == null) return;
            
            // Remove old content if exists
            var oldContent = _panelRoot.Q<VisualElement>("ContentArea");
            oldContent?.RemoveFromHierarchy();
            
            var content = new VisualElement
            {
                name = "ContentArea",
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Column,
                    paddingLeft = 8
                }
            };
            
            if (string.IsNullOrEmpty(_selectedModId))
            {
                var label = new Label("Select a mod from the list.")
                {
                    style =
                    {
                        fontSize = 14,
                        color = new Color(0.7f, 0.7f, 0.7f),
                        marginTop = 4
                    }
                };
                content.Add(label);
            }
            else
            {
                // Show mod settings
                var mod = GetOrCreateMod(_selectedModId);
                foreach (var key in mod.EntryOrder)
                {
                    if (mod.Entries.TryGetValue(key, out var entry))
                    {
                        BuildConfigEntry(content, entry);
                    }
                }
            }
            
            _panelRoot.Add(content);
        }

        private static void BuildConfigEntry(VisualElement parent, ConfigEntry entry)
        {
            var entryRow = MakeEntryRow();
            entryRow.Add(MakeEntryLabel(entry.DisplayName));
            AddEntryControl(entryRow, entry);
            parent.Add(entryRow);
        }

        private static VisualElement MakeEntryRow()
        {
            return new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginTop = 8,
                    marginBottom = 4
                }
            };
        }

        private static Label MakeEntryLabel(string text)
        {
            return new Label(text)
            {
                style =
                {
                    fontSize = 14,
                    color = Color.white,
                    width = 200
                }
            };
        }

        private static void AddEntryControl(VisualElement entryRow, ConfigEntry entry)
        {
            if (entry.Type == ConfigEntryType.Bool)
                AddBoolToggle(entryRow, entry);
            else if (entry.Type == ConfigEntryType.Int)
                AddIntSlider(entryRow, entry);
            else if (entry.Type == ConfigEntryType.Float)
                AddFloatSlider(entryRow, entry);
        }

        private static void AddBoolToggle(VisualElement entryRow, ConfigEntry entry)
        {
            var toggle = new Toggle
            {
                value = entry.BoolValue,
                style = { width = 50 }
            };
            toggle.RegisterCallback<ChangeEvent<bool>>(new Action<ChangeEvent<bool>>(evt =>
            {
                entry.BoolValue = evt.newValue;
                SaveConfig(entry);
            }));
            entryRow.Add(toggle);
        }

        private static void AddIntSlider(VisualElement entryRow, ConfigEntry entry)
        {
            var slider = new Slider(entry.IntMin, entry.IntMax)
            {
                value = entry.IntValue,
                style = { flexGrow = 1 }
            };
            slider.RegisterCallback<ChangeEvent<float>>(new Action<ChangeEvent<float>>(evt =>
            {
                entry.IntValue = (int)evt.newValue;
                SaveConfig(entry);
            }));
            entryRow.Add(slider);
        }

        private static void AddFloatSlider(VisualElement entryRow, ConfigEntry entry)
        {
            var slider = new Slider(entry.FloatMin, entry.FloatMax)
            {
                value = entry.FloatValue,
                style = { flexGrow = 1 }
            };
            slider.RegisterCallback<ChangeEvent<float>>(new Action<ChangeEvent<float>>(evt =>
            {
                entry.FloatValue = evt.newValue;
                SaveConfig(entry);
            }));
            entryRow.Add(slider);
        }

        private static void SaveConfig(ConfigEntry entry)
        {
            try
            {
                // Save logic here
                CrashLog.Log($"ModConfig: Saved {entry.Key} = {GetValueString(entry)}");
            }
            catch (Exception ex)
            {
                CrashLog.LogException("ModConfigSystem.SaveConfig", ex);
            }
        }

        private static string GetValueString(ConfigEntry entry)
        {
            return entry.Type switch
            {
                ConfigEntryType.Bool => entry.BoolValue.ToString(),
                ConfigEntryType.Int => entry.IntValue.ToString(),
                ConfigEntryType.Float => entry.FloatValue.ToString("F1"),
                _ => ""
            };
        }

        public static void HidePanel()
        {
            _showPanel = false;
            if (_panelRoot != null)
                _panelRoot.style.display = DisplayStyle.None;

            // Re-enable EventSystem
            if (_disabledEventSystem != null)
                _reenableEventSystemCountdown = 2;
        }
    }
}
