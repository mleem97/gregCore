using System;
using System.Collections.Generic;

namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Definiert das visuelle Design der gregCore CLI.
/// </summary>
public static class ConsoleTheme
{
    // Terminal Colors (MelonLoader)
    public static ConsoleColor ColorGregCore = ConsoleColor.Cyan;
    public static ConsoleColor ColorComponent = ConsoleColor.Gray;
    public static ConsoleColor ColorMessage = ConsoleColor.White;

    // Box Drawing Characters
    public const char BoxHorizontal = '═';
    public const char BoxVertical = '║';
    public const char BoxTopLeft = '╔';
    public const char BoxTopRight = '╗';
    public const char BoxBottomLeft = '╚';
    public const char BoxBottomRight = '╝';

    public static readonly string[] BannerLogo = new[]
    {
        @"  ____ ____  _____  ____  ____ ___  ____  _____ ",
        @" / ___|  _ \| ____|/ ___|/ ___/ _ \|  _ \| ____|",
        @"| |  _| |_) |  _| | |  _| |  | | | | |_) |  _|  ",
        @"| |_| |  _ <| |___| |_| | |__| |_| |  _ <| |___ ",
        @" \____|_| \_\_____|\____|\____\___/|_| \_\_____|",
        @"                                               ",
        @"  >> THE ULTIMATE MODDING FRAMEWORK <<         "
    };

    public static string GetLevelPrefix(string level) => level.ToLower() switch
    {
        "info" => "INFO",
        "success" or "ok" => "OK",
        "warn" or "warning" or "wrn" => "WRN",
        "error" or "err" => "ERR",
        "debug" or "dbg" => "DBG",
        "status" => "STATUS",
        _ => "INFO"
    };

    public static ConsoleColor GetLevelColor(string level) => level.ToLower() switch
    {
        "info" => ConsoleColor.Cyan,
        "warn" or "warning" or "wrn" => ConsoleColor.Yellow,
        "error" or "err" => ConsoleColor.Red,
        "success" or "ok" => ConsoleColor.Green,
        "debug" or "dbg" => ConsoleColor.DarkGray,
        "status" => ConsoleColor.Magenta,
        _ => ConsoleColor.Gray
    };
}
