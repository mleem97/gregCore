using System.Collections;
using MelonLoader;
using UnityEngine;

namespace greg.SaveEngine
{
    public class GregSaveScheduler
    {
        public static float AutoSaveIntervalSeconds { get; set; } = 60f;
        private static bool _isRunning;

        public static void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            MelonCoroutines.Start(AutoSaveCoroutine());
        }

        public static void Stop()
        {
            _isRunning = false;
        }

        private static IEnumerator AutoSaveCoroutine()
        {
            while (_isRunning)
            {
                yield return new WaitForSeconds(AutoSaveIntervalSeconds);
                if (frameworkSdk.GregFeatureGuard.IsEnabled("SaveEngine.Write") && GregSaveEngine.Instance != null)
                {
                    // Run save in background task
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            GregSaveEngine.Instance.SaveAll();
                        }
                        catch (System.Exception ex)
                        {
                            gregCore.API.GregAPI.LogError($"Auto-save failed: {ex}");
                        }
                    });
                }
            }
        }
    }
}
