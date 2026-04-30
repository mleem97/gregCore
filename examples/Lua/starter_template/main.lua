--[[
    gregCore Modding Framework - Starter Template
    Version: 1.1.0
    Description: Basic template demonstrating lifecycle hooks and API usage.
]]

-- Lifecycle: Called when the mod is first loaded
function on_init()
    greg.ui.log_info("Hello from Starter Mod!")
    
    -- Register a simple command or just print state
    local money = greg.player.money()
    greg.ui.log_info("Current balance: $" .. string.format("%.2f", money))
end

-- Lifecycle: Called every frame
-- @param dt: Delta time since last frame
function on_update(dt)
    -- Be careful with logging in update! 
    -- Use timers if you need periodic logic.
end

-- Lifecycle: Called when the game scene changes
-- @param name: New scene name (e.g., "MainMenu", "Office_1")
function on_scene_loaded(name)
    greg.ui.log_info("Scene loaded: " .. name)
end

-- Lifecycle: Called before the mod is reloaded (hot-reload)
-- Use this to clean up timers or listeners.
function on_shutdown()
    greg.ui.log_info("Shutting down Starter Mod...")
end

-- Lifecycle: Called AFTER a hot-reload has finished
function on_reload()
    greg.ui.log_info("Starter Mod reloaded successfully!")
end

-- Example of a custom event listener
greg.on("greg.PLAYER.CoinChanged", function(payload)
    local amount = payload.data["Amount"]
    greg.ui.log_info("Gained " .. tostring(amount) .. " coins!")
end)
