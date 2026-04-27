using MelonLoader;
using UnityEngine;

namespace greg.Multiplayer
{
    /// <summary>
    /// Minimal IMGUI overlay for the Multiplayer panel.
    /// Rendered via OnGUI – no Unity UI dependencies required.
    /// Hooked via Harmony ESC-menu Postfix (see EscMenuPatch.cs).
    /// </summary>
    public class MultiplayerHud : MonoBehaviour
    {
        private bool _visible = false;
        private string _inviteLink = string.Empty;
        private string _joinInput = string.Empty;
        private string _statusMsg = string.Empty;
        private Vector2 _scroll;

        private readonly Rect _windowRect = new(20, 80, 420, 360);

        public void Toggle() => _visible = !_visible;

        private void OnGUI()
        {
            if (!_visible) return;
            GUI.skin = null; // default skin, no Unity UI asset needed
            GUILayout.BeginArea(_windowRect, GUI.skin.box);
            DrawHeader();
            GUILayout.Space(6);
            DrawHostSection();
            GUILayout.Space(6);
            DrawJoinSection();
            GUILayout.Space(6);
            DrawStatusBar();
            GUILayout.EndArea();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>greg.Multiplayer</b>  v0.0.1", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("X", GUILayout.Width(24))) _visible = false;
            GUILayout.EndHorizontal();
        }

        private void DrawHostSection()
        {
            GUILayout.Label("── Host ──────────────────");
            if (GUILayout.Button("Start Listen-Server (LAN)"))
            {
                bool ok = GregRelayService.Instance?.StartListenServer() ?? false;
                _statusMsg = ok ? "Server started!" : "ERROR: Server failed (check MelonLoader log)";
                if (ok) _inviteLink = GregRelayService.Instance.GenerateInviteLink();
            }
            if (GUILayout.Button("Start Relay-Server (WAN)"))
            {
                bool ok = GregRelayService.Instance?.StartListenServer() ?? false;
                _statusMsg = ok ? "Relay started!" : "ERROR: check docker relay";
                if (ok) _inviteLink = GregRelayService.Instance.GenerateRelayInviteLink();
            }
            if (!string.IsNullOrEmpty(_inviteLink))
            {
                GUILayout.Label("Invite Link:");
                GUILayout.TextField(_inviteLink);
                if (GUILayout.Button("Copy to Clipboard"))
                {
                    GUIUtility.systemCopyBuffer = _inviteLink;
                    _statusMsg = "Link copied!";
                }
            }
        }

        private void DrawJoinSection()
        {
            GUILayout.Label("── Join ──────────────────");
            GUILayout.Label("Paste invite link:");
            _joinInput = GUILayout.TextField(_joinInput);
            if (GUILayout.Button("Connect"))
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
            }
        }

        private void DrawStatusBar()
        {
            GUILayout.Label($"<color=cyan>{_statusMsg}</color>");
        }
    }
}
