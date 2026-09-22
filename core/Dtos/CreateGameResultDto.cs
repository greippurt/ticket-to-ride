namespace core.Dtos;

public record CreateGameResultDto(string GameId, IReadOnlyList<PlayerSummaryDto> Players);
