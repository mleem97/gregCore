using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Il2CppTMPro;
using MelonLoader;
using greg.Core;
using greg.Core.UI;

namespace greg.Core.UI.Components;

public class GregMainMenuReplacement : MonoBehaviour
{
    public static GregMainMenuReplacement Instance { get; private set; }

    private const int SORTING_ORDER = 999;
    private const float REFERENCE_WIDTH = 1920f;
    private const float REFERENCE_HEIGHT = 1080f;

    private GameObject _menuRoot;
    private Canvas _canvas;
    private EventSystem _eventSystem;
    private bool _isVisible = false;
    private float _scaleFactor = 1f;

    private Action _onPlayClicked;
    private Action _onSettingsClicked;
    private Action _onModsClicked;
    private Action _onQuitClicked;

    private float _animTime = 0f;
    private const float ANIM_DURATION = 0.6f;
    private bool _isAnimating = false;

    public bool IsVisible => _isVisible;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    public void Configure(Action onPlay, Action onSettings, Action onMods, Action onQuit)
    {
        _onPlayClicked = onPlay;
        _onSettingsClicked = onSettings;
        _onModsClicked = onMods;
        _onQuitClicked = onQuit;
    }

    public void Show()
    {
        if (_menuRoot == null) BuildMenu();

        _menuRoot.SetActive(true);
        _isVisible = true;
        _animTime = 0f;
        _isAnimating = true;
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EnsureEventSystem();
    }

    public void Hide()
    {
        _isVisible = false;
        _isAnimating = true;
        _animTime = 0f;
    }

