-- gregCore Lua Example Mod

function on_init()
    greg.log_info("Lua Example Mod geladen!")
    greg.show_notification("Lua Mod Initialisiert")
    
    -- Event abonnieren
    greg.subscribe_event(100, function(data)
        greg.log_info("Geld hat sich geändert! Neuer Stand: " .. greg.get_player_money())
    end)
end

function on_update(dt)
    -- Wird jeden Frame aufgerufen
end

function on_event(event_id, data)
    -- Generischer Event-Handler
end

function on_scene_loaded(name)
    greg.log_info("Szene geladen: " .. name)
end

function on_shutdown()
    greg.log_info("Lua Mod wird beendet.")
end
