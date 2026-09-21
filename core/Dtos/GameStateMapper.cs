using core.Enums;
using core.Game;
using core.Models;

namespace core.Dtos;

public static class GameStateMapper
{
    public static GameStateDto ToDto(GameState state, string viewerPlayerId)
    {
        if (state.Players.All(p => p.Id != viewerPlayerId))
        {
            throw new ArgumentException(
                $"Player '{viewerPlayerId}' is not part of game '{state.Id}'.",
                nameof(viewerPlayerId)
            );
        }

        return new GameStateDto(
            Id: state.Id,
            CurrentPlayerIndex: state.CurrentPlayerIndex,
            CurrentPlayerId: state.CurrentPlayer.Id,
            IsGameOver: state.IsGameOver,
            IsLastRound: state.IsLastRound,
            FaceUpTrainCards: state.FaceUpTrainCards.ToList(),
            TrainDrawPileCount: state.TrainDrawPile.Values.Sum(),
            TicketDrawPileCount: state.TicketDrawPile.Count,
            Players: state.Players.Select(p => ToPlayerDto(p, isViewer: p.Id == viewerPlayerId)).ToList(),
            ClaimedRoutes: state
                .ClaimedRoutes.Values.Select(cr => new ClaimedRouteDto(cr.RouteId, cr.PlayerId))
                .ToList()
        );
    }

    private static PlayerDto ToPlayerDto(Player player, bool isViewer)
    {
        return new PlayerDto(
            Id: player.Id,
            Name: player.Name,
            TrainsRemaining: player.TrainsRemaining,
            Score: player.Score,
            TrainCardCount: player.TrainCards.Values.Sum(),
            DestinationTicketCount: player.DestinationTickets.Count,
            TrainCards: isViewer ? new Dictionary<TrainColor, int>(player.TrainCards) : null,
            DestinationTickets: isViewer ? player.DestinationTickets.Select(ToTicketDto).ToList() : null
        );
    }

    private static DestinationTicketDto ToTicketDto(DestinationTicket ticket) =>
        new(ticket.Id, ticket.FromCityId, ticket.ToCityId, ticket.Points);
}
