using System.Collections.Concurrent;
using core.Models;

namespace server.Hubs;

public class GameConnectionRegistry
{
    private readonly ConcurrentDictionary<string, (string GameId, string PlayerId)> _connections = new();
    private readonly ConcurrentDictionary<string, List<DestinationTicket>> _pendingTicketDraws = new();

    public void Add(string connectionId, string gameId, string playerId) =>
        _connections[connectionId] = (gameId, playerId);

    public void Remove(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
        _pendingTicketDraws.TryRemove(connectionId, out _);
    }

    public bool TryGetPlayer(string connectionId, out (string GameId, string PlayerId) player) =>
        _connections.TryGetValue(connectionId, out player);

    public IEnumerable<(string ConnectionId, string PlayerId)> GetConnectionsForGame(string gameId) =>
        _connections
            .Where(kv => kv.Value.GameId == gameId)
            .Select(kv => (kv.Key, kv.Value.PlayerId));

    public void SetPendingTicketDraw(string connectionId, List<DestinationTicket> tickets) =>
        _pendingTicketDraws[connectionId] = tickets;

    public bool TryTakePendingTicketDraw(string connectionId, out List<DestinationTicket>? tickets) =>
        _pendingTicketDraws.TryRemove(connectionId, out tickets);
}
