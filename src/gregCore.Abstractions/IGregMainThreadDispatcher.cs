namespace gregCore.PublicApi;

public interface IGregMainThreadDispatcher
{
    bool IsMainThread { get; }
    void Enqueue(Action action);
    int Drain(int maxItems = 256);
}
