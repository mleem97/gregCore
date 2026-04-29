using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataCenterModLoader
{
    public partial class ModConfigSystem
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
            catch { }

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
                        borderRadius = 8,
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
            var sidebar = new VisualElement
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
            
            if (_modOrder.Count == 0)
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
            else
            {
                foreach (var modId in _modOrder)
                {
                    var btn = new Button(() => 
                    {
                        _selectedModId = modId;
                        BuildContentArea();
                    })
                    {
                        text = modId.Length > 16 ? modId.Substring(0, 14) + ".." : modId,
                        style =
                        {
                            backgroundColor = modId == _selectedModId ? 
                                new Color(0.2f, 0.2f, 0.2f) : Color.clear,
                            color = Color.white,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            height = 32,
                            marginTop = 3,
                            borderTopWidth = 0,
                            borderBottomWidth = 0,
                            borderLeftWidth = 0,
                            borderRightWidth = 0,
                            unityTextAlign = TextAnchor.MiddleLeft
                        }
                    };
                    sidebar.Add(btn);
                }
            }
            
            _panelRoot?.Add(sidebar);
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
            var entryRow = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginTop = 8,
                    marginBottom = 4
                }
            };
            
            var label = new Label(entry.DisplayName)
            {
                style =
                {
                    fontSize = 14,
                    color = Color.white,
                    width = 200
                }
            };
            entryRow.Add(label);
            
            if (entry.Type == ConfigEntryType.Bool)
            {
                var toggle = new Toggle
                {
                    value = entry.BoolValue,
                    style = { width = 50 }
                };
                toggle.RegisterValueChangedCallback(evt => 
                {
                    entry.BoolValue = evt.newValue;
                    SaveConfig(entry);
                });
                entryRow.Add(toggle);
            }
            else if (entry.Type == ConfigEntryType.Int)
            {
                var slider = new Slider(entry.IntMin, entry.IntMax)
                {
                    value = entry.IntValue,
                    style = { flexGrow = 1 }
                };
                slider.RegisterValueChangedCallback(evt => 
                {
                    entry.IntValue = (int)evt.newValue;
                    SaveConfig(entry);
                });
                entryRow.Add(slider);
            }
            else if (entry.Type == ConfigEntryType.Float)
            {
                var slider = new Slider(entry.FloatMin, entry.FloatMax)
                {
                    value = entry.FloatValue,
                    style = { flexGrow = 1 }
                };
                slider.RegisterValueChangedCallback(evt => 
                {
                    entry.FloatValue = evt.newValue;
                    SaveConfig(entry);
                });
                entryRow.Add(slider);
            }
            
            parent.Add(entryRow);
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
