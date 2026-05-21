using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;
using Python.Runtime;
using gregCore.Bridge.PythonFFI;

namespace gregCore.Tests.Bridge.PythonFFI;

public class PythonFFIBridgeTests : IDisposable
{
    private readonly PythonFFIBridge _bridge;

    public PythonFFIBridgeTests()
    {
        // Setup Python for tests
        // Rely on pythonnet's auto-discovery or environment variables instead of hardcoding a path.
        // We only set it if explicitly provided via env to prevent DllNotFoundException on standard CI.
        var pythonDllPath = Environment.GetEnvironmentVariable("PYTHON_DLL_PATH");
        if (!string.IsNullOrEmpty(pythonDllPath) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Runtime.PythonDLL = pythonDllPath;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
             // Fallback for this specific test environment where auto-discovery might fail
             // In a real project we'd use dynamic resolution, but for this specific container we'll try to find it.
             Runtime.PythonDLL = "/usr/lib/python3.12/config-3.12-x86_64-linux-gnu/libpython3.12.so";
        }

        if (!PythonEngine.IsInitialized)
        {
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
        }

        _bridge = new PythonFFIBridge();
    }

    public void Dispose()
    {
        // PythonEngine shutdown is generally tricky in test runners
    }

    [Fact]
    public void Methods_ShouldBeCallable_FromPython()
    {
        using (Py.GIL())
        {
            using var scope = Py.CreateScope();

            // Expose the bridge instance to Python
            scope.Set("bridge", _bridge.ToPython());

            // Test logging
            var action1 = () => scope.Exec("bridge.log_info('Test info')");
            action1.Should().NotThrow();

            // Test player stats
            var action2 = () => scope.Exec("bridge.set_player_money(100.0)");
            var action3 = () => scope.Exec("bridge.set_player_xp(100.0)");
            action2.Should().NotThrow();
            action3.Should().NotThrow();

            var money = scope.Eval<double>("bridge.get_player_money()");
            money.Should().Be(0.0);

            // Test server stats
            var serverCount = scope.Eval<uint>("bridge.get_server_count()");
            serverCount.Should().Be(0u);

            // Test game state
            var timeOfDay = scope.Eval<float>("bridge.get_time_of_day()");
            timeOfDay.Should().Be(0f);
        }
    }

    [Fact]
    public void GetPlayerPosition_ShouldReturnPythonDictWithZeros_FromPython()
    {
        using (Py.GIL())
        {
            using var scope = Py.CreateScope();
            scope.Set("bridge", _bridge.ToPython());

            scope.Exec("pos = bridge.get_player_position()");

            var hasX = scope.Eval<bool>("'x' in pos");
            hasX.Should().BeTrue();

            var x = scope.Eval<float>("pos['x']");
            x.Should().Be(0f);
        }
    }

    [Fact]
    public void SubscribeEvent_ShouldNotThrow_FromPython()
    {
        using (Py.GIL())
        {
            using var scope = Py.CreateScope();
            scope.Set("bridge", _bridge.ToPython());

            scope.Exec("def my_cb(): pass");
            scope.Exec("cb = my_cb");

            var action = () => scope.Exec("bridge.subscribe_event('system_GameLoaded', cb)");
            action.Should().NotThrow();
        }
    }

    [Fact]
    public void OnHook_ShouldNotThrow_FromPython()
    {
        using (Py.GIL())
        {
            using var scope = Py.CreateScope();
            scope.Set("bridge", _bridge.ToPython());

            scope.Exec("def my_hook_cb(payload): pass");
            scope.Exec("cb = my_hook_cb");

            var action = () => scope.Exec("bridge.on('Hardware.Server.Break', cb)");
            action.Should().NotThrow();
        }
    }
}
