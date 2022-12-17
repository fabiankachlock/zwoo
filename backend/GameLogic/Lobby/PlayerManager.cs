using ZwooGameLogic.ZRP;
using log4net;

namespace ZwooGameLogic.Lobby;

// should this be in gamelogic?
public class PlayerManager
{
    // TODO: cleanup - this should be managed twice
    private Dictionary<long, HashSet<long>> _games = new Dictionary<long, HashSet<long>>();
    private INotificationAdapter _webSocketManager;
    private GameManager _gameManager;
    private ILog _logger;

    public PlayerManager(INotificationAdapter webSocketManager, GameManager gameManager)
    {
        _webSocketManager = webSocketManager;
        _gameManager = gameManager;
        _logger = LogManager.GetLogger("PlayerManager");
    }

    public async Task ConnectPlayer(long playerId, long gameId)
    {
        ZwooRoom? game = _gameManager.GetGame(gameId);
        var player = game?.Lobby.GetPlayer(playerId);
        if (game == null || player == null) return;

        _logger.Info($"{playerId} connected");
        game?.Lobby.PlayerConnected(playerId);
        if (player.Role == ZRPRole.Spectator)
        {
            // TODO: change player model to include wins
            await _webSocketManager.BroadcastGame(gameId, ZRPCode.SpectatorJoined, new SpectatorJoinedDTO(player.Username, 0));
        }
        else
        {
            await _webSocketManager.BroadcastGame(gameId, ZRPCode.PlayerReconnected, new PlayerReconnectedDTO(player.Username));
            await _webSocketManager.BroadcastGame(gameId, ZRPCode.PlayerJoined, new PlayerJoinedDTO(player.Username, 0));
        }
    }

    public async Task DisconnectPlayer(long playerId, long gameId)
    {
        ZwooRoom? game = _gameManager.GetGame(gameId);
        var player = game?.Lobby.GetPlayer(playerId);
        if (player == null)
        {
            return;
        }

        if (game != null)
        {
            LobbyResult playerRemoveResult = LobbyResult.Success;

            if (game.Game.IsRunning)
            {
                _logger.Info($"{playerId} disconnected from running game");
                await _webSocketManager.BroadcastGame(gameId, ZRPCode.PlayerDisconnected, new PlayerDisconnectedDTO(player.Username));
                game?.Lobby.PlayerDisconnected(playerId);

                if (player.Role == ZRPRole.Host)
                {
                    var newHost = game!.Lobby.SelectNewHost();
                    if (newHost != null)
                    {
                        await _webSocketManager.BroadcastGame(gameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(newHost.Username, ZRPRole.Host, 0));
                        await _webSocketManager.BroadcastGame(gameId, ZRPCode.HostChanged, new HostChangedDTO(newHost.Username));
                        await _webSocketManager.SendPlayer(newHost.Id, ZRPCode.PromotedToHost, new PromotedToHostDTO());
                    }
                    else
                    {
                        playerRemoveResult = LobbyResult.Error;
                    }
                }

            }
            else
            {
                _logger.Info($"{playerId} removed from lobby");
                playerRemoveResult = game.Lobby.RemovePlayer(playerId);

                // only send leave message when the player leaves (NOT disconnects)
                if (player.Role == ZRPRole.Spectator)
                {
                    // TODO: change player model to include wins
                    await _webSocketManager.BroadcastGame(gameId, ZRPCode.SpectatorLeft, new SpectatorLeftDTO(player.Username));
                }
                else
                {
                    await _webSocketManager.BroadcastGame(gameId, ZRPCode.PlayerLeft, new PlayerLeftDTO(player.Username));
                }
            }

            if (game == null) return;
            _logger.Info(game.Lobby.ListAll());
            if (game?.Lobby.ActivePlayerCount() == 0 || game.Game.IsRunning && game?.Lobby.PlayerCount() < 2 || playerRemoveResult == LobbyResult.Error)
            {
                _logger.Info($"force closing game {gameId} due to a lack of players");
                await _webSocketManager.DisconnectGame(gameId);
                _gameManager.RemoveGame(gameId);
            }
        }
    }

    public async Task FinishGame(long gameId)
    {
        ZwooRoom? game = _gameManager.GetGame(gameId);
        if (game == null)
        {
            return;
        }

        // transform all disconnected players into a spectator
        var disconnectedPlayers = game.Lobby.Players()
            .Select(p => game.Lobby.GetPlayer(p)!)
            .Where(p => p != null && p.State == PlayerState.Disconnected);

        // TODO: make this parallel
        foreach (var player in disconnectedPlayers)
        {
            // make them a spectator
            var result = game.Lobby.ChangeRole(player.Username, ZRPRole.Spectator);
            await _webSocketManager.BroadcastGame(gameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(player.Username, ZRPRole.Spectator, 0));
        }

        game.Lobby.ResetDisconnectedStates();
    }
}
