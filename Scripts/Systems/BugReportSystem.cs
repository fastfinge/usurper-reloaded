using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UsurperRemake.Systems;

/// <summary>
/// Bug reporting system that collects diagnostic info and opens GitHub Issues.
/// </summary>
public static class BugReportSystem
{
    private const string GitHubIssuesUrl = "https://github.com/binary-knight/usurper-reborn/issues/new";

    /// <summary>
    /// Generate a bug report and open browser to GitHub Issues with pre-filled content.
    /// </summary>
    public static async Task ReportBug(TerminalEmulator terminal, Character? player)
    {
        terminal.WriteLine("");
        terminal.SetColor("bright_yellow");
        terminal.WriteLine("===============================================================================");
        terminal.WriteLine("                              BUG REPORT                                        ");
        terminal.WriteLine("===============================================================================");
        terminal.SetColor("white");
        terminal.WriteLine("");
        terminal.WriteLine("This will open your browser to create a GitHub Issue with diagnostic info.");
        terminal.WriteLine("Please describe what happened and what you expected to happen.");
        terminal.WriteLine("");

        // Get bug description from user
        terminal.SetColor("cyan");
        terminal.WriteLine("What went wrong? (Press Enter for a blank description)");
        terminal.SetColor("white");
        var description = await terminal.GetInputAsync("> ");

        terminal.WriteLine("");
        terminal.WriteLine("Collecting diagnostic information...", "gray");

        // Collect diagnostic info
        var diagnostics = CollectDiagnostics(player);

        // Build the issue body
        var issueBody = BuildIssueBody(description, diagnostics);

        // Build the URL with pre-filled content
        var title = string.IsNullOrWhiteSpace(description)
            ? "Bug Report"
            : TruncateForTitle(description);

        var url = BuildGitHubIssueUrl(title, issueBody);

        // Copy to clipboard as backup
        bool copiedToClipboard = TryCopyToClipboard(issueBody);

        terminal.WriteLine("");
        if (copiedToClipboard)
        {
            terminal.WriteLine("Report copied to clipboard as backup.", "green");
        }

        // Try to open browser
        bool browserOpened = TryOpenBrowser(url);

        if (browserOpened)
        {
            terminal.SetColor("bright_green");
            terminal.WriteLine("");
            terminal.WriteLine("Browser opened! Please review and submit the issue.");
            terminal.WriteLine("You may need to log in to GitHub if not already logged in.");
        }
        else
        {
            terminal.SetColor("yellow");
            terminal.WriteLine("");
            terminal.WriteLine("Could not open browser automatically.");
            terminal.WriteLine("");
            terminal.WriteLine("Please visit:");
            terminal.SetColor("cyan");
            terminal.WriteLine("  https://github.com/binary-knight/usurper-reborn/issues/new");
            terminal.SetColor("yellow");
            terminal.WriteLine("");
            if (copiedToClipboard)
            {
                terminal.WriteLine("The report is in your clipboard - paste it into the issue body.");
            }
        }

        terminal.SetColor("white");
        terminal.WriteLine("");
        await terminal.PressAnyKey();
    }

    /// <summary>
    /// Collect diagnostic information about the game state.
    /// </summary>
    private static DiagnosticInfo CollectDiagnostics(Character? player)
    {
        var info = new DiagnosticInfo
        {
            GameVersion = GameConfig.Version,
            VersionName = GameConfig.VersionName,
            Platform = GetPlatformString(),
            OSVersion = Environment.OSVersion.ToString(),
            DotNetVersion = Environment.Version.ToString(),
            Is64Bit = Environment.Is64BitProcess,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            IsSteamBuild = IsSteamBuild()
        };

        if (player != null)
        {
            info.PlayerLevel = player.Level;
            info.PlayerClass = player.Class.ToString();
            info.PlayerRace = player.Race.ToString();
            info.CurrentHP = player.HP;
            info.MaxHP = player.MaxHP;
            info.CurrentLocation = ((GameLocation)player.Location).ToString();
            info.DungeonFloor = GetDungeonFloor(player);
            info.TotalPlayTime = player.Statistics?.GetFormattedPlayTime() ?? "Unknown";
        }

        // Get recent debug log entries
        info.RecentLogEntries = GetRecentLogEntries(20);

        return info;
    }

