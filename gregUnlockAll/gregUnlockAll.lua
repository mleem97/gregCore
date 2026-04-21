-- Datei: gregUnlockAll.lua
-- Ein Lua-Mod für gregCore zum schnellen Freischalten (Unlock All)

greg.log_info("[gregUnlockAll] Mod wird initialisiert...")

-- Hook auf das GameLoaded Event
greg.on("greg.lifecycle.GameLoaded", function(payload)
    greg.log_info("[gregUnlockAll] Spiel geladen. Wende 'Unlock All' an...")
    
    -- Konfigurierbare Werte auslesen (mit Fallback auf Maximalwerte)
    local money = greg.config_get_int("gregUnlockAll", "target_money", 999999999)
    local xp = greg.config_get_int("gregUnlockAll", "target_xp", 999999)
    local rep = greg.config_get_int("gregUnlockAll", "target_reputation", 999999)

    -- Neue Werte dem Spieler zuweisen
    greg.set_player_money(money)
    greg.set_player_xp(xp)
    greg.set_player_reputation(rep)

    -- Visuelles Feedback im Spiel
    greg.show_notification("Unlock All erfolgreich angewendet!")
    
    greg.log_info("[gregUnlockAll] Werte wurden maximiert: Geld, XP und Reputation gesetzt.")
end)
