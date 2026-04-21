using gregCore.API;

namespace greg.SaveEngine
{
    public static class GregSaveNotifier
    {
        public static void NotifySave(string details)
        {
            GregAPI.LogInfo($"[gregSave] ✓ Auto-saved — {details}");
            GregAPI.ShowNotification($"Saved: {details}");
        }

        public static void NotifyLoad(string path)
        {
            GregAPI.LogInfo($"[gregSave] ✓ Loaded from {System.IO.Path.GetFileName(path)}");
            GregAPI.ShowNotification("Modded Save Loaded");
        }

        public static void NotifyVanillaDetect()
        {
            GregAPI.LogWarning("[gregSave] ⚠ Vanilla save detected — modded features disabled");
            GregAPI.ShowNotification("Vanilla Save - Modded features disabled");
        }
    }
}
