---
title: "Developer Native Coop"
---
# Developer Native Coop

The multiplayer boundary every mod author must respect. Source: `docs/modding/native-coop.md`.

## The rule

**Data Center owns session, lobby, transport, and saves in co-op. gregCore is local-only and adds no replacement networking stack.**

Concretely, mods must **not**:

- Create Steam lobbies, P2P sessions, FishNet transports, relays, or custom save-transfer.
- Re-implement lobby/peer/voice/chat transport (read-only observation via `GregCoop` bridges like `GetPeers / GetHeldHandType / SendToChat(AddToChatOutput)` is fine).
- Assume remote state authority — the game is authoritative; mods react locally.

## What you can do

- Read local/co-op-visible state (`GregCoop`: `EnsureSession / ShutdownSession(CoopBootstrap)`, `FindPlayerSync / GetPeers → PeerInfo{PeerId, Position, Yaw, Hand, …} / ForceResend`, trolley/loose-item + ghost tracking, chat output).
- Build local UI, settings, diagnostics, automation, shop items, HUD/HUB entries.
- Declare multiplayer scope honestly: single-player-only mods must say so; co-op-tested mods should require **identical mod sets on all players** and use `GregModDeps` (`Declare / EnsureLoaded / CheckAll` + `GetLocalManifest / DiffManifests / FormatDiff`) as the ModSync precondition.

## FFI note

Rust FFI v7 Steam slots exist as **inert no-ops** purely for ABI stability (`API_VERSION = 19` table). Calling them does nothing — by design, not a bug.

## Audit

Co-op-sensitive changes should link the boundary doc and describe: what is read vs written, what happens with 2+ players, and what degrades (not crashes) when peers lack the mod.
