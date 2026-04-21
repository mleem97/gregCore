# gregUnlockAll

Ein Beispiel-Lua-Mod für das **gregCore** Framework, welcher demonstriert, wie man mit der MoonSharp-Lua-Integration von gregCore arbeitet.

## Funktion
Beim Laden eines Spielstandes (`greg.lifecycle.GameLoaded`) gewährt dieser Mod dem Spieler die maximale Menge an Geld, Erfahrungspunkten (XP) und Ruf (Reputation).

## Installation
Kopiere die Datei `gregUnlockAll.lua` in deinen Mod-Skript-Ordner.
Gemäß den Wiki-Spezifikationen von `gregCore` liegt dieser unter:
`Mods/Scripts/gregUnlockAll.lua`

Beim nächsten Spielstart erkennt der `GregLuaHost` die Datei und aktiviert den Hook automatisch.

## Konfiguration
Über das `gregCore` Konfigurationssystem können die Zielwerte angepasst werden (die ID lautet `gregUnlockAll`).
Standardwerte:
- `target_money`: 999999999
- `target_xp`: 999999
- `target_reputation`: 999999
