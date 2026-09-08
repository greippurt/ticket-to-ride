using core.Data;
using core.Game;
using core.Models;

namespace core.Services;

public static partial class GameEngine
{
    // AdvanceTurn, EndTurnIfComplete m.fl. kommer her
    public static void AdvanceTurn(GameState state)
    {
        if (state.IsLastRound && state.LastRoundPlayerIndex == null)
        {
            state.LastRoundPlayerIndex = state.CurrentPlayerIndex;
        }

        state.CurrentPlayerIndex = (state.CurrentPlayerIndex + 1) % state.Players.Count;
        state.TrainCardDrawsThisTurn = 0;
        state.HasClaimedRouteThisTurn = false;
        state.HasDrawnTicketsThisTurn = false;
    }

    public static void EndTurnIfComplete(GameState state, List<Route> allRoutes)
    {
        if (state.IsGameOver)
            return;

        if (!state.IsRoundComplete)
            return;

        if (
            state.IsLastRound
            && state.CurrentPlayerIndex
                == (state.LastRoundPlayerIndex - 1 + state.Players.Count) % state.Players.Count
        )
        {
            state.IsGameOver = true;
            ScoreGame(state, allRoutes);
            return;
        }

        AdvanceTurn(state);
    }
}
