using core.Game;
using core.Models;

namespace core.Services;

public static partial class GameEngine
{
    // DrawRouteCards / vælg destination tickets m.fl. kommer her
    public static List<DestinationTicket> DrawDestinationTickets(GameState state, Random rng)
    {
        var count = 3;

        if (state.TicketDrawPile.Count < count)
        {
            throw new InvalidOperationException("Not enough destination tickets in the draw pile");
        }

        var drawnTickets = new List<DestinationTicket>();
        for (int i = 0; i < count; i++)
        {
            int index = rng.Next(state.TicketDrawPile.Count);
            var ticket = state.TicketDrawPile[index];
            drawnTickets.Add(ticket);
            state.TicketDrawPile.RemoveAt(index);
        }
        return drawnTickets;
    }

    public static void ChooseDestinationTicketsForPlayer(
        GameState state,
        List<DestinationTicket> tickets
    )
    {
        var player = state.CurrentPlayer;
        player.DestinationTickets.AddRange(tickets);
        state.HasDrawnTicketsThisTurn = true;
    }
}
