using core.Dtos;
using core.Enums;
using core.Game;
using core.Models;
using TicketToRide.Core.Tests.Helpers;

namespace TicketToRide.Core.Tests.Dtos;

public class GameStateMapperTests
{
    private static readonly DestinationTicket TicketA = new("t1", "newYork", "chicago", 12);
    private static readonly DestinationTicket TicketB = new("t2", "losAngeles", "houston", 16);

    private static (Player Viewer, Player Opponent, GameState State) TwoPlayerGame()
    {
        var viewer = TestFactory.CreatePlayer("viewer");
        var opponent = TestFactory.CreatePlayer("opponent");
        var state = TestFactory.CreateState(viewer, opponent);
        return (viewer, opponent, state);
    }

    [Fact]
    public void ToDto_ExposesOwnTrainCardsByColor()
    {
        var (viewer, _, state) = TwoPlayerGame();
        TestFactory.GiveTrainCards(viewer, TrainColor.Red, 3);

        var dto = GameStateMapper.ToDto(state, viewer.Id);

        var viewerDto = dto.Players.Single(p => p.Id == viewer.Id);
        Assert.NotNull(viewerDto.TrainCards);
        Assert.Equal(3, viewerDto.TrainCards![TrainColor.Red]);
        Assert.Equal(3, viewerDto.TrainCardCount);
    }

    [Fact]
    public void ToDto_HidesOpponentTrainCardColorsButExposesCount()
    {
        var (viewer, opponent, state) = TwoPlayerGame();
        TestFactory.GiveTrainCards(opponent, TrainColor.Blue, 2);
        TestFactory.GiveTrainCards(opponent, TrainColor.Green, 1);

        var dto = GameStateMapper.ToDto(state, viewer.Id);

        var opponentDto = dto.Players.Single(p => p.Id == opponent.Id);
        Assert.Null(opponentDto.TrainCards);
        Assert.Equal(3, opponentDto.TrainCardCount);
    }

    [Fact]
    public void ToDto_ExposesOwnDestinationTickets()
    {
        var (viewer, _, state) = TwoPlayerGame();
        viewer.DestinationTickets.Add(TicketA);

        var dto = GameStateMapper.ToDto(state, viewer.Id);

        var viewerDto = dto.Players.Single(p => p.Id == viewer.Id);
        Assert.NotNull(viewerDto.DestinationTickets);
        Assert.Single(viewerDto.DestinationTickets!);
        Assert.Equal(TicketA.Id, viewerDto.DestinationTickets![0].Id);
        Assert.Equal(1, viewerDto.DestinationTicketCount);
    }

    [Fact]
    public void ToDto_HidesOpponentDestinationTicketsButExposesCount()
    {
        var (viewer, opponent, state) = TwoPlayerGame();
        opponent.DestinationTickets.Add(TicketA);
        opponent.DestinationTickets.Add(TicketB);

        var dto = GameStateMapper.ToDto(state, viewer.Id);

        var opponentDto = dto.Players.Single(p => p.Id == opponent.Id);
        Assert.Null(opponentDto.DestinationTickets);
        Assert.Equal(2, opponentDto.DestinationTicketCount);
    }

    [Fact]
    public void ToDto_MapsClaimedRoutesAndFaceUpCards()
    {
        var (viewer, opponent, state) = TwoPlayerGame();
        var route = new Route("a-b", "a", "b", 2, TrainColor.Red);
        TestFactory.GiveClaimedRoute(state, route, opponent);
        state.FaceUpTrainCards.AddRange(new[] { TrainColor.Red, TrainColor.Locomotive });

        var dto = GameStateMapper.ToDto(state, viewer.Id);

        var claimed = Assert.Single(dto.ClaimedRoutes);
        Assert.Equal(route.Id, claimed.RouteId);
        Assert.Equal(opponent.Id, claimed.PlayerId);
        Assert.Equal(new[] { TrainColor.Red, TrainColor.Locomotive }, dto.FaceUpTrainCards);
    }

    [Fact]
    public void ToDto_ThrowsWhenViewerIsNotInGame()
    {
        var (_, _, state) = TwoPlayerGame();

        Assert.Throws<ArgumentException>(() => GameStateMapper.ToDto(state, "unknown-player"));
    }
}
