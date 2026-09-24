/// <file-summary>
/// Layer:       Core (Mods)
/// Purpose:     Save persistence for mod items (ModItemSaveData): managed
///               DTO plus create/fill/read/upsert in the game's own
///               list (SaveData.modItemData), keyed by modFolderName.
///               All best-effort, so persistence never breaks the save.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregModSave
{
    // ── Managed DTO (game-independent) ─────────────────────────────────────

    public sealed class ItemSave
    {
        public string ModFolderName { get; set; } = "";
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float[] SaveValue { get; set; } = Array.Empty<float>();
        public int[] SaveIntArray { get; set; } = Array.Empty<int>();
        public int[] SaveIntArray2 { get; set; } = Array.Empty<int>();
    }

    // ── Create / fill ────────────────────────────────────────────────────────

    public static global::Il2Cpp.ModItemSaveData Create(ItemSave dto)
    {
        global::Il2Cpp.ModItemSaveData entry = null;
        try { entry = new global::Il2Cpp.ModItemSaveData(); } catch { return null; }
        Fill(entry, dto);
        return entry;
    }

    public static void Fill(global::Il2Cpp.ModItemSaveData entry, ItemSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.modFolderName = dto.ModFolderName ?? "");
        Try(() => entry.position = dto.Position);
        Try(() => entry.rotation = dto.Rotation);
        Try(() => entry.saveValue = GregModPack.ToFloatArray(dto.SaveValue));
        Try(() => entry.saveIntArray = GregModPack.ToIntArray(dto.SaveIntArray));
        Try(() => entry.saveIntArray2 = GregModPack.ToIntArray(dto.SaveIntArray2));
    }

    // ── Read ─────────────────────────────────────────────────────────────────

    public static ItemSave Read(global::Il2Cpp.ModItemSaveData entry)
    {
        var dto = new ItemSave();
        if (entry == null) return dto;
        Try(() => dto.ModFolderName = entry.modFolderName ?? "");
        Try(() => dto.Position = entry.position);
        Try(() => dto.Rotation = entry.rotation);
        Try(() => dto.SaveValue = GregModPack.FromFloatArray(entry.saveValue));
        Try(() => dto.SaveIntArray = FromInt(entry.saveIntArray));
        Try(() => dto.SaveIntArray2 = FromInt(entry.saveIntArray2));
        return dto;
    }

    public static List<ItemSave> ReadAll(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ModItemSaveData> list)
    {
        var result = new List<ItemSave>();
        if (list == null) return result;
        Try(() =>
        {
            foreach (var entry in list)
            {
                try { result.Add(Read(entry)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    // ── Upsert in the game's own list (e.g. SaveData.modItemData) ─────────────

    public static global::Il2Cpp.ModItemSaveData Upsert(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ModItemSaveData> list, ItemSave dto)
    {
        if (list == null || dto == null) return null;
        string key = dto.ModFolderName ?? "";
        global::Il2Cpp.ModItemSaveData found = null;
        Try(() =>
        {
            foreach (var entry in list)
            {
                if (entry == null) continue;
                string name = null;
                try { name = entry.modFolderName; } catch { continue; }
                if (string.Equals(name, key, StringComparison.OrdinalIgnoreCase))
                {
                    found = entry;
                    break;
                }
            }
        });
        if (found != null)
        {
            Fill(found, dto);
            return found;
        }
        var created = Create(dto);
        if (created != null)
        {
            try { list.Add(created); } catch { return null; }
        }
        return created;
    }

    public static bool Remove(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ModItemSaveData> list, string modFolderName)
    {
        if (list == null || string.IsNullOrEmpty(modFolderName)) return false;
        bool removed = false;
        Try(() =>
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                global::Il2Cpp.ModItemSaveData entry = null;
                try { entry = list[i]; } catch { continue; }
                if (entry == null) continue;
                string name = null;
                try { name = entry.modFolderName; } catch { continue; }
                if (string.Equals(name, modFolderName, StringComparison.OrdinalIgnoreCase))
                {
                    try { list.RemoveAt(i); removed = true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
        });
        return removed;
    }

    private static int[] FromInt(Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int> src)
    {
        return GregModPack.FromIntArray(src);
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Mods] ModSave field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
