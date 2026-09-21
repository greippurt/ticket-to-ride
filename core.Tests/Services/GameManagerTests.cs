using core.Data;
using core.Services;

namespace TicketToRide.Core.Tests.Services;

public class GameManagerTests
{
    [Fact]
    public void CreateGame_CreatesRequestedNumberOfPlayersWithUniqueIds()
    {
        var manager = new GameManager();

        var state = manager.CreateGame(new List<string> { "Alice", "Bob", "Carol" }, new Random(1));

        Assert.Equal(3, state.Players.Count);
        Assert.Equal(new[] { "Alice", "Bob", "Carol" }, state.Players.Select(p => p.Name));
        Assert.Equal(3, state.Players.Select(p => p.Id).Distinct().Count());
    }

    [Fact]
    public void CreateGame_FillsTicketDrawPileFromBoard()
    {
        var manager = new GameManager();

        var state = manager.CreateGame(new List<string> { "Alice", "Bob" }, new Random(1));

        Assert.Equal(PlaceholderBoard.DestinationTickets.Count, state.TicketDrawPile.Count);
    }

    [Fact]
    public void CreateGame_DealsFourTrainCardsToEachPlayer()
    {
        var manager = new GameManager();

        var state = manager.CreateGame(new List<string> { "Alice", "Bob" }, new Random(1));

        foreach (var player in state.Players)
        {
            Assert.Equal(4, player.TrainCards.Values.Sum());
        }
    }

    [Fact]
    public void CreateGame_AddsFiveFaceUpTrainCards()
    {
        var manager = new GameManager();

        var state = manager.CreateGame(new List<string> { "Alice", "Bob" }, new Random(1));

        Assert.Equal(5, state.FaceUpTrainCards.Count);
    }

    [Fact]
    public void CreateGame_IsDeterministicGivenSeededRng()
    {
        var manager = new GameManager();

        var stateA = manager.CreateGame(new List<string> { "Alice", "Bob" }, new Random(42));
        var stateB = manager.CreateGame(new List<string> { "Alice", "Bob" }, new Random(42));

        Assert.Equal(stateA.FaceUpTrainCards, stateB.FaceUpTrainCards);
        for (int i = 0; i < stateA.Players.Count; i++)
        {
            Assert.Equal(stateA.Players[i].TrainCards, stateB.Players[i].TrainCards);
        }
    }

    [Fact]
    public void GetGame_ReturnsGameCreatedWithCreateGame()
    {
        var manager = new GameManager();
        var created = manager.CreateGame(new List<string> { "Alice", "Bob" }, new Random(1));

        var fetched = manager.GetGame(created.Id);

        Assert.Same(created, fetched);
    }

    [Fact]
    public void TryGetGame_ReturnsFalseForUnknownGameId()
    {
        var manager = new GameManager();

        var found = manager.TryGetGame("unknown-id", out var state);

        Assert.False(found);
        Assert.Null(state);
    }

    [Fact]
    public void GetGame_ThrowsForUnknownGameId()
    {
        var manager = new GameManager();

        Assert.Throws<KeyNotFoundException>(() => manager.GetGame("unknown-id"));
    }

    [Fact]
    public void RemoveGame_MakesGameNoLongerRetrievable()
    {
        var manager = new GameManager();
        var created = manager.CreateGame(new List<string> { "Alice", "Bob" }, new Random(1));

        manager.RemoveGame(created.Id);

        Assert.False(manager.TryGetGame(created.Id, out _));
    }
}
