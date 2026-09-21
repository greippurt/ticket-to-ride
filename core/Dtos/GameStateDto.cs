using core.Enums;

namespace core.Dtos;

public record GameStateDto(
    string Id,
    int CurrentPlayerIndex,
    string CurrentPlayerId,
    bool IsGameOver,
    bool IsLastRound,
    IReadOnlyList<TrainColor> FaceUpTrainCards,
    int TrainDrawPileCount,
    int TicketDrawPileCount,
    IReadOnlyList<PlayerDto> Players,
    IReadOnlyList<ClaimedRouteDto> ClaimedRoutes
);
