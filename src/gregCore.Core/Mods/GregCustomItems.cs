/// <file-summary>
/// Layer:       Core (Mods)
/// Purpose:     Bring custom items into the game: shop/static-item DTO plus
///               pack folder loaded via the vanilla ModLoader (LoadShopItem /
///               LoadStaticItem). Mesh is validated via GregObjImport beforehand
///               (clear error instead of a silent vanilla drop), all best-effort.
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
    // Load a shop item from DTO + pack folder. Returns true if the
    // vanilla loader completed without exception (the game may additionally
    // report the import success itself).
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
            Warn("ModLoader.instance not available (scene not ready yet?).");
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
            Warn($"ShopItemConfig build failed: {Base(ex)}");
            return false;
        }
        if (config == null) return false;
        try
        {
            loader.LoadShopItem(folderPath, folderName, config);
            MelonLogger.Msg($"[gregCore][Mods] Custom shop item registered: '{dto.ItemName}' ({folderName}).");
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LoadShopItem failed ('{folderName}'): {Base(ex)}");
            return false;
        }
    }

    // Load a static item (deco/static) from DTO + pack folder.
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
            Warn("ModLoader.instance not available (scene not ready yet?).");
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
            Warn($"StaticItemConfig build failed: {Base(ex)}");
            return false;
        }
        if (config == null) return false;
        try
        {
            loader.LoadStaticItem(folderPath, folderName, config);
            MelonLogger.Msg($"[gregCore][Mods] Custom static item registered: '{dto.ItemName}' ({folderName}).");
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LoadStaticItem failed ('{folderName}'): {Base(ex)}");
            return false;
        }
    }

    // Get the template prefab of a loaded mod item (e.g. for backplane-
    // like variants/overlays). Null when not (yet) loaded.
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
            Warn($"GetPrefab failed ('{folderName}'): {Base(ex)}");
            return null;
        }
    }

    private static bool TryValidatePackFolder(string folderPath, string folderName, string modelFile)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(folderName))
        {
            Warn("Pack folder or folder name empty.");
            return false;
        }
        try
        {
            if (!Directory.Exists(folderPath))
            {
                Warn($"Pack folder missing: '{folderPath}'.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Warn($"Folder validation failed: {Base(ex)}");
            return false;
        }
        if (string.IsNullOrWhiteSpace(modelFile))
        {
            Warn("No ModelFile specified.");
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
                Warn($"Mesh precheck negative ('{modelFile}') - registration aborted.");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Mesh precheck failed: {Base(ex)}");
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
