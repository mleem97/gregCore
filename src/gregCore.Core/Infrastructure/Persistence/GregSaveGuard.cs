/// <file-summary>
/// Schicht:      Infrastructure (Persistence)
/// Zweck:        Vanillakompatibles Save-System (gregCore Baukasten):
///               1) Vanilla-Backup: Vor jedem Spiel-Save werden <savename>.save
///               + .meta nach ~/GregFrameworkBackups/<save>/<timestamp>/
///               kopiert (max. N behalten). Rollback ohne Mods jederzeit.
///               2) Mod-Sidecars: Mods registrieren Save/Load-Handler; Inhalte
///               landen als greg_<modId>.<save>.tsv NEBEN den Saves. Das Spiel
///               listet nur *.save -> ohne Mods inert = Vanilla-Fallback.
///               3) Vanilla-Fallback: VOR SerializeToBytes und NACH jedem
///               Load werden ungueltige Fremd-/Out-of-Range-Ids in
///               networkData.sfpModules[].prefabID auf einen gueltigen
///               Vanilla-Prefab gemappt (generisch: hoechste Vanilla-Id,
///               oder mod-genau via RegisterVanillaModuleMap()). Dadurch
///               bleibt jedes Save ohne jedes Modding ladbar und ein
///               modded-Modul faellt im Save auf ein normales Modul zurueck.
/// Maintainer:   NurDiese Datei kennt SaveSystem-Details. Hooks defensiv.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HarmonyLib;
using MelonLoader;

namespace gregCore.Infrastructure.Persistence;

[ExcludeFromCodeCoverage(Justification = "File IO + live game save hooks; needs running game.")]
public static class GregSaveGuard
{
    private sealed class Sidecar
    {
        public Func<string> Save;
        public Action<string> Load;
    }

    private static readonly Dictionary<string, Sidecar> _sidecars =
        new Dictionary<string, Sidecar>(StringComparer.OrdinalIgnoreCase);
    private static string _loadedKey;
    private static bool _hooksInstalled;

    private sealed class VanillaMap
    {
        public string ModId;
        public int Fallback;
        public Func<int, int?> ToVanilla;
    }

    private static readonly List<VanillaMap> _vanillaMaps = new List<VanillaMap>();
    private static int _lastKnownVanillaCount = -1;

    /// <summary>
    /// Mods melden ihre prefabID-Bereiche: toVanilla bildet eine Modded-Id
    /// auf die feine Vanilla-Basis ab (rueckgabe null = Id gehoert nicht
    /// diesem Mod). fallbackVanillaPrefabId dient als Mod-eigener Ersatz,
    /// falls die Vanilla-Id-Zahl zur Laufzeit nicht lesbar ist.
    /// </summary>
    public static void RegisterVanillaModuleMap(string modId, Func<int, int?> toVanilla, int fallbackVanillaPrefabId)
    {
        if (string.IsNullOrWhiteSpace(modId) || toVanilla == null) return;
        lock (_vanillaMaps)
        {
            _vanillaMaps.RemoveAll(m => string.Equals(m.ModId, modId, StringComparison.OrdinalIgnoreCase));
            _vanillaMaps.Add(new VanillaMap { ModId = modId, ToVanilla = toVanilla, Fallback = fallbackVanillaPrefabId });
        }
        MelonLogger.Msg($"[gregCore][Save] Vanilla-Modul-Map registriert: {modId}.");
    }

    public static bool BackupEnabled = true;
    public static int MaxBackupsPerSave = 3;

    public static string BackupRoot
    {
        get
        {
            try
            {
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (string.IsNullOrWhiteSpace(home))
                    home = Environment.GetEnvironmentVariable("HOME") ?? ".";
                return Path.Combine(home, "GregFrameworkBackups");
            }
            catch { return null; }
        }
    }

