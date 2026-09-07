using core.Enums;
using core.Game;
using core.Models;

namespace core.Services;

public static partial class GameEngine
{
    // PlaceTrains / ClaimRoute m.fl. kommer her
    public static void ClaimRoute(GameState state, Route route, TrainColor color)
    {
        var player = state.CurrentPlayer;

        if (player.TrainsRemaining < route.Length)
        {
            throw new InvalidOperationException("Not enough trains remaining to claim the route");
        }

        if (state.IsRouteClaimed(route.Id))
        {
            throw new InvalidOperationException("Route is already claimed");
        }

        // Deduct trains from the player
        player.TrainsRemaining -= route.Length;

        // Add the claimed route to the game state
        state.ClaimedRoutes[route.Id] = new ClaimedRoute
        {
            RouteId = route.Id,
            PlayerId = player.Id,
        };

        state.HasClaimedRouteThisTurn = true;
    }
}
