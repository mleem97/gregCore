# example_mod/main.py
def on_init():
    greg.log_info("Python Example Mod initialized!")
    
    # Subscribe to coin changed hook
    def on_coin_changed(payload):
        amount = payload["data"]["Amount"]
        total = payload["data"]["Total"]
        greg.log_info(f"Python received money update: {amount} (Total: {total})")
        
        # Fire a custom hook back
        greg.fire("greg.CUSTOM.PythonResponse", {
            "msg": "Python heard that!",
            "received_total": total
        })

    greg.on("greg.PLAYER.CoinChanged", on_coin_changed)

def on_update(dt):
    # Update logic
    pass

def on_shutdown():
    greg.log_info("Python Example Mod shutdown.")
