using core.Data;
using core.Game;

namespace core.Services;

public class GameManager
{
    private readonly Dictionary<string, GameState> _games = new();

    public GameState CreateGame(List<string> playerNames, Random? rng = null)
    {
        rng ??= Random.Shared;

        var state = new GameState { Id = Guid.NewGuid().ToString() };

        foreach (var name in playerNames)
        {
            state.Players.Add(new Player { Id = Guid.NewGuid().ToString(), Name = name });
        }

        state.TicketDrawPile.AddRange(PlaceholderBoard.DestinationTickets);

        GameEngine.DealStartingHands(state, rng);

        for (int i = 0; i < 5; i++)
        {
            GameEngine.AddFaceUpTrainCard(state, rng);
        }

        _games[state.Id] = state;
        return state;
    }

    public bool TryGetGame(string gameId, out GameState? state) =>
        _games.TryGetValue(gameId, out state);

    public GameState GetGame(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var state))
        {
            throw new KeyNotFoundException($"Game '{gameId}' was not found.");
        }

        return state;
    }

    public void RemoveGame(string gameId)
    {
        _games.Remove(gameId);
    }
}
