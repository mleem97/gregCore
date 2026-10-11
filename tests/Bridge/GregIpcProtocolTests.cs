/// <file-summary>
/// Schicht:      Tests
/// Zweck:        GregIpcBridge-Protokoll ohne Spiel/Netz: JSON-Helfer
///               (Quote/Extract*) plus Handle-Fehlpfade (unknown/missing method).
/// Maintainer:   Reine Funktionen, kein Dispatcher, kein Socket.
/// </file-summary>

using Xunit;
using gregCore.Bridge.Ipc;

namespace gregCore.Tests.Bridge;

public class GregIpcProtocolTests
{
    [Fact]
    public void Quote_Escapes()
    {
        Assert.Equal("\"a\\\"b\\\\c\\n\"", GregIpcBridge.Quote("a\"b\\c\n"));
        Assert.Equal("\"plain\"", GregIpcBridge.Quote("plain"));
    }

    [Fact]
    public void ExtractString_BasicAndEscaped()
    {
        Assert.Equal("ping", GregIpcBridge.ExtractString("{\"method\":\"ping\"}", "method"));
        Assert.Equal("a\"b", GregIpcBridge.ExtractString("{\"t\":\"a\\\"b\"}", "t"));
        Assert.Null(GregIpcBridge.ExtractString("{\"a\":1}", "missing"));
    }

    [Fact]
    public void ExtractRaw_NumbersAndNull()
    {
        Assert.Equal("42", GregIpcBridge.ExtractRaw("{\"id\":42}", "id"));
        Assert.Equal("null", GregIpcBridge.ExtractRaw("{\"id\":null}", "id"));
        Assert.Null(GregIpcBridge.ExtractRaw("{}", "id"));
    }

    [Fact]
    public void ExtractInt_Fallback()
    {
        Assert.Equal(7, GregIpcBridge.ExtractInt("{\"n\":7}", "n", 50));
        Assert.Equal(50, GregIpcBridge.ExtractInt("{}", "n", 50));
    }

    [Fact]
    public void Handle_MissingMethod()
    {
        string res = GregIpcBridge.Handle("{\"id\":3}");
        Assert.Contains("\"ok\":false", res);
        Assert.Contains("missing method", res);
    }

    [Fact]
    public void Handle_UnknownMethod()
    {
        string res = GregIpcBridge.Handle("{\"id\":9,\"method\":\"nope\"}");
        Assert.Contains("\"ok\":false", res);
        Assert.Contains("unknown method", res);
        Assert.Contains("\"id\":9", res);
    }
}
