// Plugins/Js/ExampleMod.js
greg.logInfo("JS Example Mod initializing...");

// Subscribe to the player's coin changed hook
greg.on("greg.PLAYER.CoinChanged", (payload) => {
    const amount = payload.Data.Amount;
    const total = payload.Data.Total;
    greg.logInfo(`JS received money update: ${amount} (Total: ${total})`);
    
    // Fire a custom hook back
    greg.fire("greg.CUSTOM.JsResponse", {
        msg: "JS heard that!",
        received_total: total
    });
});

greg.logInfo("JS Example Mod initialized!");
