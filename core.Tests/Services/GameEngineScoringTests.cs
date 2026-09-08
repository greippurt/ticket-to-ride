using core.Services;
using TicketToRide.Core.Tests.Helpers;

namespace TicketToRide.Core.Tests.Services;

public class GameEngineScoringTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 4)]
    [InlineData(4, 7)]
    [InlineData(5, 10)]
    [InlineData(6, 15)]
    public void GetRoutePoints_ReturnsOfficialPointTable(int length, int expectedPoints)
    {
        var route = TestBoard.AB with { Length = length };

        var points = GameEngine.GetRoutePoints(route);

        Assert.Equal(expectedPoints, points);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    public void GetRoutePoints_ThrowsForInvalidLength(int length)
    {
        var route = TestBoard.AB with { Length = length };

        Assert.Throws<InvalidOperationException>(() => GameEngine.GetRoutePoints(route));
    }

    [Fact]
    public void ClaimRoute_AddsRoutePointsToPlayerScoreImmediately()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        TestFactory.GiveTrainCards(player, TestBoard.AB.Color, TestBoard.AB.Length);

        GameEngine.ClaimRoute(state, TestBoard.AB, TestBoard.AB.Color);

        Assert.Equal(1, player.Score); // AB has length 1 => 1 point
    }

    [Fact]
    public void CalculateTicketScore_AddsPoints_WhenConnectedViaDirectRoute()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        player.DestinationTickets.Add(TestBoard.TicketAB);
        TestFactory.GiveClaimedRoute(state, TestBoard.AB, player);

        GameEngine.CalculateTicketScore(player, state, TestBoard.AllRoutes);

        Assert.Equal(TestBoard.TicketAB.Points, player.Score);
    }

    [Fact]
    public void CalculateTicketScore_AddsPoints_WhenConnectedViaMultiHopPath()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        player.DestinationTickets.Add(TestBoard.TicketAD);
        TestFactory.GiveClaimedRoute(state, TestBoard.AB, player);
        TestFactory.GiveClaimedRoute(state, TestBoard.BC, player);
        TestFactory.GiveClaimedRoute(state, TestBoard.CD, player);

        GameEngine.CalculateTicketScore(player, state, TestBoard.AllRoutes);

        Assert.Equal(TestBoard.TicketAD.Points, player.Score);
    }

    [Fact]
    public void CalculateTicketScore_SubtractsPoints_WhenNotConnected()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        player.DestinationTickets.Add(TestBoard.TicketAD);
        // Only the first leg is claimed, so A cannot reach D.
        TestFactory.GiveClaimedRoute(state, TestBoard.AB, player);

        GameEngine.CalculateTicketScore(player, state, TestBoard.AllRoutes);

        Assert.Equal(-TestBoard.TicketAD.Points, player.Score);
    }

    [Fact]
    public void CalculateTicketScore_IgnoresRoutesClaimedByOtherPlayers()
    {
        var player = TestFactory.CreatePlayer("p1");
        var opponent = TestFactory.CreatePlayer("p2");
        var state = TestFactory.CreateState(player, opponent);
        player.DestinationTickets.Add(TestBoard.TicketAB);
        // The route exists on the board, but the opponent owns it, not the player.
        TestFactory.GiveClaimedRoute(state, TestBoard.AB, opponent);

        GameEngine.CalculateTicketScore(player, state, TestBoard.AllRoutes);

        Assert.Equal(-TestBoard.TicketAB.Points, player.Score);
    }

    [Fact]
    public void CalculateLongestPathBonus_AwardsBonusToPlayerWithLongestPath()
    {
        var strongPlayer = TestFactory.CreatePlayer("strong");
        var weakPlayer = TestFactory.CreatePlayer("weak");
        var state = TestFactory.CreateState(strongPlayer, weakPlayer);
        // strongPlayer: A-B-C-D chain, total length 1+2+3 = 6
        TestFactory.GiveClaimedRoute(state, TestBoard.AB, strongPlayer);
        TestFactory.GiveClaimedRoute(state, TestBoard.BC, strongPlayer);
        TestFactory.GiveClaimedRoute(state, TestBoard.CD, strongPlayer);
        // weakPlayer: single disconnected route, length 2
        TestFactory.GiveClaimedRoute(state, TestBoard.XY, weakPlayer);

        GameEngine.CalculateLongestPathBonus(state, TestBoard.AllRoutes);

        Assert.Equal(10, strongPlayer.Score);
        Assert.Equal(0, weakPlayer.Score);
    }

    [Fact]
    public void CalculateLongestPathBonus_SplitsBonusOnTie()
    {
        var playerOne = TestFactory.CreatePlayer("p1");
        var playerTwo = TestFactory.CreatePlayer("p2");
        var state = TestFactory.CreateState(playerOne, playerTwo);
        // Both players own routes summing to the same length (2).
        TestFactory.GiveClaimedRoute(state, TestBoard.BC, playerOne); // length 2
        TestFactory.GiveClaimedRoute(state, TestBoard.XY, playerTwo); // length 2

        GameEngine.CalculateLongestPathBonus(state, TestBoard.AllRoutes);

        Assert.Equal(10, playerOne.Score);
        Assert.Equal(10, playerTwo.Score);
    }

    [Fact]
    public void CalculateLongestPathBonus_GivesNoBonusWhenNobodyHasClaimedRoutes()
    {
        var playerOne = TestFactory.CreatePlayer("p1");
        var playerTwo = TestFactory.CreatePlayer("p2");
        var state = TestFactory.CreateState(playerOne, playerTwo);

        GameEngine.CalculateLongestPathBonus(state, TestBoard.AllRoutes);

        Assert.Equal(0, playerOne.Score);
        Assert.Equal(0, playerTwo.Score);
    }
}