    /// <summary>
    /// Build the issue body with all diagnostic info.
    /// </summary>
    private static string BuildIssueBody(string description, DiagnosticInfo info)
    {
        var sb = new StringBuilder();

        sb.AppendLine("## Bug Description");
        if (!string.IsNullOrWhiteSpace(description))
        {
            sb.AppendLine(description);
        }
        else
        {
            sb.AppendLine("_Please describe what happened and what you expected to happen._");
        }
        sb.AppendLine();

        sb.AppendLine("## Steps to Reproduce");
        sb.AppendLine("1. ");
        sb.AppendLine("2. ");
        sb.AppendLine("3. ");
        sb.AppendLine();

        sb.AppendLine("## Game State");
        sb.AppendLine("| Property | Value |");
        sb.AppendLine("|----------|-------|");
        sb.AppendLine($"| Version | {info.GameVersion} ({info.VersionName}) |");
        sb.AppendLine($"| Build Type | {(info.IsSteamBuild ? "Steam" : "Standard")} |");
        sb.AppendLine($"| Platform | {info.Platform} |");
        sb.AppendLine($"| OS | {info.OSVersion} |");
        if (info.PlayerLevel > 0)
        {
            sb.AppendLine($"| Character | Level {info.PlayerLevel} {info.PlayerRace} {info.PlayerClass} |");
            sb.AppendLine($"| HP | {info.CurrentHP}/{info.MaxHP} |");
            sb.AppendLine($"| Location | {info.CurrentLocation} |");
            if (info.DungeonFloor > 0)
            {
                sb.AppendLine($"| Dungeon Floor | {info.DungeonFloor} |");
            }
            sb.AppendLine($"| Play Time | {info.TotalPlayTime} |");
        }
        sb.AppendLine();

        if (!string.IsNullOrEmpty(info.RecentLogEntries))
        {
            sb.AppendLine("<details>");
            sb.AppendLine("<summary>Recent Debug Log (click to expand)</summary>");
            sb.AppendLine();
            sb.AppendLine("```");
            sb.AppendLine(info.RecentLogEntries);
            sb.AppendLine("```");
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine($"_Reported via in-game bug reporter at {info.Timestamp}_");

        return sb.ToString();
    }

    /// <summary>
    /// Build the GitHub issue URL with pre-filled title and body.
    /// </summary>
    private static string BuildGitHubIssueUrl(string title, string body)
    {
        // URL encode the parameters
        var encodedTitle = Uri.EscapeDataString(title);
        var encodedBody = Uri.EscapeDataString(body);

        // GitHub has a URL length limit, so we may need to truncate
        var url = $"{GitHubIssuesUrl}?title={encodedTitle}&body={encodedBody}&labels=bug,in-game-report";

        // Most browsers support URLs up to ~2000 chars, GitHub accepts longer
        // but we'll truncate the body if needed
        if (url.Length > 8000)
        {
            var truncatedBody = body.Substring(0, Math.Min(body.Length, 3000)) + "\n\n_[Log truncated due to URL length]_";
            encodedBody = Uri.EscapeDataString(truncatedBody);
            url = $"{GitHubIssuesUrl}?title={encodedTitle}&body={encodedBody}&labels=bug,in-game-report";
        }

        return url;
    }

    /// <summary>
    /// Try to open the URL in the default browser.
    /// </summary>
    private static bool TryOpenBrowser(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: use cmd /c start
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start \"\" \"{url}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
                return true;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogError("BUG_REPORT", $"Failed to open browser: {ex.Message}");
        }

        return false;
    }

    /// <summary>
    /// Try to copy text to clipboard.
    /// </summary>
    private static bool TryCopyToClipboard(string text)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Use clip.exe on Windows
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = "/c clip",
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.StandardInput.Write(text);
                process.StandardInput.Close();
                process.WaitForExit(2000);
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Try xclip or xsel
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "xclip",
                            Arguments = "-selection clipboard",
                            RedirectStandardInput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    process.StandardInput.Write(text);
                    process.StandardInput.Close();
                    process.WaitForExit(2000);
                    return true;
                }
                catch
                {
                    // xclip not available
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "pbcopy",
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.StandardInput.Write(text);
                process.StandardInput.Close();
                process.WaitForExit(2000);
                return true;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance?.LogWarning("BUG_REPORT", $"Failed to copy to clipboard: {ex.Message}");
        }

        return false;
    }

    /// <summary>
    /// Get a platform string for diagnostics.
    /// </summary>
    private static string GetPlatformString()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return $"Windows {(Environment.Is64BitProcess ? "x64" : "x86")}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return $"Linux {RuntimeInformation.OSArchitecture}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return $"macOS {RuntimeInformation.OSArchitecture}";
        return "Unknown";
    }

    /// <summary>
    /// Check if this is a Steam build.
    /// </summary>
    private static bool IsSteamBuild()
    {
#if STEAM_BUILD
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// Get the current dungeon floor if in dungeon.
    /// </summary>
    private static int GetDungeonFloor(Character? player)
    {
        try
        {
            if (player != null && (GameLocation)player.Location == GameLocation.Dungeons)
            {
                // Try to get dungeon floor from statistics
                return player.Statistics?.DeepestDungeonLevel ?? 0;
            }
        }
        catch { }
        return 0;
    }

    /// <summary>
    /// Get recent entries from the debug log file.
    /// </summary>
    private static string GetRecentLogEntries(int lineCount)
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "debug.log");
            if (File.Exists(logPath))
            {
                var lines = File.ReadAllLines(logPath);
                var startIndex = Math.Max(0, lines.Length - lineCount);
                var recentLines = new string[Math.Min(lineCount, lines.Length)];
                Array.Copy(lines, startIndex, recentLines, 0, recentLines.Length);
                return string.Join("\n", recentLines);
            }
        }
        catch
        {
            // Silently fail if we can't read the log
        }
        return "";
    }

    /// <summary>
    /// Truncate a string to be suitable for a GitHub issue title.
    /// </summary>
    private static string TruncateForTitle(string text)
    {
        // Clean up and truncate for title
        var cleaned = text.Replace("\n", " ").Replace("\r", " ").Trim();
        if (cleaned.Length > 80)
        {
            cleaned = cleaned.Substring(0, 77) + "...";
        }
        return $"[Bug] {cleaned}";
    }

    /// <summary>
    /// Diagnostic information container.
    /// </summary>
    private class DiagnosticInfo
    {
        public string GameVersion { get; set; } = "";
        public string VersionName { get; set; } = "";
        public string Platform { get; set; } = "";
        public string OSVersion { get; set; } = "";
        public string DotNetVersion { get; set; } = "";
        public bool Is64Bit { get; set; }
        public bool IsSteamBuild { get; set; }
        public string Timestamp { get; set; } = "";
        public int PlayerLevel { get; set; }
        public string PlayerClass { get; set; } = "";
        public string PlayerRace { get; set; } = "";
        public long CurrentHP { get; set; }
        public long MaxHP { get; set; }
        public string CurrentLocation { get; set; } = "";
        public int DungeonFloor { get; set; }
        public string TotalPlayTime { get; set; } = "";
        public string RecentLogEntries { get; set; } = "";
    }
}
