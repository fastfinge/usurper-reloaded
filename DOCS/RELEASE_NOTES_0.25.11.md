# Usurper Reborn - v0.25.11 Release Notes

## BBS Door Mode Bug Fixes

### Critical: Opening Story Crash Fixed
- **Console.KeyAvailable crash in BBS door mode**: Creating a new character in BBS door mode would crash with `InvalidOperationException: Cannot see if a key has been pressed when either application does not have a console`. The opening story sequence's "press SPACE to skip" feature was using `Console.KeyAvailable` which is unavailable when stdin is redirected by BBS software. Fixed by detecting BBS door mode and skipping the console key check, with a try/catch fallback for safety.

### Critical: Magic Shop Crash Fixed
- **16 direct Console I/O calls replaced**: The Magic Shop (`MagicShopLocation.cs`) was using raw `Console.ReadKey()` and `Console.ReadLine()` calls instead of the terminal abstraction layer. This would crash in BBS door mode when a user tried to buy/sell items, identify items, remove curses, enchant items, or browse paginated item lists. All 16 calls replaced with `terminal.GetInputSync()` which routes through the proper BBS I/O system (socket or stdio).

### Files Changed
- `OpeningStorySystem.cs` - Wrapped `Console.KeyAvailable`/`Console.ReadKey` in BBS door mode check with try/catch fallback
- `MagicShopLocation.cs` - Replaced all 16 `Console.ReadKey()`/`Console.ReadLine()` calls with `terminal.GetInputSync()`
- `GameConfig.cs` - Version 0.25.11-alpha
- `README.md` - Updated version
