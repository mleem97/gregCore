:rotating_light: **Major Update: gregCore Pre-Release 7 is LIVE! (Phase 5 & Namespace Unification)**

Hey everyone,

We've just reached a massive milestone in the evolution of the Greg Framework! Today we are dropping **gregCore Pre-Release 7**, which brings the long-awaited **Phase 5: Economy & Data** and a complete overhaul of our internal architecture.

---

:wrench: **What's new in Pre-Release 7?**

This update focuses on deep data integration and professionalizing our codebase.

:dna: **Unified 'greg' Namespace** — We have converted the entire framework to a clean, unified namespace structure. No more `Modig` or `gregModLoader` clutter. Everything is now under `greg.Core`, `greg.Sdk`, and `greg.UI`.
:money_with_wings: **Phase 5: Economy & Data** — Full access to the game's economic simulation.
    *   **GregBalanceService**: Real-time access to income, expenses, and monthly snapshots.
    *   **GregSaveService**: Persistent data bridges for mods to store custom state.
:map: **Universal Localisation** — Direct bridge to `Il2Cpp.Localisation`. Mods can now register their own terms, enabling full translation support for the entire ecosystem.
:factory: **SysAdmin Reborn** — The SysAdmin mod has been refactored into **`greg.AutoEmployees`**, now maintained by **teamGreg / Joniii11**.

---

:bug: **Join the Bug Hunt!**

We need you to push these new data systems to their limits:

:small_blue_diamond: **Test Economy Bridges:** Verify that money and balance snapshots match the in-game UI.
:small_blue_diamond: **Mod Translation:** Try using `GregLocalisation.GetTerm` in your mods and let us know if the injection works correctly.
:small_blue_diamond: **Save Data Integrity:** Check if your custom mod data persists correctly through game restarts.

---

:package: **Download**

:point_right: **[Download gregCore v1.0.0.7 on GitHub](https://github.com/mleem97/gregCore/releases/tag/v1.0.0.7)**

Thank you for being part of this journey! — Your **teamGreg**
