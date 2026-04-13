using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using TMPro;
using greg.Core;

namespace greg.Core.UI.Components;

public class GregMainMenuReplacement : MonoBehaviour
{
    public static GregMainMenuReplacement Instance { get; private set; }

    private GameObject _menuRoot;
    private Canvas _canvas;
    private bool _isVisible = true;
    private Vector2 _baseResolution = new Vector2(1920, 1080);
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

    public void Configure(
        Action onPlay,
        Action onSettings,
        Action onMods,
        Action onQuit)
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

    private float EaseOutExpo(float t)
    {
        return t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
    }

    private void BuildMenu()
    {
        _scaleFactor = CalculateScaleFactor();

        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 999;

        gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        _menuRoot = new GameObject("GregMainMenu");
        _menuRoot.transform.SetParent(transform, false);

        var rootRect = _menuRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.sizeDelta = Vector2.zero;

        var canvasGroup = _menuRoot.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        BuildBackground(rootRect);
        BuildLogo(rootRect);
        BuildMenuButtons(rootRect);
        BuildVersionInfo(rootRect);
        BuildAmbientEffects(rootRect);
    }

    private float CalculateScaleFactor()
    {
        float screenHeight = Screen.height;
        return screenHeight / _baseResolution.y;
    }

    private void BuildBackground(RectTransform parent)
    {
        var bg = new GameObject("Background");
        bg.transform.SetParent(parent, false);

        var rect = bg.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        var img = bg.AddComponent<Image>();
        img.color = GregUITheme.Surface;

        var vignette = new GameObject("Vignette");
        vignette.transform.SetParent(parent, false);
        var vigRect = vignette.AddComponent<RectTransform>();
        vigRect.anchorMin = Vector2.zero;
        vigRect.anchorMax = Vector2.one;
        vigRect.sizeDelta = Vector2.zero;

        var vigImg = vignette.AddComponent<Image>();
        vigImg.color = new Color(0f, 0.07f, 0.06f, 0.95f);
    }

    private void BuildLogo(RectTransform parent)
    {
        var logoContainer = new GameObject("LogoContainer");
        logoContainer.transform.SetParent(parent, false);

        var rect = logoContainer.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.7f);
        rect.anchorMax = new Vector2(0.5f, 0.7f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600 * _scaleFactor, 120 * _scaleFactor);
        rect.anchoredPosition = Vector2.zero;

        var titleText = CreateTextMeshPro(logoContainer, "GREG", new Vector2(0, 0), rect.sizeDelta);
        titleText.fontSize = 72 * _scaleFactor;
        titleText.fontStyle = FontStyles.Bold;
        titleText.characterSpacing = 8;
        titleText.color = GregUITheme.Primary;
        titleText.alignment = TextAlignmentOptions.Center;

