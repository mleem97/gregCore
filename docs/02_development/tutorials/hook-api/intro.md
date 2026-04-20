# gregCore Hook API Tutorial

The gregCore Hook API is a powerful, event-driven system that allows mods to interact with the game in real-time. It is designed to be cross-language, stable, and easy to use.

## Core Concepts

- **Hook Name**: A string that identifies the hook, following the schema `greg.DOMAIN.Class.Method` (e.g., `greg.PLAYER.CoinChanged`).
- **Trigger**: When the hook is fired (e.g., "NativePatch", "LuaMod").
- **Payload**: A standardized data object containing `hook_name`, `trigger`, and a `data` dictionary.

## Supported Languages

Click on a language to view its specific tutorial:

- [C# Tutorial](./csharp.md)
- [Lua Tutorial](./lua.md)
- [Python Tutorial](./python.md)
- [Rust Tutorial](./rust.md)
- [Go Tutorial](./go.md)
- [JavaScript/TypeScript Tutorial](./javascript.md)

## Common Hook List

You can find the full list of 1771 available hooks in the [Hooks Catalog](../../api-reference/hooks-catalog.md).

### Example Hooks:
- `greg.PLAYER.CoinChanged`: Fired when the player's money changes.
- `greg.SYSTEM.GameSaved`: Fired when the game is saved.
- `greg.UI.PauseMenu.Opened`: Fired when the pause menu is opened.
