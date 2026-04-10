local mod = {
  id = "lua:sysadmin-example",
  name = "Sysadmin Lua Example",
  version = "0.1.0"
}

local heartbeatSeconds = 10.0
local elapsed = 0.0

function mod_on_init()
  print("[gregCore][Sysadmin][Lua] initialized")
end

function mod_on_update(delta)
  elapsed = elapsed + delta
  if elapsed >= heartbeatSeconds then
    elapsed = 0.0
    print("[gregCore][Sysadmin][Lua] heartbeat")
  end
end

function mod_on_shutdown()
  print("[gregCore][Sysadmin][Lua] shutdown")
end

return mod
