# greg.Multiplayer

> **FishNet Listen-Server / Relay Multiplayer Mod**  
> MelonLoader 0.7.2 | Unity 6000.4.2f | IL2CPP | FishNet Latest

## Quick Start

### Requirements
- Game with MelonLoader 0.7.2 installed
- `FishNet.Runtime.dll` placed in `lib/FishNet/`  
  _(extract from a Unity 6000.4.2f test project after importing FishNet from Package Manager)_
- .NET 6 SDK

### Build
```powershell
dotnet build src/greg.Multiplayer/greg.Multiplayer.csproj `
  /p:GameDir="C:\Path\To\DataCenterSim"
```
Output DLL lands in `bin/Mods/greg.Multiplayer.dll` – copy to your game's `Mods/` folder.

### First Run (LAN)
1. Host: press ESC in-game → Multiplayer tab → **Start Listen-Server (LAN)**
2. Host: copy invite link (`gregmp://192.168.x.x:7777#ABCD1234`)
3. Client: press ESC → paste link → **Connect**

### Relay / WAN (Docker)
```bash
docker compose -f docker/docker-compose.yml up -d
```
Then use **Start Relay-Server (WAN)** – link will point to your relay host.

## Architecture
```
GregMultiplayerMod (MelonMod)
  └─ GregRelayService (MonoBehaviour, singleton)
       ├─ FishNet NetworkManager
       ├─ RackSyncer    ← RackPatch (Harmony Postfix)
       └─ CableSyncer   ← CablePatch (Harmony Postfix)
MultiplayerHud (IMGUI overlay, injected on scene load)
```

## TODO before v0.1.0
- [ ] ILSpy: confirm `Rack`, `CableManager`, `PauseMenuUI` method signatures
- [ ] Replace reflection field access with direct IL2CPP cast after type dump
- [ ] Implement FishNet `[ObserversRpc]` for actual network broadcast
- [ ] Add `ApprovalQueue` host-side accept/deny UI
- [ ] FishNet SyncVar for IOPS delta

## Roadmap
See root [TODO.md](../../TODO.md) for full v1.0 checklist.
