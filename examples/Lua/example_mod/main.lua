-- example_mod/main.lua
function on_init()
    greg.ui.log_info("Lua Example Mod initialized!")
    
    -- Subscribe to coin changed hook
    greg.on("greg.PLAYER.CoinChanged", function(payload)
        local amount = payload.data["Amount"]
        local total = payload.data["Total"]
        greg.ui.log_info("Lua received money update: " .. tostring(amount) .. " (Total: " .. tostring(total) .. ")")
        
        -- Fire a custom hook back
        greg.fire("greg.CUSTOM.LuaResponse", {
            msg = "Lua heard that!",
            received_total = total
        })
    end)
end

function on_update(dt)
    -- Update logic
end

function on_shutdown()
    greg.ui.log_info("Lua Example Mod shutdown.")
end
