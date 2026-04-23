using UnityEngine;
using gregCore.UI;

namespace greg.Furniture
{
    public static class PlacementWidget
    {
        public static void Initialize()
        {
            var builder = GregUIBuilder.CreateWidget("PlacementWidget", 20, Screen.height - 150)
                .SetSize(250, 100)
                .AddHeadline("Build Mode")
                .AddLabel("Grid: 0.5m (1sq=4sq)")
                .AddLabel("L-CLICK: Place | ESC: Exit");
            
            var widget = builder.Build();
            widget.SetActive(false);
        }
    }
}
