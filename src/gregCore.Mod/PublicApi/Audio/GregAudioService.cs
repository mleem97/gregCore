/// <file-summary>
/// Schicht:      PublicApi (Audio)
/// Zweck:        Zentraler Audio-Service (gregCore Baukasten): Datei-Wiedergabe
///               mit ClipCache (Sofort-Start), Dual-Source-Crossfade,
///               zeitgescheiteltem Preload, sauberem Stop/Pause. Mods spielen
///               damit Musik/Sounds ohne eigene Codec-/Playback-Logik.
///               TrackEnded-Event fuer Queue-Weiterschaltung (Mod ruft Poll
///               pro Frame auf oder abonniert das Event).
/// </file-summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking;

namespace gregCore.PublicApi.Audio;

[ExcludeFromCodeCoverage(Justification = "Runtime audio over live game objects; needs running game.")]
public static class GregAudioService
{
    public const float CrossfadeDuration = 2.5f;
    public const int CacheCapacity = 3;

    // Feuert, wenn ein Titel natuerlich zu Ende ist (nicht bei Stop/Pause).
    public static event Action TrackEnded;

    // Feuert bei jedem Start (manuell + Queue): (Pfad, Titel).
    public static event Action<string, string> TrackStarted;

    private static string _currentTitle = "";
    private static string _currentPath = "";
    private static bool _wasPlaying;
    private static Il2Cpp.AudioManager _cachedManager;
    private static float _managerCacheTime;
    private const float ManagerCacheTtl = 2f;

    private static GameObject _fadeObj;
    private static AudioSource _fadeSource;
    private static bool _activeIsA = true;
    private static bool _fading;
    private static float _fadeT;
    private static AudioSource _fadeFrom;
    private static AudioSource _fadeTo;
    private static float _fadeVol;

    private static string _preloadingPath;

    public static string CurrentTitle => _currentTitle;
    public static string CurrentPath => _currentPath;

    public static bool IsPlaying
    {
        get
        {
            try
            {
                var src = ActiveSource();
                return src != null && src.isPlaying;
            }
            catch { return false; }
        }
    }

