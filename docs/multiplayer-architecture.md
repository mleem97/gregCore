# greg.Multiplayer – Architecture & Design Decisions

## Stack
| Layer | Technology | Why |
|-------|-----------|-----|
| Mod Loader | MelonLoader 0.7.2 | IL2CPP injection, .NET 6 |
| Networking | FishNet Latest | Free, IL2CPP/AOT-ready, server-authority |
| Relay | FishBait (Docker) | Self-hosted, no Photon subscription |
| Game Hooks | HarmonyLib 2.x | Prefix/Postfix without source access |
| Serialisation | System.Text.Json | .NET 6 built-in, no extra DLL |

## Connection Modes

### Mode A – LAN Listen-Server
```
Host PC: ServerManager.StartConnection(:7777)
         ClientManager.StartConnection("localhost", 7777)
Client:  ClientManager.StartConnection("192.168.x.x", 7777)
```
The host runs server + client simultaneously.  
NAT-only problem if players are not on the same LAN.

### Mode B – WAN via FishBait Relay
```
Host:   ServerManager.StartConnection(:7777)
        Registers room on relay (fishbait://relay:7778)
Client: Connects to relay → relayed to host
```
No port forwarding needed. Docker compose spins relay locally or on VPS.

## State Synchronisation Strategy

Only **deltas** are transmitted. `RackSyncPayload.ChangedSlots` contains only
the slots that changed since last broadcast. IOPS is sent via
unreliable channel (low priority, high frequency); structural changes
(PlaceDevice, RemoveDevice, ConnectCable) use reliable ordered channel.

## Server Authority
All write operations (PlaceDevice, RemoveDevice, ConnectCable) are
patched with a Harmony **Postfix** that checks `InstanceFinder.IsServerStarted`
before broadcasting. Clients that try to trigger these via RPC are
validated server-side before application.

## IL2CPP Notes
- Classes attached to GameObjects must use `ClassInjector.RegisterTypeInIl2Cpp<T>()`
- Generic types need explicit specialisation or `[Il2CppSetOption]` attributes
- Reflection field access is a temporary workaround until IL2CppDumper
  provides exact field offsets for the game build

## Roadmap
```
v0.0.1  Research + bare Listen-Server + HUD scaffold
v0.1.0  RackSync + CableSync via FishNet ObserversRpc
v0.2.0  QR invite + Approval Queue + Anti-Cheat validation
v0.3.0  FishBait Docker relay + reconnect/heartbeat
v1.0.0  Full IOPS LOD sync + ModSync hooks + Thunderstore release
v2.0.0  Dedicated server mode + Admin panel
```
