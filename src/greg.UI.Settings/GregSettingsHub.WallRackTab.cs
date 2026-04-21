using System;
using gregCore.API;
using greg.Logging;

namespace greg.WallRack
{
    public static class GregSettingsHubWallRackTab
    {
        private static readonly GregModLogger _log = new GregModLogger("WallRack");

        public static void Register()
        {
            /*
            GregSettingsHub.RegisterTab("greg.WallRack.settings", "WallRack", b => {
                if (!frameworkSdk.GregFeatureGuard.IsEnabled("WallRack"))
                {
                    b.AddBanner("⚠ Vanilla Save detected -- Wall Rack features disabled", GregUITheme.Error);
                }

                b.AddSection("General")
                 .AddToggle("Wall Rack Placement Active", true, v => { })
                 .AddToggle("Show Wall Grid Overlay", true, v => GregWallPlacementController.Instance.showGridOverlay = v)
                 .AddToggle("Show Slot Labels", false, v => GregWallPlacementController.Instance.showSlotLabels = v);

                b.AddSection("Grid Size")
                 .AddSlider("Default Wall Columns", 4, 1, 16, v => { })
                 .AddSlider("Default Wall Rows", 3, 1, 12, v => { })
                 .AddLabel("[Info] Grid size applies to newly purchased walls");

                b.AddSection("Keybinds")
                 .AddLabel("Wall Build Mode:    W (in Build Mode)")
                 .AddLabel("Interact / Swap:    E")
                 .AddLabel("Undo:               CTRL+Z")
                 .AddLabel("Redo:               CTRL+Y")
                 .AddLabel("Close Context Menu: ESC");

                b.AddSection("Undo / Redo")
                 .AddLabel($"Undo Stack: {GregWallUndoRedoService.Instance.UndoCount} actions")
                 .AddButton("Clear Undo History", () => GregWallUndoRedoService.Instance.Clear());

                b.AddSection("Statistics")
                 .AddLabel("Registered Walls: 0")
                 .AddLabel("Mounted Devices:  0")
                 .AddLabel("Customer Devices: 0");

                b.AddSection("Debug")
                 .AddToggle("Debug Grid Overlay (alle Wände)", false, v => { })
                 .AddButton("Dump Wall Registry to Log", () => {
                     _log.Debug("Dump Wall Registry...");
                 });
            });
            */
            _log.Msg("F8 WallRack Tab registered.");
        }
    }
}
