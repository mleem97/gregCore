# Native co-op assembly audit

Reviewed against the checked-in Data Center reference assemblies on 2026-08-13.
The hashes below identify the exact files inspected; they are not a substitute
for a runtime smoke test against the installed game.

| Assembly | SHA-256 | Relevant metadata observed |
|---|---|---|
| `lib/references/MelonLoader/Il2CppAssemblies/Assembly-CSharp.dll` | `30b53b19a1ebaa61ae604de5cac206fea8fcae26765be5082bb2d9b1be19ca69` | `NetworkSaveData`, `WaypointInitializationSystem.LoadNetworkState`, `NetworkMap`, `NetworkSwitch`, `SteamManager` and `SteamAPIDebugTextHook` |
| `lib/references/MelonLoader/Il2CppAssemblies/UnityEngine.MultiplayerModule.dll` | `6f555174ff18a3713a5548d2332c29506fb05a300195b34306cc356f9b311219` | `Unity.Multiplayer.PlayMode.CurrentPlayer`, `MultiplayerManager`, `MultiplayerRole`, `ClientAndServer` and active role-mask APIs |
| `lib/references/MelonLoader/Il2CppAssemblies/Il2Cppcom.rlabrecque.steamworks.net.dll` | `a025963cb4433ae8840da96e8951cdeacfb9730be1bff9fd356c358549b12182` | `SteamMatchmaking`, lobby callbacks, `SteamNetworking`, `SteamNetworkingMessages`, `SendP2PPacket`, `ReadP2PPacket` and related session types |

## Conclusion

The references support the native-co-op boundary used by GregCore:

1. Unity exposes a multiplayer role/player module.
2. The game assembly owns network-save loading and network-map state and has a
   native Steam manager.
3. The bundled Steamworks Il2Cpp wrapper exposes the lobby/P2P and networking
   primitives needed by the game.

This is sufficient evidence that GregCore must not ship its former FishNet,
relay or `dc_multiplayer.dll` replacement stack. It does not, by itself, prove
which exact Unity/Steam callback path is active in every game scene. That
remaining fact requires an in-game Windows/Linux smoke test with two native
co-op players.

## Reproduction

The metadata was checked with:

```bash
monodis --typedef lib/references/MelonLoader/Il2CppAssemblies/UnityEngine.MultiplayerModule.dll
monodis --typedef lib/references/MelonLoader/Il2CppAssemblies/Assembly-CSharp.dll
monodis --typedef lib/references/MelonLoader/Il2CppAssemblies/Il2Cppcom.rlabrecque.steamworks.net.dll
```

Additional symbol checks used `strings` with the names listed in the table.
