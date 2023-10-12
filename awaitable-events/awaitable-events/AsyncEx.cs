namespace awaitable_events
{
    public static class AsyncEx
    {
        public static async Task WaitEvent(Action<Action> subscriber,
                                           Action<Action> unsubscriber,
                                           CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource();
            void SetResult() => tcs.SetResult();
            cancellationToken.Register(() => tcs.TrySetCanceled());

            subscriber(SetResult);
            try
            {
                await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                unsubscriber(SetResult);
            }
        }
    }
}
