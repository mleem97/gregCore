# AI Agent Instructions

This file is the primary source of truth for AI coding agents working in the gregCore repository.

## Repository Mission

gregCore is a .NET 6 MelonLoader mod framework for IL2CPP Unity games. The product name is gregCore.

## Source of Truth Hierarchy

1. **AGENTS.md** (this file) - Main instruction source
2. **DEVELOPMENT.md** - Local setup and build
3. **ARCHITECTURE.md** - Structural guidance
4. **CONTRIBUTING.md** - Contribution workflow

## Allowed Changes

AI agents may propose:
- Bug fixes in src/ files
- Documentation improvements
- Test additions in tests/
- Build script improvements
- Non-breaking enhancements

## Forbidden Changes

AI agents must NOT:
- Modify `lib/references/` (game-specific DLLs)
- Modify existing hook signatures (breaking)
- Add external dependencies without discussion
- Push directly to main branch
- Modify release process without approval

---
*Migrated from GameFramework/AGENTS.md*