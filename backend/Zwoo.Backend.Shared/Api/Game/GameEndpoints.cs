using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Authentication;
using Zwoo.GameEngine;
using Zwoo.GameEngine.Lobby;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.Backend.Shared.Api.Game;

public class GameEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/game/list", ([FromQuery] bool recommended,
            [FromQuery] int offset,
            [FromQuery] int limit,
            [FromQuery] string filter,
            [FromQuery] bool publicOnly,
            IGameProviderService _games) =>
        {
            return Results.Ok(new GamesList()
            {
                Games = _games.QueryGames(offset, limit, new GameQueryOptions()
                {
                    Filter = filter,
                    PublicOnly = publicOnly,
                    Recommended = recommended
                })
            });
        });

        app.MapGet("/game/{id}", ([FromRoute] long id, IGameProviderService _games) =>
        {
            var game = _games.GetGame(id);
            if (game == null)
            {
                return Results.NotFound(ApiError.GameNotFound.ToProblem(new ProblemDetails()
                {
                    Title = "Game not found",
                    Detail = "The specified game could not be found.",
                    Instance = $"/game/{id}"
                }));
            }
            return Results.Ok(game);
        });

        app.MapPost("/game/create", ([FromBody] CreateGame body,
            HttpContext context,
            IGameConnectionsService _connections,
            IGameEngineService _games,
            ILogger<GameEndpoints> _logger) =>
        {
            if (string.IsNullOrEmpty(body.Name) || body.Name.Count() < 3 || (body.UsePassword && string.IsNullOrEmpty(body.Password)))
            {
                return Results.BadRequest(ApiError.InvalidGameName.ToProblem(new ProblemDetails()
                {
                    Title = "Missing information",
                    Detail = "The game name should be at least 3 characters long and if the game is not public a password should be set.",
                    Instance = context.Request.Path
                }));
            }

            var activeSession = context.GetActiveUser();
            if (_connections.HasConnection((long)activeSession.User.Id))
            {
                return Results.BadRequest(ApiError.AlreadyInGame.ToProblem(new ProblemDetails()
                {
                    Title = "Already in game",
                    Detail = "This account has already an connection registered",
                    Instance = context.Request.Path
                }));
            }

            var game = _games.CreateGame(body.Name, !body.UsePassword);
            game.Lobby.Initialize((long)activeSession.User.Id, activeSession.Username, body.Password ?? "", !body.UsePassword);
            _logger.LogInformation($"{activeSession.User.Id} created game {game.Id}");
            var lobbyEntry = game.Lobby.GetPossiblyPreparedPlayerByUserId((long)activeSession.User.Id);
            return Results.Ok(new JoinedGame()
            {
                GameId = game.Id,
                IsRunning = game.Game.IsRunning,
                OwnId = lobbyEntry?.LobbyId ?? 1,
                Role = lobbyEntry?.Role ?? ZRPRole.Host
            });
        });

        app.MapPost("/game/join", ([FromBody] JoinGame body,
            HttpContext context,
            IGameConnectionsService _connections,
            IGameEngineService _games,
            ILogger<GameEndpoints> _logger) =>
        {
            var game = _games.GetGame(body.GameId);
            if (game == null)
            {
                return Results.NotFound(ApiError.GameNotFound.ToProblem(new ProblemDetails()
                {
                    Title = "Game not found",
                    Detail = "A game with this id cannot be found",
                    Instance = context.Request.Path
                }));
            }

            var activeSession = context.GetActiveUser();
            if (_connections.HasConnection((long)activeSession.User.Id) || game.Lobby.GetPossiblyPreparedPlayerByUserId((long)activeSession.User.Id) != null)
            {
                return Results.BadRequest(ApiError.AlreadyInGame.ToProblem(new ProblemDetails()
                {
                    Title = "Already in game",
                    Detail = "This account has already an connection registered",
                    Instance = context.Request.Path
                }));
            }

            if (game.Game.IsRunning)
            {
                // join as spectator when game is already running and its an new player
                // don't make the player a spectator when he his already in the game, because then hes rejoining
                body.Role = ZRPRole.Spectator;
            }
            LobbyResult result = game.Lobby.AddPlayer((long)activeSession.User.Id, game.NextId(), activeSession.User.Username, body.Role, body.Password ?? "");
            if (result != LobbyResult.Success)
            {
                return Results.BadRequest(result.ToApi().ToProblem(new ProblemDetails()
                {
                    Title = "Already in game",
                    Detail = "This account has already an connection registered",
                    Instance = context.Request.Path
                }));
            }

            _logger.LogInformation($"{activeSession.User.Id} joined game {game.Id}");
            var lobbyEntry = game.Lobby.GetPossiblyPreparedPlayerByUserId((long)activeSession.User.Id);
            return Results.Ok(new JoinedGame()
            {
                GameId = game.Id,
                IsRunning = game.Game.IsRunning,
                OwnId = lobbyEntry?.LobbyId ?? 0,
                Role = lobbyEntry?.Role ?? ZRPRole.Player
            });
        });

        app.MapGet("/game/{id}/connect", async ([FromRoute] long id,
            HttpContext context,
            IGameConnectionsService _connections,
            IGameConnectionHandlerService _handler,
            IGameEngineService _games,
            ILogger<GameEndpoints> _logger) =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return Results.BadRequest(new ProblemDetails()
                {
                    Title = "Needs websocket request",
                    Detail = "This endpoint needs to be access via a websocket connection.",
                    Instance = context.Request.Path
                });
            }


            var activeSession = context.GetActiveUser();
            var game = _games.GetGame(id);
            if (game == null)
            {
                return Results.NotFound(ApiError.GameNotFound.ToProblem(new ProblemDetails()
                {
                    Title = "Game not found",
                    Detail = "A game with this id cannot be found",
                    Instance = context.Request.Path
                }));
            }

            LobbyResult result = game.Lobby.IsPlayerAllowedToConnect((long)activeSession.User.Id);
            if (result != LobbyResult.Success)
            {
                return Results.BadRequest(result.ToApi().ToProblem(new ProblemDetails()
                {
                    Title = "Cant connect to game",
                    Detail = "The request to connect was rejected by the lobby.",
                    Instance = context.Request.Path
                }));
            }

            _logger.LogInformation($"[{activeSession.User.Id}] accepting websocket");
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            CancellationTokenSource shouldStop = new CancellationTokenSource();
            TaskCompletionSource finished = new TaskCompletionSource();

            _logger.LogInformation($"[{activeSession.User.Id}] adding connection");
            bool success = _connections.AddConnection(id, (long)activeSession.User.Id, webSocket, shouldStop);
            if (!success)
            {
                _logger.LogError($"[{activeSession.User.Id}] cant add connection");
                throw new Exception("cant connect user");
            }

            _logger.LogInformation($"[{activeSession.User.Id}] notify playermanager");
            await game.PlayerManager.ConnectPlayer((long)activeSession.User.Id);

            _logger.LogInformation($"[{activeSession.User.Id}] handle");
            try
            {
                _handler.Handle(id, (long)activeSession.User.Id, webSocket, shouldStop.Token, finished);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"[{activeSession.User.Id}] task cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[{activeSession.User.Id}] an error happened while handling a socket", ex);
            }
            await finished.Task;

            _logger.LogInformation($"[{activeSession.User.Id}] remove connection");
            success = _connections.RemoveConnection(id, (long)activeSession.User.Id);
            if (!success)
            {
                _logger.LogError($"[{activeSession.User.Id}] cant remove connection");
            }

            _logger.LogInformation($"[{activeSession.User.Id}] disconnect");
            await game.PlayerManager.DisconnectPlayer((long)activeSession.User.Id);
            return Results.Empty;
        });
    }
}