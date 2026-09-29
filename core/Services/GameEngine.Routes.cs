using core.Enums;
using core.Game;
using core.Models;

namespace core.Services;

public static partial class GameEngine
{
    // PlaceTrains / ClaimRoute m.fl. kommer her
    public static void ClaimRoute(GameState state, Route route, TrainColor color, int locomotives = 0)
    {
        var player = state.CurrentPlayer;

        if (state.TrainCardDrawsThisTurn > 0 || state.HasDrawnTicketsThisTurn)
        {
            throw new InvalidOperationException("You have already taken an action this turn");
        }

        if (locomotives < 0 || locomotives > route.Length)
        {
            throw new InvalidOperationException("Invalid number of locomotives");
        }

        if (color == TrainColor.Locomotive && locomotives > 0)
        {
            throw new InvalidOperationException("Choose a color to pay with alongside locomotives");
        }

        int colorCards = route.Length - locomotives;

        if (player.TrainsRemaining < route.Length)
        {
            throw new InvalidOperationException("Not enough trains remaining to claim the route");
        }

        if (state.IsRouteClaimed(route.Id))
        {
            throw new InvalidOperationException("Route is already claimed");
        }

        if (route.Color != color && route.Color != TrainColor.Grey)
        {
            throw new InvalidOperationException("Route color does not match the chosen color");
        }

        if (color == TrainColor.Grey || !player.TrainCards.ContainsKey(color))
        {
            throw new InvalidOperationException("Invalid train card color chosen");
        }

        if (player.TrainCards[color] < colorCards)
        {
            throw new InvalidOperationException(
                "Not enough train cards of the chosen color to claim the route"
            );
        }

        if (player.TrainCards[TrainColor.Locomotive] < locomotives)
        {
            throw new InvalidOperationException("Not enough locomotives to claim the route");
        }

        player.TrainCards[color] -= colorCards;
        player.TrainCards[TrainColor.Locomotive] -= locomotives;

        player.TrainsRemaining -= route.Length;

        state.TrainDiscardPile[color] += colorCards;
        state.TrainDiscardPile[TrainColor.Locomotive] += locomotives;

        // Add the claimed route to the game state
        state.ClaimedRoutes[route.Id] = new ClaimedRoute
        {
            RouteId = route.Id,
            PlayerId = player.Id,
        };

        var points = GetRoutePoints(route);
        player.Score += points;

        state.HasClaimedRouteThisTurn = true;
    }
}
