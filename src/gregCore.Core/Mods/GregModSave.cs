/// <file-summary>
/// Schicht:      Core (Mods)
/// Zweck:        Savepersistenz fuer Mod-Items (ModItemSaveData): verwaltetes
///               DTO plus Erzeugen/Fuellen/Lesen/Upsert in der spieleigenen
///               Liste (SaveData.modItemData), geschluesselt per modFolderName.
///               Alles best-effort, damit Persistence nie den Spielstand reißt.
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
    // ── Verwaltetes DTO (spiel-unabhaengig) ──────────────────────────────────

    public sealed class ItemSave
    {
        public string ModFolderName = "";
        public Vector3 Position;
        public Quaternion Rotation;
        public float[] SaveValue = Array.Empty<float>();
        public int[] SaveIntArray = Array.Empty<int>();
        public int[] SaveIntArray2 = Array.Empty<int>();
    }

    // ── Erzeugen / Fuellen ───────────────────────────────────────────────────

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

    // ── Lesen ────────────────────────────────────────────────────────────────

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
                try { result.Add(Read(entry)); } catch { }
            }
        });
        return result;
    }

    // ── Upsert in der spieleigenen Liste (z.B. SaveData.modItemData) ──────────

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
                    try { list.RemoveAt(i); removed = true; } catch { }
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
            try { MelonLogger.Warning($"[gregCore][Mods] ModSave-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { }
        }
    }
}
