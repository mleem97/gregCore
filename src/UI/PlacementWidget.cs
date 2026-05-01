using UnityEngine;
using gregCore.UI;

namespace greg.Furniture
{
    public static class PlacementWidget
    {
        private static GregUIBuilder? _uiBuilder;

        public static void Initialize()
        {
            _uiBuilder = GregUIBuilder.CreateWidget("Placement Engine v2", 30, Screen.height - 180)
                .SetSize(320, 150)
                .AddHeadline("Build Mode")
                .AddLabel("Grid: 0.5m Precision")
                .AddLabel("L-CLICK: Place Object")
                .AddLabel("R-CLICK: Rotate | ESC: Cancel")
                .AddSpacer(5)
                .AddSwitch("Show Snap Points", true, v => { })
                .Build();
                
            _uiBuilder.IsVisible = false;
        }

        public static void SetVisible(bool visible)
        {
            if (_uiBuilder == null) Initialize();
            if (_uiBuilder != null) _uiBuilder.IsVisible = visible;
        }
    }
}
