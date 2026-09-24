using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;

namespace gregCore.PublicApi.Audio;

// Managed audio decoding: UnityWebRequest audio is stripped from the IL2CPP build,
// so we decode to float PCM (interleaved) ourselves.
// Formats: WAV (PCM 8/16/24/32 + IEEE float, manual), MP3 (NLayer, managed).
// OGG currently not supported (clear warning instead of a crash).
public static class AudioDecoder
{
    public static bool TryDecode(string filePath, byte[] data, out float[] samples, out int channels, out int frequency)
    {
        samples = null;
        channels = 0;
        frequency = 0;
        if (data == null || data.Length < 16) return false;
        try
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext == ".wav" || ext == ".wave") return TryDecodeWav(data, out samples, out channels, out frequency);
            if (ext == ".mp3") return TryDecodeMp3(data, out samples, out channels, out frequency);
            MelonLogger.Warning("[MusicPlayer] Format nicht unterstuetzt (nur .wav/.mp3): " + ext);
            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[MusicPlayer] Decode-Ausnahme: " + ex.GetBaseException().Message);
            return false;
        }
    }

    // --- WAV (RIFF) ---
    private static bool TryDecodeWav(byte[] data, out float[] samples, out int channels, out int frequency)
    {
        samples = null;
        channels = 0;
        frequency = 0;
        try
        {
            if (data.Length < 44) return false;
            if (ReadAscii(data, 0, 4) != "RIFF" || ReadAscii(data, 8, 4) != "WAVE") return false;

            int fmtChannels = 0;
            int fmtRate = 0;
            int fmtBits = 0;
            int fmtAudio = 0;
            int dataOff = -1;
            int dataLen = 0;

            int pos = 12;
            while (pos + 8 <= data.Length)
            {
                string id = ReadAscii(data, pos, 4);
                int size = ReadInt32LE(data, pos + 4);
                if (size < 0 || pos + 8 + size > data.Length) break;
                if (id == "fmt " && size >= 16)
                {
                    fmtAudio = ReadInt16LE(data, pos + 8);
                    fmtChannels = ReadInt16LE(data, pos + 10);
                    fmtRate = ReadInt32LE(data, pos + 12);
                    fmtBits = ReadInt16LE(data, pos + 22);
                }
                else if (id == "data")
                {
                    dataOff = pos + 8;
                    dataLen = size;
                }
                pos += 8 + size + (size & 1);
            }

            if (dataOff < 0 || fmtChannels <= 0 || fmtRate <= 0) return false;
            if (fmtAudio != 1 && fmtAudio != 3) return false; // only PCM + IEEE float
            if (fmtAudio == 3 && fmtBits != 32) return false;

            int bytesPerSample = fmtBits / 8;
            int frames = dataLen / (bytesPerSample * fmtChannels);
            if (frames <= 0) return false;
            var out_ = new float[frames * fmtChannels];
            for (int f = 0; f < frames; f++)
            {
                for (int c = 0; c < fmtChannels; c++)
                {
                    int o = dataOff + (f * fmtChannels + c) * bytesPerSample;
                    float v = 0f;
                    if (fmtAudio == 3)
                    {
                        v = BitConverter.ToSingle(data, o);
                    }
                    else if (fmtBits == 8)
                    {
                        v = (data[o] - 128) / 128f;
                    }
                    else if (fmtBits == 16)
                    {
                        v = (short)(data[o] | (data[o + 1] << 8)) / 32768f;
                    }
                    else if (fmtBits == 24)
                    {
                        int s = data[o] | (data[o + 1] << 8) | (data[o + 2] << 16);
                        if ((s & 0x800000) != 0) s |= unchecked((int)0xFF000000);
                        v = s / 8388608f;
                    }
                    else if (fmtBits == 32)
                    {
                        v = (int)((uint)data[o] | ((uint)data[o + 1] << 8) | ((uint)data[o + 2] << 16) | ((uint)data[o + 3] << 24)) / 2147483648f;
                    }
                    else return false;
                    if (v > 1f) v = 1f;
                    else if (v < -1f) v = -1f;
                    out_[f * fmtChannels + c] = v;
                }
            }
            samples = out_;
            channels = fmtChannels;
            frequency = fmtRate;
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[MusicPlayer] WAV-Decode failed: " + ex.GetBaseException().Message);
            return false;
        }
    }

    // --- MP3 (NLayer, purely managed) ---
    private static bool TryDecodeMp3(byte[] data, out float[] samples, out int channels, out int frequency)
    {
        samples = null;
        channels = 0;
        frequency = 0;
        try
        {
            using (var ms = new MemoryStream(data, false))
            using (var mpeg = new NLayer.MpegFile(ms))
            {
                int ch = mpeg.Channels;
                int rate = mpeg.SampleRate;
                if (ch <= 0 || rate <= 0) return false;
                var all = new List<float>(rate * ch * 8);
                var buf = new float[rate * ch];
                while (true)
                {
                    int read = 0;
                    try { read = mpeg.ReadSamples(buf, 0, buf.Length); }
                    catch { break; }
                    if (read <= 0) break;
                    for (int i = 0; i < read; i++) all.Add(buf[i]);
                    if (read < buf.Length) break;
                }
                if (all.Count == 0) return false;
                samples = all.ToArray();
                channels = ch;
                frequency = rate;
                return true;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[MusicPlayer] MP3-Decode failed: " + ex.GetBaseException().Message);
            return false;
        }
    }

    private static string ReadAscii(byte[] d, int off, int len)
    {
        char[] c = new char[len];
        for (int i = 0; i < len; i++) c[i] = (char)d[off + i];
        return new string(c);
    }

    private static int ReadInt16LE(byte[] d, int off) => d[off] | (d[off + 1] << 8);

    private static int ReadInt32LE(byte[] d, int off)
        => d[off] | (d[off + 1] << 8) | (d[off + 2] << 16) | (d[off + 3] << 24);
}
