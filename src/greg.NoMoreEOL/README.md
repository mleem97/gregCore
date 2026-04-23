# greg.NoMoreEOL

## What it does
- Prevents servers & switches from reaching end-of-life
- Automatically repairs broken devices
- Runs continuously during gameplay
- Includes an in-game config menu (via RustBridge)

## Why I made it
I wanted to remove the repetitive maintenance part of the game and focus more on building and optimizing the network.

## Details
This module is fully integrated into the `gregCore` framework. It utilizes the new Settings API to expose configuration toggles directly in the F8 Settings Hub.

### Configuration Options
- **Auto Repair Switches**: Automatically repairs broken network switches.
- **Auto Repair Servers**: Automatically repairs broken servers.
- **Disable Switches EOL**: Prevents switches from reaching End-Of-Life.
- **Disable Servers EOL**: Prevents servers from reaching End-Of-Life.
