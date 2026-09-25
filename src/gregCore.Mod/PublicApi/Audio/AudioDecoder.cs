using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;

namespace gregCore.PublicApi.Audio;

// Managed audio decoding: UnityWebRequest audio is stripped from the IL2CPP build,
// so we decode to float PCM (interleaved) ourselves.
// Formats: WAV (PCM 8/16/24/32 + IEEE float, manual), MP3 (NLayer, managed).
// OGG currently not supported (clear warning instead of a crash).
public static partial class AudioDecoder
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
            if (!HasRiffHeader(data)) return false;
            if (!TryParseChunks(data, out int fmtChannels, out int fmtRate, out int fmtBits, out int fmtAudio, out int dataOff, out int dataLen)) return false;
            if (!IsSupportedFormat(fmtChannels, fmtRate, fmtAudio, fmtBits)) return false;
            var fmt = new PcmFormat(fmtChannels, fmtRate, fmtBits, fmtAudio);
            return TryConvertPcm(data, dataOff, dataLen, fmt, out samples, out channels, out frequency);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[MusicPlayer] WAV-Decode failed: " + ex.GetBaseException().Message);
            return false;
        }
    }

    private static bool HasRiffHeader(byte[] data)
    {
        try
        {
            if (data.Length < 44) return false;
            return ReadAscii(data, 0, 4) == "RIFF" && ReadAscii(data, 8, 4) == "WAVE";
        }
        catch { return false; }
    }

    private static bool TryParseChunks(byte[] data, out int fmtChannels, out int fmtRate, out int fmtBits, out int fmtAudio, out int dataOff, out int dataLen)
    {
        fmtChannels = 0; fmtRate = 0; fmtBits = 0; fmtAudio = 0; dataOff = -1; dataLen = 0;
        try
        {
            int pos = 12;
            while (pos + 8 <= data.Length)
            {
                string id = ReadAscii(data, pos, 4);
                int size = ReadInt32LE(data, pos + 4);
                if (size < 0 || pos + 8 + size > data.Length) break;
                if (id == "fmt " && size >= 16) ReadFmt(data, pos, out fmtAudio, out fmtChannels, out fmtRate, out fmtBits);
                else if (id == "data") { dataOff = pos + 8; dataLen = size; }
                pos += 8 + size + (size & 1);
            }
            return true;
        }
        catch { return false; }
    }

    private static void ReadFmt(byte[] data, int pos, out int audio, out int channels, out int rate, out int bits)
    {
        audio = 0; channels = 0; rate = 0; bits = 0;
        try
        {
            audio = ReadInt16LE(data, pos + 8);
            channels = ReadInt16LE(data, pos + 10);
            rate = ReadInt32LE(data, pos + 12);
            bits = ReadInt16LE(data, pos + 22);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static bool IsSupportedFormat(int channels, int rate, int audio, int bits)
    {
        try
        {
            if (!HasValidChannelRate(channels, rate)) return false;
            if (!IsKnownAudioKind(audio)) return false;
            return HasSupportedBitDepth(audio, bits);
        }
        catch { return false; }
    }

    private readonly struct PcmFormat
    {
        public readonly int Channels;
        public readonly int Rate;
        public readonly int Bits;
        public readonly int Audio;

        public PcmFormat(int channels, int rate, int bits, int audio)
        {
            Channels = channels;
            Rate = rate;
            Bits = bits;
            Audio = audio;
        }
    }

    private static bool TryConvertPcm(byte[] data, int dataOff, int dataLen, PcmFormat fmt, out float[] samples, out int channels, out int frequency)
    {
        samples = null; channels = 0; frequency = 0;
        try
        {
            if (dataOff < 0) return false;
            int bytesPerSample = fmt.Bits / 8;
            int frames = dataLen / (bytesPerSample * fmt.Channels);
            if (frames <= 0) return false;
            var out_ = new float[frames * fmt.Channels];
            for (int f = 0; f < frames; f++)
            {
                for (int c = 0; c < fmt.Channels; c++)
                {
                    int o = dataOff + (f * fmt.Channels + c) * bytesPerSample;
                    float v = DecodeSample(data, o, fmt.Bits, fmt.Audio);
                    out_[f * fmt.Channels + c] = ClampSample(v);
                }
            }
            samples = out_;
            channels = fmt.Channels;
            frequency = fmt.Rate;
            return true;
        }
        catch { return false; }
    }

    private static float DecodeSample(byte[] data, int o, int bits, int audio)
    {
        try
        {
            if (audio == 3) return BitConverter.ToSingle(data, o);
            if (bits == 8) return (data[o] - 128) / 128f;
            if (bits == 16) return (short)(data[o] | (data[o + 1] << 8)) / 32768f;
            if (bits == 24) return Decode24(data, o);
            if (bits == 32) return (int)((uint)data[o] | ((uint)data[o + 1] << 8) | ((uint)data[o + 2] << 16) | ((uint)data[o + 3] << 24)) / 2147483648f;
            return 0f;
        }
        catch { return 0f; }
    }

    private static float Decode24(byte[] data, int o)
    {
        try
        {
            int s = data[o] | (data[o + 1] << 8) | (data[o + 2] << 16);
            if ((s & 0x800000) != 0) s |= unchecked((int)0xFF000000);
            return s / 8388608f;
        }
        catch { return 0f; }
    }

    private static float ClampSample(float v)
    {
        try
        {
            if (v > 1f) return 1f;
            if (v < -1f) return -1f;
            return v;
        }
        catch { return 0f; }
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
                if (!TryReadMp3Header(mpeg, out int ch, out int rate)) return false;
                if (!TryReadMp3Samples(mpeg, ch, rate, out List<float> all)) return false;
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

    private static bool TryReadMp3Header(NLayer.MpegFile mpeg, out int ch, out int rate)
    {
        ch = 0; rate = 0;
        try
        {
            ch = mpeg.Channels;
            rate = mpeg.SampleRate;
            return ch > 0 && rate > 0;
        }
        catch { return false; }
    }

    private static bool TryReadMp3Samples(NLayer.MpegFile mpeg, int ch, int rate, out List<float> all)
    {
        all = new List<float>(rate * ch * 8);
        try
        {
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
            return all.Count > 0;
        }
        catch { return false; }
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
