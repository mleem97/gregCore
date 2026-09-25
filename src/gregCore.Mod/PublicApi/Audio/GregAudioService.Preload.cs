using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;

namespace gregCore.PublicApi.Audio;

public static partial class GregAudioService
{
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
        byte[] data = null;
        if (!TryStartPreloadRequest(path, out string url))
            yield break;
        UnityWebRequest req = null;
        try { req = UnityWebRequest.Get(url); } catch { req = null; }
        if (req == null) { ClearPreloadIfCurrent(path); yield break; }
        req.timeout = 30;
        yield return req.SendWebRequest();
        if (!TryReadPreloadBytes(req, path, out data))
            yield break;
        string ext = GetExtension(path);
        if (ext == ".mp3")
        {
            foreach (var step in PreloadMp3Steps(path, title, data))
                yield return step;
            yield break;
        }
        FinishDecodedPreload(path, title, data);
    }

    private static bool TryStartPreloadRequest(string path, out string url)
    {
        url = "";
        try
        {
            url = ToFileUrl(path);
            return true;
        }
        catch { return false; }
    }

    private static bool TryReadPreloadBytes(UnityWebRequest req, string path, out byte[] data)
    {
        data = null;
        try
        {
            if (_preloadingPath != path) return false;
            bool ok = false;
            try { ok = req.result == UnityWebRequest.Result.Success; } catch { ok = false; }
            if (!ok) { ClearPreloadIfCurrent(path); return false; }
            try { data = req.downloadHandler != null ? req.downloadHandler.data : null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (data == null || data.Length == 0) { ClearPreloadIfCurrent(path); return false; }
            return true;
        }
        catch { return false; }
    }

    private static string GetExtension(string path)
    {
        try { return Path.GetExtension(path).ToLowerInvariant(); }
        catch { return ""; }
    }

    private static void ClearPreloadIfCurrent(string path)
    {
        try { if (_preloadingPath == path) _preloadingPath = null; } catch { }
    }

    private static IEnumerable<object> PreloadMp3Steps(string path, string title, byte[] data)
    {
        var result = TryDecodeMp3Staged(path, data, out float[] samples, out int ch, out int rate);
        if (!result)
        {
            ClearPreloadIfCurrent(path);
            yield break;
        }
        if (_preloadingPath != path || samples == null || samples.Length == 0 || ch <= 0 || rate <= 0)
        {
            ClearPreloadIfCurrent(path);
            yield break;
        }
        FinishPreload(path, title, samples, ch, rate);
        yield break;
    }

    private static bool TryDecodeMp3Staged(string path, byte[] data, out float[] samples, out int ch, out int rate)
    {
        samples = null;
        ch = 0;
        rate = 0;
        NLayer.MpegFile mpeg = null;
        MemoryStream ms = null;
        try
        {
            if (!TryOpenMpeg(data, out mpeg, out ms, out ch, out rate))
                return false;
            var all = DrainMpeg(path, mpeg);
            if (all == null || all.Count == 0) return false;
            samples = all.ToArray();
            return true;
        }
        catch { return false; }
        finally
        {
            try { if (mpeg != null) mpeg.Dispose(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { if (ms != null) ms.Dispose(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static bool TryOpenMpeg(byte[] data, out NLayer.MpegFile mpeg, out MemoryStream ms, out int ch, out int rate)
    {
        mpeg = null;
        ms = null;
        ch = 0;
        rate = 0;
        try
        {
            ms = new MemoryStream(data, false);
            mpeg = new NLayer.MpegFile(ms);
            ch = mpeg.Channels;
            rate = mpeg.SampleRate;
            if (ch <= 0 || rate <= 0) return false;
            return true;
        }
        catch { return false; }
    }

    private static List<float> DrainMpeg(string path, NLayer.MpegFile mpeg)
    {
        var all = new List<float>();
        try
        {
            int slice = Math.Max(1024, (mpeg.SampleRate * mpeg.Channels) / 10);
            var buf = new float[slice];
            while (true)
            {
                if (_preloadingPath != path) break;
                int read = 0;
                try { read = mpeg.ReadSamples(buf, 0, buf.Length); } catch { break; }
                if (read <= 0) break;
                for (int i = 0; i < read; i++) all.Add(buf[i]);
                if (read < buf.Length) break;
            }
        }
        catch { }
        return all;
    }

    private static void FinishDecodedPreload(string path, string title, byte[] data)
    {
        try
        {
            if (!TryDecodePreload(path, data, out float[] samples, out int channels, out int frequency))
                return;
            if (_preloadingPath != path) return;
            FinishPreload(path, title, samples, channels, frequency);
        }
        catch { }
    }

    private static bool TryDecodePreload(string path, byte[] data, out float[] samples, out int channels, out int frequency)
    {
        samples = null;
        channels = 0;
        frequency = 0;
        try
        {
            if (!AudioDecoder.TryDecode(path, data, out samples, out channels, out frequency))
            {
                ClearPreloadIfCurrent(path);
                return false;
            }
            return true;
        }
        catch
        {
            ClearPreloadIfCurrent(path);
            return false;
        }
    }

    private static void FinishPreload(string filePath, string title, float[] samples, int channels, int frequency)
    {
        string path = filePath;
        try
        {
            if (!IsValidPreload(samples, channels, frequency)) return;
            var clip = AudioClip.Create(title, samples.Length / channels, channels, frequency, false);
            if (!TryStorePreload(path, title, clip, samples)) return;
            GregAudioClipCache.Put(path, clip);
            MelonLogger.Msg("[gregCore][Audio] Preloaded: '" + title + "'.");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        finally
        {
            if (_preloadingPath == path) _preloadingPath = null;
        }
    }

    private static bool IsValidPreload(float[] samples, int channels, int frequency)
    {
        try
        {
            return samples != null && samples.Length > 0 && channels > 0 && frequency > 0;
        }
        catch { return false; }
    }

    private static bool TryStorePreload(string path, string title, AudioClip clip, float[] samples)
    {
        try
        {
            if (clip == null) return false;
            if (!clip.SetData(samples, 0))
            {
                try { UnityEngine.Object.Destroy(clip); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return false;
            }
            return true;
        }
        catch { return false; }
    }
}
