using core.Enums;
using core.Game;

namespace core.Services;

public static partial class GameEngine
{
    public static TrainColor DrawCard(GameState state, Random rng)
    {
        int total = state.TrainDrawPile.Values.Sum();
        if (total == 0)
        {
            foreach (var color in state.TrainDiscardPile.Keys.ToList())
            {
                state.TrainDrawPile[color] =
                    state.TrainDrawPile.GetValueOrDefault(color) + state.TrainDiscardPile[color];
                state.TrainDiscardPile[color] = 0;
            }
            total = state.TrainDrawPile.Values.Sum();
        }

        int roll = rng.Next(total);
        foreach (var (color, count) in state.TrainDrawPile)
        {
            if (roll < count)
            {
                state.TrainDrawPile[color] = count - 1;
                return color;
            }
            roll -= count;
        }
        throw new InvalidOperationException("Draw pile empty");
    }

    public static TrainColor DrawTrainCardForPlayer(GameState state, Player player, Random rng)
    {
        var color = DrawCard(state, rng);
        player.TrainCards[color]++;
        return color;
    }

    public static TrainColor DrawTrainCardForCurrentPlayer(GameState state, Random rng)
    {
        state.TrainCardDrawsThisTurn++;
        return DrawTrainCardForPlayer(state, state.CurrentPlayer, rng);
    }

    public static void DealStartingHands(GameState state, Random rng)
    {
        foreach (var player in state.Players)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawTrainCardForPlayer(state, player, rng);
            }
        }
    }

    public static void AddFaceUpTrainCard(GameState state, Random rng)
    {
        if (state.FaceUpTrainCards.Count >= 5)
        {
            throw new InvalidOperationException("Cannot add more than 5 face-up train cards");
        }

        var color = DrawCard(state, rng);
        state.FaceUpTrainCards.Add(color);
    }

    public static TrainColor DrawFaceUpTrainCardForPlayer(
        GameState state,
        TrainColor color,
        Random rng
    )
    {
        if (!state.FaceUpTrainCards.Remove(color))
        {
            throw new ArgumentException($"Color {color} is not currently face-up", nameof(color));
        }

        state.CurrentPlayer.TrainCards[color]++;
        state.TrainCardDrawsThisTurn++;
        AddFaceUpTrainCard(state, rng);

        return color;
    }
}
