using core.Game;

namespace core.Services;

public static partial class GameEngine
{
    // AdvanceTurn, EndTurnIfComplete m.fl. kommer her
    public static void AdvanceTurn(GameState state)
    {
        state.CurrentPlayerIndex = (state.CurrentPlayerIndex + 1) % state.Players.Count;
        state.TrainCardDrawsThisTurn = 0;
        state.HasClaimedRouteThisTurn = false;
        state.HasDrawnTicketsThisTurn = false;

        if (state.IsLastRound && state.LastRoundPlayerIndex == null)
        {
            state.LastRoundPlayerIndex = state.CurrentPlayerIndex;
        }
    }

    public static void EndTurnIfComplete(GameState state)
    {
        if (!state.IsRoundComplete)
            return;

        if (
            state.IsLastRound
            && state.CurrentPlayerIndex
                == (state.LastRoundPlayerIndex - 1 + state.Players.Count) % state.Players.Count
        )
        {
            state.IsGameOver = true;
            return;
        }

        AdvanceTurn(state);
    }
}
