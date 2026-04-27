/**
 * gregCore JavaScript SDK – Example Mod
 * 
 * Demonstrates how to use the gregCore JS API via Jint scripting engine.
 * Place this file in: Data Center/Mods/JsMods/your_mod/main.js
 * 
 * Available APIs:
 *   greg.subscribe(eventId, callback)   – Subscribe to a greg event
 *   greg.fire_event(eventId, data)      – Fire a custom event
 *   greg.log(message)                   – Log to MelonLogger
 */

// ─── Mod Metadata ────────────────────────────────────────────────────
const MOD_NAME = "JsExampleMod";
const MOD_VERSION = "1.0.0";

greg.log(`[${MOD_NAME} v${MOD_VERSION}] Loading...`);

// ─── Event IDs (must match gregCore.Core.Events.EventIds) ────────────
const Events = {
    COINS_CHANGED: 1001,
    XP_CHANGED: 1002,
    REPUTATION_CHANGED: 1003,
    GAME_SAVED: 2001,
    SERVER_STATUS: 3001,
    RACK_POSITION: 3002,
    CABLE_CREATED: 4001,
};

// ─── Event Handlers ──────────────────────────────────────────────────

greg.subscribe(Events.COINS_CHANGED, (data) => {
    greg.log(`[${MOD_NAME}] Coins changed! Data: ${data}`);
});

greg.subscribe(Events.XP_CHANGED, (data) => {
    greg.log(`[${MOD_NAME}] XP changed! Data: ${data}`);
});

greg.subscribe(Events.GAME_SAVED, (data) => {
    greg.log(`[${MOD_NAME}] Game saved at: ${data}`);
});

greg.subscribe(Events.RACK_POSITION, (data) => {
    greg.log(`[${MOD_NAME}] Rack position queried: ${data}`);
});

greg.subscribe(Events.CABLE_CREATED, (data) => {
    greg.log(`[${MOD_NAME}] New cable created with ID: ${data}`);
});

// ─── Custom Logic ────────────────────────────────────────────────────

function onModReady() {
    greg.log(`[${MOD_NAME}] Mod initialized successfully!`);
    
    // Example: Custom analytics
    let eventCount = 0;
    greg.subscribe(Events.COINS_CHANGED, () => {
        eventCount++;
        if (eventCount % 10 === 0) {
            greg.log(`[${MOD_NAME}] ${eventCount} coin events processed`);
        }
    });
}

onModReady();
