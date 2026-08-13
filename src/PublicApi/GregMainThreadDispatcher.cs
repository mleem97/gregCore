using System.Collections.Concurrent;

namespace gregCore.PublicApi;

public sealed class GregMainThreadDispatcher : IGregMainThreadDispatcher
{
    private readonly int _mainThreadId = Environment.CurrentManagedThreadId;
    private readonly ConcurrentQueue<Action> _queue = new();

    public bool IsMainThread => Environment.CurrentManagedThreadId == _mainThreadId;
    public void Enqueue(Action action) => _queue.Enqueue(action ?? throw new ArgumentNullException(nameof(action)));

    public int Drain(int maxItems = 256)
    {
        var count = 0;
        while (count < maxItems && _queue.TryDequeue(out var action))
        {
            try { action(); }
            catch { /* callers receive isolation; runtime logger reports at the integration boundary */ }
            count++;
        }
        return count;
    }
}
