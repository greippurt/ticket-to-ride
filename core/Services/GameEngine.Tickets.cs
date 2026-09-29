using core.Game;
using core.Models;

namespace core.Services;

public static partial class GameEngine
{
    // DrawRouteCards / vælg destination tickets m.fl. kommer her
    public static List<DestinationTicket> DrawDestinationTickets(GameState state, Random rng)
    {
        var count = 3;

        if (state.TrainCardDrawsThisTurn > 0)
        {
            throw new InvalidOperationException("Cannot draw destination tickets after drawing train cards");
        }

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

        // Set now, not when choosing, so no other action can happen while the player chooses
        state.HasDrawnTicketsThisTurn = true;
        return drawnTickets;
    }

    public static void ChooseDestinationTicketsForCurrentPlayer(
        GameState state,
        List<DestinationTicket> drawnTickets,
        List<DestinationTicket> tickets
    )
    {
        if (tickets.Count < 1 || tickets.Count > 3)
        {
            throw new InvalidOperationException("Must choose between 1 and 3 destination tickets");
        }

        var player = state.CurrentPlayer;
        player.DestinationTickets.AddRange(tickets);

        // Tickets the player did not keep go back into the draw pile
        state.TicketDrawPile.AddRange(drawnTickets.Except(tickets));

        state.HasDrawnTicketsThisTurn = true;
    }

    public static void ChooseDestinationTicketsForPlayer(
        GameState state,
        Player player,
        List<DestinationTicket> tickets
    )
    {
        if (tickets.Count < 2 || tickets.Count > 3)
        {
            throw new InvalidOperationException("Must choose between 2 and 3 destination tickets");
        }

        player.DestinationTickets.AddRange(tickets);
    }
}
