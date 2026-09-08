using core.Enums;
using core.Game;
using core.Models;

namespace TicketToRide.Core.Tests.Helpers;

public static class TestFactory
{
    public static Player CreatePlayer(string id, int trainsRemaining = 45) =>
        new()
        {
            Id = id,
            Name = id,
            TrainsRemaining = trainsRemaining,
        };

    public static GameState CreateState(params Player[] players)
    {
        var state = new GameState { Id = "test-game" };
        state.Players.AddRange(players);
        return state;
    }

    public static void GiveClaimedRoute(GameState state, Route route, Player player)
    {
        state.ClaimedRoutes[route.Id] = new ClaimedRoute
        {
            RouteId = route.Id,
            PlayerId = player.Id,
        };
    }

    public static void GiveTrainCards(Player player, TrainColor color, int count)
    {
        player.TrainCards[color] = count;
    }
}
