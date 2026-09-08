using core.Enums;
using core.Models;

namespace TicketToRide.Core.Tests.Helpers;

/// <summary>
/// Small, isolated board used only by tests: A-B-C-D is a chain, D-E extends it,
/// and X-Y is a disconnected route used for "not connected" scenarios.
/// </summary>
public static class TestBoard
{
    public static readonly Route AB = new("AB", "A", "B", 1, TrainColor.Red);
    public static readonly Route BC = new("BC", "B", "C", 2, TrainColor.Blue);
    public static readonly Route CD = new("CD", "C", "D", 3, TrainColor.Green);
    public static readonly Route DE = new("DE", "D", "E", 1, TrainColor.Yellow);
    public static readonly Route XY = new("XY", "X", "Y", 2, TrainColor.Black);

    public static readonly List<Route> AllRoutes = new() { AB, BC, CD, DE, XY };

    public static readonly DestinationTicket TicketAB = new("ticket-AB", "A", "B", 2);
    public static readonly DestinationTicket TicketAD = new("ticket-AD", "A", "D", 10);
    public static readonly DestinationTicket TicketXY = new("ticket-XY", "X", "Y", 5);
}
