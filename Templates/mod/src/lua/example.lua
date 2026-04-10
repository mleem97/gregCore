-- gregCore Lua bridge example (discovery scaffold)
-- Place in Mods/ScriptMods/lua/

local mod = {
  id = "example.lua.mod",
  name = "Lua Example Mod"
}

function mod.onSceneLoaded(scene)
  print("[lua] scene loaded: " .. tostring(scene))
end

function mod.onUpdate(dt)
  -- Called from a future Lua runtime host.
end

return mod
