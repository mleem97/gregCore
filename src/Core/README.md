# gregCore Architektur

## Schichten
Core/           → Framework-Kern, kein Unity-Coupling, vollständig testbar
Infrastructure/ → Implementierungen, kennt MelonLoader/Win32/Mono
GameLayer/      → IL2CPP + Harmony, nur Daten-Extraktion + Dispatch
PublicApi/      → Was Modder nutzen dürfen

## Goldene Regeln
1. Patches haben KEINE Business-Logic
2. GregCoreLoader hat MAX 50 Zeilen
3. GameAPITable: Neue Felder NUR ans Ende
4. Alle Services via Interface — nie direkt instanziieren
5. MelonLogger nur in MelonLoggerAdapter
6. Assembly.LoadFrom VERBOTEN — immer Mono.Cecil
