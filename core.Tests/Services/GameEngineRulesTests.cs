using core.Enums;
using core.Models;
using core.Services;
using TicketToRide.Core.Tests.Helpers;

namespace TicketToRide.Core.Tests.Services;

public class GameEngineRulesTests
{
    [Fact]
    public void ClaimRoute_ThrowsAfterDrawingATrainCard()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        TestFactory.GiveTrainCards(player, TrainColor.Red, 1);
        state.TrainCardDrawsThisTurn = 1;

        Assert.Throws<InvalidOperationException>(
            () => GameEngine.ClaimRoute(state, TestBoard.AB, TrainColor.Red)
        );
    }

    [Fact]
    public void ClaimRoute_CanUseLocomotivesAsWildcards()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        TestFactory.GiveTrainCards(player, TrainColor.Green, 1);
        TestFactory.GiveTrainCards(player, TrainColor.Locomotive, 2);

        // CD is a green route of length 3: pay with 1 green + 2 locomotives
        GameEngine.ClaimRoute(state, TestBoard.CD, TrainColor.Green, locomotives: 2);

        Assert.True(state.IsRouteClaimed(TestBoard.CD.Id));
        Assert.Equal(0, player.TrainCards[TrainColor.Green]);
        Assert.Equal(0, player.TrainCards[TrainColor.Locomotive]);
    }

    [Fact]
    public void DrawFaceUpTrainCard_RejectedCardStaysFaceUp()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        state.FaceUpTrainCards.Add(TrainColor.Locomotive);
        state.TrainCardDrawsThisTurn = 1;

        // A face-up locomotive cannot be taken as the second card
        Assert.Throws<InvalidOperationException>(
            () => GameEngine.DrawFaceUpTrainCardForPlayer(state, TrainColor.Locomotive, new Random(1))
        );

        Assert.Contains(TrainColor.Locomotive, state.FaceUpTrainCards);
    }

    [Fact]
    public void DrawTrainCard_ThrowsWhileChoosingDestinationTickets()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        state.TicketDrawPile.AddRange(new[] { TestBoard.TicketAB, TestBoard.TicketAD, TestBoard.TicketXY });
        var rng = new Random(1);

        GameEngine.DrawDestinationTickets(state, rng);

        Assert.Throws<InvalidOperationException>(
            () => GameEngine.DrawTrainCardForCurrentPlayer(state, rng)
        );
        Assert.Equal(0, state.TrainCardDrawsThisTurn);
    }

    [Fact]
    public void ChooseDestinationTickets_PutsUnchosenTicketsBackInThePile()
    {
        var player = TestFactory.CreatePlayer("p1");
        var state = TestFactory.CreateState(player);
        var drawn = new List<DestinationTicket>
        {
            TestBoard.TicketAB,
            TestBoard.TicketAD,
            TestBoard.TicketXY,
        };

        GameEngine.ChooseDestinationTicketsForCurrentPlayer(state, drawn, new() { TestBoard.TicketAB });

        Assert.Equal(new[] { TestBoard.TicketAB }, player.DestinationTickets);
        Assert.Equal(new[] { TestBoard.TicketAD, TestBoard.TicketXY }, state.TicketDrawPile);
    }
}
