using core.Game;
using core.Services;
using TicketToRide.Core.Tests.Helpers;

namespace TicketToRide.Core.Tests.Services;

public class GameEngineTurnsTests
{
    private static (Player A, Player B, Player C, Player D, GameState State) FourPlayerGame()
    {
        var a = TestFactory.CreatePlayer("A");
        var b = TestFactory.CreatePlayer("B");
        var c = TestFactory.CreatePlayer("C");
        var d = TestFactory.CreatePlayer("D");
        var state = TestFactory.CreateState(a, b, c, d);
        return (a, b, c, d, state);
    }

    [Fact]
    public void AdvanceTurn_IncrementsPlayerIndexAndWrapsAround()
    {
        var (_, _, _, _, state) = FourPlayerGame();
        state.CurrentPlayerIndex = 3;

        GameEngine.AdvanceTurn(state);

        Assert.Equal(0, state.CurrentPlayerIndex);
    }

    [Fact]
    public void AdvanceTurn_ResetsPerTurnFlags()
    {
        var (_, _, _, _, state) = FourPlayerGame();
        state.TrainCardDrawsThisTurn = 2;
        state.HasClaimedRouteThisTurn = true;
        state.HasDrawnTicketsThisTurn = true;

        GameEngine.AdvanceTurn(state);

        Assert.Equal(0, state.TrainCardDrawsThisTurn);
        Assert.False(state.HasClaimedRouteThisTurn);
        Assert.False(state.HasDrawnTicketsThisTurn);
    }

    [Fact]
    public void AdvanceTurn_RecordsTriggeringPlayerOnFirstLastRoundTrigger()
    {
        var (_, _, c, _, state) = FourPlayerGame();
        state.CurrentPlayerIndex = 2; // C's turn
        c.TrainsRemaining = 2; // triggers IsLastRound

        GameEngine.AdvanceTurn(state);

        Assert.Equal(2, state.LastRoundPlayerIndex);
    }

    [Fact]
    public void AdvanceTurn_DoesNotOverwriteLastRoundPlayerIndexOnSubsequentTrigger()
    {
        var (_, b, c, _, state) = FourPlayerGame();
        state.LastRoundPlayerIndex = 1; // B already triggered earlier
        state.CurrentPlayerIndex = 2; // now C also drops low, on C's turn
        c.TrainsRemaining = 1;

        GameEngine.AdvanceTurn(state);

        Assert.Equal(1, state.LastRoundPlayerIndex);
    }

    [Fact]
    public void EndTurnIfComplete_DoesNothingWhenRoundIsNotComplete()
    {
        var (_, _, _, _, state) = FourPlayerGame();
        state.CurrentPlayerIndex = 0;

        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes);

        Assert.Equal(0, state.CurrentPlayerIndex);
        Assert.False(state.IsGameOver);
    }

    [Fact]
    public void EndTurnIfComplete_AdvancesTurnWhenRoundCompleteAndNotLastRound()
    {
        var (_, _, _, _, state) = FourPlayerGame();
        state.CurrentPlayerIndex = 0;
        state.HasClaimedRouteThisTurn = true;

        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes);

        Assert.Equal(1, state.CurrentPlayerIndex);
        Assert.False(state.IsGameOver);
    }

    [Fact]
    public void EndTurnIfComplete_DoesNotEndGameOnTriggeringPlayersOwnTurn()
    {
        var (_, _, _, d, state) = FourPlayerGame();
        state.CurrentPlayerIndex = 3; // D's turn
        d.TrainsRemaining = 2; // D triggers last round right now
        state.HasClaimedRouteThisTurn = true; // the claim that caused the drop

        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes);

        Assert.False(state.IsGameOver);
        Assert.Equal(0, state.CurrentPlayerIndex); // turn moved on to A, not stuck on D
        Assert.Equal(3, state.LastRoundPlayerIndex); // trigger was recorded
    }

    [Fact]
    public void EndTurnIfComplete_EndsGameAfterEveryoneElseHadOneMoreTurn()
    {
        var (a, b, c, d, state) = FourPlayerGame();

        // D triggers last round on D's own turn.
        state.CurrentPlayerIndex = 3;
        d.TrainsRemaining = 2;
        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes); // -> A's turn

        // A, B get their final turn, nothing special happens yet.
        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes); // -> B's turn
        Assert.False(state.IsGameOver);

        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes); // -> C's turn
        Assert.False(state.IsGameOver);

        // C is the player right before D (the trigger) - game ends after C's turn.
        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes);

        Assert.True(state.IsGameOver);
        Assert.Equal(2, state.CurrentPlayerIndex); // stayed on C, never advanced to D again
    }

    [Fact]
    public void EndTurnIfComplete_ScoresTheGameWhenItEnds()
    {
        var (a, b, c, d, state) = FourPlayerGame();
        c.DestinationTickets.Add(TestBoard.TicketAB);
        TestFactory.GiveClaimedRoute(state, TestBoard.AB, c);

        state.CurrentPlayerIndex = 3;
        d.TrainsRemaining = 2;
        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes); // D triggers -> A's turn
        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes); // A's turn -> B's turn
        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes); // B's turn -> C's turn
        state.HasClaimedRouteThisTurn = true;
        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes); // C's turn -> game ends here

        Assert.True(state.IsGameOver);
        // C's ticket is fulfilled via the claimed AB route, plus the longest-path bonus
        // (C is the only one with any claimed route, so C also wins the 10 point bonus).
        Assert.Equal(TestBoard.TicketAB.Points + 10, c.Score);
    }

    [Fact]
    public void EndTurnIfComplete_DoesNothingOnceGameIsAlreadyOver()
    {
        var (_, _, _, _, state) = FourPlayerGame();
        state.IsGameOver = true;
        state.CurrentPlayerIndex = 2;
        state.HasClaimedRouteThisTurn = true;

        GameEngine.EndTurnIfComplete(state, TestBoard.AllRoutes);

        Assert.Equal(2, state.CurrentPlayerIndex);
    }
}