    public static Il2Cpp.AudioManager FindAudioManager()
    {
        if (_cachedManager != null && Time.realtimeSinceStartup - _managerCacheTime < ManagerCacheTtl)
            return _cachedManager;
        try
        {
            var mgr = Il2Cpp.AudioManager.instance;
            if (mgr != null && mgr.musicAudioSource != null)
            {
                _cachedManager = mgr;
                _managerCacheTime = Time.realtimeSinceStartup;
                return mgr;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        _cachedManager = null;
        return null;
    }

    private static AudioSource ActiveSource()
    {
        try
        {
            if (_activeIsA)
            {
                var mgr = FindAudioManager();
                return mgr != null ? mgr.musicAudioSource : null;
            }
            return _fadeSource;
        }
        catch { return null; }
    }

    private static void EnsureFadeSource()
    {
        if (_fadeSource != null) return;
        try
        {
            _fadeObj = new GameObject("gregAudioFadeSource");
            UnityEngine.Object.DontDestroyOnLoad(_fadeObj);
            _fadeSource = _fadeObj.AddComponent<AudioSource>();
            _fadeSource.playOnAwake = false;
            _fadeSource.loop = false;
        }
        catch { _fadeSource = null; }
    }

    public static void PlayFile(string filePath, string title, float volume)
    {
        if (string.IsNullOrEmpty(filePath)) return;
        if (string.IsNullOrEmpty(title)) title = Path.GetFileNameWithoutExtension(filePath);
        try
        {
            CancelPreload();
            var cached = GregAudioClipCache.Get(filePath);
            if (cached != null)
            {
                StartClip(filePath, title, cached, volume);
                return;
            }
            MelonCoroutines.Start(LoadAndPlay(filePath, title, volume));
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] Play: " + ex.GetBaseException().Message);
        }
    }

    private static void StartClip(string filePath, string title, AudioClip clip, float volume)
    {
        if (clip == null) return;
        try
        {
            EnsureFadeSource();
            var active = ActiveSource();
            bool audible = false;
            try
            {
                var f = _fading ? _fadeTo : active;
                audible = f != null && f.clip != null && (f.isPlaying || _fading);
                if (!audible && active != null)
                    audible = active.clip != null && active.isPlaying;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            _currentTitle = title;
            _currentPath = filePath;
            try { TrackStarted?.Invoke(filePath, title); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (!audible || _fadeSource == null || active == null)
            {
                _fading = false;
                if (active != null)
                {
                    try { active.Stop(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    active.clip = clip;
                    active.volume = volume;
                    active.loop = false;
                    active.Play();
                }
            }
            else
            {
                AudioSource to = _activeIsA ? _fadeSource : FindAudioManager()?.musicAudioSource;
                if (to == null || to == active)
                {
                    try { active.Stop(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    active.clip = clip;
                    active.volume = volume;
                    active.loop = false;
                    active.Play();
                }
                else
                {
                    _fadeFrom = active;
                    _fadeTo = to;
                    _fadeT = 0f;
                    _fadeVol = volume;
                    try
                    {
                        to.clip = clip;
                        to.volume = 0f;
                        to.loop = false;
                        to.Play();
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    _fading = true;
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] StartClip: " + ex.GetBaseException().Message);
        }
    }

    // Pro Frame aufrufen (Crossfade). Feuert TrackEnded bei Titelende.
    public static void Tick(float dt)
    {
        try
        {
            if (_fading)
            {
                _fadeT += dt / CrossfadeDuration;
                float k = _fadeT >= 1f ? 1f : _fadeT;
                k = k * k * (3f - 2f * k);
                if (_fadeFrom != null) { try { _fadeFrom.volume = _fadeVol * (1f - k); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
                if (_fadeTo != null) { try { _fadeTo.volume = _fadeVol * k; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
                if (_fadeT >= 1f)
                {
                    try { if (_fadeFrom != null) _fadeFrom.Stop(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    _activeIsA = !_activeIsA;
                    _fading = false;
                }
            }
            if (PollEnded()) { try { TrackEnded?.Invoke(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
        }
        catch { _fading = false; }
    }

    // True genau an der Ende-Flanke (Titel natuerlich zu Ende).
    public static bool Poll()
    {
        try { return PollEnded(); } catch { return false; }
    }

    private static bool PollEnded()
    {
        try
        {
            var src = ActiveSource();
            if (src == null) { _wasPlaying = false; return false; }
            bool playing = src.isPlaying;
            bool ended = _wasPlaying && !playing && !string.IsNullOrEmpty(_currentTitle);
            _wasPlaying = playing;
            return ended;
        }
        catch { return false; }
    }

    private static void SnapFade()
    {
        if (!_fading) return;
        try { if (_fadeFrom != null) _fadeFrom.Stop(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try { if (_fadeTo != null) _fadeTo.volume = _fadeVol; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        _activeIsA = !_activeIsA;
        _fading = false;
    }

    public static void Pause()
    {
        try
        {
            SnapFade();
            var src = ActiveSource();
            if (src == null) return;
            src.Pause();
            _wasPlaying = false;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void Resume(float volume)
    {
        try
        {
            var src = ActiveSource();
            if (src == null) return;
            src.volume = volume;
            src.UnPause();
            _wasPlaying = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void Stop()
    {
        try
        {
            CancelPreload();
            SnapFade();
            var a = FindAudioManager()?.musicAudioSource;
            try { if (a != null) { a.Stop(); a.clip = null; } } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { if (_fadeSource != null) { _fadeSource.Stop(); _fadeSource.clip = null; } } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            _wasPlaying = false;
            _currentTitle = "";
            _currentPath = "";
            _activeIsA = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void ApplyVolume(float volume)
    {
        try
        {
            if (volume < 0f) volume = 0f;
            if (volume > 1f) volume = 1f;
            var src = ActiveSource();
            if (src != null) { try { src.volume = volume; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
            _fadeVol = volume;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static string ProgressText()
    {
        try
        {
            var src = PlayedSource();
            if (src == null || src.clip == null) return "";
            float pos = src.time;
            float len = src.clip.length;
            if (len <= 0f) return "";
            if (pos < 0f) pos = 0f;
            if (pos > len) pos = len;
            return FormatTime(pos) + " / " + FormatTime(len);
        }
        catch { return ""; }
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
            var src = PlayedSource();
            if (src == null || src.clip == null) return -1f;
            float len = src.clip.length;
            if (len <= 0f) return -1f;
            float pos = src.time;
            if (pos < 0f) pos = 0f;
            if (pos > len) pos = len;
            return pos / len;
        }
        catch { return -1f; }
    }

    public static float RemainingSeconds()
    {
        try
        {
            var src = PlayedSource();
            if (src == null || src.clip == null || !src.isPlaying) return -1f;
            float len = src.clip.length;
            if (len <= 0f) return -1f;
            float rem = len - src.time;
            return rem < 0f ? 0f : rem;
        }
        catch { return -1f; }
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
            if (fraction < 0f) fraction = 0f;
            if (fraction > 1f) fraction = 1f;
            var src = PlayedSource();
            if (src == null || src.clip == null) return;
            float len = src.clip.length;
            if (len <= 0f) return;
            src.time = fraction * len;
            _wasPlaying = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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

    private static IEnumerator LoadAndPlay(string filePath, string title, float volume)
    {
        string url = ToFileUrl(filePath);
        byte[] data = null;
        string fail = null;
        UnityWebRequest req = null;
        try { req = UnityWebRequest.Get(url); } catch { fail = "Request"; }
        if (req != null)
        {
            req.timeout = 30;
            yield return req.SendWebRequest();
            try
            {
                if (req.result != UnityWebRequest.Result.Success)
                    fail = "Download: " + req.error;
                else
                {
                    try { data = req.downloadHandler != null ? req.downloadHandler.data : null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    if (data == null || data.Length == 0) fail = "Leer";
                }
            }
            catch (Exception ex) { fail = "Download: " + ex.GetBaseException().Message; }
        }
        if (fail != null)
        {
            MelonLogger.Warning("[gregCore][Audio] " + fail + " (" + title + ")");
            yield break;
        }

        AudioClip clip = DecodeToClip(title, filePath, data);
        if (clip == null) yield break;
        GregAudioClipCache.Put(filePath, clip);
        MelonLogger.Msg("[gregCore][Audio] Spielt: '" + title + "' (" + clip.length.ToString("F1") + "s)");
        StartClip(filePath, title, clip, volume);
    }

    private static AudioClip DecodeToClip(string title, string filePath, byte[] data)
    {
        try
        {
            float[] samples = null;
            int channels = 0;
            int frequency = 0;
            if (!AudioDecoder.TryDecode(filePath, data, out samples, out channels, out frequency)
                || samples == null || samples.Length == 0 || channels <= 0 || frequency <= 0)
            {
                MelonLogger.Warning("[gregCore][Audio] Dekodieren fehlgeschlagen (" + title + ")");
                return null;
            }
            int frames = samples.Length / channels;
            var clip = AudioClip.Create(title, frames, channels, frequency, false);
            if (clip == null || !clip.SetData(samples, 0))
            {
                MelonLogger.Warning("[gregCore][Audio] Clip-Erstellung fehlgeschlagen (" + title + ")");
                try { if (clip != null) UnityEngine.Object.Destroy(clip); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            }
            return clip;
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] Decode: " + ex.GetBaseException().Message);
            return null;
        }
    }

    public static bool IsCached(string filePath) => GregAudioClipCache.Has(filePath);

    public static bool IsPreloading(string filePath)
    {
        try { return !string.IsNullOrEmpty(filePath) && _preloadingPath == filePath; }
        catch { return false; }
    }

    public static void PreloadFile(string filePath, string title)
    {
        if (string.IsNullOrEmpty(filePath)) return;
        if (string.IsNullOrEmpty(title)) title = Path.GetFileNameWithoutExtension(filePath);
        try
        {
            if (GregAudioClipCache.Has(filePath)) return;
            if (_preloadingPath == filePath) return;
            _preloadingPath = filePath;
            MelonCoroutines.Start(PreloadSlices(filePath, title));
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void CancelPreload()
    {
        _preloadingPath = null;
    }

    private static IEnumerator PreloadSlices(string filePath, string title)
    {
        string path = filePath;
        string url = ToFileUrl(path);
        byte[] data = null;
        UnityWebRequest req = null;
        try { req = UnityWebRequest.Get(url); } catch { req = null; }
        if (req == null) { if (_preloadingPath == path) _preloadingPath = null; yield break; }
        req.timeout = 30;
        yield return req.SendWebRequest();
        if (_preloadingPath != path) yield break;
        bool ok = false;
        try { ok = req.result == UnityWebRequest.Result.Success; } catch { ok = false; }
        if (!ok) { if (_preloadingPath == path) _preloadingPath = null; yield break; }
        try { data = req.downloadHandler != null ? req.downloadHandler.data : null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        if (data == null || data.Length == 0) { if (_preloadingPath == path) _preloadingPath = null; yield break; }

        string ext = Path.GetExtension(path).ToLowerInvariant();
        if (ext == ".mp3")
        {
            NLayer.MpegFile mpeg = null;
            MemoryStream ms = null;
            var all = new List<float>();
            int ch = 0;
            int rate = 0;
            bool failed = false;
            try
            {
                ms = new MemoryStream(data, false);
                mpeg = new NLayer.MpegFile(ms);
                ch = mpeg.Channels;
                rate = mpeg.SampleRate;
                if (ch <= 0 || rate <= 0) failed = true;
            }
            catch { failed = true; }
            if (!failed)
            {
                int slice = Math.Max(1024, (rate * ch) / 10);
                var buf = new float[slice];
                try
                {
                    while (true)
                    {
                        if (_preloadingPath != path) break;
                        int read = 0;
                        try { read = mpeg.ReadSamples(buf, 0, buf.Length); } catch { break; }
                        if (read <= 0) break;
                        for (int i = 0; i < read; i++) all.Add(buf[i]);
                        if (read < buf.Length) break;
                        yield return null;
                    }
                }
                finally
                {
                    try { if (mpeg != null) mpeg.Dispose(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    try { if (ms != null) ms.Dispose(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
            else
            {
                try { if (mpeg != null) mpeg.Dispose(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { if (ms != null) ms.Dispose(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            if (_preloadingPath != path || all.Count == 0 || ch <= 0 || rate <= 0)
            {
                if (_preloadingPath == path) _preloadingPath = null;
                yield break;
            }
            FinishPreload(path, title, all.ToArray(), ch, rate);
            yield break;
        }

        float[] samples = null;
        int channels = 0;
        int frequency = 0;
        try
        {
            if (!AudioDecoder.TryDecode(path, data, out samples, out channels, out frequency))
            {
                if (_preloadingPath == path) _preloadingPath = null;
                yield break;
            }
        }
        catch
        {
            if (_preloadingPath == path) _preloadingPath = null;
            yield break;
        }
        if (_preloadingPath != path) yield break;
        FinishPreload(path, title, samples, channels, frequency);
    }

    private static void FinishPreload(string filePath, string title, float[] samples, int channels, int frequency)
    {
        string path = filePath;
        try
        {
            if (samples == null || samples.Length == 0 || channels <= 0 || frequency <= 0) return;
            int frames = samples.Length / channels;
            var clip = AudioClip.Create(title, frames, channels, frequency, false);
            if (clip == null) return;
            if (!clip.SetData(samples, 0))
            {
                try { UnityEngine.Object.Destroy(clip); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return;
            }
            GregAudioClipCache.Put(path, clip);
            MelonLogger.Msg("[gregCore][Audio] Vorgeladen: '" + title + "'.");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        finally
        {
            if (_preloadingPath == path) _preloadingPath = null;
        }
    }
}