    public static void RegisterSidecar(string modId, Func<string> save, Action<string> load)
    {
        if (string.IsNullOrWhiteSpace(modId) || save == null || load == null) return;
        lock (_sidecars) { _sidecars[modId] = new Sidecar { Save = save, Load = load }; }
        MelonLogger.Msg($"[gregCore][Save] Sidecar registriert: {modId}.");
    }

    public static void Install(HarmonyLib.Harmony harmony)
    {
        if (_hooksInstalled || harmony == null) return;
        var installed = 0;
        try
        {
            // Backup + Sidecars vor dem Spiel-Save
            var saveGame = AccessTools.Method(typeof(global::Il2Cpp.SaveSystem), "SaveGame",
                new Type[] { typeof(string), typeof(string) });
            if (saveGame != null)
            {
                harmony.Patch(saveGame, prefix: new HarmonyMethod(typeof(GregSaveGuard), nameof(SaveGamePrefix)));
                installed++;
            }

            // Vanilla-Fallback (Speichern): exakt am Serialisierungs-Eingang
            var serialize = AccessTools.Method(typeof(global::Il2Cpp.SaveSystem), "SerializeToBytes",
                new Type[] { typeof(global::Il2Cpp.SaveData) });
            if (serialize != null)
            {
                harmony.Patch(serialize, prefix: new HarmonyMethod(typeof(GregSaveGuard), nameof(SaveSerializePrefix)));
                installed++;
            }
            var saveGameData = AccessTools.Method(typeof(global::Il2Cpp.SaveSystem), "SaveGameData");
            if (saveGameData != null)
            {
                harmony.Patch(saveGameData, prefix: new HarmonyMethod(typeof(GregSaveGuard), nameof(SaveGameDataPrefix)));
                installed++;
            }

            // Vanilla-Fallback (Laden): direkt nach der Deserialisierung.
            // LoadGame liefert das Save als Rückgabe (Singleton bleibt im
            // Hauptmenü unangetastet — dort ist SaveData.instance noch nicht
            // konstruierbar und wirft NRE). Die void-Einstiege nur in
            // Spiel-Szenen (Menü lädt nur Vorschaudaten, kein Mod-Content).
            var loadGame = AccessTools.Method(typeof(global::Il2Cpp.SaveSystem), "LoadGame",
                new Type[] { typeof(string) });
            if (loadGame != null)
            {
                harmony.Patch(loadGame, postfix: new HarmonyMethod(typeof(GregSaveGuard), nameof(LoadGamePostfix)));
                installed++;
            }
            foreach (string name in new[] { "LoadFromBytes", "LoadGameData" })
            {
                var target = AccessTools.Method(typeof(global::Il2Cpp.SaveSystem), name);
                if (target == null) continue;
                harmony.Patch(target, postfix: new HarmonyMethod(typeof(GregSaveGuard), nameof(LoadGuardedPostfix)));
                installed++;
            }

            // Save-Inventar: nach dem Healing (Prefix) sind alle Device-IDs
            // final - hier wird alles im Save inventarisiert + mit stabilen
            // UIDs versehen (unsichtbar, Sidecar + Speicher). Extrahiert
            // (Codeline-Limit): nur Hook-Registrierung bleibt hier.
            var loadNetworkState = AccessTools.Method(typeof(global::Il2Cpp.WaypointInitializationSystem),
                "LoadNetworkState");
            if (loadNetworkState != null && PatchLoadNetworkState(harmony, loadNetworkState))
                installed++;

            if (installed == 0)
            {
                MelonLogger.Warning("[gregCore][Save] Keine SaveSystem-Methode gefunden (API-Drift?).");
                return;
            }
            _hooksInstalled = true;
            MelonLogger.Msg($"[gregCore][Save] Save-Hooks installiert ({installed}): Backup, Sidecars, Vanilla-Fallback, Inventar.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Save] Hook-Installation fehlgeschlagen: " + ex.GetBaseException().Message);
        }
    }

