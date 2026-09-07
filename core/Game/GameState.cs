using core.Enums;
using core.Models;

namespace core.Game;

public class GameState
{
    public required string Id { get; init; }
    public List<Player> Players { get; } = new();
    public int CurrentPlayerIndex { get; set; }
    public int TrainCardDrawsThisTurn { get; set; } = 0;
    public bool HasClaimedRouteThisTurn { get; set; } = false;
    public bool HasDrawnTicketsThisTurn { get; set; } = false;
    public bool IsRoundComplete =>
        TrainCardDrawsThisTurn >= 2 || HasClaimedRouteThisTurn || HasDrawnTicketsThisTurn;
    public Dictionary<string, ClaimedRoute> ClaimedRoutes { get; } = new();
    public Dictionary<TrainColor, int> TrainDrawPile { get; } =
        new()
        {
            { TrainColor.Red, 12 },
            { TrainColor.Blue, 12 },
            { TrainColor.Green, 12 },
            { TrainColor.Yellow, 12 },
            { TrainColor.Black, 12 },
            { TrainColor.White, 12 },
            { TrainColor.Orange, 12 },
            { TrainColor.Purple, 12 },
            { TrainColor.Locomotive, 14 },
        };
    public List<TrainColor> FaceUpTrainCards { get; } = new();
    public Dictionary<TrainColor, int> TrainDiscardPile { get; } =
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
    public List<DestinationTicket> TicketDrawPile { get; } = new();

    public Player CurrentPlayer => Players[CurrentPlayerIndex];

    public bool IsRouteClaimed(string routeId) => ClaimedRoutes.ContainsKey(routeId);

    public bool IsLastRound => Players.Any(p => p.TrainsRemaining <= 2);

    public int? LastRoundPlayerIndex { get; set; } = null;

    public bool IsGameOver { get; set; } = false;
}
