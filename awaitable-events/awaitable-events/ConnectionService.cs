using System.Collections.Concurrent;

namespace awaitable_events
{
    public enum ConnectionState
    {
        Connected,
        Error
    }

    public class Connection
    {
        public event Action Connected;
        public event Action<object, ConnectionState> OnConnectionStateChanged;

        public void Connect()
        {
            Connected?.Invoke();
        }

        public void ChangeConnectionState(ConnectionState newState)
        {
            OnConnectionStateChanged?.Invoke(this, newState);
        }
    }

    public class ConnectionService
    {
        private ConcurrentDictionary<string, Connection> _connections = new ConcurrentDictionary<string, Connection>(StringComparer.OrdinalIgnoreCase);
        private static readonly Random R = new ();

        public async Task ConnectAsync(string id, CancellationToken cancellationToken)
        {
            var connection = new Connection();
            ConfigureConnection(connection);
            _connections.TryAdd(id, connection);
            var task = AsyncEx.WaitEvent(handler => connection.Connected += handler, handler => connection.Connected -= handler, cancellationToken)
                .ConfigureAwait(false);
            RaiseAsync(id);
            await task;
        }

        public async Task ConnectAsyncV2(string id, CancellationToken cancellationToken)
        {
            var connection = new Connection();
            ConfigureConnection(connection);
            _connections.TryAdd(id, connection);

            var eventAwaiter = new EventAwaiter();
            connection.Connected += eventAwaiter.EventRaised;
            RaiseAsync(id);
            await eventAwaiter;
            connection.Connected -= eventAwaiter.EventRaised;
        }

        private async Task RaiseAsync(string id)
        {
            await Task.Delay(R.Next(2000, 5000));
            if (_connections.TryGetValue(id, out var connection))
            {
                connection.ChangeConnectionState(ConnectionState.Connected);
            }
        }

        private void ConfigureConnection(Connection connection)
        {
            connection.OnConnectionStateChanged += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object o, ConnectionState state)
        {
            var connection = (Connection)o;
            if (state == ConnectionState.Connected)
            {
                connection.Connect();
            }
        }
    }
}
