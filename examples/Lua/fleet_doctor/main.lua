--[[
    Fleet Doctor - gregCore Lua SDK showcase (complex mod example).
    Watches servers AND switches, auto-dispatches technicians when crew is
    free, persists counters across sessions, exposes its state as JSON.

    Exercises: greg.server.*, greg.switch.*, greg.tech.*, greg.config.*,
    greg.save.*, greg.json.*, greg.ui.*, timers, lifecycle hooks.
]]

local interval = 30
local auto_dispatch = true

function on_init()
    interval = tonumber(greg.config.get_or("check_interval", "30")) or 30
    auto_dispatch = greg.config.get_or("auto_dispatch", "true") == "true"
    greg.ui.log_info("[FleetDoctor] init (interval=" .. interval .. "s, auto=" .. tostring(auto_dispatch) .. ")")
    greg.every(interval, function()
        check_fleet("server")
        check_fleet("switch")
        report()
    end)
end

function on_shutdown()
    greg.save.set("last_seen", os.date("%Y-%m-%d %H:%M"))
    greg.save.save_now()
end

function check_fleet(kind)
    local devices, broken
    if kind == "server" then
        devices = greg.server.get_all()
        broken = greg.server.broken_count()
    else
        devices = greg.switch.get_all()
        broken = greg.switch.broken_count()
    end
    if broken == 0 then return end
    greg.ui.log_warning("[FleetDoctor] " .. broken .. " broken " .. kind .. "(s) detected.")
    if not auto_dispatch then return end
    if greg.tech.free_count() == 0 then
        greg.ui.log_warning("[FleetDoctor] no free technicians - skipping dispatch.")
        return
    end
    local sent = 0
    for _, dev in ipairs(devices) do
        if dev.is_broken then
            local ok = false
            if kind == "server" then ok = greg.server.repair(dev.id)
            else ok = greg.switch.repair(dev.id) end
            if ok then sent = sent + 1 end
        end
    end
    bump_stat("dispatched_" .. kind, sent)
    greg.ui.log_info("[FleetDoctor] dispatched " .. sent .. " " .. kind .. " repair(s).")
end

function bump_stat(key, delta)
    local cur = tonumber(greg.save.get_or(key, "0")) or 0
    greg.save.set(key, tostring(cur + delta))
end

function report()
    local stats = {
        servers_broken = greg.server.broken_count(),
        switches_broken = greg.switch.broken_count(),
        tech_free = greg.tech.free_count(),
        tech_total = greg.tech.total_count(),
        dispatched_server = tonumber(greg.save.get_or("dispatched_server", "0")) or 0,
        dispatched_switch = tonumber(greg.save.get_or("dispatched_switch", "0")) or 0,
    }
    greg.ui.log_info("[FleetDoctor] " .. greg.json.stringify(stats))
    greg.save.set("last_report", greg.json.stringify(stats))
end

-- Manual trigger example: repair everything right now.
function repair_now()
    local s = greg.server.repair_all()
    local w = 0
    for _, dev in ipairs(greg.switch.get_all()) do
        if dev.is_broken and greg.switch.repair(dev.id) then w = w + 1 end
    end
    return s, w
end
