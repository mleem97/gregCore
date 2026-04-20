using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

namespace greg.Diagnostic;

public sealed class GregPerformanceHud : MelonMod
{
    private bool _visible = false;
    private string _displayText = "";
    private float _updateTimer = 0f;

    public override void OnUpdate()
    {
        try
        {
            if (Keyboard.current?.f9Key?.wasPressedThisFrame == true)
                _visible = !_visible;
        }
        catch { }

        if (!_visible) return;

        _updateTimer += Time.unscaledDeltaTime;
        if (_updateTimer < 1f) return;
        _updateTimer = 0f;

        float currentFps = Time.unscaledDeltaTime > 0 ? 1f / Time.unscaledDeltaTime : 0f;
        float targetFps = Application.targetFrameRate;
        string state = GregFrameLimiterService.Instance?.CurrentStateName ?? "?";
        long ramMb = System.GC.GetTotalMemory(false) / 1024 / 1024;
        int gpuMb = SystemInfo.graphicsMemorySize;

        _displayText =
            $"gregCore Performance\n" + 
            $"FPS: {targetFps}/{currentFps:F0}\n" +
            $"State: {state}\n" +
            $"RAM: {ramMb}MB\n" +
            $"GPU: {gpuMb}MB\n" +
            $"[F9] hide";
    }

    public override void OnGUI()
    {
        if (!_visible) return;
        GUI.Box(new Rect(10, 10, 200, 160), _displayText);
    }
}