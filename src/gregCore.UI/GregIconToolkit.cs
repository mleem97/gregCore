/// <file-summary>
/// Layer:      UI
/// Purpose:     Icon provider (gregCore.UI kit). Loads PNG/JPG icons
///              from folders (no game injection needed - files live in the
///              Mods folder, e.g. Mods/gregIcons/) via file bytes +
///              ImageConversion into cached textures. Mods apply them via
///              ApplyBackground to VisualElements or build buttons with
///              IconButton. Note: export SVGs as PNG first.
/// </file-summary>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "File/Texture IO over live game filesystem; needs running game.")]
public static class GregIconToolkit
{
    private static readonly Dictionary<string, Texture2D> _cache =
        new Dictionary<string, Texture2D>(System.StringComparer.OrdinalIgnoreCase);
    private static bool _initialized;

    // Default folder: <game>/Mods/gregIcons. Scanned once.
    // Embedded icons (DLL) form the base and are always available;
    // files in the folder act as override/custom layer (win).
    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        try
        {
            int embedded = LoadEmbedded();
            string dir = System.IO.Path.Combine(
                global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory, "Mods", "gregIcons");
            int files = RegisterFolderCounted(dir);
            MelonLogger.Msg($"[gregCore][UI] Icons: {embedded} embedded, {files} aus Ordner.");
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Icon-Init failed: {ex.Message}");
        }
    }

    private static int LoadEmbedded()
    {
        int added = 0;
        try
        {
            var asm = typeof(GregIconToolkit).Assembly;
            foreach (string res in asm.GetManifestResourceNames())
            {
                if (!res.StartsWith("gregIcons.", System.StringComparison.OrdinalIgnoreCase)) continue;
                string key = res.Substring("gregIcons.".Length);
                int dot = key.LastIndexOf('.');
                if (dot > 0) key = key.Substring(0, dot);
                if (string.IsNullOrEmpty(key) || _cache.ContainsKey(key)) continue;
                try
                {
                    using (var stream = asm.GetManifestResourceStream(res))
                    {
                        if (stream == null) continue;
                        var bytes = new byte[stream.Length];
                        int read = 0;
                        while (read < bytes.Length)
                        {
                            int n = stream.Read(bytes, read, bytes.Length - read);
                            if (n <= 0) break;
                            read += n;
                        }
                        if (read != bytes.Length) continue;
                        var tex = BytesToTexture(bytes);
                        if (tex == null) continue;
                        _cache[key] = tex;
                        added++;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return added;
    }

    private static Texture2D BytesToTexture(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) return null;
        try
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            bool ok = false;
            try { ok = ImageConversion.LoadImage(tex, bytes); } catch { ok = false; }
            if (!ok)
            {
                try { UnityEngine.Object.Destroy(tex); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            }
            try { tex.filterMode = FilterMode.Bilinear; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return tex;
        }
        catch { return null; }
    }

    private static int RegisterFolderCounted(string folder)
    {
        int added = 0;
        if (string.IsNullOrWhiteSpace(folder)) return 0;
        try
        {
            if (!Directory.Exists(folder)) return 0;
            foreach (string file in Directory.GetFiles(folder))
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg") continue;
                string key = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrEmpty(key)) continue;
                try
                {
                    byte[] bytes = File.ReadAllBytes(file);
                    var tex = BytesToTexture(bytes);
                    if (tex == null) continue;
                    // Override: release old texture, adopt new one.
                    Texture2D old;
                    if (_cache.TryGetValue(key, out old) && old != null)
                    {
                        try { UnityEngine.Object.Destroy(old); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                    _cache[key] = tex;
                    added++;
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return added;
    }

    // Register an additional folder (e.g. mod-owned icons). Files
    // overwrite embedded ones (custom layer).
    public static void RegisterFolder(string folder)
    {
        try { RegisterFolderCounted(folder); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static bool Has(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        lock (_cache) { return _cache.ContainsKey(name); }
    }

    public static Texture2D Get(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        Initialize();
        lock (_cache)
        {
            Texture2D tex;
            return _cache.TryGetValue(name, out tex) ? tex : null;
        }
    }

    // Set icon as background image (for buttons/bars). size<=0 =
    // size unchanged. Aspect ratio is preserved (Contain).
    // Return: icon found?
    public static bool ApplyBackground(VisualElement el, string name)
    {
        return ApplyBackground(el, name, 0f);
    }

    public static bool ApplyBackground(VisualElement el, string name, float size)
    {
        try
        {
            if (el == null) return false;
            var tex = Get(name);
            if (tex == null) return false;
            el.style.backgroundImage = new StyleBackground(Background.FromTexture2D(tex));
            try { el.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (size > 0f)
            {
                el.style.width = size;
                el.style.height = size;
            }
            return true;
        }
        catch { return false; }
    }

    // Finished icon element (image, square). Null on failure.
    public static VisualElement Icon(string name)
    {
        return Icon(name, 24f);
    }

    public static VisualElement Icon(string name, float size)
    {
        try
        {
            var tex = Get(name);
            if (tex == null) return null;
            var el = new VisualElement();
            el.style.backgroundImage = new StyleBackground(Background.FromTexture2D(tex));
            try { el.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            el.style.width = size;
            el.style.height = size;
            el.pickingMode = PickingMode.Ignore;
            return el;
        }
        catch { return null; }
    }
}
