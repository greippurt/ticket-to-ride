using core.Enum;

namespace core.Game;

public class ClaimedRoute
{
    public required string RouteId { get; set; }
    public required string PlayerId { get; set; }
    public required TrainColor ColorUsed { get; set; }
}