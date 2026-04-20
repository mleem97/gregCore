using gregCore.PublicApi.Modules;

namespace gregCore.PublicApi;

public static class greg
{
    internal static GregApiContext? _context;
    private static GregApiContext Context => _context ?? throw new InvalidOperationException("gregCore nicht initialisiert.");

    internal static gregCore.Infrastructure.Performance.GregPerformanceGovernor? _governor;

    private static GregEconomyModule? _economy;
    private static GregNetworkModule? _network;
    private static GregPlayerModule? _player;
    private static GregTimeModule? _time;
    private static GregServerModule? _server;
    private static GregFacilityModule? _facility;
    private static GregUIModule? _ui;
    private static GregSaveModule? _save;
    private static GregAutomationModule? _automation;
    private static GregNpcModule? _npc;
    private static GregPerformanceModule? _performance;

    public static GregEconomyModule Economy => _economy ??= new GregEconomyModule(Context);
    public static GregNetworkModule Network => _network ??= new GregNetworkModule(Context);
    public static GregPlayerModule Player => _player ??= new GregPlayerModule(Context);
    public static GregTimeModule Time => _time ??= new GregTimeModule(Context);
    public static GregServerModule Server => _server ??= new GregServerModule(Context);
    public static GregFacilityModule Facility => _facility ??= new GregFacilityModule(Context);
    public static GregUIModule UI => _ui ??= new GregUIModule(Context);
    public static GregSaveModule Save => _save ??= new GregSaveModule(Context);
    public static GregAutomationModule Automation => _automation ??= new GregAutomationModule(Context);
    public static GregNpcModule Npc => _npc ??= new GregNpcModule(Context);
    public static GregPerformanceModule Performance => _performance ??= new GregPerformanceModule(Context, _governor ?? throw new InvalidOperationException("Governor not initialized."));

    public static string Version => typeof(greg).Assembly.GetName().Version?.ToString() ?? "1.0.0";
    public static bool IsInitialized => _context != null;
}