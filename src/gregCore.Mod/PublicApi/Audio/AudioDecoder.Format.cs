namespace gregCore.PublicApi.Audio;

public static partial class AudioDecoder
{
    // Validates channel count and sample rate.
    private static bool HasValidChannelRate(int channels, int rate)
    {
        try
        {
            if (channels <= 0) return false;
            return rate > 0;
        }
        catch { return false; }
    }

    // Checks for PCM (1) or IEEE float (3) encoding.
    private static bool IsKnownAudioKind(int audio)
    {
        try
        {
            return IsPcmKind(audio) || IsFloatKind(audio);
        }
        catch { return false; }
    }

    // PCM integer encoding marker.
    private static bool IsPcmKind(int audio)
    {
        try { return audio == 1; }
        catch { return false; }
    }

    // IEEE float encoding marker.
    private static bool IsFloatKind(int audio)
    {
        try { return audio == 3; }
        catch { return false; }
    }

    // Validates bit depth for the given encoding.
    private static bool HasSupportedBitDepth(int audio, int bits)
    {
        try
        {
            if (IsFloatKind(audio)) return IsFloatDepth(bits);
            return IsPcmDepth(bits);
        }
        catch { return false; }
    }

    // Float WAV must be 32-bit.
    private static bool IsFloatDepth(int bits)
    {
        try { return bits == 32; }
        catch { return false; }
    }

    // PCM supports 8, 16, 24 and 32-bit depths.
    private static bool IsPcmDepth(int bits)
    {
        try
        {
            if (bits == 8) return true;
            if (bits == 16) return true;
            return IsWidePcmDepth(bits);
        }
        catch { return false; }
    }

    // Checks the wider PCM depths (24 and 32-bit).
    private static bool IsWidePcmDepth(int bits)
    {
        try
        {
            if (bits == 24) return true;
            return bits == 32;
        }
        catch { return false; }
    }
}
