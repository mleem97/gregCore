using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Il2CppTMPro;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Runtime font provider for GregCore mods.
    ///
    /// Load order:
    ///   1. Resources.Load direct on known TMP font paths (deterministic, scene-independent)
    ///   2. Resources.FindObjectsOfTypeAll&lt;TextMeshProUGUI&gt; → font.sourceFontFile
    ///   3. Resources.FindObjectsOfTypeAll&lt;TMP_FontAsset&gt; → sourceFontFile
    ///   4. Resources.FindObjectsOfTypeAll&lt;Font&gt;
    ///   5. Resources.GetBuiltinResource&lt;Font&gt;("LegacyRuntime.ttf")
    ///   6. UI Toolkit: FontDefinition via reflection (FromSDFFont preferred, FromFont fallback)
    /// </summary>
    public static class GregFontLoader
    {
        private const string VERSION = "1.1.0-F4"; // Font Fix Build 4
        private static readonly Dictionary<string, FontAsset> _fontAssetCache = new();
        private static FontAsset? _defaultFontAsset;
        private static TMP_FontAsset? _defaultTMPFontAsset;
        private static Material? _defaultTMPMaterial;
        private static TextMeshProUGUI? _tmpTemplate;
        private static Font? _defaultUGUIFont;
        private static object? _defaultFontDefinition;
        private static string _defaultFontName = "Inter";
        private static int _searchAttempt;
        private static bool _hasFoundGameFont;
        private static float _lastSearchTime;

        // Reflection handles for UI Toolkit FontDefinition
        private static Type? _fontDefinitionType;
        private static Type? _styleFontDefinitionType;
        private static ConstructorInfo? _styleFontDefCtor;
        private static MethodInfo? _fontDefFromFontMethod;
        private static MethodInfo? _fontDefFromSDFFontMethod;
        private static bool _reflectionInitialized;

        // Known TMP font paths in the game's Resources folder
        private static readonly string[] KnownTMPFontPaths =
        {
            "fonts & materials/LiberationSans SDF",
            "fonts & materials/Roboto-Bold SDF",
            "fonts & materials/Oswald Bold SDF",
            "fonts & materials/Bangers SDF",
            "fonts & materials/Electronic Highway Sign SDF",
            "fonts & materials/Anton SDF",
        };

        public static string DefaultFontName
        {
            get => _defaultFontName;
            set => _defaultFontName = value;
        }

        public static bool HasFoundGameFont => _hasFoundGameFont;

        /// <summary>Early init — safe to call in OnInitializeMelon(). Sets up reflection only.</summary>
        public static void Initialize()
        {
            MelonLogger.Msg("[FontLoader] Initializing font provider (early)...");
            InitializeReflection();
        }

        /// <summary>Full font search. Call after scene load or periodically via Tick().</summary>
        public static void SearchFonts()
        {
            if (_hasFoundGameFont && _defaultTMPMaterial != null) return;
            
            _lastSearchTime = Time.time;
            _searchAttempt++;
            if (_searchAttempt > 20) return;

            MelonLogger.Msg($"[FontLoader] Search attempt #{_searchAttempt} ({VERSION})...");

            try
            {
                if (!_reflectionInitialized) InitializeReflection();
                LoadTMPFontAssetDirect();
                FindFontsFromTMPText();
                
                // Only do heavy searches if still not found
                if (!_hasFoundGameFont)
                {
                    FindTMPFontAssets();
                    FindGameFonts();
                    FindGameFontAssets();
                }
                SetupDefaultFonts();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[FontLoader] Font search failed: {ex.Message}");
            }
        }

        /// <summary>Retry every 5 seconds until game font/material are found.</summary>
        public static void Tick()
        {
            if (_hasFoundGameFont && _defaultTMPMaterial != null) return;
            if (_searchAttempt >= 20) return; 

            if (Time.time - _lastSearchTime > 5.0f)
                SearchFonts();
        }

        private static void InitializeReflection()
        {
            if (_reflectionInitialized) return;

            try
            {
                var ueAsm = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "UnityEngine.UIElementsModule");

                if (ueAsm != null)
                {
                    _fontDefinitionType = ueAsm.GetType("UnityEngine.UIElements.FontDefinition");
                    _styleFontDefinitionType = ueAsm.GetType("UnityEngine.UIElements.StyleFontDefinition");

                    if (_fontDefinitionType != null)
                    {
                        _fontDefFromFontMethod = _fontDefinitionType.GetMethod("FromFont",
                            BindingFlags.Public | BindingFlags.Static);
                        _fontDefFromSDFFontMethod = _fontDefinitionType.GetMethod("FromSDFFont",
                            BindingFlags.Public | BindingFlags.Static);
                    }

                    if (_styleFontDefinitionType != null && _fontDefinitionType != null)
                    {
                        _styleFontDefCtor = _styleFontDefinitionType.GetConstructor(new[] { _fontDefinitionType });
                    }

                    MelonLogger.Msg($"[FontLoader] Reflection: FromFont={_fontDefFromFontMethod != null}, FromSDFFont={_fontDefFromSDFFontMethod != null}");
                }
                else
                {
                    MelonLogger.Warning("[FontLoader] UnityEngine.UIElementsModule not found — UI Toolkit font support unavailable");
                }

                _reflectionInitialized = true;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] Reflection init failed: {ex.Message}");
            }
        }

        /// <summary>Primary strategy: direct Resources.Load on known paths. Scene-independent.</summary>
        private static void LoadTMPFontAssetDirect()
        {
            if (_defaultTMPFontAsset != null) return;

            foreach (var path in KnownTMPFontPaths)
            {
                try
                {
                    var asset = Resources.Load<TMP_FontAsset>(path);
                    if (asset == null) continue;

                    MelonLogger.Msg($"[FontLoader] TMP_FontAsset loaded direct: '{asset.name}' from '{path}'");
                    _defaultTMPFontAsset = asset;
                    _hasFoundGameFont = true;

                    if (_defaultUGUIFont == null && asset.sourceFontFile != null)
                    {
                        _defaultUGUIFont = asset.sourceFontFile;
                        MelonLogger.Msg($"[FontLoader] UGUI font from sourceFontFile: '{_defaultUGUIFont.name}'");
                    }

                    return;
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[FontLoader] Direct load '{path}' failed: {ex.Message}");
                }
            }
        }

        private static void FindFontsFromTMPText()
        {
            try
            {
                var tmpTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
                if (tmpTexts.Length == 0) return;

                foreach (var txt in tmpTexts)
                {
                    if (txt == null) continue;

                    // Capture a working template from the game scene
                    if (_tmpTemplate == null && txt.gameObject.activeInHierarchy)
                    {
                        _tmpTemplate = txt;
                        MelonLogger.Msg($"[FontLoader]   Captured TMP template from: '{txt.gameObject.name}' (Layer: {txt.gameObject.layer})");
                    }

                    var fontAsset = txt.font;
                    if (fontAsset == null) continue;

                    // Capture font asset if we don't have one
                    if (_defaultTMPFontAsset == null)
                    {
                        _defaultTMPFontAsset = fontAsset;
                        _hasFoundGameFont = true;
                        MelonLogger.Msg($"[FontLoader]   TMP font from scene: '{fontAsset.name}'");
                    }

                    // CRITICAL: Capture material from a real scene object even if we already have the font asset
                    if (_defaultTMPMaterial == null && txt.fontSharedMaterial != null)
                    {
                        _defaultTMPMaterial = txt.fontSharedMaterial;
                        MelonLogger.Msg($"[FontLoader]   TMP material captured from scene: '{_defaultTMPMaterial.name}'");
                    }

                    if (_defaultUGUIFont == null && fontAsset.sourceFontFile != null)
                        _defaultUGUIFont = fontAsset.sourceFontFile;
                        
                    // If we have both now, we can stop iterating
                    if (_defaultTMPFontAsset != null && _defaultTMPMaterial != null) break;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] TextMeshProUGUI search failed: {ex.Message}");
            }
        }

        private static void FindTMPFontAssets()
        {
            try
            {
                var tmpFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                MelonLogger.Msg($"[FontLoader] Found {tmpFonts.Count} TMP_FontAsset(s)");

                foreach (var tmp in tmpFonts)
                {
                    if (tmp == null) continue;
                    var name = tmp.name;

                    if (_defaultTMPFontAsset == null)
                    {
                        _defaultTMPFontAsset = tmp;
                        _hasFoundGameFont = true;
                        MelonLogger.Msg($"[FontLoader]   TMP_FontAsset (scan): '{name}'");
                    }

                    if (_defaultUGUIFont == null && tmp.sourceFontFile != null)
                        _defaultUGUIFont = tmp.sourceFontFile;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] TMP_FontAsset search failed: {ex.Message}");
            }
        }

        private static void FindGameFonts()
        {
            try
            {
                var fonts = Resources.FindObjectsOfTypeAll<Font>();
                MelonLogger.Msg($"[FontLoader] Found {fonts.Length} UnityEngine.Font(s)");

                foreach (var font in fonts)
                {
                    if (font == null) continue;
                    var name = font.name ?? "Unnamed";
                    MelonLogger.Msg($"[FontLoader]   Font: '{name}'");

                    if (_defaultUGUIFont == null)
                    {
                        _defaultUGUIFont = font;
                        if (name != "LegacyRuntime")
                            _hasFoundGameFont = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] Font search failed: {ex.Message}");
            }
        }

        private static void FindGameFontAssets()
        {
            try
            {
                var assets = Resources.FindObjectsOfTypeAll<FontAsset>();
                MelonLogger.Msg($"[FontLoader] Found {assets.Length} TextCore FontAsset(s)");

                foreach (var asset in assets)
                {
                    if (asset == null) continue;
                    var name = asset.name ?? "Unnamed";
                    if (!_fontAssetCache.ContainsKey(name))
                    {
                        _fontAssetCache[name] = asset;
                        MelonLogger.Msg($"[FontLoader]   TextCore FontAsset: '{name}'");
                    }

                    if (_defaultFontAsset == null)
                        _defaultFontAsset = asset;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] FontAsset search failed: {ex.Message}");
            }
        }

        private static void SetupDefaultFonts()
        {
            // UGUI fallback if nothing else worked
            if (_defaultUGUIFont == null)
            {
                try
                {
                    _defaultUGUIFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    if (_defaultUGUIFont != null)
                        MelonLogger.Msg("[FontLoader] UGUI fallback: LegacyRuntime.ttf");
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[FontLoader] Built-in UGUI font failed: {ex.Message}");
                }
            }

            MelonLogger.Msg(_defaultTMPFontAsset != null
                ? $"[FontLoader] ✅ TMP font: '{_defaultTMPFontAsset.name}'"
                : "[FontLoader] ❌ No TMP_FontAsset found");

            MelonLogger.Msg(_defaultUGUIFont != null
                ? $"[FontLoader] ✅ UGUI font: '{_defaultUGUIFont.name}' (gameFont={_hasFoundGameFont})"
                : "[FontLoader] ❌ No UGUI font found — text may be invisible");

            // If no TextCore FontAsset exists in the game, dynamically build one from the legacy Font.
            // UI Toolkit's FromSDFFont needs a TextCore FontAsset — this gives crisp text instead of
            // FromFont's legacy path which renders blank in Unity 6 UIElements.
            if (_defaultFontAsset == null && _defaultUGUIFont != null)
            {
                try
                {
                    _defaultFontAsset = FontAsset.CreateFontAsset(_defaultUGUIFont);
                    if (_defaultFontAsset != null)
                    {
                        _fontAssetCache[_defaultUGUIFont.name] = _defaultFontAsset;
                        MelonLogger.Msg($"[FontLoader] ✅ Built TextCore FontAsset from '{_defaultUGUIFont.name}'");
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[FontLoader] FontAsset.CreateFontAsset failed: {ex.Message}");
                }
            }

            // Build UI Toolkit FontDefinition
            if (_defaultFontDefinition == null)
            {
                // Prefer SDF for crisp text. In Unity 6000+ TMP_FontAsset inherits from FontAsset
                // at the IL2CPP level, but Il2CppInterop 1.5.x exposes them as unrelated C# types.
                // Use Il2CppObjectBase.TryCast<T>() to perform a proper IL2CPP cast.
                if (_defaultTMPFontAsset != null && _fontDefFromSDFFontMethod != null)
                {
                    try
                    {
                        var asFontAsset = _defaultTMPFontAsset?.TryCast<FontAsset>();
                        if (asFontAsset != null)
                        {
                            _defaultFontDefinition = _fontDefFromSDFFontMethod.Invoke(null, new object[] { asFontAsset });
                            MelonLogger.Msg("[FontLoader] ✅ UI Toolkit FontDefinition via FromSDFFont (TryCast)");
                        }
                        else
                        {
                            MelonLogger.Warning("[FontLoader] TMP_FontAsset.TryCast<FontAsset>() returned null");
                        }
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning($"[FontLoader] FromSDFFont failed: {ex.Message}");
                    }
                }

                // Fallback: legacy Font
                if (_defaultFontDefinition == null && _defaultUGUIFont != null && _fontDefFromFontMethod != null)
                {
                    try
                    {
                        _defaultFontDefinition = _fontDefFromFontMethod.Invoke(null, new object[] { _defaultUGUIFont });
                        MelonLogger.Msg("[FontLoader] ✅ UI Toolkit FontDefinition via FromFont");
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning($"[FontLoader] FromFont failed: {ex.Message}");
                    }
                }
            }
        }

        // ---- Public API ----

        public static TMP_FontAsset? GetTMPFontAsset()
        {
            if (_defaultTMPFontAsset == null) SearchFonts();
            return _defaultTMPFontAsset;
        }

        public static Material? GetTMPMaterial()
        {
            if (_defaultTMPMaterial == null) SearchFonts();
            return _defaultTMPMaterial;
        }

        public static TextMeshProUGUI? GetTMPTemplate()
        {
            if (_tmpTemplate == null) SearchFonts();
            return _tmpTemplate;
        }

        public static Font? GetUGUIFont()
        {
            if (_defaultUGUIFont == null) SearchFonts();
            return _defaultUGUIFont;
        }

        public static object? GetFontDefinition()
        {
            if (_defaultFontDefinition == null) SearchFonts();
            return _defaultFontDefinition;
        }

        public static object? CreateFontDefinition(Font font)
        {
            if (font == null) return null;
            if (_fontDefFromFontMethod == null) return null;
            try { return _fontDefFromFontMethod.Invoke(null, new object[] { font }); }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] CreateFontDefinition failed: {ex.Message}");
                return null;
            }
        }

        public static object? CreateStyleFontDefinition(Font font)
        {
            var fontDef = CreateFontDefinition(font);
            if (fontDef == null || _styleFontDefCtor == null) return null;
            return _styleFontDefCtor.Invoke(new object[] { fontDef });
        }

        public static FontAsset? LoadFontAsset(string fontName)
        {
            if (_defaultUGUIFont == null) SearchFonts();
            if (string.IsNullOrEmpty(fontName)) return GetDefaultFontAsset();
            if (_fontAssetCache.TryGetValue(fontName, out var cached)) return cached;
            if (_fontAssetCache.TryGetValue(fontName.ToLowerInvariant(), out cached)) return cached;
            return GetDefaultFontAsset();
        }

        public static FontAsset? LoadDefaultFontAsset() => GetDefaultFontAsset();

        /// <summary>
        /// Applies the resolved UI Toolkit font to a VisualElement's style.
        /// Children inherit -unity-font-definition, so calling this on a root
        /// element is enough to make all descendant Labels render text.
        /// Uses typed C# bindings (Il2CppInterop reflection on IStyle properties is unreliable).
        /// </summary>
        public static bool ApplyFontTo(VisualElement element)
        {
            if (element == null) return false;
            if (_defaultUGUIFont == null && _defaultTMPFontAsset == null) SearchFonts();

            bool any = false;

            // 1) unityFontDefinition (preferred — UI Toolkit's primary font slot in Unity 2022+)
            try
            {
                FontDefinition fd = default;
                bool fdValid = false;
                string fdSource = "";

                // Preferred: TextCore FontAsset (SDF) — built dynamically from legacy Font in SetupDefaultFonts.
                if (_defaultFontAsset != null)
                {
                    fd = FontDefinition.FromSDFFont(_defaultFontAsset);
                    fdValid = true;
                    fdSource = $"SDF FontAsset '{_defaultFontAsset.name}'";
                }
                // TMP fallback (rarely works because TryCast<FontAsset> typically fails on IL2CPP)
                else if (_defaultTMPFontAsset != null)
                {
                    var asFontAsset = _defaultTMPFontAsset?.TryCast<FontAsset>();
                    if (asFontAsset != null)
                    {
                        fd = FontDefinition.FromSDFFont(asFontAsset);
                        fdValid = true;
                        fdSource = $"TMP cast '{_defaultTMPFontAsset.name}'";
                    }
                }
                // Last resort: legacy Font (may render blank in Unity 6 UIElements)
                if (!fdValid && _defaultUGUIFont != null)
                {
                    fd = FontDefinition.FromFont(_defaultUGUIFont);
                    fdValid = true;
                    fdSource = $"legacy Font '{_defaultUGUIFont.name}'";
                }

                if (fdValid)
                {
                    element.style.unityFontDefinition = new StyleFontDefinition(fd);
                    MelonLogger.Msg($"[FontLoader] ApplyFontTo: unityFontDefinition set via {fdSource}.");
                    any = true;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] ApplyFontTo: unityFontDefinition failed: {ex.Message}");
            }

            // 2) unityFont (legacy fallback — some IL2CPP builds ignore unityFontDefinition for non-SDF fonts)
            try
            {
                if (_defaultUGUIFont != null)
                {
                    element.style.unityFont = new StyleFont(_defaultUGUIFont);
                    if (!any) MelonLogger.Msg("[FontLoader] ApplyFontTo: unityFont (legacy) set.");
                    any = true;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] ApplyFontTo: unityFont failed: {ex.Message}");
            }

            if (!any)
                MelonLogger.Warning("[FontLoader] ApplyFontTo: no font slot could be set.");
            return any;
        }

        public static FontAsset? GetDefaultFontAsset()
        {
            if (_defaultFontAsset == null) SearchFonts();
            return _defaultFontAsset;
        }

        public static void Unload()
        {
            _fontAssetCache.Clear();
            _defaultFontAsset = null;
            _defaultTMPFontAsset = null;
            _defaultUGUIFont = null;
            _defaultFontDefinition = null;
            _hasFoundGameFont = false;
            _searchAttempt = 0;
        }
    }
}
