/// <file-summary>
/// Schicht:      Core (Mods)
/// Zweck:        ModPackConfig-Erweiterung: baut spiel-kompatible ModPacks
///               (ModPackConfig + ShopItemConfig + StaticItemConfig + DllEntry)
///               und liest beliebige Packs in verwaltete Snapshots aus.
///               Alle Zugriffe best-effort (pro Feld try/catch), damit ein
///               unbekannter/unvollstaendiger Typ nie den Aufrufer reisst.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregModPack
{
    // ── Verwaltete DTOs (spiel-unabhaengig, testbar) ─────────────────────────

    public sealed class ShopItem
    {
        public string ItemName = "";
        public int Price;
        public int XpToUnlock;
        public int SizeInU = 1;
        public float Mass = 1f;
        public float ModelScale = 1f;
        public float[] ColliderSize = Array.Empty<float>();
        public float[] ColliderCenter = Array.Empty<float>();
        public string ModelFile = "";
        public string TextureFile = "";
        public string IconFile = "";
        public string ObjectType = "";
    }

    public sealed class StaticItem
    {
        public string ItemName = "";
        public float ModelScale = 1f;
        public float[] ColliderSize = Array.Empty<float>();
        public float[] ColliderCenter = Array.Empty<float>();
        public string ModelFile = "";
        public string TextureFile = "";
        public float[] Position = Array.Empty<float>();
        public float[] Rotation = Array.Empty<float>();
        public bool IsKinematic;
    }

    public sealed class DllRef
    {
        public string FileName = "";
        public string EntryClass = "";
    }

    public sealed class Snapshot
    {
        public string ModName = "";
        public List<ShopItem> ShopItems = new List<ShopItem>();
        public List<StaticItem> StaticItems = new List<StaticItem>();
        public List<DllRef> Dlls = new List<DllRef>();
    }

    // ── Fabrik: leeres, sofort nutzbares ModPackConfig ───────────────────────

    public static global::Il2Cpp.ModPackConfig Create(string modName)
    {
        global::Il2Cpp.ModPackConfig cfg = null;
        try { cfg = new global::Il2Cpp.ModPackConfig(); } catch { return null; }
        try { cfg.modName = modName ?? ""; } catch { }
        EnsureLists(cfg);
        return cfg;
    }

    // Null-Listen heilen (Vanilla/Workshop-Packs sind nicht immer vollstaendig).
    public static void EnsureLists(global::Il2Cpp.ModPackConfig cfg)
    {
        if (cfg == null) return;
        try { if (cfg.shopItems == null) cfg.shopItems = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ShopItemConfig>(); } catch { }
        try { if (cfg.staticItems == null) cfg.staticItems = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.StaticItemConfig>(); } catch { }
        try { if (cfg.dlls == null) cfg.dlls = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.DllEntry>(); } catch { }
    }

    // ── Builder: DTO -> Spieltyp (jeweils best-effort pro Feld) ──────────────

    public static global::Il2Cpp.ShopItemConfig AddShopItem(global::Il2Cpp.ModPackConfig cfg, ShopItem dto)
    {
        if (cfg == null || dto == null) return null;
        EnsureLists(cfg);
        global::Il2Cpp.ShopItemConfig item = null;
        try { item = new global::Il2Cpp.ShopItemConfig(); } catch { return null; }
        Try(() => item.itemName = dto.ItemName ?? "");
        Try(() => item.price = dto.Price);
        Try(() => item.xpToUnlock = dto.XpToUnlock);
        Try(() => item.sizeInU = dto.SizeInU);
        Try(() => item.mass = dto.Mass);
        Try(() => item.modelScale = dto.ModelScale);
        Try(() => item.colliderSize = ToFloatArray(dto.ColliderSize));
        Try(() => item.colliderCenter = ToFloatArray(dto.ColliderCenter));
        Try(() => item.modelFile = dto.ModelFile ?? "");
        Try(() => item.textureFile = dto.TextureFile ?? "");
        Try(() => item.iconFile = dto.IconFile ?? "");
        Try(() =>
        {
            if (Enum.TryParse<global::Il2Cpp.PlayerManager.ObjectInHand>(dto.ObjectType, true, out var t))
                item.objectType = t;
        });
        Try(() => cfg.shopItems.Add(item));
        return item;
    }

    public static global::Il2Cpp.StaticItemConfig AddStaticItem(global::Il2Cpp.ModPackConfig cfg, StaticItem dto)
    {
        if (cfg == null || dto == null) return null;
        EnsureLists(cfg);
        global::Il2Cpp.StaticItemConfig item = null;
        try { item = new global::Il2Cpp.StaticItemConfig(); } catch { return null; }
        Try(() => item.itemName = dto.ItemName ?? "");
        Try(() => item.modelScale = dto.ModelScale);
        Try(() => item.colliderSize = ToFloatArray(dto.ColliderSize));
        Try(() => item.colliderCenter = ToFloatArray(dto.ColliderCenter));
        Try(() => item.modelFile = dto.ModelFile ?? "");
        Try(() => item.textureFile = dto.TextureFile ?? "");
        Try(() => item.position = ToFloatArray(dto.Position));
        Try(() => item.rotation = ToFloatArray(dto.Rotation));
        Try(() => item.isKinematic = dto.IsKinematic);
        Try(() => cfg.staticItems.Add(item));
        return item;
    }

    public static global::Il2Cpp.DllEntry AddDll(global::Il2Cpp.ModPackConfig cfg, DllRef dto)
    {
        if (cfg == null || dto == null) return null;
        EnsureLists(cfg);
        global::Il2Cpp.DllEntry entry = null;
        try { entry = new global::Il2Cpp.DllEntry(); } catch { return null; }
        Try(() => entry.fileName = dto.FileName ?? "");
        Try(() => entry.entryClass = dto.EntryClass ?? "");
        Try(() => cfg.dlls.Add(entry));
        return entry;
    }

    // ── Reader: beliebiges Pack -> verwalteter Snapshot ──────────────────────

    public static Snapshot Read(global::Il2Cpp.ModPackConfig cfg)
    {
        var snap = new Snapshot();
        if (cfg == null) return snap;
        Try(() => snap.ModName = cfg.modName ?? "");
        Try(() =>
        {
            var list = cfg.shopItems;
            if (list == null) return;
            foreach (var item in list)
            {
                try { snap.ShopItems.Add(ReadShopItem(item)); } catch { }
            }
        });
        Try(() =>
        {
            var list = cfg.staticItems;
            if (list == null) return;
            foreach (var item in list)
            {
                try { snap.StaticItems.Add(ReadStaticItem(item)); } catch { }
            }
        });
        Try(() =>
        {
            var list = cfg.dlls;
            if (list == null) return;
            foreach (var entry in list)
            {
                try
                {
                    snap.Dlls.Add(new DllRef
                    {
                        FileName = entry != null ? entry.fileName ?? "" : "",
                        EntryClass = entry != null ? entry.entryClass ?? "" : "",
                    });
                }
                catch { }
            }
        });
        return snap;
    }

    public static ShopItem ReadShopItem(global::Il2Cpp.ShopItemConfig item)
    {
        var dto = new ShopItem();
        if (item == null) return dto;
        Try(() => dto.ItemName = item.itemName ?? "");
        Try(() => dto.Price = item.price);
        Try(() => dto.XpToUnlock = item.xpToUnlock);
        Try(() => dto.SizeInU = item.sizeInU);
        Try(() => dto.Mass = item.mass);
        Try(() => dto.ModelScale = item.modelScale);
        Try(() => dto.ColliderSize = FromFloatArray(item.colliderSize));
        Try(() => dto.ColliderCenter = FromFloatArray(item.colliderCenter));
        Try(() => dto.ModelFile = item.modelFile ?? "");
        Try(() => dto.TextureFile = item.textureFile ?? "");
        Try(() => dto.IconFile = item.iconFile ?? "");
        Try(() => dto.ObjectType = item.objectType.ToString());
        return dto;
    }

    public static StaticItem ReadStaticItem(global::Il2Cpp.StaticItemConfig item)
    {
        var dto = new StaticItem();
        if (item == null) return dto;
        Try(() => dto.ItemName = item.itemName ?? "");
        Try(() => dto.ModelScale = item.modelScale);
        Try(() => dto.ColliderSize = FromFloatArray(item.colliderSize));
        Try(() => dto.ColliderCenter = FromFloatArray(item.colliderCenter));
        Try(() => dto.ModelFile = item.modelFile ?? "");
        Try(() => dto.TextureFile = item.textureFile ?? "");
        Try(() => dto.Position = FromFloatArray(item.position));
        Try(() => dto.Rotation = FromFloatArray(item.rotation));
        Try(() => dto.IsKinematic = item.isKinematic);
        return dto;
    }

    // ── Array-Brücken (managed <-> Il2Cpp) ───────────────────────────────────

    public static Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float> ToFloatArray(float[] src)
    {
        if (src == null || src.Length == 0) return null;
        var arr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float>(src.Length);
        for (int i = 0; i < src.Length; i++) arr[i] = src[i];
        return arr;
    }

    public static Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int> ToIntArray(int[] src)
    {
        if (src == null || src.Length == 0) return null;
        var arr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int>(src.Length);
        for (int i = 0; i < src.Length; i++) arr[i] = src[i];
        return arr;
    }

    public static float[] FromFloatArray(Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float> src)
    {
        if (src == null) return Array.Empty<float>();
        int n = 0;
        try { n = src.Length; } catch { return Array.Empty<float>(); }
        var dst = new float[Math.Max(0, n)];
        for (int i = 0; i < dst.Length; i++)
        {
            try { dst[i] = src[i]; } catch { dst[i] = 0f; }
        }
        return dst;
    }

    public static int[] FromIntArray(Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int> src)
    {
        if (src == null) return Array.Empty<int>();
        int n = 0;
        try { n = src.Length; } catch { return Array.Empty<int>(); }
        var dst = new int[Math.Max(0, n)];
        for (int i = 0; i < dst.Length; i++)
        {
            try { dst[i] = src[i]; } catch { dst[i] = 0; }
        }
        return dst;
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Mods] ModPack-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { }
        }
    }
}
