using core.Enums;

namespace core.Dtos;

public record PlayerDto(
    string Id,
    string Name,
    int TrainsRemaining,
    int Score,
    int TrainCardCount,
    int DestinationTicketCount,
    IReadOnlyDictionary<TrainColor, int>? TrainCards,
    IReadOnlyList<DestinationTicketDto>? DestinationTickets
);