    // Prefix VOR dem Spiel-Save: erst Vanilla-Backup, dann Sidecars schreiben.
    public static void SaveGamePrefix(string savename, string stringNameOfSave)
    {
        try
        {
            string dir = null, name = savename;
            try { dir = global::Il2Cpp.SaveSystem.saveDirPath; } catch { dir = null; }
            if (string.IsNullOrWhiteSpace(name))
            {
                try { name = global::Il2Cpp.SaveSystem.loadSaveName; } catch { name = null; }
            }
            if (string.IsNullOrWhiteSpace(dir) || string.IsNullOrWhiteSpace(name)) return;

            gregCore.Infrastructure.Logging.DevLog.Msg("SaveGamePrefix: dir=" + dir + ", save=" + name
                + ", BackupEnabled=" + BackupEnabled + ", Sidecars=" + _sidecars.Count + ".");
            if (BackupEnabled) BackupVanillaSave(dir, name);
            WriteSidecars(dir, name);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[gregCore][Save] SaveGamePrefix fehlgeschlagen: " + ex.Message);
        }
    }

    // Prefix am Serialisierungs-Eingang: modded prefabIDs vor dem Schreiben
    // auf Vanilla zurueckstufen, damit die Datei ohne Mods ladbar bleibt.
    public static void SaveSerializePrefix(global::Il2Cpp.SaveData __0)
    {
        try { SanitizeSaveData(__0, "Speichern"); }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (SerializeToBytes): " + ex.Message); }
    }

    public static void SaveGameDataPrefix()
    {
        try { SanitizeSaveData(global::Il2Cpp.SaveData.instance, "Speichern"); }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (SaveGameData): " + ex.Message); }
    }

