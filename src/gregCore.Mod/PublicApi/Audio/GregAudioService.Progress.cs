using System;
using MelonLoader;
using UnityEngine;

namespace gregCore.PublicApi.Audio;

public static partial class GregAudioService
{
    public static string ProgressText()
    {
        try
        {
            var src = PlayedSource();
            if (src == null || src.clip == null) return "";
            float pos = ClampPosition(src.time, src.clip.length, out float len);
            if (len <= 0f) return "";
            return FormatTime(pos) + " / " + FormatTime(len);
        }
        catch { return ""; }
    }

    private static float ClampPosition(float pos, float len, out float outLen)
    {
        outLen = len;
        try
        {
            if (pos < 0f) pos = 0f;
            if (pos > len) pos = len;
            return pos;
        }
        catch { return 0f; }
    }

    private static string FormatTime(float s)
    {
        int m = (int)(s / 60f);
        int sec = (int)(s % 60f);
        return m + ":" + (sec < 10 ? "0" : "") + sec;
    }

    public static float ProgressFraction()
    {
        try
        {
            return TryProgressFraction(out float v) ? v : -1f;
        }
        catch { return -1f; }
    }

    private static bool TryProgressFraction(out float value)
    {
        value = -1f;
        try
        {
            var src = PlayedSource();
            if (src == null || src.clip == null) return false;
            float len = src.clip.length;
            if (len <= 0f) return false;
            float pos = src.time;
            if (pos < 0f) pos = 0f;
            if (pos > len) pos = len;
            value = pos / len;
            return true;
        }
        catch { return false; }
    }

    public static float RemainingSeconds()
    {
        try
        {
            return TryRemaining(out float v) ? v : -1f;
        }
        catch { return -1f; }
    }

    private static bool TryRemaining(out float value)
    {
        value = -1f;
        try
        {
            var src = PlayedSource();
            if (src == null || src.clip == null || !src.isPlaying) return false;
            float len = src.clip.length;
            if (len <= 0f) return false;
            float rem = len - src.time;
            value = rem < 0f ? 0f : rem;
            return true;
        }
        catch { return false; }
    }

    private static AudioSource PlayedSource()
    {
        try
        {
            if (_fading && _fadeTo != null) return _fadeTo;
            return ActiveSource();
        }
        catch { return null; }
    }

    public static void Seek(float fraction)
    {
        try
        {
            float clamped = Clamp01(fraction);
            var src = PlayedSource();
            if (src == null || src.clip == null) return;
            float len = src.clip.length;
            if (len <= 0f) return;
            src.time = clamped * len;
            _wasPlaying = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static float Clamp01(float v)
    {
        try
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
        catch { return 0f; }
    }

    private static string ToFileUrl(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        try { return new Uri(path).AbsoluteUri; }
        catch
        {
            string url = path.Replace("\\", "/");
            return "file:///" + Uri.EscapeDataString(url.Replace(":", "|")).Replace("|", ":");
        }
    }
}
