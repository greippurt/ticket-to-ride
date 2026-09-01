namespace core.Game;

public class GameState
{
    public required string Id { get; init; }
    public List<Player> Players { get; } = new();
    public int CurrentPlayerIndex { get; set; }
    public Dictionary<string, ClaimedRoute> ClaimedRoutes { get; } = new();

    public Player CurrentPlayer => Players[CurrentPlayerIndex];
    public bool IsRouteClaimed(string routeId) => ClaimedRoutes.ContainsKey(routeId);
}