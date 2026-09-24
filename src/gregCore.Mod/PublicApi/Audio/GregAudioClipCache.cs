using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace gregCore.PublicApi.Audio;

// Vorgeladene Clips (max. N, LRU): Sofort-Start ohne Decode-Pause.
// Schlüssel: Dateipfad. Evict zerstört den Clip (RAM freigeben).
public static class GregAudioClipCache
{
    public sealed class Entry
    {
        public AudioClip Clip;
        public long LastUsed;
    }

    public const int Capacity = 3;

    private static readonly Dictionary<string, Entry> _cache =
        new Dictionary<string, Entry>(System.StringComparer.OrdinalIgnoreCase);
    private static long _tick;

    public static int Count
    {
        get { try { lock (_cache) { return _cache.Count; } } catch { return 0; } }
    }

    public static bool Has(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return false;
        try { lock (_cache) { return _cache.ContainsKey(filePath); } }
        catch { return false; }
    }

    public static AudioClip Get(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return null;
        try
        {
            lock (_cache)
            {
                Entry e;
                if (!_cache.TryGetValue(filePath, out e) || e == null || e.Clip == null)
                    return null;
                e.LastUsed = ++_tick;
                return e.Clip;
            }
        }
        catch { return null; }
    }

    public static void Put(string filePath, AudioClip clip)
    {
        if (string.IsNullOrEmpty(filePath) || clip == null) return;
        try
        {
            lock (_cache)
            {
                Entry old;
                if (_cache.TryGetValue(filePath, out old) && old != null && old.Clip != null && old.Clip != clip)
                {
                    try { UnityEngine.Object.Destroy(old.Clip); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                _cache[filePath] = new Entry { Clip = clip, LastUsed = ++_tick };
                while (_cache.Count > Capacity)
                {
                    string lru = null;
                    long best = long.MaxValue;
                    foreach (var kv in _cache)
                    {
                        if (kv.Value != null && kv.Value.LastUsed < best)
                        {
                            best = kv.Value.LastUsed;
                            lru = kv.Key;
                        }
                    }
                    if (lru == null) break;
                    Entry ev;
                    if (_cache.TryGetValue(lru, out ev) && ev != null && ev.Clip != null)
                    {
                        try { UnityEngine.Object.Destroy(ev.Clip); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                    _cache.Remove(lru);
                    MelonLogger.Msg("[MusicPlayer] ClipCache: Evict '" + lru + "'.");
                }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void Clear()
    {
        try
        {
            lock (_cache)
            {
                foreach (var kv in _cache)
                {
                    try { if (kv.Value != null && kv.Value.Clip != null) UnityEngine.Object.Destroy(kv.Value.Clip); }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                _cache.Clear();
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
