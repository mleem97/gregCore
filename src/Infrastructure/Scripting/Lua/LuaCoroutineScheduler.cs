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

        // Update timers
        for (int i = _timers.Count - 1; i >= 0; i--)
        {
            var t = _timers[i];
            t.Remaining -= deltaTime;

            if (t.Remaining <= 0f)
            {
                try
                {
                    t.Callback.Call();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[LuaCoroutineScheduler] Timer callback error: {ex.Message}");
                }

                if (t.Repeating)
                {
                    t.Remaining = t.Interval;
                }
                else
                {
                    _timers.RemoveAt(i);
                }
            }
        }

        // Update coroutines
        for (int i = _coroutines.Count - 1; i >= 0; i--)
        {
            var co = _coroutines[i];

            if (co.Coroutine.State != CoroutineState.Dead)
            {
                try
                {
                    // Resume if waiting for time
                    if (co.WaitRemaining.HasValue)
                    {
                        co.WaitRemaining -= deltaTime;
                        if (co.WaitRemaining.Value > 0f) continue;
                        co.WaitRemaining = null;
                    }

                    var result = co.Coroutine.Resume();
                    
                    // Check if yielded with WAIT + time
                    if (co.Coroutine.State != CoroutineState.Dead && result.Type == DataType.Tuple)
                    {
                        var tuple = result.Tuple;
                        if (tuple.Length >= 2 
                            && tuple[0].String == "__WAIT__"
                            && tuple[1].Type == DataType.Number)
                        {
                            co.WaitRemaining = (float)tuple[1].Number;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[LuaCoroutineScheduler] Coroutine error: {ex.Message}");
                    _coroutines.RemoveAt(i);
                }
            }
            else
            {
                _coroutines.RemoveAt(i);
            }
        }
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