    // Postfix nach LoadGame: sanitiert das zurückgegebene Save — der
    // Singleton wird nie berührt (im Hauptmenü noch nicht konstruierbar).
    public static void LoadGamePostfix(global::Il2Cpp.SaveData __result)
    {
        try
        {
            if (__result == null) return;
            SanitizeSaveData(__result, "Laden");
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (Laden): " + ex.Message); }
    }

    // Postfix für void-Lade-Einstiege: nur in Spiel-Szenen. Im Hauptmenü
    // lädt das Spiel nur Vorschaudaten (kein Mod-Content) — und der
    // Singleton-Zugriff wirft dort NRE (still übersprungen, kein Warning).
    public static void LoadGuardedPostfix()
    {
        try
        {
            if (!IsGameplayScene()) return;
            SanitizeSaveData(global::Il2Cpp.SaveData.instance, "Laden");
            gregCore.Infrastructure.Logging.DevLog.Msg("Load-Postfix ausgeführt (SaveData.instance verfügbar).");
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (Laden): " + ex.Message); }
    }

    private static bool IsGameplayScene()
    {
        try
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded) return false;
            return !string.Equals(scene.name, "MainMenu", StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    // Postfix nach WaypointInitializationSystem.LoadNetworkState: Healing-
    // Prefix lief bereits, alle Device-IDs sind final. Hier Inventar bauen.
    public static void LoadNetworkStatePostfix(global::Il2Cpp.NetworkSaveData networkData)
    {
        try
        {
            GregEntityInventory.RebuildFromNetworkData(networkData);
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Inventar (LoadNetworkState): " + ex.Message); }
    }

    private static bool PatchLoadNetworkState(HarmonyLib.Harmony harmony, System.Reflection.MethodInfo loadNetworkState)
    {
        try
        {
            if (harmony == null || loadNetworkState == null) return false;
            harmony.Patch(loadNetworkState,
                postfix: new HarmonyMethod(typeof(GregSaveGuard), nameof(LoadNetworkStatePostfix)));
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Normalisiert networkData.sfpModules[].prefabID. Ungueltige Mod-Ids
    /// werden entweder ueber eine registrierte Mod-Map oder generisch auf die
    /// hoechste gueltige Vanilla-Id zurueckgestuft. Ohne Lesbarkeit der
    /// Vanilla-Prefab-Zahl werden nur explizit registrierte Mod-Ids ersetzt.
    /// </summary>
    internal static void SanitizeSaveData(global::Il2Cpp.SaveData data, string phase)
    {
        if (data == null) return;
        global::Il2Cpp.NetworkSaveData network = null;
        try { network = data.networkData; } catch { return; }
        if (network == null) return;
        var modules = network.sfpModules;
        if (modules == null || modules.Count == 0) return;

        int vanillaCount = CurrentVanillaModuleCount();
        if (vanillaCount > 0) _lastKnownVanillaCount = vanillaCount;
        else if (_lastKnownVanillaCount > 0) vanillaCount = _lastKnownVanillaCount;

        gregCore.Infrastructure.Logging.DevLog.Msg("Sanitize (" + phase + "): " + modules.Count
            + " SFP-Module, vanillaCount=" + vanillaCount + " (gecacht=" + _lastKnownVanillaCount
            + "), Mod-Maps=" + _vanillaMaps.Count + ".");

        int sanitized = 0;
        for (int i = 0; i < modules.Count; i++)
        {
            global::Il2Cpp.SFPSaveData sfd = null;
            try { sfd = modules[i]; } catch { continue; }
            if (sfd == null) continue;
            int id;
            try { id = sfd.prefabID; } catch { continue; }
            if (id < 0) continue;
            if (vanillaCount > 0 && id < vanillaCount) continue; // gueltige Vanilla-Id

            int target = ResolveVanillaTarget(id, vanillaCount);
            if (target < 0 || target == id) continue;
            try { sfd.prefabID = target; } catch { continue; }
            sanitized++;
            MelonLogger.Warning($"[gregCore][Save] SFP-Modul #{i}: prefabID {id} -> {target} ({phase}, Vanilla-Fallback).");
        }
        if (sanitized > 0)
            MelonLogger.Warning($"[gregCore][Save] {phase}: {sanitized} modded SFP-Module auf Vanilla-Module zurueckgestuft.");
    }

    private static int CurrentVanillaModuleCount()
    {
        try
        {
            var mgm = global::Il2Cpp.MainGameManager.instance;
            if (mgm == null) return -1;
            var arr = mgm.sfpPrefabs;
            return arr == null ? -1 : arr.Length;
        }
        catch { return -1; }
    }

    private static int ResolveVanillaTarget(int customId, int vanillaCount)
    {
        lock (_vanillaMaps)
        {
            foreach (var m in _vanillaMaps)
            {
                int? r = null;
                try { r = m.ToVanilla(customId); } catch { }
                if (r.HasValue) return r.Value;
            }
        }
        return vanillaCount > 0 ? vanillaCount - 1 : -1;
    }

    // Nach Szenen-Laden: Sidecars des aktuellen Saves einlesen (idempotent).
    public static void LoadSidecarsForCurrentSave()
    {
        try
        {
            string dir = null, name = null;
            try { dir = global::Il2Cpp.SaveSystem.saveDirPath; } catch { dir = null; }
            try { name = global::Il2Cpp.SaveSystem.loadSaveName; } catch { name = null; }
            if (string.IsNullOrWhiteSpace(dir) || string.IsNullOrWhiteSpace(name)) return;
            string key = dir + "|" + name;
            lock (_sidecars)
            {
                if (string.Equals(_loadedKey, key, StringComparison.Ordinal)) return;
                _loadedKey = key;
                foreach (var kv in _sidecars)
                {
                    string path = SidecarPath(dir, name, kv.Key);
                    string content = null;
                    try { if (File.Exists(path)) content = File.ReadAllText(path); } catch { }
                    if (content == null) continue;
                    try { kv.Value.Load(content); } catch { }
                }
            }
        }
        catch { }
    }

    private static void WriteSidecars(string dir, string name)
    {
        try
        {
            lock (_sidecars)
            {
                foreach (var kv in _sidecars)
                {
                    string content = null;
                    try { content = kv.Value.Save(); } catch { continue; }
                    if (content == null) continue;
                    string path = SidecarPath(dir, name, kv.Key);
                    try
                    {
                        string tmp = path + ".tmp";
                        File.WriteAllText(tmp, content);
                        if (File.Exists(path))
                            File.Copy(path, path + ".bak", true);
                        if (File.Exists(path))
                            File.Delete(path);
                        File.Move(tmp, path);
                    }
                    catch { }
                }
            }
        }
        catch { }
    }

    private static void BackupVanillaSave(string dir, string name)
    {
        try
        {
            string root = BackupRoot;
            if (string.IsNullOrWhiteSpace(root)) return;
            string srcSave = Path.Combine(dir, name + ".save");
            string srcMeta = Path.Combine(dir, name + ".meta");
            if (!File.Exists(srcSave) && !File.Exists(srcMeta)) return; // nichts zu sichern (neuer Slot)
            // Pre-Greg: aeltester bekannter Stand, wird NIE rotiert/geloescht.
            try
            {
                string preGreg = Path.Combine(root, Sanitize(name), "pre-greg");
                bool hasSave = File.Exists(Path.Combine(preGreg, name + ".save"));
                bool hasMeta = File.Exists(Path.Combine(preGreg, name + ".meta"));
                if (!hasSave && !hasMeta)
                {
                    Directory.CreateDirectory(preGreg);
                    if (File.Exists(srcSave)) File.Copy(srcSave, Path.Combine(preGreg, name + ".save"), true);
                    if (File.Exists(srcMeta)) File.Copy(srcMeta, Path.Combine(preGreg, name + ".meta"), true);
                    MelonLogger.Msg($"[gregCore][Save] Pre-Greg-Backup (permanent): {preGreg}");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[gregCore][Save] Pre-Greg-Backup fehlgeschlagen: " + ex.Message);
            }
            string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string dest = Path.Combine(root, Sanitize(name), stamp);
            Directory.CreateDirectory(dest);
            if (File.Exists(srcSave)) File.Copy(srcSave, Path.Combine(dest, name + ".save"), true);
            if (File.Exists(srcMeta)) File.Copy(srcMeta, Path.Combine(dest, name + ".meta"), true);
            MelonLogger.Msg($"[gregCore][Save] Vanilla-Backup: {dest}");
            PruneBackups(root, name);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[gregCore][Save] Backup fehlgeschlagen: " + ex.Message);
        }
    }

    private static void PruneBackups(string root, string name)
    {
        try
        {
            string slot = Path.Combine(root, Sanitize(name));
            if (!Directory.Exists(slot)) return;
            var dirs = new List<string>(Directory.GetDirectories(slot));
            // pre-greg ist permanent ausgenommen.
            dirs.RemoveAll(d => string.Equals(Path.GetFileName(d), "pre-greg", StringComparison.OrdinalIgnoreCase));
            dirs.Sort(StringComparer.Ordinal);
            while (dirs.Count > Math.Max(1, MaxBackupsPerSave))
            {
                try { Directory.Delete(dirs[0], true); } catch { }
                dirs.RemoveAt(0);
            }
        }
        catch { }
    }

    internal static string SidecarPath(string dir, string save, string modId)
    {
        return Path.Combine(dir, $"greg_{modId}.{Sanitize(save)}.tsv");
    }

    internal static string Sanitize(string name)
    {
        if (string.IsNullOrEmpty(name)) return "save";
        char[] invalid = Path.GetInvalidFileNameChars();
        var buf = new char[name.Length];
        int n = 0;
        foreach (char c in name.Trim())
        {
            bool bad = c == '.' || c == ' ';
            if (!bad)
            {
                foreach (char inv in invalid)
                {
                    if (c == inv) { bad = true; break; }
                }
            }
            buf[n++] = bad ? '_' : c;
        }
        string clean = new string(buf, 0, n).Trim('_');
        return clean.Length == 0 ? "save" : clean;
    }
}
