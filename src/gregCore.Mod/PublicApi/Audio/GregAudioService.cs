/// <file-summary>
/// Layer:       PublicApi (Audio)
/// Purpose:     Central audio service (gregCore kit): file playback
///               with clip cache (instant start), dual-source crossfade,
///               staggered preload, clean stop/pause. Mods play
///               music/sounds without their own codec/playback logic.
///               TrackEnded event for queue advancement (mod calls Poll
///               per frame or subscribes to the event).
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
public static partial class GregAudioService
{
    public const float CrossfadeDuration = 2.5f;
    public const int CacheCapacity = 3;

    // Fires when a track ends naturally (not on stop/pause).
    public static event Action TrackEnded;

    // Fires on every start (manual + queue): (path, title).
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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  _fadeSource = null; }
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
            bool audible = IsAudible(active);
            _currentTitle = title;
            _currentPath = filePath;
            try { TrackStarted?.Invoke(filePath, title); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (!audible || _fadeSource == null || active == null)
            {
                PlayDirect(active, clip, volume);
            }
            else
            {
                PlayWithCrossfade(active, clip, volume);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] StartClip: " + ex.GetBaseException().Message);
        }
    }

    private static bool IsAudible(AudioSource active)
    {
        try
        {
            var f = _fading ? _fadeTo : active;
            bool audible = f != null && f.clip != null && (f.isPlaying || _fading);
            if (!audible && active != null)
                audible = active.clip != null && active.isPlaying;
            return audible;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private static void PlayDirect(AudioSource active, AudioClip clip, float volume)
    {
        try
        {
            _fading = false;
            if (active == null) return;
            try { active.Stop(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            active.clip = clip;
            active.volume = volume;
            active.loop = false;
            active.Play();
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] PlayDirect: " + ex.GetBaseException().Message);
        }
    }

    private static void PlayWithCrossfade(AudioSource active, AudioClip clip, float volume)
    {
        try
        {
            AudioSource to = _activeIsA ? _fadeSource : FindAudioManager()?.musicAudioSource;
            if (to == null || to == active)
            {
                PlayDirect(active, clip, volume);
                return;
            }
            StartCrossfade(active, to, clip, volume);
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] Crossfade: " + ex.GetBaseException().Message);
        }
    }

    private static void StartCrossfade(AudioSource active, AudioSource to, AudioClip clip, float volume)
    {
        try
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // Call per frame (crossfade). Fires TrackEnded at end of track.
    public static void Tick(float dt)
    {
        try
        {
            TickFade(dt);
            TickEnded();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  _fading = false; }
    }

    private static void TickFade(float dt)
    {
        try
        {
            if (!_fading) return;
            _fadeT += dt / CrossfadeDuration;
            float k = _fadeT >= 1f ? 1f : _fadeT;
            k = k * k * (3f - 2f * k);
            ApplyFadeVolumes(k);
            if (_fadeT >= 1f) CompleteFade();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  _fading = false; }
    }

    private static void ApplyFadeVolumes(float k)
    {
        try
        {
            if (_fadeFrom != null) { try { _fadeFrom.volume = _fadeVol * (1f - k); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
            if (_fadeTo != null) { try { _fadeTo.volume = _fadeVol * k; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void CompleteFade()
    {
        try { if (_fadeFrom != null) _fadeFrom.Stop(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        _activeIsA = !_activeIsA;
        _fading = false;
    }

    private static void TickEnded()
    {
        try
        {
            if (PollEnded()) { try { TrackEnded?.Invoke(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // True exactly on the trailing edge (track ended naturally).
    public static bool Poll()
    {
        try { return PollEnded(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
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

    private static IEnumerator LoadAndPlay(string filePath, string title, float volume)
    {
        string url = ToFileUrl(filePath);
        byte[] data = null;
        string fail = null;
        UnityWebRequest req = null;
        try { req = UnityWebRequest.Get(url); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  fail = "Request"; }
        if (req != null)
        {
            req.timeout = 30;
            yield return req.SendWebRequest();
            fail = ReadDownload(req, title, out data);
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

    private static string ReadDownload(UnityWebRequest req, string title, out byte[] data)
    {
        data = null;
        try
        {
            if (req.result != UnityWebRequest.Result.Success)
                return "Download: " + req.error;
            try { data = req.downloadHandler != null ? req.downloadHandler.data : null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (data == null || data.Length == 0) return "Leer";
            return null;
        }
        catch (Exception ex) { return "Download: " + ex.GetBaseException().Message; }
    }

    private static AudioClip DecodeToClip(string title, string filePath, byte[] data)
    {
        try
        {
            if (!TryGetSamples(title, filePath, data, out float[] samples, out int channels, out int frequency))
                return null;
            return CreateClip(title, samples, channels, frequency);
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] Decode: " + ex.GetBaseException().Message);
            return null;
        }
    }

    private static bool TryGetSamples(string title, string filePath, byte[] data, out float[] samples, out int channels, out int frequency)
    {
        samples = null;
        channels = 0;
        frequency = 0;
        try
        {
            if (!AudioDecoder.TryDecode(filePath, data, out samples, out channels, out frequency)
                || samples == null || samples.Length == 0 || channels <= 0 || frequency <= 0)
            {
                MelonLogger.Warning("[gregCore][Audio] Decoding failed (" + title + ")");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] Decode: " + ex.GetBaseException().Message);
            return false;
        }
    }

    private static AudioClip CreateClip(string title, float[] samples, int channels, int frequency)
    {
        try
        {
            int frames = samples.Length / channels;
            var clip = AudioClip.Create(title, frames, channels, frequency, false);
            if (clip == null || !clip.SetData(samples, 0))
            {
                MelonLogger.Warning("[gregCore][Audio] Clip creation failed (" + title + ")");
                try { if (clip != null) UnityEngine.Object.Destroy(clip); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            }
            return clip;
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Audio] Clip: " + ex.GetBaseException().Message);
            return null;
        }
    }

    public static bool IsCached(string filePath) => GregAudioClipCache.Has(filePath);

    public static bool IsPreloading(string filePath)
    {
        try { return !string.IsNullOrEmpty(filePath) && _preloadingPath == filePath; }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }
}
