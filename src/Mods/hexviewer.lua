-- HexViewer.lua
-- JADE-like telemetry overlay for gregCore

local ModId = "HexViewerLua"

function on_update(dt)
    -- 1. Get info about what we are looking at
    local target = greg.sdk.targeting.get_target_info(10.0)
    
    if target.type == "None" then
        greg.sdk.hud.hide_jade_box()
        return
    end

    -- 2. Fetch standard metadata for the target
    local meta = greg.sdk.metadata.get_metadata(10.0)
    
    -- 3. Update the HUD
    -- We use the entries list returned by the metadata service
    greg.sdk.hud.update_jade_box(meta.title, meta.sub_header, meta.entries)
    
    -- Debug logging (optional)
    -- greg.log("Looking at: " .. target.name .. " (" .. target.type .. ")")
end

greg.log("HexViewer Lua Module Loaded.")
