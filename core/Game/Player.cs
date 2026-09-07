using core.Enums;
using core.Models;

namespace core.Game;

public class Player
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public int TrainsRemaining { get; set; } = 45;
    public int Score { get; set; } = 0;
    public List<DestinationTicket> DestinationTickets { get; } = new();
    public Dictionary<TrainColor, int> TrainCards { get; set; } =
        new()
        {
            { TrainColor.Red, 0 },
            { TrainColor.Blue, 0 },
            { TrainColor.Green, 0 },
            { TrainColor.Yellow, 0 },
            { TrainColor.Black, 0 },
            { TrainColor.White, 0 },
            { TrainColor.Orange, 0 },
            { TrainColor.Purple, 0 },
            { TrainColor.Locomotive, 0 },
        };
}
