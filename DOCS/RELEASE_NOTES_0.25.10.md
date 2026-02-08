# Usurper Reborn - v0.25.10 Release Notes

## SysOp Console Improvements

### SysOp Console Menu Integration
- **Integrated Menu Option**: The SysOp Administration Console (`[%]`) is now a standard menu item on the BBS character selection screen, appearing alongside Load/New/Quit options. Previously it was a separate prompt with a banner that appeared before the menu, which was disruptive to the login flow.
- **Hidden from Players**: The `[%]` option only appears for users who pass the SysOp security level check. Regular BBS users see no trace of it.
- **Works With or Without Saves**: The SysOp Console option appears whether the user has an existing character or not.

### Configurable SysOp Security Level
- **New `--sysop-level` Flag**: SysOps can now set the minimum security level required for admin access via command-line: `UsurperReborn --door32 door32.sys --sysop-level 99`
- **Persistent Setting**: The security level is saved to `sysop_config.json`, so it only needs to be specified once. Subsequent launches will use the saved value.
- **Default Threshold**: 100 (standard for most BBS software). Different BBSes use different security level schemes, so this allows SysOps to match their configuration.

### Version Display
- **Version shown everywhere**: The game version is now displayed on the title screen, main menu, and BBS door welcome screen across all modes (local, Steam, BBS door).

### Files Changed
- `GameEngine.cs` - Integrated SysOp Console into character selection menu; added version display to title screen, main menu, and BBS welcome banner
- `DoorMode.cs` - Added `--sysop-level <number>` command-line flag
- `SysOpConfigSystem.cs` - Added `SysOpSecurityLevel` to persistent config with sync to DoorMode
- `GameConfig.cs` - Version 0.25.10-alpha
- `README.md` - Updated version, revised SysOp Console description
- `DOCS/BBS_DOOR_SETUP.md` - Documented `--sysop-level` flag

### BBS Setup Example

```bash
# Standard setup (default security level 100)
UsurperReborn --door32 %f

# Custom security level for BBSes that use different thresholds
UsurperReborn --door32 %f --sysop-level 99

# With verbose debugging
UsurperReborn --door32 %f --sysop-level 99 --verbose
```
