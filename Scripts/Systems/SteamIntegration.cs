using System;
using UsurperRemake.Systems;

/// <summary>
/// Steam integration for achievements and other Steam features.
/// Only activates when the game is launched through Steam.
/// Uses conditional compilation to avoid Steamworks.NET dependency when not available.
/// </summary>
public static class SteamIntegration
{
    private static bool _initialized = false;
    private static bool _steamAvailable = false;

    /// <summary>
    /// Whether Steam is available and initialized
    /// </summary>
    public static bool IsAvailable => _initialized && _steamAvailable;

    /// <summary>
    /// Initialize Steam integration. Call this at game startup.
    /// Returns true if Steam is available, false otherwise.
    /// </summary>
    public static bool Initialize()
    {
        if (_initialized)
            return _steamAvailable;

        _initialized = true;

        try
        {
#if STEAM_BUILD
            // Only attempt Steam initialization if compiled with STEAM_BUILD flag
            if (!Steamworks.SteamAPI.Init())
            {
                DebugLogger.Instance?.LogInfo("STEAM", "Steam not running - achievements will be local only");
                _steamAvailable = false;
                return false;
            }

            _steamAvailable = true;
            DebugLogger.Instance?.LogInfo("STEAM", "Steam initialized successfully");

            // Request current stats from Steam
            Steamworks.SteamUserStats.RequestCurrentStats();

            return true;
#else
            // Not a Steam build - Steam features disabled
            DebugLogger.Instance?.LogInfo("STEAM", "Non-Steam build - Steam features disabled");
            _steamAvailable = false;
            return false;
#endif
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Failed to initialize Steam: {ex.Message}");
            _steamAvailable = false;
            return false;
        }
    }

    /// <summary>
    /// Call this periodically (e.g., in your game loop) to process Steam callbacks.
    /// </summary>
    public static void RunCallbacks()
    {
        if (!_steamAvailable) return;

#if STEAM_BUILD
        try
        {
            Steamworks.SteamAPI.RunCallbacks();
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Error running Steam callbacks: {ex.Message}");
        }
#endif
    }

    /// <summary>
    /// Shutdown Steam integration. Call this when the game exits.
    /// </summary>
    public static void Shutdown()
    {
        if (!_steamAvailable) return;

#if STEAM_BUILD
        try
        {
            Steamworks.SteamAPI.Shutdown();
            DebugLogger.Instance?.LogInfo("STEAM", "Steam shutdown complete");
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Error during Steam shutdown: {ex.Message}");
        }
#endif

        _steamAvailable = false;
        _initialized = false;
    }

    /// <summary>
    /// Unlock a Steam achievement by its API name.
    /// The API name should match what's configured in Steamworks.
    /// </summary>
    /// <param name="achievementId">The achievement API name (e.g., "first_blood")</param>
    /// <returns>True if the achievement was unlocked, false if Steam unavailable or already unlocked</returns>
    public static bool UnlockAchievement(string achievementId)
    {
        if (!_steamAvailable) return false;

#if STEAM_BUILD
        try
        {
            // Check if already unlocked
            if (Steamworks.SteamUserStats.GetAchievement(achievementId, out bool alreadyUnlocked) && alreadyUnlocked)
            {
                return false; // Already had it
            }

            // Unlock the achievement
            if (Steamworks.SteamUserStats.SetAchievement(achievementId))
            {
                // Store stats to sync with Steam
                Steamworks.SteamUserStats.StoreStats();
                DebugLogger.Instance?.LogInfo("STEAM", $"Achievement unlocked on Steam: {achievementId}");
                return true;
            }
            else
            {
                DebugLogger.Instance?.LogWarning("STEAM", $"Failed to set Steam achievement: {achievementId}");
                return false;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Error unlocking Steam achievement {achievementId}: {ex.Message}");
            return false;
        }
#else
        return false;
#endif
    }

    /// <summary>
    /// Check if a Steam achievement is unlocked.
    /// </summary>
    public static bool IsAchievementUnlocked(string achievementId)
    {
        if (!_steamAvailable) return false;

#if STEAM_BUILD
        try
        {
            if (Steamworks.SteamUserStats.GetAchievement(achievementId, out bool unlocked))
            {
                return unlocked;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Error checking Steam achievement {achievementId}: {ex.Message}");
        }
#endif

        return false;
    }

    /// <summary>
    /// Clear a Steam achievement (for testing only).
    /// </summary>
    public static bool ClearAchievement(string achievementId)
    {
        if (!_steamAvailable) return false;

#if STEAM_BUILD
        try
        {
            if (Steamworks.SteamUserStats.ClearAchievement(achievementId))
            {
                Steamworks.SteamUserStats.StoreStats();
                DebugLogger.Instance?.LogInfo("STEAM", $"Achievement cleared on Steam: {achievementId}");
                return true;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Error clearing Steam achievement {achievementId}: {ex.Message}");
        }
#endif

        return false;
    }

    /// <summary>
    /// Get the Steam user's display name, if available.
    /// </summary>
    public static string? GetPlayerName()
    {
        if (!_steamAvailable) return null;

#if STEAM_BUILD
        try
        {
            return Steamworks.SteamFriends.GetPersonaName();
        }
        catch
        {
            return null;
        }
#else
        return null;
#endif
    }

    /// <summary>
    /// Get the Steam App ID (useful for verifying Steam context).
    /// </summary>
    public static uint GetAppId()
    {
#if STEAM_BUILD
        if (!_steamAvailable) return 0;

        try
        {
            return Steamworks.SteamUtils.GetAppID().m_AppId;
        }
        catch
        {
            return 0;
        }
#else
        return 0;
#endif
    }

    /// <summary>
    /// Update a Steam stat (integer value).
    /// Stats must be configured in Steamworks dashboard.
    /// </summary>
    public static bool SetStat(string statName, int value)
    {
        if (!_steamAvailable) return false;

#if STEAM_BUILD
        try
        {
            if (Steamworks.SteamUserStats.SetStat(statName, value))
            {
                Steamworks.SteamUserStats.StoreStats();
                return true;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Error setting Steam stat {statName}: {ex.Message}");
        }
#endif

        return false;
    }

    /// <summary>
    /// Get a Steam stat (integer value).
    /// </summary>
    public static int GetStat(string statName)
    {
        if (!_steamAvailable) return 0;

#if STEAM_BUILD
        try
        {
            if (Steamworks.SteamUserStats.GetStat(statName, out int value))
            {
                return value;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("STEAM", $"Error getting Steam stat {statName}: {ex.Message}");
        }
#endif

        return 0;
    }
}
