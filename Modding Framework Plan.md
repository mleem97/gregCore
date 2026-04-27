# 🛠️ gregCore Modding Framework Plan
*Stand: 28.04.2026 – v1.1.0*

## 1. 🚨 Kritische Lücken (Was im Original ist, aber in GregCore fehlt)

- [x] **Klasse:** `Rack` -> **Patch:** `Prefix` für Methode `IsPositionAvailable`. Greg-seitige Position-Registry mit Thread-Safety. Implementiert in `src/GameLayer/Patches/Hardware/RackPatch.cs`.
- [x] **Klasse:** `InputController` -> **Patch:** Kompletter Bypass via `Prefix` für Move/Look/Interact Properties. Nutzt Legacy `UnityEngine.Input.GetAxis`. Implementiert in `src/GameLayer/Patches/Input/InputControllerPatch.cs`.
- [x] **Klasse:** `CablePositions` -> **Patch:** `Prefix` für `CreateNewCable`. Atomarer ID-Counter via `Interlocked.Increment`. Implementiert in `src/GameLayer/Patches/Networking/CablePositionsPatch.cs`.
- [x] **Klasse:** `Player` -> **Patch:** `Prefix` für `UpdateCoin` / `UpdateXP`. Setzt `__result = true` und dispatcht Events. Implementiert in `src/GameLayer/Patches/Economy/PlayerPatch.cs`.

## 2. 🔄 FishNet Multiplayer Sync Todos

- [x] **Klasse:** `Rack` -> **NetworkBehaviour:** SyncVar-Wrapper für `isPositionUsed` Array. Implementiert in `src/Compatibility/FishNet/GregNetworkRack.cs`.
- [x] **Klasse:** `Server` -> **NetworkBehaviour:** SyncVar für `isOn` (Power State) und RPC für Port-Connects. Implementiert in `src/Compatibility/FishNet/GregNetworkServer.cs`.
- [x] **Klasse:** `CablePositions` -> **NetworkBehaviour:** SyncList für Kabel-Punkte (Bézier-Vektoren). Implementiert in `src/Compatibility/FishNet/GregNetworkCables.cs`.

## 3. ✅ Bereits abgedeckt (In Sync)

- **SaveSystem:** Erfolgreich durch `GregSaveEngine` (LiteDB) ersetzt (siehe `src\greg.SaveEngine\GregSaveEngine.cs`).
- **MainGameManager:** Event-Hooks für Lifecycle vorhanden (siehe `GregSystemHooks.cs`).
- **Event-System:** Stabiler Hook-Registry und Hook-Bus in `GregCore.Infrastructure`.
- **UI:** Grundgerüst für Settings-Bridges und Dev-Console vorhanden.
- **SDKs:** Beispiele für Lua, JS, Python, Go und Rust unter `examples/`.
- **Multiplayer-Service:** `GregMultiplayerService` koordiniert FishNet-Layer.
