using MelonLoader;
using UnityEngine.InputSystem;

namespace greg.Mods.ResetSwitch.Config;

public static class ModConfig
{
    private static MelonPreferences_Category _category;

    public static MelonPreferences_Entry<Key> OpenPanelKey;
    public static MelonPreferences_Entry<bool> AutoScanOnOpen;
    public static MelonPreferences_Entry<bool> CreateBackup;
    public static MelonPreferences_Entry<bool> ShowOkSwitches;
    public static MelonPreferences_Entry<int> ConfirmThreshold;

    public static void Init()
    {
        _category = MelonPreferences.CreateCategory("greg.Mods.ResetSwitch");

        OpenPanelKey = _category.CreateEntry("OpenPanelKey", Key.F8, "Open UI Hotkey");
        AutoScanOnOpen = _category.CreateEntry("AutoScanOnOpen", true, "Auto scan when UI opens");
        CreateBackup = _category.CreateEntry("CreateBackup", true, "Create JSON backup before reset");
        ShowOkSwitches = _category.CreateEntry("ShowOkSwitches", false, "Show switches with active flow in list");
        ConfirmThreshold = _category.CreateEntry("ConfirmThreshold", 1, "Confirm dialog appears when resetting at least this many switches");
    }
}
