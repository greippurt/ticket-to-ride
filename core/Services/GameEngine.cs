using core.Enums;
using core.Game;

namespace core.Services;

public class GameEngine
{
    public TrainColor DrawCard(GameState state, Random rng)
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
            state.TrainDiscardPile.Clear();
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
}
