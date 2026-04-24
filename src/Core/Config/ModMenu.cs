using System;
using System.Collections.Generic;
using UnityEngine;
using gregCore.UI;

namespace gregCore.Core.Config
{
    /// <summary>
    /// Centralized registry for all mod configurations.
    /// Mods register their settings here, and the SettingsHub generates the UI.
    /// </summary>
    public static class ModMenu
    {
        private static readonly Dictionary<string, ModCategory> _categories = new();
        public static IEnumerable<ModCategory> Categories => _categories.Values;

        public static ModCategory AddCategory(string modName)
        {
            if (!_categories.TryGetValue(modName, out var category))
            {
                category = new ModCategory(modName);
                _categories[modName] = category;
            }
            return category;
        }
    }

    public class ModCategory
    {
        public string Name { get; }
        private readonly List<Action<GregUIBuilder>> _uiBuilders = new();

        public ModCategory(string name) => Name = name;

        public ModCategory CreateToggle(string label, bool defaultValue, Action<bool> onValueChanged)
        {
            _uiBuilders.Add(builder => builder.AddToggle(label, defaultValue, onValueChanged));
            return this;
        }

        public ModCategory CreateSlider(string label, float min, float max, float defaultValue, Action<float> onValueChanged)
        {
            _uiBuilders.Add(builder => builder.AddSlider(label, min, max, defaultValue, onValueChanged));
            return this;
        }

        public ModCategory CreateButton(string label, Action onClick)
        {
            _uiBuilders.Add(builder => builder.AddPrimaryButton(label, onClick));
            return this;
        }

        public void BuildUI(GregUIBuilder builder)
        {
            builder.AddHeadline(Name);
            foreach (var action in _uiBuilders) action(builder);
        }
    }
}
