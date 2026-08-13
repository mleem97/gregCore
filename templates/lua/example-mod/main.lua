local subscription = greg.on("gregMod.lifecycle.sceneLoaded", function(payload)
    greg.log("Loaded scene: " .. tostring(payload.data.sceneName or payload.data.SceneName))
end)

function on_update(delta_time)
    -- Keep frame work small; use the documented main-thread API for game actions.
end

function on_shutdown()
    greg.off(subscription)
end
