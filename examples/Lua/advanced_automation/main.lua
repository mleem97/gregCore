--[[
    gregCore Advanced Automation - Auto-Repair Mod
    Version: 1.1.0
    Description: Automatically detects and repairs broken servers every 30 seconds.
                  Demonstrates Coroutines, Timers, and Domain APIs.
]]

local mod_id = "automation_pro"
local repair_interval = 30 -- seconds
local stats = { repairs_total = 0 }

function on_init()
    greg.log_info("[AutoRepair] Initializing advanced automation...")

    -- Load saved stats
    local saved = greg.io.read_json("stats.json")
    if saved then stats = saved end

    -- Start the background automation loop via a coroutine
    greg.start_coroutine(automation_loop)

    -- Register a timer for periodic status report
    greg.every(60, function()
        greg.log_info(string.format("[AutoRepair] Status: %d total repairs this session.", stats.repairs_total))
        greg.io.write_json("stats.json", stats) -- Save progress
    end)
end

-- Coroutine: Non-blocking background logic
function automation_loop()
    while true do
        local broken_count = greg.server.get_broken_count()
        
        if broken_count > 0 then
            greg.log_info("[AutoRepair] Found " .. broken_count .. " broken servers. Dispatching repairs...")
            
            -- Call the domain API to dispatch a repair job
            local success = greg.server.dispatch_repair_all()
            
            if success then
                stats.repairs_total = stats.repairs_total + broken_count
                greg.ui.show_notification("Auto-Repair started for " .. broken_count .. " servers.")
            end
        end

        -- Yield execution for the specified interval
        -- greg.wait(seconds) yields the coroutine
        greg.wait(repair_interval)
    end
end

-- React to manual repair events to keep stats in sync
greg.hooks.server_repaired_manual(function(serverId)
    greg.log_info("[AutoRepair] Noted manual repair of server: " .. serverId)
    stats.repairs_total = stats.repairs_total + 1
end)

function on_shutdown()
    greg.io.write_json("stats.json", stats)
    greg.log_info("[AutoRepair] Stats saved. See you next time!")
end
