namespace TicTacToe.Web.Services;

public sealed class PlayerConnectionTracker
{
    private readonly object _gate = new();
    private readonly Dictionary<string, PlayerConnection> _connections = new();
    private readonly Dictionary<Guid, int> _connectionCounts = new();

    public bool Connect(string connectionId, string roomCode, Guid playerId)
    {
        lock (_gate)
        {
            if (_connections.TryGetValue(connectionId, out PlayerConnection? existing))
            {
                if (existing.PlayerId == playerId && existing.RoomCode == roomCode)
                {
                    return false;
                }

                RemoveConnection(connectionId, existing);
            }

            _connections[connectionId] = new PlayerConnection(roomCode, playerId);
            bool wasConnected = _connectionCounts.TryGetValue(playerId, out int count) && count > 0;
            _connectionCounts[playerId] = count + 1;
            return !wasConnected;
        }
    }

    public PlayerConnection? Disconnect(string connectionId)
    {
        lock (_gate)
        {
            if (!_connections.TryGetValue(connectionId, out PlayerConnection? existing))
            {
                return null;
            }

            _connections.Remove(connectionId);
            int remaining = _connectionCounts[existing.PlayerId] - 1;
            if (remaining > 0)
            {
                _connectionCounts[existing.PlayerId] = remaining;
                return null;
            }

            _connectionCounts.Remove(existing.PlayerId);
            return existing;
        }
    }

    private void RemoveConnection(string connectionId, PlayerConnection connection)
    {
        _connections.Remove(connectionId);
        int remaining = _connectionCounts[connection.PlayerId] - 1;
        if (remaining > 0)
        {
            _connectionCounts[connection.PlayerId] = remaining;
        }
        else
        {
            _connectionCounts.Remove(connection.PlayerId);
        }
    }

    public sealed record PlayerConnection(string RoomCode, Guid PlayerId);
}
