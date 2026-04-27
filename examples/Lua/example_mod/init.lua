--[[
    gregCore Lua SDK – Example Mod
    
    Demonstrates how to use the gregCore Lua API via MoonSharp scripting engine.
    Place this file in: Data Center/Mods/LuaMods/your_mod/init.lua
    
    Available APIs:
      greg.subscribe(eventId, callback)   – Subscribe to a greg event
      greg.fire_event(eventId, data)      – Fire a custom event
      greg.log(message)                   – Log to MelonLogger
      greg.get_player_coins()             – Get current coin count
      greg.get_player_xp()                – Get current XP
]]

-- ─── Mod Metadata ────────────────────────────────────────────────────
local MOD_NAME = "LuaExampleMod"
local MOD_VERSION = "1.0.0"

greg.log("[" .. MOD_NAME .. " v" .. MOD_VERSION .. "] Loading...")

-- ─── Event IDs (must match gregCore.Core.Events.EventIds) ────────────
local EVENT_COINS_CHANGED = 1001
local EVENT_XP_CHANGED = 1002
local EVENT_GAME_SAVED = 2001
local EVENT_SERVER_STATUS = 3001
local EVENT_RACK_POSITION = 3002
local EVENT_CABLE_CREATED = 4001

-- ─── Event Handlers ──────────────────────────────────────────────────

-- React to coin changes
greg.subscribe(EVENT_COINS_CHANGED, function(data)
    greg.log("[" .. MOD_NAME .. "] Coins changed! New value: " .. tostring(data))
end)

-- React to XP changes
greg.subscribe(EVENT_XP_CHANGED, function(data)
    greg.log("[" .. MOD_NAME .. "] XP changed! New value: " .. tostring(data))
end)

-- React to game saves
greg.subscribe(EVENT_GAME_SAVED, function(data)
    greg.log("[" .. MOD_NAME .. "] Game was saved at: " .. tostring(data))
end)

-- React to rack position queries
greg.subscribe(EVENT_RACK_POSITION, function(data)
    greg.log("[" .. MOD_NAME .. "] Rack position queried: " .. tostring(data))
end)

-- React to cable creation
greg.subscribe(EVENT_CABLE_CREATED, function(data)
    greg.log("[" .. MOD_NAME .. "] New cable created with ID: " .. tostring(data))
end)

-- ─── Custom Logic ────────────────────────────────────────────────────

-- Example: Fire a custom event
local function on_mod_ready()
    greg.log("[" .. MOD_NAME .. "] Mod initialized successfully!")
    greg.fire_event(9999, 0) -- Custom event for mod-ready signal
end

-- Initialize
on_mod_ready()
