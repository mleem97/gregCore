# Native Data Center co-op boundary

Data Center owns the multiplayer session, lobby lifecycle, transport and
shared world state. GregCore must remain a framework around that game-owned
authority; it must not create a second session or attempt to replicate game
objects independently.

## What GregCore may do

- read local game state through the public API;
- provide local UI, settings, diagnostics and performance helpers;
- observe game-owned lifecycle or object events when the game exposes them;
- make a local change only when the feature is explicitly safe for native
  co-op and the game remains the authority.

## What GregCore must not do

- create or join Steam lobbies;
- open Steam P2P or Steam Networking sessions;
- install a FishNet/relay/network-manager replacement;
- transfer saves or world objects through a custom multiplayer protocol;
- make profile, unlock, money, tower or simulation changes on behalf of other
  players.

The legacy Rust FFI v7 Steam/lobby/P2P function-pointer positions are retained
for ABI layout compatibility with older plugins. They are inert no-ops. New
plugins must not depend on them; they are not an adapter to Data Center's
native session.

## Mod author rules

Single-player-only features must declare or enforce their single-player scope.
Features that are safe in native co-op should operate on local presentation or
read-only diagnostics. Any shared-world mutation needs a documented game-owned
authority path and an in-game test before it can be enabled.

The assembly evidence for the current reference set is recorded in
[`../codebase/native-coop-assembly-audit.md`](../codebase/native-coop-assembly-audit.md).
