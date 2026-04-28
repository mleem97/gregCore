using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace greg.Multiplayer
{
    /// <summary>
    /// UI Toolkit overlay for the Multiplayer panel.
    /// Hooked via Harmony ESC-menu Postfix (see EscMenuPatch.cs).
    /// </summary>
    public class MultiplayerHud : MonoBehaviour
    {
        private bool _visible = false;
        private string _inviteLink = string.Empty;
        private string _joinInput = string.Empty;
        private string _statusMsg = string.Empty;
        private VisualElement? _root;

        public void Toggle() => _visible = !_visible;

        private void OnEnable()
        {
            if (_root == null)
            {
                BuildUI();
            }
            _root!.style.display = _visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _visible)
            {
                Toggle();
                if (_root != null)
                    _root.style.display = DisplayStyle.None;
            }
        }

        private void BuildUI()
        {
            _root = new VisualElement
            {
                name = "MultiplayerHud",
                style =
                {
                    position = Position.Absolute,
                    top = 80,
                    left = 20,
                    width = 420,
                    height = 360,
                    flexDirection = FlexDirection.Column,
                    backgroundColor = new Color(0.07f, 0.07f, 0.07f, 0.96f),
                    borderTopColor = new Color(0f, 0.75f, 0.65f),
                    borderBottomColor = new Color(0f, 0.75f, 0.65f),
                    borderLeftColor = new Color(0f, 0.75f, 0.65f),
                    borderRightColor = new Color(0f, 0.75f, 0.65f),
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderRadius = 8,
                    paddingTop = 10,
                    paddingBottom = 10,
                    paddingLeft = 10,
                    paddingRight = 10,
                    display = DisplayStyle.None
                }
            };

            // Header
            var headerRow = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    justifyContent = Justify.SpaceBetween,
                    marginBottom = 12,
                    borderBottomColor = new Color(0.2f, 0.2f, 0.2f),
                    borderBottomWidth = 1,
                    paddingBottom = 6
                }
            };

            var titleLabel = new Label("greg.Multiplayer  v0.0.1")
            {
                style =
                {
                    fontSize = 16,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = new Color(0f, 0.75f, 0.65f),
                    flexGrow = 1
                }
            };
            headerRow.Add(titleLabel);

            var closeBtn = new Button(() =>
            {
                _visible = false;
                _root!.style.display = DisplayStyle.None;
            })
            {
                text = "X",
                style =
                {
                    width = 24,
                    height = 24,
                    backgroundColor = Color.clear,
                    color = new Color(0.7f, 0.7f, 0.7f),
                    unityFontStyleAndWeight = FontStyle.Bold,
                    borderTopWidth = 0,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0
                }
            };
            headerRow.Add(closeBtn);
            _root.Add(headerRow);

            // Host section
            BuildHostSection(_root);

            // Join section
            BuildJoinSection(_root);

            // Status bar
            var statusLabel = new Label(_statusMsg)
            {
                name = "StatusLabel",
                style =
                {
                    fontSize = 12,
                    color = new Color(0f, 0.9f, 0.9f),
                    marginTop = 8,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };
            _root.Add(statusLabel);

            GregUIManager.RegisterPanel("MultiplayerHud", _root);
        }

        private void BuildHostSection(VisualElement root)
        {
            var sectionLabel = new Label("── Host ──────────────────")
            {
                style =
                {
                    fontSize = 12,
                    color = new Color(0.7f, 0.7f, 0.7f),
                    marginBottom = 6
                }
            };
            root.Add(sectionLabel);

            var btnLAN = new Button(() =>
            {
                bool ok = GregRelayService.Instance?.StartListenServer() ?? false;
                _statusMsg = ok ? "Server started!" : "ERROR: Server failed (check MelonLoader log)";
                if (ok) _inviteLink = GregRelayService.Instance.GenerateInviteLink();
                UpdateStatus();
            })
            {
                text = "Start Listen-Server (LAN)",
                style =
                {
                    backgroundColor = new Color(0f, 0.75f, 0.65f),
                    color = Color.black,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 30,
                    marginBottom = 6,
                    borderRadius = 4
                }
            };
            root.Add(btnLAN);

            var btnRelay = new Button(() =>
            {
                bool ok = GregRelayService.Instance?.StartListenServer() ?? false;
                _statusMsg = ok ? "Relay started!" : "ERROR: check docker relay";
                if (ok) _inviteLink = GregRelayService.Instance.GenerateRelayInviteLink();
                UpdateStatus();
            })
            {
                text = "Start Relay-Server (WAN)",
                style =
                {
                    backgroundColor = new Color(0f, 0.75f, 0.65f),
                    color = Color.black,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 30,
                    marginBottom = 6,
                    borderRadius = 4
                }
            };
            root.Add(btnRelay);

            var inviteContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    marginTop = 6,
                    marginBottom = 6
                }
            };

            var inviteLabel = new Label("Invite Link:")
            {
                style = { fontSize = 12, color = new Color(0.7f, 0.7f, 0.7f) }
            };
            inviteContainer.Add(inviteLabel);

            var inviteField = new TextField
            {
                value = _inviteLink,
                style =
                {
                    backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                    color = Color.white,
                    height = 24,
                    marginBottom = 4
                }
            };
            inviteField.RegisterValueChangedCallback(evt => _inviteLink = evt.newValue);
            inviteContainer.Add(inviteField);

            var copyBtn = new Button(() =>
            {
                GUIUtility.systemCopyBuffer = _inviteLink;
                _statusMsg = "Link copied!";
                UpdateStatus();
            })
            {
                text = "Copy to Clipboard",
                style =
                {
                    backgroundColor = new Color(0.15f, 0.15f, 0.15f),
                    color = new Color(0f, 0.75f, 0.65f),
                    height = 24,
                    borderRadius = 4
                }
            };
            inviteContainer.Add(copyBtn);
            root.Add(inviteContainer);
        }

        private void BuildJoinSection(VisualElement root)
        {
            var sectionLabel = new Label("── Join ──────────────────")
            {
                style =
                {
                    fontSize = 12,
                    color = new Color(0.7f, 0.7f, 0.7f),
                    marginTop = 8,
                    marginBottom = 6
                }
            };
            root.Add(sectionLabel);

            var joinLabel = new Label("Paste invite link:")
            {
                style = { fontSize = 12, color = new Color(0.7f, 0.7f, 0.7f) }
            };
            root.Add(joinLabel);

            var joinField = new TextField
            {
                value = _joinInput,
                style =
                {
                    backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                    color = Color.white,
                    height = 24,
                    marginBottom = 6
                }
            };
            joinField.RegisterValueChangedCallback(evt => _joinInput = evt.newValue);
            root.Add(joinField);

            var connectBtn = new Button(() =>
            {
                if (GregRelayService.ParseInviteLink(_joinInput, out string host, out int port, out _))
                {
                    bool ok = GregRelayService.Instance?.JoinServer(host, port) ?? false;
                    _statusMsg = ok ? $"Connecting to {host}:{port}..." : "Parse error – invalid link";
                }
                else
                {
                    _statusMsg = "Invalid invite link format";
                }
                UpdateStatus();
            })
            {
                text = "Connect",
                style =
                {
                    backgroundColor = new Color(0f, 0.75f, 0.65f),
                    color = Color.black,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 30,
                    borderRadius = 4
                }
            };
            root.Add(connectBtn);
        }

        private void UpdateStatus()
        {
            if (_root == null) return;
            var statusLabel = _root.Q<Label>("StatusLabel");
            if (statusLabel != null)
            {
                statusLabel.text = _statusMsg;
            }
        }
    }
}
