using UnityEngine;

namespace greg.Mods.ResetSwitch.UI
{
    // This file is now a wrapper around the new FixedTableUI
    // for backward compatibility with the hotkey.
    public static class ResetPanelUI
    {
        private static FixedTableUI _tableUI;

        public static bool IsOpen => _tableUI != null && _tableUI.IsOpen();

        public static void Toggle()
        {
            if (_tableUI == null)
            {
                var canvas = StandaloneUiFactory.CreateCanvas("greg_ResetSwitchCanvas", 1000200);
                _tableUI = new FixedTableUI(canvas.transform);
            }

            _tableUI.Toggle(!_tableUI.IsOpen());
        }

        public static void UpdateFocus()
        {
            _tableUI?.Update();
        }
    }
}

