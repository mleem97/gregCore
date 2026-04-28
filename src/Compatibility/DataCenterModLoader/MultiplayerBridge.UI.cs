using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataCenterModLoader
{
    public partial class MultiplayerBridge
    {
        private void BuildUI()
        {
            _panelRoot = new VisualElement
            {
                name = "MultiplayerPanel",
                style =
                {
                    position = Position.Absolute,
                    top = (Screen.height - 420) / 2,
                    left = (Screen.width - 400) / 2,
                    width = 400,
                    height = 420,
                    backgroundColor = new Color(0.12f, 0.12f, 0.15f, 0.95f),
                    borderTopColor = new Color(0f, 0.6f, 0.7f),
                    borderBottomColor = new Color(0f, 0.6f, 0.7f),
                    borderLeftColor = new Color(0f, 0.6f, 0.7f),
                    borderRightColor = new Color(0f, 0.6f, 0.7f),
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderRadius = 8,
                    flexDirection = FlexDirection.Column,
                    paddingTop = 15,
                    paddingBottom = 15,
                    paddingLeft = 25,
                    paddingRight = 25,
                    display = DisplayStyle.None
                }
            };

            // Title
            var title = new Label("MULTIPLAYER")
            {
                style =
                {
                    fontSize = 22,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = Color.white,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginBottom = 30
                }
            };
            _panelRoot.Add(title);

            // Room code section
            var codeLabel = new Label("ROOM CODE (UPPERCASE)")
            {
                style = { fontSize = 16, color = new Color(0.8f, 0.8f, 0.8f), marginBottom = 8 }
            };
            _panelRoot.Add(codeLabel);

            var codeField = new TextField
            {
                value = _roomCode ?? "",
                style =
                {
                    backgroundColor = new Color(0.2f, 0.2f, 0.25f),
                    color = Color.white,
                    fontSize = 16,
                    height = 40,
                    marginBottom = 20
                }
            };
            codeField.RegisterValueChangedCallback(evt =>
            {
                _roomCode = evt.newValue;
                _roomCodeFieldFocused = !string.IsNullOrEmpty(evt.newValue);
            });
            _panelRoot.Add(codeField);

            // Join button
            var joinBtn = new Button(() =>
            {
                _roomCodeFieldFocused = false;
                DoConnect();
            })
            {
                text = "JOIN GAME",
                style =
                {
                    backgroundColor = new Color(0f, 0.6f, 0.7f),
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 50,
                    fontSize = 18,
                    marginBottom = 20,
                    borderRadius = 4
                }
            };
            _panelRoot.Add(joinBtn);

            // Separator
            var separator = new VisualElement
            {
                style =
                {
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f),
                    height = 1,
                    marginBottom = 20
                }
            };
            _panelRoot.Add(separator);

            // Host button
            var hostBtn = new Button(() =>
            {
                _roomCodeFieldFocused = false;
                DoHost();
            })
            {
                text = "HOST GAME",
                style =
                {
                    backgroundColor = new Color(0f, 0.6f, 0.7f),
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 50,
                    fontSize = 18,
                    borderRadius = 4
                }
            };
            _panelRoot.Add(hostBtn);

            // Close button
            var closeBtn = new Button(() => HideMultiplayerPanel())
            {
                text = "X",
                style =
                {
                    position = Position.Absolute,
                    top = 10,
                    right = 10,
                    width = 25,
                    height = 25,
                    backgroundColor = Color.clear,
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0
                }
            };
            _panelRoot.Add(closeBtn);

            // Register with GregUIManager
            GregUIManager.RegisterPanel("MultiplayerPanel", _panelRoot);
        }

        private void UpdateUI()
        {
            if (_panelRoot == null) return;

            bool connected = _isConnected() != 0;
            _panelRoot.Clear();

            if (!connected)
            {
                BuildUI();
            }
            else
            {
                BuildConnectedUI();
            }
        }

        private void BuildConnectedUI()
        {
            if (_panelRoot == null) return;

            _panelRoot.Clear();

            // Status
            var status = new Label(_isHosting ? "HOSTING SESSION" : "CONNECTED TO SESSION")
            {
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = new Color(0f, 0.9f, 0.6f),
                    marginBottom = 20
                }
            };
            _panelRoot.Add(status);

            // Room code display
            string codeToDisplay = _isHosting ? _displayRoomCode : _roomCode;
            var codeLabel = new Label($"ROOM: {codeToDisplay}")
            {
                style = { fontSize = 16, color = new Color(0.8f, 0.8f, 0.8f), marginBottom = 20 }
            };
            _panelRoot.Add(codeLabel);

            // Player count
            uint players = _getPlayerCount != null ? _getPlayerCount() : 1;
            var playerLabel = new Label($"Players: {players}")
            {
                style = { fontSize = 16, color = new Color(0.8f, 0.8f, 0.8f), marginBottom = 30 }
            };
            _panelRoot.Add(playerLabel);

            // Stop/Disconnect button
            var actionBtn = new Button(() =>
            {
                if (_isHosting) DoStopHosting();
                else DoDisconnect();
            })
            {
                text = _isHosting ? "STOP HOSTING" : "DISCONNECT",
                style =
                {
                    backgroundColor = new Color(0.7f, 0.2f, 0.2f),
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 50,
                    fontSize = 18,
                    borderRadius = 4
                }
            };
            _panelRoot.Add(actionBtn);

            // Close button
            var closeBtn = new Button(() => HideMultiplayerPanel())
            {
                text = "X",
                style =
                {
                    position = Position.Absolute,
                    top = 10,
                    right = 10,
                    width = 25,
                    height = 25,
                    backgroundColor = Color.clear,
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0
                }
            };
            _panelRoot.Add(closeBtn);
        }

        public void DrawGUI()
        {
            if (!_showPanel) return;

            if (!_stylesInitialized)
            {
                BuildUI();
                _stylesInitialized = true;
            }

            _panelRoot!.style.display = DisplayStyle.Flex;
        }

        private void HideMultiplayerPanel()
        {
            _showPanel = false;
            if (_panelRoot != null)
                _panelRoot.style.display = DisplayStyle.None;
        }

        private void ShowMultiplayerPanel()
        {
            _showPanel = true;
            if (_panelRoot == null)
            {
                BuildUI();
            }
            _panelRoot!.style.display = DisplayStyle.Flex;
        }

        private void InitStyles() { } // No longer needed - UI Toolkit handles styling
    }
}
