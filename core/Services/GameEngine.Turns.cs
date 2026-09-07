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
    }

    public static void EndTurnIfComplete(GameState state)
    {
        if (state.IsRoundComplete)
        {
            AdvanceTurn(state);
        }
    }
}
