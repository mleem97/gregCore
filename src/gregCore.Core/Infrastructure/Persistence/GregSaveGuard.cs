/// <file-summary>
/// Layer:       Infrastructure (Persistence)
/// Purpose:     Vanilla-compatible save system (gregCore kit):
///               0) GUARANTEE: no framework write (sidecars, sanitize-gated
///               paths) happens without a fresh vanilla backup first. If the
///               backup fails, mod writes for that save are skipped — the
///               game's own save always proceeds untouched.
///               1) Vanilla backup: before every game save, <savename>.save
///               + .meta are copied to <BackupRoot>/<save>/<timestamp>/
///               (keeps max. N) plus a permanent pre-greg/ snapshot.
///               Rollback without mods at any time.
///               2) Mod sidecars: mods register save/load handlers; contents
///               land as greg_<modId>.<save>.tsv NEXT TO the saves. The game
///               only lists *.save -> inert without mods = vanilla fallback.
///               3) Vanilla fallback: BEFORE SerializeToBytes and AFTER every
///               load, invalid foreign/out-of-range IDs in
///               networkData.sfpModules[].prefabID are mapped onto a valid
///               vanilla prefab (generic: highest vanilla ID,
///               or mod-specific via RegisterVanillaModuleMap()). This keeps
///               every save loadable without any modding, and a
///               modded module falls back to a normal module in the save.
/// Maintainer:  Only this file knows SaveSystem details. Defensive hooks.
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
    /// Mods report their prefabID ranges: toVanilla maps a modded ID
    /// onto the matching vanilla base (returning null = ID does not belong
    /// to this mod). fallbackVanillaPrefabId serves as the mod's own replacement
    /// if the vanilla ID count cannot be read at runtime.
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
    private static bool _optOutWarned;

    /// <summary>
    /// Canonical backup location, Windows + Linux compatible:
    ///   Windows: %USERPROFILE%\Documents\DatacenterBackups
    ///   Linux:   ~/Documents/DatacenterBackups (~/DatacenterBackups fallback)
    /// Existing backups under the legacy ~/GregFrameworkBackups are left
    /// untouched (never migrated, never deleted by us).
    /// </summary>
    public static string BackupRoot
    {
        get
        {
            try
            {
                string myDocs = null;
                try { myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                catch { myDocs = null; }
                string home = null;
                try { home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }
                catch { home = null; }
                if (string.IsNullOrWhiteSpace(home))
                {
                    try { home = Environment.GetEnvironmentVariable("HOME"); }
                    catch { home = null; }
                }
                return ResolveBackupRoot(myDocs, home, Directory.Exists);
            }
            catch { return null; }
        }
    }

    /// <summary>
    /// Pure path resolution (unit-tested): prefers a real Documents folder,
    /// falls back to ~/DatacenterBackups. On Linux MyDocuments resolves to
    /// $HOME itself, hence the explicit ~/Documents probe.
    /// </summary>
    internal static string ResolveBackupRoot(string myDocs, string home, Func<string, bool> exists)
    {
        try
        {
            string docsDir = null;
            if (!string.IsNullOrWhiteSpace(myDocs))
            {
                try
                {
                    if (string.Equals(Path.GetFileName(myDocs.TrimEnd(Path.DirectorySeparatorChar,
                            Path.AltDirectorySeparatorChar)), "Documents", StringComparison.OrdinalIgnoreCase)
                        && exists(myDocs))
                        docsDir = myDocs;
                }
                catch { docsDir = null; }
            }
            if (docsDir == null && !string.IsNullOrWhiteSpace(home))
            {
                try
                {
                    string lxDocs = Path.Combine(home, "Documents");
                    if (exists(lxDocs)) docsDir = lxDocs;
                }
                catch { docsDir = null; }
            }
            string baseDir = docsDir
                ?? (!string.IsNullOrWhiteSpace(home) ? home : ".");
            return Path.Combine(baseDir, "DatacenterBackups");
        }
        catch { return null; }
    }

    /// <summary>
    /// The backup guarantee as a pure decision (unit-tested): framework
    /// writes happen only when backups are enabled AND the fresh backup
    /// succeeded. BackupEnabled=false is an explicit opt-out that also
    /// disables mod writes (logged once per session).
    /// </summary>
    internal static bool ResolveSaveWriteGate(bool backupEnabled, bool backupSucceeded)
    {
        return backupEnabled && backupSucceeded;
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
        try
        {
            int installed = PatchSaveEntryPoints(harmony) + PatchLoadEntryPoints(harmony);
            if (installed == 0)
            {
                MelonLogger.Warning("[gregCore][Save] No SaveSystem method found (API drift?).");
                return;
            }
            _hooksInstalled = true;
            MelonLogger.Msg($"[gregCore][Save] Save hooks installed ({installed}): backup, sidecars, vanilla fallback, inventory.");
            try { MelonLogger.Msg("[gregCore][Save] BackupRoot: " + (BackupRoot ?? "?")); } catch { }
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Save] Hook installation failed: " + ex.GetBaseException().Message);
        }
    }

    private static int PatchSaveEntryPoints(HarmonyLib.Harmony harmony)
    {
        var installed = 0;
        // Backup + sidecars before the game save
        var saveGame = AccessTools.Method(typeof(global::Il2Cpp.SaveSystem), "SaveGame",
            new Type[] { typeof(string), typeof(string) });
        if (saveGame != null)
        {
            harmony.Patch(saveGame, prefix: new HarmonyMethod(typeof(GregSaveGuard), nameof(SaveGamePrefix)));
            installed++;
        }

        // Vanilla fallback (saving): exactly at the serialization entry point
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
        return installed;
    }

    private static int PatchLoadEntryPoints(HarmonyLib.Harmony harmony)
    {
        var installed = 0;
        // Vanilla fallback (loading): directly after deserialization.
        // LoadGame returns the save (the singleton stays untouched in
        // the main menu — SaveData.instance is not yet constructible
        // there and throws NRE). The void entry points only run in
        // gameplay scenes (the menu only loads preview data, no mod content).
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

        // Save inventory: after healing (prefix) all device IDs are
        // final - here everything in the save is inventoried + given stable
        // UIDs (invisible, sidecar + memory). Extracted
        // (codeline limit): only hook registration stays here.
        var loadNetworkState = AccessTools.Method(typeof(global::Il2Cpp.WaypointInitializationSystem),
            "LoadNetworkState");
        if (loadNetworkState != null && PatchLoadNetworkState(harmony, loadNetworkState))
            installed++;
        return installed;
    }

    // Prefix BEFORE the game save: FIRST vanilla backup, THEN sidecars —
    // and sidecars only when the backup succeeded (guarantee).
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
            bool backedUp = BackupEnabled && BackupVanillaSave(dir, name);
            if (ResolveSaveWriteGate(BackupEnabled, backedUp))
            {
                WriteSidecars(dir, name);
            }
            else
            {
                WarnOptOutOnce();
                MelonLogger.Error("[gregCore][Save] NO backup possible — mod sidecar files for '"
                    + name + "' will NOT be written this run (vanilla save continues).");
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[gregCore][Save] SaveGamePrefix failed: " + ex.Message);
        }
    }

    private static void WarnOptOutOnce()
    {
        try
        {
            if (!BackupEnabled && !_optOutWarned)
            {
                _optOutWarned = true;
                MelonLogger.Warning("[gregCore][Save] BackupEnabled=false: es werden no Backups "
                    + "angelegt and no Mod-Seitendateien geschrieben (Opt-out).");
            }
        }
        catch { }
    }

    // Prefix at the serialization entry point: ensure a backup exists BEFORE
    // the (lossy-for-mods) sanitize mutation, so non-SaveGame write paths
    // are covered too. Sanitize itself always runs — it protects loadability.
    public static void SaveSerializePrefix(global::Il2Cpp.SaveData __0)
    {
        try
        {
            EnsureBackedUpForCurrentSlot("SerializeToBytes");
            SanitizeSaveData(__0, "Speichern");
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (SerializeToBytes): " + ex.Message); }
    }

    public static void SaveGameDataPrefix()
    {
        try
        {
            EnsureBackedUpForCurrentSlot("SaveGameData");
            SanitizeSaveData(global::Il2Cpp.SaveData.instance, "Speichern");
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (SaveGameData): " + ex.Message); }
    }

    private static void EnsureBackedUpForCurrentSlot(string phase)
    {
        try
        {
            if (!BackupEnabled) { WarnOptOutOnce(); return; }
            string dir = null, name = null;
            try { dir = global::Il2Cpp.SaveSystem.saveDirPath; } catch { dir = null; }
            try { name = global::Il2Cpp.SaveSystem.loadSaveName; } catch { name = null; }
            if (string.IsNullOrWhiteSpace(dir) || string.IsNullOrWhiteSpace(name)) return;
            BackupVanillaSave(dir, name);
        }
        catch { }
    }

    // Postfix after LoadGame: sanitizes the returned save — the
    // singleton is never touched (not yet constructible in the main menu).
    public static void LoadGamePostfix(global::Il2Cpp.SaveData __result)
    {
        try
        {
            if (__result == null) return;
            SanitizeSaveData(__result, "Laden");
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (Loading): " + ex.Message); }
    }

    // Postfix for void load entry points: gameplay scenes only. In the main menu
    // the game only loads preview data (no mod content) — and
    // singleton access throws NRE there (silently skipped, no warning).
    public static void LoadGuardedPostfix()
    {
        try
        {
            if (!IsGameplayScene()) return;
            SanitizeSaveData(global::Il2Cpp.SaveData.instance, "Laden");
            gregCore.Infrastructure.Logging.DevLog.Msg("Load postfix executed (SaveData.instance available).");
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Sanitize (Loading): " + ex.Message); }
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

    // Postfix after WaypointInitializationSystem.LoadNetworkState: healing
    // prefix already ran, all device IDs are final. Build inventory here.
    public static void LoadNetworkStatePostfix(global::Il2Cpp.NetworkSaveData networkData)
    {
        try
        {
            GregEntityInventory.RebuildFromNetworkData(networkData);
        }
        catch (Exception ex) { MelonLogger.Warning("[gregCore][Save] Inventory (LoadNetworkState): " + ex.Message); }
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
    /// Normalizes networkData.sfpModules[].prefabID. Invalid mod IDs
    /// are downgraded either via a registered mod map or generically onto the
    /// highest valid vanilla ID. Without readability of the
    /// vanilla prefab count, only explicitly registered mod IDs are replaced.
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
            + " SFP modules, vanillaCount=" + vanillaCount + " (cached=" + _lastKnownVanillaCount
            + "), mod maps=" + _vanillaMaps.Count + ".");

        int sanitized = SanitizeModules(modules, vanillaCount, phase);
        if (sanitized > 0)
            MelonLogger.Warning($"[gregCore][Save] {phase}: {sanitized} modded SFP modules downgraded to vanilla modules.");
    }

    private static int SanitizeModules(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.SFPSaveData> modules,
        int vanillaCount, string phase)
    {
        int sanitized = 0;
        for (int i = 0; i < modules.Count; i++)
        {
            if (TrySanitizeModule(modules, i, vanillaCount, phase)) sanitized++;
        }
        return sanitized;
    }

    private static bool TrySanitizeModule(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.SFPSaveData> modules,
        int index, int vanillaCount, string phase)
    {
        global::Il2Cpp.SFPSaveData sfd = null;
        try { sfd = modules[index]; } catch { return false; }
        if (sfd == null) return false;
        int id;
        try { id = sfd.prefabID; } catch { return false; }
        if (id < 0) return false;
        if (vanillaCount > 0 && id < vanillaCount) return false; // valid vanilla ID

        int target = ResolveVanillaTarget(id, vanillaCount);
        if (target < 0 || target == id) return false;
        try { sfd.prefabID = target; } catch { return false; }
        MelonLogger.Warning($"[gregCore][Save] SFP module #{index}: prefabID {id} -> {target} ({phase}, vanilla fallback).");
        return true;
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
                try { r = m.ToVanilla(customId); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                if (r.HasValue) return r.Value;
            }
        }
        // Unknown ID (third-party range, no map): leave alone. Morphing it to
        // an arbitrary vanilla module would silently corrupt foreign items —
        // and a stale vanillaCount cache after game updates must never eat
        // new vanilla IDs either. Caller skips negative targets.
        return -1;
    }

    // After scene load: read sidecars of the current save (idempotent).
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
                    try { if (File.Exists(path)) content = File.ReadAllText(path); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    if (content == null) continue;
                    try { kv.Value.Load(content); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    /// <returns>
    /// True when the slot is protected: backup written, or nothing existed
    /// yet (new slot — nothing to lose). False on any failure; callers must
    /// skip framework writes then (guarantee). Never throws.
    /// </returns>
    private static bool BackupVanillaSave(string dir, string name)
    {
        try
        {
            string root = BackupRoot;
            if (string.IsNullOrWhiteSpace(root)) return false;
            string srcSave = Path.Combine(dir, name + ".save");
            string srcMeta = Path.Combine(dir, name + ".meta");
            if (!File.Exists(srcSave) && !File.Exists(srcMeta)) return true; // nothing to back up (new slot)
            // Pre-Greg: oldest known state, NEVER rotated/deleted.
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
                MelonLogger.Warning("[gregCore][Save] Pre-Greg backup failed: " + ex.Message);
            }
            string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string dest = Path.Combine(root, Sanitize(name), stamp);
            Directory.CreateDirectory(dest);
            if (File.Exists(srcSave)) File.Copy(srcSave, Path.Combine(dest, name + ".save"), true);
            if (File.Exists(srcMeta)) File.Copy(srcMeta, Path.Combine(dest, name + ".meta"), true);
            MelonLogger.Msg($"[gregCore][Save] Vanilla-Backup: {dest}");
            PruneBackups(root, name);
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[gregCore][Save] Backup failed: " + ex.Message);
            return false;
        }
    }

    private static void PruneBackups(string root, string name)
    {
        try
        {
            string slot = Path.Combine(root, Sanitize(name));
            if (!Directory.Exists(slot)) return;
            var dirs = new List<string>(Directory.GetDirectories(slot));
            // pre-greg is permanently excluded.
            dirs.RemoveAll(d => string.Equals(Path.GetFileName(d), "pre-greg", StringComparison.OrdinalIgnoreCase));
            dirs.Sort(StringComparer.Ordinal);
            while (dirs.Count > Math.Max(1, MaxBackupsPerSave))
            {
                try { Directory.Delete(dirs[0], true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dirs.RemoveAt(0);
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