    private void Update()
    {
        if (_isAnimating)
        {
            _animTime += Time.deltaTime;
            float t = Mathf.Clamp01(_animTime / ANIM_DURATION);
            float eased = EaseOutExpo(t);

            var canvasGroup = _menuRoot?.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = _isVisible ? eased : 1f - eased;
            }

            if (t >= 1f)
            {
                _isAnimating = false;
                if (!_isVisible)
                {
                    _menuRoot?.SetActive(false);
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private float EaseOutExpo(float t) => t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);

    private void EnsureEventSystem()
    {
        if (_eventSystem != null) return;

        _eventSystem = FindObjectOfType<EventSystem>();
        if (_eventSystem == null)
        {
            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.transform.SetParent(transform);
            _eventSystem = AddComponentSafe<EventSystem>(eventSystemGo);
            AddComponentSafe<StandaloneInputModule>(eventSystemGo);
            MelonLogger.Msg("[GregMainMenu] EventSystem created.");
        }
    }

    private static T AddComponentSafe<T>(GameObject go) where T : Component
    {
        return AddComponentSafe(go, typeof(T)) as T;
    }

    private static Component AddComponentSafe(GameObject go, Type componentType)
    {
        try
        {
            var method = typeof(GameObject).GetMethod("AddComponent", new Type[] { typeof(Type) });
            return method?.Invoke(go, new object[] { componentType }) as Component;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregMainMenu] AddComponentSafe failed: {ex.Message}");
            return null;
        }
    }

    private void BuildMenu()
    {
        try
        {
            _scaleFactor = CalculateScaleFactor();

            _canvas = AddComponentSafe<Canvas>(gameObject);
            if (_canvas != null)
            {
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = SORTING_ORDER;
            }

            var scaler = AddComponentSafe<CanvasScaler>(gameObject);
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(REFERENCE_WIDTH, REFERENCE_HEIGHT);
                scaler.matchWidthOrHeight = 0.5f;
            }

            AddComponentSafe<GraphicRaycaster>(gameObject);

            _menuRoot = new GameObject("GregMainMenu");
            _menuRoot.transform.SetParent(transform, false);

            var rootRect = AddComponentSafe<RectTransform>(_menuRoot);
            if (rootRect != null)
            {
                rootRect.anchorMin = Vector2.zero;
                rootRect.anchorMax = Vector2.one;
                rootRect.offsetMin = Vector2.zero;
                rootRect.offsetMax = Vector2.zero;
            }

            var canvasGroup = AddComponentSafe<CanvasGroup>(_menuRoot);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            BuildBackground();
            BuildLogo();
            BuildMenuButtons();
            BuildVersionInfo();
            BuildAmbientEffects();

            EnsureEventSystem();

            MelonLogger.Msg("[GregMainMenu] Menu built successfully.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregMainMenu] BuildMenu failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private float CalculateScaleFactor() => Screen.height / REFERENCE_HEIGHT;

    private void BuildBackground()
    {
        var bg = new GameObject("Background");
        bg.transform.SetParent(_menuRoot.transform, false);

        var bgRect = AddComponentSafe<RectTransform>(bg);
        if (bgRect != null) {
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
        }

        var img = AddComponentSafe<Image>(bg);
        if (img != null) img.color = GregUITheme.Surface;

        var vignette = new GameObject("Vignette");
        vignette.transform.SetParent(_menuRoot.transform, false);
        var vigRect = AddComponentSafe<RectTransform>(vignette);
        if (vigRect != null) {
            vigRect.anchorMin = Vector2.zero;
            vigRect.anchorMax = Vector2.one;
            vigRect.offsetMin = Vector2.zero;
            vigRect.offsetMax = Vector2.zero;
        }

        var vigImg = AddComponentSafe<Image>(vignette);
        if (vigImg != null) vigImg.color = new Color(0f, 0.07f, 0.06f, 0.95f);
    }

    private void BuildLogo()
    {
        var logoContainer = new GameObject("LogoContainer");
        logoContainer.transform.SetParent(_menuRoot.transform, false);

        var logoRect = AddComponentSafe<RectTransform>(logoContainer);
        if (logoRect != null)
        {
            logoRect.anchorMin = new Vector2(0.5f, 0.7f);
            logoRect.anchorMax = new Vector2(0.5f, 0.85f);
            logoRect.pivot = new Vector2(0.5f, 0.5f);
            logoRect.sizeDelta = Vector2.zero;
            logoRect.anchoredPosition = Vector2.zero;
        }

        var titleText = CreateText(logoContainer, "GREG");
        if (titleText != null)
        {
            titleText.fontSize = 72 * _scaleFactor;
            titleText.fontStyle = FontStyles.Bold;
            titleText.characterSpacing = 8;
            titleText.color = GregUITheme.Primary;
            titleText.alignment = TextAlignmentOptions.Center;
            AddTextGlow(titleText, GregUITheme.Primary, 32f);
        }
    }

    private void BuildMenuButtons()
    {
        var buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(_menuRoot.transform, false);

        var buttonRect = AddComponentSafe<RectTransform>(buttonContainer);
        if (buttonRect != null)
        {
            buttonRect.anchorMin = new Vector2(0.5f, 0.3f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.55f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.sizeDelta = new Vector2(320 * _scaleFactor, 0);
            buttonRect.anchoredPosition = Vector2.zero;
        }

        var layout = AddComponentSafe<VerticalLayoutGroup>(buttonContainer);
        if (layout != null)
        {
            layout.spacing = 16f * _scaleFactor;
            layout.padding = new RectOffset();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        var fitter = AddComponentSafe<ContentSizeFitter>(buttonContainer);
        if (fitter != null)
        {
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        float btnHeight = 56f * _scaleFactor;

        CreateMenuButton("PLAY", GregButtonStyle.Primary, btnHeight, () => _onPlayClicked?.Invoke());
        CreateMenuButton("SETTINGS", GregButtonStyle.Secondary, btnHeight, () => _onSettingsClicked?.Invoke());
        CreateMenuButton("MODS", GregButtonStyle.Secondary, btnHeight, () => _onModsClicked?.Invoke());
        CreateMenuButton("QUIT", GregButtonStyle.Danger, btnHeight, () => _onQuitClicked?.Invoke());
    }

    private void CreateMenuButton(string label, GregButtonStyle style, float height, Action onClick)
    {
        var btnGo = new GameObject($"Button_{label}");
        btnGo.transform.SetParent(_menuRoot.transform, false);

        var btnRect = AddComponentSafe<RectTransform>(btnGo);
        if (btnRect != null)
        {
            btnRect.sizeDelta = new Vector2(0, height);
        }

        var img = AddComponentSafe<Image>(btnGo);
        ApplyButtonStyle(img, style);

        var btn = AddComponentSafe<Button>(btnGo);
        if (btn != null)
        {
            btn.transition = Selectable.Transition.None;

            var colors = btn.colors;
            colors.pressedColor = ApplyBrightness(GetStyleColor(style), 0.8f);
            colors.highlightedColor = ApplyBrightness(GetStyleColor(style), 1.2f);
            btn.colors = colors;

            btn.onClick.AddListener((UnityAction)(() => {
                PlayButtonSound();
                onClick?.Invoke();
            }));
        }

        var content = new GameObject("Content");
        content.transform.SetParent(btnGo.transform, false);
        var contentRect = AddComponentSafe<RectTransform>(content);
        if (contentRect != null)
        {
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
        }

        var text = CreateText(content, label);
        if (text != null)
        {
            text.fontSize = 18 * _scaleFactor;
            text.fontStyle = FontStyles.Bold;
            text.characterSpacing = 4;
            text.color = GetStyleTextColor(style);
            text.alignment = TextAlignmentOptions.Center;
        }
    }

    private void ApplyButtonStyle(Image img, GregButtonStyle style)
    {
        if (img == null) return;
        img.color = style switch
        {
            GregButtonStyle.Primary => GregUITheme.Primary,
            GregButtonStyle.Secondary => new Color(0f, 0.15f, 0.14f, 0.9f),
            GregButtonStyle.Danger => new Color(0.93f, 0.26f, 0.27f, 0.9f),
            _ => GregUITheme.Primary
        };
    }

    private Color GetStyleColor(GregButtonStyle style) => style switch
    {
        GregButtonStyle.Primary => GregUITheme.Primary,
        GregButtonStyle.Secondary => new Color(0f, 0.15f, 0.14f, 0.9f),
        GregButtonStyle.Danger => new Color(0.93f, 0.26f, 0.27f, 0.9f),
        _ => GregUITheme.Primary
    };

    private Color GetStyleTextColor(GregButtonStyle style) => style switch
    {
        GregButtonStyle.Primary => GregUITheme.OnPrimary,
        GregButtonStyle.Secondary => GregUITheme.OnSurface,
        GregButtonStyle.Danger => Color.white,
        _ => GregUITheme.OnPrimary
    };

    private Color ApplyBrightness(Color color, float factor) => new Color(
        Mathf.Clamp01(color.r * factor),
        Mathf.Clamp01(color.g * factor),
        Mathf.Clamp01(color.b * factor),
        color.a
    );

    private void BuildVersionInfo()
    {
        var versionGo = new GameObject("VersionInfo");
        versionGo.transform.SetParent(_menuRoot.transform, false);

        var versionRect = AddComponentSafe<RectTransform>(versionGo);
        if (versionRect != null)
        {
            versionRect.anchorMin = new Vector2(0, 0);
            versionRect.anchorMax = new Vector2(0, 0);
            versionRect.pivot = new Vector2(0, 0);
            versionRect.sizeDelta = new Vector2(400, 30);
            versionRect.anchoredPosition = new Vector2(20, 15);
        }

        var versionText = CreateText(versionGo, $"gregCore v{gregReleaseVersion.Current} <color=#61F4D8>[teamGreg]</color>");
        if (versionText != null)
        {
            versionText.fontSize = 12 * _scaleFactor;
            versionText.color = new Color(0.38f, 0.96f, 0.85f, 0.6f);
            versionText.alignment = TextAlignmentOptions.Left;
        }
    }

    private void BuildAmbientEffects()
    {
        var glow = new GameObject("AmbientGlow");
        glow.transform.SetParent(_menuRoot.transform, false);

        var glowRect = AddComponentSafe<RectTransform>(glow);
        if (glowRect != null)
        {
            glowRect.anchorMin = new Vector2(0.5f, 0.5f);
            glowRect.anchorMax = new Vector2(0.5f, 0.5f);
            glowRect.pivot = new Vector2(0.5f, 0.5f);
            glowRect.sizeDelta = new Vector2(800 * _scaleFactor, 600 * _scaleFactor);
            glowRect.anchoredPosition = new Vector2(0, -50 * _scaleFactor);
        }

        var glowImg = AddComponentSafe<Image>(glow);
        if (glowImg != null) glowImg.color = new Color(0.38f, 0.96f, 0.85f, 0.03f);
    }

    private TextMeshProUGUI CreateText(GameObject parent, string text)
    {
        var textGo = new GameObject("Text");
        textGo.transform.SetParent(parent.transform, false);

        var rect = AddComponentSafe<RectTransform>(textGo);
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        var tmp = AddComponentSafe<TextMeshProUGUI>(textGo);
        if (tmp != null)
        {
            tmp.text = text;
            tmp.font = GetTMPFont();
            tmp.enableWordWrapping = false;
            tmp.richText = true;
        }

        return tmp;
    }

    private TMP_FontAsset GetTMPFont()
    {
        var fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        foreach (var font in fonts)
        {
            if (font.name.Contains("Inter") || font.name.Contains("Space"))
                return font;
        }
        return fonts.Length > 0 ? fonts[0] : null;
    }

    private void AddTextGlow(TextMeshProUGUI text, Color glowColor, float blur)
    {
        if (text == null) return;
        text.fontMaterial = new Material(text.fontMaterial);
        text.fontMaterial.EnableKeyword("GLOW_ON");
        text.fontMaterial.SetColor("_GlowColor", glowColor);
        text.fontMaterial.SetFloat("_GlowOuter", blur / 100f);
        text.fontMaterial.SetFloat("_GlowPower", 0.8f);
    }

    private void PlayButtonSound() { }

    private void OnDestroy() => Instance = null;
}
