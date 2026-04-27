"""
gregCore Python SDK – Example Mod

Demonstrates how to use the gregCore Python API via pythonnet bridge.
Place this file in: Data Center/Mods/PyMods/your_mod/main.py

Available APIs:
    greg.subscribe(event_id, callback)   – Subscribe to a greg event
    greg.fire_event(event_id, data)      – Fire a custom event
    greg.log(message)                    – Log to MelonLogger
"""

# ─── Mod Metadata ─────────────────────────────────────────────────────
MOD_NAME = "PyExampleMod"
MOD_VERSION = "1.0.0"

greg.log(f"[{MOD_NAME} v{MOD_VERSION}] Loading...")


# ─── Event IDs (must match gregCore.Core.Events.EventIds) ─────────────
class Events:
    COINS_CHANGED = 1001
    XP_CHANGED = 1002
    REPUTATION_CHANGED = 1003
    GAME_SAVED = 2001
    SERVER_STATUS = 3001
    RACK_POSITION = 3002
    CABLE_CREATED = 4001


# ─── Event Handlers ───────────────────────────────────────────────────

def on_coins_changed(data):
    greg.log(f"[{MOD_NAME}] Coins changed! Data: {data}")


def on_xp_changed(data):
    greg.log(f"[{MOD_NAME}] XP changed! Data: {data}")


def on_game_saved(data):
    greg.log(f"[{MOD_NAME}] Game saved at: {data}")


def on_rack_position(data):
    greg.log(f"[{MOD_NAME}] Rack position queried: {data}")


def on_cable_created(data):
    greg.log(f"[{MOD_NAME}] New cable created with ID: {data}")


# ─── Register Handlers ───────────────────────────────────────────────

greg.subscribe(Events.COINS_CHANGED, on_coins_changed)
greg.subscribe(Events.XP_CHANGED, on_xp_changed)
greg.subscribe(Events.GAME_SAVED, on_game_saved)
greg.subscribe(Events.RACK_POSITION, on_rack_position)
greg.subscribe(Events.CABLE_CREATED, on_cable_created)


# ─── Custom Logic ────────────────────────────────────────────────────

class CoinTracker:
    """Example: Track total coin earnings across a session."""
    
    def __init__(self):
        self.total_earned = 0.0
        self.transaction_count = 0
    
    def on_coin_change(self, data):
        self.transaction_count += 1
        self.total_earned += float(data) if data else 0.0
        
        if self.transaction_count % 5 == 0:
            greg.log(
                f"[{MOD_NAME}] Session stats: "
                f"{self.transaction_count} transactions, "
                f"${self.total_earned:.2f} total"
            )


tracker = CoinTracker()
greg.subscribe(Events.COINS_CHANGED, tracker.on_coin_change)

greg.log(f"[{MOD_NAME}] Mod initialized successfully!")
