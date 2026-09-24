/// <file-summary>
/// Schicht:      Core (Mods)
/// Zweck:        Custom Items ins Spiel bringen: Shop-/StaticItem-DTO plus
///               Pack-Ordner per Vanilla-ModLoader laden (LoadShopItem /
///               LoadStaticItem). Mesh wird vorher via GregObjImport validiert
///               (klarer Fehler statt stillem Vanilla-Drop), alles best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregCustomItems
{
    // Shop-Item aus DTO + Pack-Ordner laden. Gibt true zurück, wenn der
    // Vanilla-Loader ohne Exception durchlief (Erfolg des Imports meldet
    // das Spiel ggf. zusätzlich selbst).
    public static bool RegisterShopItem(string folderPath, string folderName, GregModPack.ShopItem dto)
    {
        if (!TryValidatePackFolder(folderPath, folderName, dto?.ModelFile))
            return false;
        if (!TryPrevalidateMesh(folderPath, dto.ModelFile))
            return false;
        global::Il2Cpp.ModLoader loader = null;
        try { loader = global::Il2Cpp.ModLoader.instance; } catch { loader = null; }
        if (loader == null)
        {
            Warn("ModLoader.instance nicht verfügbar (Szene noch nicht bereit?).");
            return false;
        }
        global::Il2Cpp.ShopItemConfig config = null;
        try
        {
            var pack = GregModPack.Create(folderName);
            config = GregModPack.AddShopItem(pack, dto);
        }
        catch (Exception ex)
        {
            Warn($"ShopItemConfig-Bau fehlgeschlagen: {Base(ex)}");
            return false;
        }
        if (config == null) return false;
        try
        {
            loader.LoadShopItem(folderPath, folderName, config);
            MelonLogger.Msg($"[gregCore][Mods] Custom shop item registriert: '{dto.ItemName}' ({folderName}).");
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LoadShopItem fehlgeschlagen ('{folderName}'): {Base(ex)}");
            return false;
        }
    }

    // Static-Item (Deko/Statik) aus DTO + Pack-Ordner laden.
    public static bool RegisterStaticItem(string folderPath, string folderName, GregModPack.StaticItem dto)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(folderName) || dto == null)
            return false;
        if (!TryValidatePackFolder(folderPath, folderName, dto.ModelFile))
            return false;
        if (!TryPrevalidateMesh(folderPath, dto.ModelFile))
            return false;
        global::Il2Cpp.ModLoader loader = null;
        try { loader = global::Il2Cpp.ModLoader.instance; } catch { loader = null; }
        if (loader == null)
        {
            Warn("ModLoader.instance nicht verfügbar (Szene noch nicht bereit?).");
            return false;
        }
        global::Il2Cpp.StaticItemConfig config = null;
        try
        {
            var pack = GregModPack.Create(folderName);
            config = GregModPack.AddStaticItem(pack, dto);
        }
        catch (Exception ex)
        {
            Warn($"StaticItemConfig-Bau fehlgeschlagen: {Base(ex)}");
            return false;
        }
        if (config == null) return false;
        try
        {
            loader.LoadStaticItem(folderPath, folderName, config);
            MelonLogger.Msg($"[gregCore][Mods] Custom static item registriert: '{dto.ItemName}' ({folderName}).");
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LoadStaticItem fehlgeschlagen ('{folderName}'): {Base(ex)}");
            return false;
        }
    }

    // Template-Prefab eines geladenen Mod-Items holen (z.B. für Backplanes-
    // artige Varianten/Overlays). Null wenn (noch) nicht geladen.
    public static GameObject GetPrefab(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName)) return null;
        try
        {
            var loader = global::Il2Cpp.ModLoader.instance;
            if (loader == null) return null;
            return loader.GetModPrefabByFolder(folderName);
        }
        catch (Exception ex)
        {
            Warn($"GetPrefab fehlgeschlagen ('{folderName}'): {Base(ex)}");
            return null;
        }
    }

    private static bool TryValidatePackFolder(string folderPath, string folderName, string modelFile)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(folderName))
        {
            Warn("Pack-Ordner oder Folder-Name leer.");
            return false;
        }
        try
        {
            if (!Directory.Exists(folderPath))
            {
                Warn($"Pack-Ordner fehlt: '{folderPath}'.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Warn($"Ordnerprüfung fehlgeschlagen: {Base(ex)}");
            return false;
        }
        if (string.IsNullOrWhiteSpace(modelFile))
        {
            Warn("Kein ModelFile angegeben.");
            return false;
        }
        return true;
    }

    private static bool TryPrevalidateMesh(string folderPath, string modelFile)
    {
        try
        {
            var mesh = GregObjImport.ImportMeshForPack(folderPath, modelFile);
            if (mesh == null)
            {
                Warn($"Mesh-Vorabcheck negativ ('{modelFile}') — Registrierung abgebrochen.");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Mesh-Vorabcheck fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Mods] CustomItems: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
