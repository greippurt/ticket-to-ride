using core.Game;
using core.Models;

namespace core.Services;

public static partial class GameEngine
{
    public static int GetRoutePoints(Route route)
    {
        var points = route.Length switch
        {
            1 => 1,
            2 => 2,
            3 => 4,
            4 => 7,
            5 => 10,
            6 => 15,
            _ => throw new InvalidOperationException("Invalid route length"),
        };

        return points;
    }

    public static void CalculateTicketScore(Player player, GameState state, List<Route> allRoutes)
    {
        foreach (var ticket in player.DestinationTickets)
        {
            if (IsConnected(player, state, allRoutes, ticket.FromCityId, ticket.ToCityId))
                player.Score += ticket.Points;
            else
                player.Score -= ticket.Points;
        }
    }

    private static bool IsConnected(
        Player player,
        GameState state,
        List<Route> allRoutes,
        string fromCityId,
        string toCityId
    )
    {
        var playerRoutes = state
            .ClaimedRoutes.Values.Where(cr => cr.PlayerId == player.Id)
            .Select(cr => allRoutes.First(r => r.Id == cr.RouteId))
            .ToList();

        var visited = new HashSet<string> { fromCityId };
        var queue = new Queue<string>();
        queue.Enqueue(fromCityId);

        while (queue.Count > 0)
        {
            var city = queue.Dequeue();
            if (city == toCityId)
                return true;

            foreach (var route in playerRoutes)
            {
                var neighbor =
                    route.FromCityId == city ? route.ToCityId
                    : route.ToCityId == city ? route.FromCityId
                    : null;
                if (neighbor != null && visited.Add(neighbor))
                    queue.Enqueue(neighbor);
            }
        }
        return false;
    }

    public static void CalculateLongestPathBonus(GameState state, List<Route> allRoutes)
    {
        var longestByPlayer = state.Players.ToDictionary(
            p => p,
            p => LongestPathForPlayer(p, state, allRoutes)
        );

        var maxLength = longestByPlayer.Values.Max();
        if (maxLength == 0)
            return; // ingen har claimet ruter endnu

        foreach (var (player, length) in longestByPlayer)
        {
            if (length == maxLength)
                player.Score += 10;
        }
    }

    private static int LongestPathForPlayer(Player player, GameState state, List<Route> allRoutes)
    {
        var playerRoutes = state
            .ClaimedRoutes.Values.Where(cr => cr.PlayerId == player.Id)
            .Select(cr => allRoutes.First(r => r.Id == cr.RouteId))
            .ToList();

        if (playerRoutes.Count == 0)
            return 0;

        var cities = playerRoutes.SelectMany(r => new[] { r.FromCityId, r.ToCityId }).Distinct();

        return cities.Max(city => Dfs(city, playerRoutes, new HashSet<string>()));
    }

    private static int Dfs(string cityId, List<Route> playerRoutes, HashSet<string> usedRouteIds)
    {
        int best = 0;
        foreach (var route in playerRoutes)
        {
            if (usedRouteIds.Contains(route.Id))
                continue;
            if (route.FromCityId != cityId && route.ToCityId != cityId)
                continue;

            var nextCity = route.FromCityId == cityId ? route.ToCityId : route.FromCityId;

            usedRouteIds.Add(route.Id);
            best = Math.Max(best, route.Length + Dfs(nextCity, playerRoutes, usedRouteIds));
            usedRouteIds.Remove(route.Id);
        }
        return best;
    }

    public static void ScoreGame(GameState state, List<Route> allRoutes)
    {
        foreach (var player in state.Players)
        {
            CalculateTicketScore(player, state, allRoutes);
        }

        CalculateLongestPathBonus(state, allRoutes);
    }
}
