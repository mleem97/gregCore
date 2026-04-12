:rotating_light: **Update: All Changes since Pre-Release 4!** :rotating_light:

Hi everyone! Here is a compact overview of all the new features, fixes, and bridges that have landed in **gregCore** since `pre4`. We have reached some huge milestones!

:joystick: **Game Compatibility & Networking**
- **Data Center Patch v1.0.45.5**: Compatibility with the latest game version has been verified!
- **VLAN Management**: Added `SetVlanAllowed`, `SetVlanDisallowed`, and `IsVlanAllowed` to `gregGameHooks` and the API (v9).
- **Route Evaluation**: New hooks for the improved in-game routing system have been added and documented.

:rocket: **New SDK Features & Bridges (Pre-Release 5 & 6)**
- **TS/JS Engine Integration:** Full integration of the TypeScript/JavaScript engine, including associated API documentation.
- **IL2CPP Game System Bridges:** 9 completely new SDK services for direct control of in-game systems.
- **Hardware Bridges:** Direct interaction possibilities with physical server and rack objects in the game.
- **Phase 5 (Economy & Data):**
  - `GregBalanceService`: Access to financial data, salaries, and income simulation.
  - `GregLocalisationService`: Direct access to the game's internal translation system (mod-specific translations are now possible).

:blue_book: **Documentation & Roadmap**
- All framework documentation has been translated into English and enhanced with Material Symbols.
- Reference documentation (`gregReferences`) has been synchronized for the new game version.
- The Master Roadmap has been updated (including preparations for Phase 8: Unity Scripting API).

:bug: **Bugfixes & Polish (up to v1.0.0.17)**
- **UI & Config:** Resolved an issue with UI transparency and improved the accessibility of mod configurations in the menu.

Grab the latest build on GitHub and happy modding! If you notice any bugs, feel free to report them. :fire:
