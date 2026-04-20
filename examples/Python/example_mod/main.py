def on_init():
    greg.log_info("Python Example Mod initialized!")
    greg.show_notification("Python Mod Active")

def on_update(dt):
    # dt is deltaTime
    pass

def on_event(event_id, data):
    if event_id == 100: # MoneyChanged
        greg.log_info("Money changed! Current: " + str(greg.get_player_money()))

def on_scene_loaded(name):
    greg.log_info("Entered scene: " + name)

def on_shutdown():
    greg.log_info("Python Mod shutting down.")