        AddTextGlow(titleText, GregUITheme.Primary, 32f);
    }

    private void BuildMenuButtons(RectTransform parent)
    {
        var buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(parent, false);

        var rect = buttonContainer.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(320 * _scaleFactor, 400 * _scaleFactor);
        rect.anchoredPosition = new Vector2(0, -20 * _scaleFactor);

        var layout = buttonContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 16f * _scaleFactor;
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = buttonContainer.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        float btnWidth = 280f * _scaleFactor;
        float btnHeight = 56f * _scaleFactor;

        CreateMenuButton("PLAY", GregButtonStyle.Primary, btnWidth, btnHeight, () => _onPlayClicked?.Invoke(), buttonContainer.transform);
        CreateMenuButton("SETTINGS", GregButtonStyle.Secondary, btnWidth, btnHeight, () => _onSettingsClicked?.Invoke(), buttonContainer.transform);
        CreateMenuButton("MODS", GregButtonStyle.Secondary, btnWidth, btnHeight, () => _onModsClicked?.Invoke(), buttonContainer.transform);
        CreateMenuButton("QUIT", GregButtonStyle.Danger, btnWidth, btnHeight, () => _onQuitClicked?.Invoke(), buttonContainer.transform);
    }

    private void CreateMenuButton(string label, GregButtonStyle style, float width, float height, Action onClick, Transform parent)
    {
        var btnGo = new GameObject($"Button_{label}");
        btnGo.transform.SetParent(parent, false);

        var rect = btnGo.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);

        var img = btnGo.AddComponent<Image>();
        ApplyButtonStyle(img, style);

        var btn = btnGo.AddComponent<Button>();
        btn.transition = Selectable.Transition.None;

        var colors = btn.colors;
        colors.pressedColor = ApplyBrightness(GetStyleColor(style), 0.8f);
        colors.highlightColor = ApplyBrightness(GetStyleColor(style), 1.2f);
        btn.colors = colors;

        btn.onClick.AddListener(new Action(() => {
            PlayButtonSound();
            onClick?.Invoke();
        }));

        var content = new GameObject("Content");
        content.transform.SetParent(btnGo.transform, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;

        var text = CreateTextMeshPro(content, label, Vector2.zero, Vector2.zero);
        text.fontSize = 18 * _scaleFactor;
        text.fontStyle = FontStyles.Bold;
        text.characterSpacing = 4;
        text.color = GetStyleTextColor(style);
        text.alignment = TextAlignmentOptions.Center;
    }

    private void ApplyButtonStyle(Image img, GregButtonStyle style)
    {
        img.color = style switch
        {
            GregButtonStyle.Primary => GregUITheme.Primary,
            GregButtonStyle.Secondary => new Color(0f, 0.15f, 0.14f, 0.9f),
            GregButtonStyle.Danger => new Color(0.93f, 0.26f, 0.27f, 0.9f),
            _ => GregUITheme.Primary
        };
    }

    private Color GetStyleColor(GregButtonStyle style)
    {
        return style switch
        {
            GregButtonStyle.Primary => GregUITheme.Primary,
            GregButtonStyle.Secondary => new Color(0f, 0.15f, 0.14f, 0.9f),
            GregButtonStyle.Danger => new Color(0.93f, 0.26f, 0.27f, 0.9f),
            _ => GregUITheme.Primary
        };
    }

    private Color GetStyleTextColor(GregButtonStyle style)
    {
        return style switch
        {
            GregButtonStyle.Primary => GregUITheme.OnPrimary,
            GregButtonStyle.Secondary => GregUITheme.OnSurface,
            GregButtonStyle.Danger => Color.white,
            _ => GregUITheme.OnPrimary
        };
    }

    private Color ApplyBrightness(Color color, float factor)
    {
        return new Color(
            Mathf.Clamp01(color.r * factor),
            Mathf.Clamp01(color.g * factor),
            Mathf.Clamp01(color.b * factor),
            color.a
        );
    }

    private void BuildVersionInfo(RectTransform parent)
    {
        var versionGo = new GameObject("VersionInfo");
        versionGo.transform.SetParent(parent, false);

        var rect = versionGo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(400 * _scaleFactor, 30 * _scaleFactor);
        rect.anchoredPosition = new Vector2(20 * _scaleFactor, 15 * _scaleFactor);

        var versionText = CreateTextMeshPro(versionGo, $"gregCore v{gregReleaseVersion.Current} <color=#61F4D8>[teamGreg]</color>", Vector2.zero, rect.sizeDelta);
        versionText.fontSize = 12 * _scaleFactor;
        versionText.color = new Color(0.38f, 0.96f, 0.85f, 0.6f);
        versionText.alignment = TextAlignmentOptions.Left;
    }

    private void BuildAmbientEffects(RectTransform parent)
    {
        var glow = new GameObject("AmbientGlow");
        glow.transform.SetParent(parent, false);

        var rect = glow.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(800 * _scaleFactor, 600 * _scaleFactor);
        rect.anchoredPosition = new Vector2(0, -50 * _scaleFactor);

        var img = glow.AddComponent<Image>();
        img.color = new Color(0.38f, 0.96f, 0.85f, 0.03f);
    }

    private TextMeshProUGUI CreateTextMeshPro(GameObject parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        var textGo = new GameObject("Text");
        textGo.transform.SetParent(parent.transform, false);

        var rect = textGo.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = sizeDelta;
        rect.anchoredPosition = anchoredPosition;

        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.font = GetTMPFont();
        tmp.enableWordWrapping = false;
        tmp.richText = true;

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
        text.fontMaterial = new Material(text.fontMaterial);
        text.fontMaterial.EnableKeyword("GLOW_ON");
        text.fontMaterial.SetColor("_GlowColor", glowColor);
        text.fontMaterial.SetFloat("_GlowOuter", blur / 100f);
        text.fontMaterial.SetFloat("_GlowPower", 0.8f);
    }

    private void PlayButtonSound()
    {
        // TODO: Play UI click sound
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
