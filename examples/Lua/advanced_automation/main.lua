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
    greg.ui.log_info("[AutoRepair] Initializing advanced automation...")

    -- Load saved stats (JSON stored as string in data/stats.json)
    if greg.io.file_exists("stats.json") then
        local content = greg.io.read_file("stats.json")
        -- Simple parsing: extract number after "repairs_total":
        local num = tonumber(content:match('"repairs_total":%s*(%d+)'))
        if num then stats.repairs_total = num end
    end

    -- Start the background automation loop via a coroutine
    greg.start_coroutine(automation_loop)

    -- Register a timer for periodic status report
    greg.every(60, function()
        greg.ui.log_info(string.format("[AutoRepair] Status: %d total repairs this session.", stats.repairs_total))
        -- Save progress as simple JSON string
        greg.io.write_file("stats.json", '{"repairs_total": ' .. stats.repairs_total .. '}')
    end)
end

-- Coroutine: Non-blocking background logic
function automation_loop()
    while true do
        local broken_count = greg.server.broken_count()
        
        if broken_count > 0 then
            greg.ui.log_info("[AutoRepair] Found " .. broken_count .. " broken servers. Dispatching repairs...")
            
            -- Call the domain API to dispatch a repair job
            local repaired = greg.server.repair_all()
            
            if repaired > 0 then
                stats.repairs_total = stats.repairs_total + repaired
                greg.ui.notify("Auto-Repair: " .. repaired .. " servers fixed")
            end
        end

        -- Yield execution for the specified interval
        -- greg.wait(seconds) yields the coroutine
        greg.wait(repair_interval)
    end
end

-- React to manual repair events to keep stats in sync
greg.on("greg.SERVER.Repaired", function(payload)
    local serverId = payload.data and payload.data["id"] or "unknown"
    greg.ui.log_info("[AutoRepair] Noted manual repair of server: " .. tostring(serverId))
    stats.repairs_total = stats.repairs_total + 1
end)

function on_shutdown()
    greg.io.write_file("stats.json", '{"repairs_total": ' .. stats.repairs_total .. '}')
    greg.ui.log_info("[AutoRepair] Stats saved. See you next time!")
end
