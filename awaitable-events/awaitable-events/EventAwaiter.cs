using System.Runtime.CompilerServices;

namespace awaitable_events;

public class EventAwaiter : INotifyCompletion
{
    public Action _continuation;
    private volatile bool _isRaised = false;
    public EventAwaiter GetAwaiter() => this;
    public bool IsCompleted { get; }

    public void GetResult()
    {
    }

    public void OnCompleted(Action continuation)
    {
        Volatile.Write(ref _continuation, continuation);
    }

    public void EventRaised()
    {
        Volatile.Write(ref _isRaised, true);
        if (_continuation != null)
        {
            var continuation = Interlocked.Exchange(ref _continuation, null);
            continuation();
        }
    }
}
