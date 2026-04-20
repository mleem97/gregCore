using System;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;
using greg.Core.UI;
using greg.Core.UI.Components;

namespace greg.Core.UI.Components;

public class GregShopReplacement : MonoBehaviour
{
    public static GregShopReplacement Instance { get; private set; }

    // private GameObject _root;
    private GregPanel _mainPanel;
    private bool _isVisible = false;

    private Action _onCloseClicked;
    private Action _onCheckoutClicked;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUI();
        gameObject.SetActive(false);
    }

    public void Configure(Action onClose, Action onCheckout)
    {
        _onCloseClicked = onClose;
        _onCheckoutClicked = onCheckout;
    }

    private void InitializeUI()
    {
        _mainPanel = GregUIBuilder.Panel("shop_replacement")
            .Title("🛒 COMPUTER SHOP")
            .Position(GregUIAnchor.Center)
            .Size(800, 600)
            .AddLabel("Select hardware components for your Data Center / Wählen Sie Hardware für Ihr Rechenzentrum", null, 14)
            .AddSeparator()
            .AddLabel("(Hardware inventory loading...)")
            .AddSeparator()
            .AddButton("CHECKOUT", () => _onCheckoutClicked?.Invoke(), GregButtonStyle.Primary)
            .AddButton("CLOSE", () => _onCloseClicked?.Invoke(), GregButtonStyle.Secondary)
            .Build();

        if (_mainPanel.PanelRoot != null)
        {
            _mainPanel.PanelRoot.transform.SetParent(this.transform, false);
        }
    }

    public void Show()
    {
        _isVisible = true;
        gameObject.SetActive(true);
        _mainPanel.Show();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        _isVisible = false;
        _mainPanel.Hide();
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (_isVisible) Hide();
        else Show();
    }
}
