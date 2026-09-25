using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua;

/// <summary>
/// Manages coroutines and timers (wait, every) for Lua mods.
/// Updated each frame via OnUpdate.
/// </summary>
public class LuaCoroutineScheduler
{
    private readonly Script _script;
    private readonly List<ScheduledTimer> _timers = new();
    private readonly List<LuaCoroutine> _coroutines = new();
    private float _currentFrameTime = 0f;

    public LuaCoroutineScheduler(Script script)
    {
        _script = script;
    }

    /// <summary>
    /// Register greg.wait, greg.every, greg.start_coroutine in the greg table.
    /// </summary>
    public void Register(Table greg)
    {
        greg["wait"] = (Action<double, Closure>)((seconds, callback) => {
            RegisterTimer(seconds, callback, false);
        });

        greg["every"] = (Action<double, Closure>)((seconds, callback) => {
            RegisterTimer(seconds, callback, true);
        });

        greg["start_coroutine"] = (Func<Closure, DynValue>)(coroutineFn => {
            return StartCoroutine(coroutineFn);
        });

        // Expose WAIT constant for yield-based coroutines
        greg["WAIT"] = DynValue.NewString("__WAIT__");
    }

    /// <summary>
    /// Call this every frame with deltaTime.
    /// </summary>
    public void OnUpdate(float deltaTime)
    {
        _currentFrameTime = deltaTime;
        UpdateTimers(deltaTime);
        UpdateCoroutines(deltaTime);
    }

    private void UpdateTimers(float deltaTime)
    {
        try
        {
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                try
                {
                    TickTimer(i, deltaTime);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[LuaCoroutineScheduler] Timer tick error: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaCoroutineScheduler] Timer update error: {ex.Message}");
        }
    }

    private void TickTimer(int index, float deltaTime)
    {
        try
        {
            var t = _timers[index];
            t.Remaining -= deltaTime;
            if (t.Remaining > 0f) return;
            FireTimer(t);
            if (t.Repeating)
            {
                t.Remaining = t.Interval;
            }
            else
            {
                _timers.RemoveAt(index);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaCoroutineScheduler] Timer tick error: {ex.Message}");
        }
    }

    private static void FireTimer(ScheduledTimer t)
    {
        try
        {
            t.Callback.Call();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaCoroutineScheduler] Timer callback error: {ex.Message}");
        }
    }

    private void UpdateCoroutines(float deltaTime)
    {
        try
        {
            for (int i = _coroutines.Count - 1; i >= 0; i--)
            {
                try
                {
                    TickCoroutine(i, deltaTime);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[LuaCoroutineScheduler] Coroutine tick error: {ex.Message}");
                    try { _coroutines.RemoveAt(i); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaCoroutineScheduler] Coroutine update error: {ex.Message}");
        }
    }

    private void TickCoroutine(int index, float deltaTime)
    {
        var co = _coroutines[index];
        if (co.Coroutine.State == CoroutineState.Dead)
        {
            try { _coroutines.RemoveAt(index); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            return;
        }
        try
        {
            if (TryConsumeWait(co, deltaTime)) return;
            var result = co.Coroutine.Resume();
            TryApplyWaitResult(co, result);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaCoroutineScheduler] Coroutine error: {ex.Message}");
            try { _coroutines.RemoveAt(index); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }
    }

    private static bool TryConsumeWait(LuaCoroutine co, float deltaTime)
    {
        try
        {
            if (!co.WaitRemaining.HasValue) return false;
            co.WaitRemaining -= deltaTime;
            if (co.WaitRemaining.Value > 0f) return true;
            co.WaitRemaining = null;
            return false;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private static void TryApplyWaitResult(LuaCoroutine co, DynValue result)
    {
        try
        {
            if (co.Coroutine.State == CoroutineState.Dead) return;
            if (result.Type != DataType.Tuple) return;
            var tuple = result.Tuple;
            if (tuple.Length < 2) return;
            if (tuple[0].String != "__WAIT__") return;
            if (tuple[1].Type != DataType.Number) return;
            co.WaitRemaining = (float)tuple[1].Number;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private void RegisterTimer(double seconds, Closure callback, bool repeating)
    {
        _timers.Add(new ScheduledTimer
        {
            Remaining = (float)seconds,
            Interval = (float)seconds,
            Callback = callback,
            Repeating = repeating
        });
    }

    private DynValue StartCoroutine(Closure coroutineFn)
    {
        var coDynVal = _script.CreateCoroutine(coroutineFn);
        var coroutine = coDynVal.Coroutine;
        var entry = new LuaCoroutine { Coroutine = coroutine };
        _coroutines.Add(entry);
        
        try
        {
            var result = coroutine.Resume();
            
            // Handle initial yield
            if (coroutine.State != CoroutineState.Dead && result.Type == DataType.Tuple)
            {
                var tuple = result.Tuple;
                if (tuple.Length >= 2 
                    && tuple[0].String == "__WAIT__"
                    && tuple[1].Type == DataType.Number)
                {
                    entry.WaitRemaining = (float)tuple[1].Number;
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaCoroutineScheduler] Coroutine start error: {ex.Message}");
        }

        return coDynVal;
    }

    public void Clear()
    {
        _timers.Clear();
        _coroutines.Clear();
    }

    private class ScheduledTimer
    {
        public float Remaining;
        public float Interval;
        public Closure Callback = null!;
        public bool Repeating;
    }

    private class LuaCoroutine
    {
        public MoonSharp.Interpreter.Coroutine Coroutine = null!;
        public float? WaitRemaining;
    }
}
