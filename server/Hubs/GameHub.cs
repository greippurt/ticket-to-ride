using core.Data;
using core.Dtos;
using core.Enums;
using core.Game;
using core.Models;
using core.Services;
using Microsoft.AspNetCore.SignalR;

namespace server.Hubs;

public class GameHub : Hub
{
    private readonly GameManager _gameManager;
    private readonly GameConnectionRegistry _registry;

    public GameHub(GameManager gameManager, GameConnectionRegistry registry)
    {
        _gameManager = gameManager;
        _registry = registry;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _registry.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public Task<CreateGameResultDto> CreateGame(List<string> playerNames)
    {
        var state = _gameManager.CreateGame(playerNames);
        var players = state.Players.Select(p => new PlayerSummaryDto(p.Id, p.Name)).ToList();
        return Task.FromResult(new CreateGameResultDto(state.Id, players));
    }

    public async Task<GameStateDto> JoinGame(string gameId, string playerId)
    {
        var state = GetStateOrThrow(gameId);
        if (state.Players.All(p => p.Id != playerId))
        {
            throw new HubException($"Player '{playerId}' is not part of game '{gameId}'.");
        }

        _registry.Add(Context.ConnectionId, gameId, playerId);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

        return GameStateMapper.ToDto(state, playerId);
    }

    public async Task DrawTrainCardFromDeck()
    {
        var (state, gameId, _) = GetCallerContext();

        if (state.TrainCardDrawsThisTurn >= 2)
        {
            throw new HubException("Cannot draw more than 2 train cards in a turn.");
        }

        RunEngineAction(() =>
        {
            GameEngine.DrawTrainCardForCurrentPlayer(state, Random.Shared);
            GameEngine.EndTurnIfComplete(state, PlaceholderBoard.Routes);
        });

        await BroadcastState(gameId, state);
    }

    public async Task DrawFaceUpTrainCard(TrainColor color)
    {
        var (state, gameId, _) = GetCallerContext();

        RunEngineAction(() =>
        {
            GameEngine.DrawFaceUpTrainCardForPlayer(state, color, Random.Shared);
            GameEngine.EndTurnIfComplete(state, PlaceholderBoard.Routes);
        });

        await BroadcastState(gameId, state);
    }

    public async Task ClaimRoute(string routeId, TrainColor color)
    {
        var (state, gameId, _) = GetCallerContext();
        var route =
            PlaceholderBoard.Routes.FirstOrDefault(r => r.Id == routeId)
            ?? throw new HubException($"Unknown route '{routeId}'.");

        RunEngineAction(() =>
        {
            GameEngine.ClaimRoute(state, route, color);
            GameEngine.EndTurnIfComplete(state, PlaceholderBoard.Routes);
        });

        await BroadcastState(gameId, state);
    }

    public Task<List<DestinationTicketDto>> DrawDestinationTickets()
    {
        var (state, _, _) = GetCallerContext();

        if (state.HasDrawnTicketsThisTurn)
        {
            throw new HubException("You have already drawn destination tickets this turn.");
        }

        List<DestinationTicket> tickets = null!;
        RunEngineAction(() =>
        {
            tickets = GameEngine.DrawDestinationTickets(state, Random.Shared);
        });

        _registry.SetPendingTicketDraw(Context.ConnectionId, tickets);
        var dtos = tickets.Select(t => new DestinationTicketDto(t.Id, t.FromCityId, t.ToCityId, t.Points)).ToList();
        return Task.FromResult(dtos);
    }

    public async Task ChooseDestinationTickets(List<string> ticketIds)
    {
        var (state, gameId, _) = GetCallerContext();

        if (!_registry.TryTakePendingTicketDraw(Context.ConnectionId, out var drawnTickets) || drawnTickets is null)
        {
            throw new HubException("No destination tickets have been drawn yet.");
        }

        var chosen = drawnTickets.Where(t => ticketIds.Contains(t.Id)).ToList();
        if (chosen.Count != ticketIds.Count)
        {
            _registry.SetPendingTicketDraw(Context.ConnectionId, drawnTickets);
            throw new HubException("Chosen tickets do not match the drawn tickets.");
        }

        try
        {
            RunEngineAction(() =>
            {
                GameEngine.ChooseDestinationTicketsForCurrentPlayer(state, chosen);
                GameEngine.EndTurnIfComplete(state, PlaceholderBoard.Routes);
            });
        }
        catch (HubException)
        {
            _registry.SetPendingTicketDraw(Context.ConnectionId, drawnTickets);
            throw;
        }

        await BroadcastState(gameId, state);
    }

    private (GameState State, string GameId, Player Player) GetCallerContext()
    {
        if (!_registry.TryGetPlayer(Context.ConnectionId, out var info))
        {
            throw new HubException("You must join a game before performing this action.");
        }

        var state = GetStateOrThrow(info.GameId);
        var player =
            state.Players.FirstOrDefault(p => p.Id == info.PlayerId)
            ?? throw new HubException("Player not found in game.");

        if (state.CurrentPlayer.Id != player.Id)
        {
            throw new HubException("It is not your turn.");
        }

        return (state, info.GameId, player);
    }

    private GameState GetStateOrThrow(string gameId)
    {
        if (!_gameManager.TryGetGame(gameId, out var state) || state is null)
        {
            throw new HubException($"Game '{gameId}' was not found.");
        }

        return state;
    }

    private static void RunEngineAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            throw new HubException(ex.Message);
        }
    }

    private async Task BroadcastState(string gameId, GameState state)
    {
        foreach (var (connectionId, playerId) in _registry.GetConnectionsForGame(gameId))
        {
            var dto = GameStateMapper.ToDto(state, playerId);
            await Clients.Client(connectionId).SendAsync("GameStateUpdated", dto);
        }
    }
}
