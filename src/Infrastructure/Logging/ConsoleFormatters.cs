using System;
using System.Text;

namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Hilfsklasse zur Erzeugung der gregCore-spezifischen CLI-Strings.
/// </summary>
public static class ConsoleFormatters
{
    public static string CreatePrefix(string level, string component, bool showTimestamp)
    {
        var sb = new StringBuilder();
        
        if (showTimestamp)
        {
            sb.Append($"{DateTime.Now:HH:mm:ss} ");
        }

        sb.Append("» ");
        sb.Append(ConsoleTheme.GetLevelPrefix(level).PadRight(5));
        
        if (!string.IsNullOrEmpty(component))
        {
            sb.Append($" | {component.ToUpper()} |");
        }
        else
        {
            sb.Append(" | gregCore |");
        }
        
        return sb.ToString();
    }

    public static string FormatStatusLine(string statusContent)
    {
        return $"[gregCore][STATUS] {statusContent}";
    }

    public static string[] CreateBox(string[] lines)
    {
        int maxWidth = 0;
        foreach (var line in lines) maxWidth = Math.Max(maxWidth, line.Length);
        
        int boxWidth = maxWidth + 4;
        var result = new string[lines.Length + 2];
        
        result[0] = ConsoleTheme.BoxTopLeft + new string(ConsoleTheme.BoxHorizontal, boxWidth - 2) + ConsoleTheme.BoxTopRight;
        
        for (int i = 0; i < lines.Length; i++)
        {
            result[i + 1] = ConsoleTheme.BoxVertical + " " + lines[i].PadRight(maxWidth) + " " + ConsoleTheme.BoxVertical;
        }
        
        result[result.Length - 1] = ConsoleTheme.BoxBottomLeft + new string(ConsoleTheme.BoxHorizontal, boxWidth - 2) + ConsoleTheme.BoxBottomRight;
        
        return result;
    }
}
