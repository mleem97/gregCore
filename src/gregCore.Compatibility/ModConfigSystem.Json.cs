/// <file-summary>
/// Layer:       Compatibility
/// Purpose:     Further parts of ModConfigSystem (partial): registry API /
///               JSON codec. Split due to codeline limits.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine.UIElements;

namespace DataCenterModLoader;

public static partial class ModConfigSystem
{
    private static Dictionary<string, ConfigEntry> ParseConfigJson(string json)
    {
        var result = new Dictionary<string, ConfigEntry>();

        try
        {
            string entriesBlock = ExtractEntriesBlock(json);
            if (entriesBlock == null) return result;
            ParseEntryBlock(entriesBlock, result);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.ParseConfigJson", ex);
        }

        return result;
    }

    private static string ExtractEntriesBlock(string json)
    {
        // Find the "entries" object
        int entriesIdx = json.IndexOf("\"entries\"", StringComparison.Ordinal);
        if (entriesIdx < 0) return null;

        // Find the opening brace of entries object
        int braceStart = json.IndexOf('{', entriesIdx + 9);
        if (braceStart < 0) return null;

        // Find the matching closing brace
        int braceEnd = FindMatchingBrace(json, braceStart);
        if (braceEnd < 0) return null;

        return json.Substring(braceStart + 1, braceEnd - braceStart - 1);
    }

    private static void ParseEntryBlock(string entriesBlock, Dictionary<string, ConfigEntry> result)
    {
        // Parse each entry: "key": { "type": "...", "value": ... }
        int pos = 0;
        while (pos < entriesBlock.Length)
        {
            int next = ParseOneEntry(entriesBlock, pos, result);
            if (next <= pos) break; // no-progress guard
            pos = next;
        }
    }

    // Returns the position after the parsed entry (or entriesBlock.Length at the end).
    private static int ParseOneEntry(string entriesBlock, int pos, Dictionary<string, ConfigEntry> result)
    {
        // Find the next key
        int keyStart = entriesBlock.IndexOf('"', pos);
        if (keyStart < 0) return entriesBlock.Length;

        int keyEnd = entriesBlock.IndexOf('"', keyStart + 1);
        if (keyEnd < 0) return entriesBlock.Length;

        string entryKey = UnescapeJsonString(entriesBlock.Substring(keyStart + 1, keyEnd - keyStart - 1));

        // Find the entry object opening brace
        int entryBraceStart = entriesBlock.IndexOf('{', keyEnd);
        if (entryBraceStart < 0) return entriesBlock.Length;

        int entryBraceEnd = FindMatchingBrace(entriesBlock, entryBraceStart);
        if (entryBraceEnd < 0) return entriesBlock.Length;

        string entryBody = entriesBlock.Substring(entryBraceStart + 1, entryBraceEnd - entryBraceStart - 1);

        // Parse type
        string? type = ExtractJsonStringValue(entryBody, "type");
        if (type == null) return entryBraceEnd + 1;

        // Parse value
        string? valueStr = ExtractJsonRawValue(entryBody, "value");
        if (valueStr == null) return entryBraceEnd + 1;

        var entry = new ConfigEntry { Key = entryKey };

        switch (type)
        {
            case "bool":
                entry.Type = ConfigEntryType.Bool;
                entry.BoolValue = valueStr.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                break;
            case "int":
                entry.Type = ConfigEntryType.Int;
                if (int.TryParse(valueStr.Trim(), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out int iv))
                    entry.IntValue = iv;
                break;
            case "float":
                entry.Type = ConfigEntryType.Float;
                if (float.TryParse(valueStr.Trim(), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out float fv))
                    entry.FloatValue = fv;
                break;
            default:
                return entryBraceEnd + 1;
        }

        result[entryKey] = entry;
        return entryBraceEnd + 1;
    }

    private static int FindMatchingBrace(string text, int openPos)
    {
        int depth = 0;
        for (int i = openPos; i < text.Length; i++)
        {
            if (text[i] == '{') depth++;
            else if (text[i] == '}') depth--;

            if (depth == 0) return i;
        }
        return -1;
    }

    private static string? ExtractJsonStringValue(string body, string key)
    {
        string pattern = "\"" + key + "\"";
        int idx = body.IndexOf(pattern, StringComparison.Ordinal);
        if (idx < 0) return null;

        int colonIdx = body.IndexOf(':', idx + pattern.Length);
        if (colonIdx < 0) return null;

        // Find the opening quote of the value
        int quoteStart = body.IndexOf('"', colonIdx + 1);
        if (quoteStart < 0) return null;

        int quoteEnd = body.IndexOf('"', quoteStart + 1);
        if (quoteEnd < 0) return null;

        return body.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
    }

    private static string? ExtractJsonRawValue(string body, string key)
    {
        string pattern = "\"" + key + "\"";
        int idx = body.IndexOf(pattern, StringComparison.Ordinal);
        if (idx < 0) return null;

        int colonIdx = body.IndexOf(':', idx + pattern.Length);
        if (colonIdx < 0) return null;

        // Skip whitespace after colon
        int start = colonIdx + 1;
        while (start < body.Length && (body[start] == ' ' || body[start] == '\t'))
            start++;

        if (start >= body.Length) return null;

        // If it's a quoted string, extract it
        if (body[start] == '"')
        {
            int quoteEnd = body.IndexOf('"', start + 1);
            if (quoteEnd < 0) return null;
            return body.Substring(start + 1, quoteEnd - start - 1);
        }

        // Otherwise read until comma, brace, bracket, or end
        int end = start;
        while (end < body.Length && body[end] != ',' && body[end] != '}' && body[end] != ']'
               && body[end] != '\r' && body[end] != '\n')
            end++;

        return body.Substring(start, end - start).Trim();
    }

    private static string EscapeJsonString(string? s)
    {
        if (s == null) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"")
                .Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }

    private static string UnescapeJsonString(string? s)
    {
        if (s == null) return "";
        return s.Replace("\\\"", "\"").Replace("\\\\", "\\")
                .Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
    }


}
