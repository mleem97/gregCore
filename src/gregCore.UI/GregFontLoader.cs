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
    public static partial class GregFontLoader
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

        // UGUI/Toolkit font for mod UIs (LegacyRuntime from the built-in
        // Resources or game font, once found). Null until SearchFonts
        // succeeded - callers must be null-tolerant.
        public static Font? DefaultUGUIFont => _defaultUGUIFont;

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
                    try
                    {
                        if (txt == null) continue;
                        CaptureTemplate(txt);
                        CaptureFontFromText(txt);
                        if (_defaultTMPFontAsset != null && _defaultTMPMaterial != null) break;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] TextMeshProUGUI search failed: {ex.Message}");
            }
        }

        private static void CaptureTemplate(TextMeshProUGUI txt)
        {
            try
            {
                if (_tmpTemplate != null) return;
                if (!txt.gameObject.activeInHierarchy) return;
                _tmpTemplate = txt;
                MelonLogger.Msg($"[FontLoader]   Captured TMP template from: '{txt.gameObject.name}' (Layer: {txt.gameObject.layer})");
            }
            catch { }
        }

        private static void CaptureFontFromText(TextMeshProUGUI txt)
        {
            try
            {
                var fontAsset = txt.font;
                if (fontAsset == null) return;
                if (_defaultTMPFontAsset == null)
                {
                    _defaultTMPFontAsset = fontAsset;
                    _hasFoundGameFont = true;
                    MelonLogger.Msg($"[FontLoader]   TMP font from scene: '{fontAsset.name}'");
                }
                CaptureMaterial(txt);
                if (_defaultUGUIFont == null && fontAsset.sourceFontFile != null)
                    _defaultUGUIFont = fontAsset.sourceFontFile;
            }
            catch { }
        }

        private static void CaptureMaterial(TextMeshProUGUI txt)
        {
            try
            {
                if (_defaultTMPMaterial != null) return;
                if (txt.fontSharedMaterial == null) return;
                _defaultTMPMaterial = txt.fontSharedMaterial;
                MelonLogger.Msg($"[FontLoader]   TMP material captured from scene: '{_defaultTMPMaterial.name}'");
            }
            catch { }
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
            EnsureUguiFallback();
            LogFontStatus();
            EnsureTextCoreAsset();
            EnsureFontDefinition();
        }

        private static void EnsureUguiFallback()
        {
            try
            {
                if (_defaultUGUIFont != null) return;
                _defaultUGUIFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (_defaultUGUIFont != null)
                    MelonLogger.Msg("[FontLoader] UGUI fallback: LegacyRuntime.ttf");
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] Built-in UGUI font failed: {ex.Message}");
            }
        }

        private static void LogFontStatus()
        {
            try
            {
                MelonLogger.Msg(_defaultTMPFontAsset != null
                    ? $"[FontLoader] TMP font: '{_defaultTMPFontAsset.name}'"
                    : "[FontLoader] No TMP_FontAsset found");
                MelonLogger.Msg(_defaultUGUIFont != null
                    ? $"[FontLoader] UGUI font: '{_defaultUGUIFont.name}' (gameFont={_hasFoundGameFont})"
                    : "[FontLoader] No UGUI font found - text may be invisible");
            }
            catch { }
        }

        private static void EnsureTextCoreAsset()
        {
            try
            {
                if (_defaultFontAsset != null || _defaultUGUIFont == null) return;
                _defaultFontAsset = FontAsset.CreateFontAsset(_defaultUGUIFont);
                if (_defaultFontAsset != null)
                {
                    _fontAssetCache[_defaultUGUIFont.name] = _defaultFontAsset;
                    MelonLogger.Msg($"[FontLoader] Built TextCore FontAsset from '{_defaultUGUIFont.name}'");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] FontAsset.CreateFontAsset failed: {ex.Message}");
            }
        }

        private static void EnsureFontDefinition()
        {
            try
            {
                if (_defaultFontDefinition != null) return;
                TrySdfDefinition();
                TryLegacyDefinition();
            }
            catch { }
        }

        private static void TrySdfDefinition()
        {
            try
            {
                if (_defaultTMPFontAsset == null || _fontDefFromSDFFontMethod == null) return;
                if (_defaultFontDefinition != null) return;
                var asFontAsset = _defaultTMPFontAsset?.TryCast<FontAsset>();
                if (asFontAsset == null)
                {
                    MelonLogger.Msg("[FontLoader] TMP_FontAsset is no TextCore FontAsset here - using FromFont fallback.");
                    return;
                }
                _defaultFontDefinition = _fontDefFromSDFFontMethod.Invoke(null, new object[] { asFontAsset });
                MelonLogger.Msg("[FontLoader] UI Toolkit FontDefinition via FromSDFFont (TryCast)");
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] FromSDFFont failed: {ex.Message}");
            }
        }

        private static void TryLegacyDefinition()
        {
            try
            {
                if (_defaultFontDefinition != null || _defaultUGUIFont == null || _fontDefFromFontMethod == null) return;
                _defaultFontDefinition = _fontDefFromFontMethod.Invoke(null, new object[] { _defaultUGUIFont });
                MelonLogger.Msg("[FontLoader] UI Toolkit FontDefinition via FromFont");
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[FontLoader] FromFont failed: {ex.Message}");
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
