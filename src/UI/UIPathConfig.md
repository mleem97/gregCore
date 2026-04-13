# gregCore UI Paths Configuration
# =========================================
# Bearbeite diese Datei um Canvas-Pfade anzupassen
# 
# UIMode Enum:
#   - MainMenu        = Hauptmenü
#   - Playing         = Im Spiel (HUD sichtbar)
#   - Paused          = Pause (Pause-Menü offen)
#   - Settings        = Einstellungen
#   - ModConfig       = Mod-Konfigurationsmenü
#   - ComputerShop    = Shop im Spiel
#   - AssetManagement  = Asset-Verwaltung
#   - BalanceSheet     = Bilanzübersicht
#   - Hire            = Mitarbeiter einstellen
#   - Tutorial         = Tutorial overlays
#   - Loading         = Ladebildschirm

## Canvas: Canvas_OverAll
# Enthält: Notification, Tooltip, BlackOver, InputTextOverlay, OSK
# Sollte: Immer sichtbar bleiben (außer Loading)

## Canvas: Canvas_Main
# Enthält: TopLeft (Stats), BottomLeft (Messages), BottomRight, TESTTEXT
# Sollte: Bei Playing und Paused sichtbar sein

## Canvas: CountersCanvas
# Enthält: Zähler für Reputation, XP, Money etc.
# Sollte: Bei Playing sichtbar sein

## Canvas: PauseMenuCanvas
# Enthält: Pause menu, Settings, Tutorials, Console
# Sollte: Nur bei Paused/Settings sichtbar sein

## Canvas: Canvas_ComputerShop
# Enthält: Shop, NetworkMap, AssetManagement, BalanceSheet, Hire
# Sollte: Bei ComputerShop, AssetManagement, BalanceSheet, Hire sichtbar sein

## Canvas: Canvas_SetIP
# Enthält: IP-Setzer
# Sollte: Bei Playing und Subnetz-Interaktion sichtbar sein

## Canvas: Canvas_ChooseCustomer
# Enthält: Kunden-Auswahl
# Sollte: Bei Playing und Kundeninteraktion sichtbar sein

## Canvas: Canvas_SwitchSeting
# Enthält: Switch-Konfiguration
# Sollte: Bei Playing und Switch-Interaktion sichtbar sein

## Canvas: Tutorials
# Enthält: Tutorial-Overlays
# Sollte: Bei Tutorial sichtbar sein

## Canvas: Canvas_BuyWAll
# Enthält: Wall kaufen UI
# Sollte: Bei Playing und Wall-Kauf sichtbar sein

# =========================================
# Beispiel-Konfiguration (in UIRouter.cs):
# =========================================
# 
# [UIMode.Playing] = new UIPageConfig
# {
#     CanvasNames = new[] { "Canvas_Main", "Canvas_OverAll", "CountersCanvas" },
#     HideOnEnter = new[] { "PauseMenuCanvas", "Canvas_ComputerShop", "Tutorials" },
#     ShowOnEnter = Array.Empty<string>(),
#     LockCursor = true,
#     ShowCursor = false
# }
