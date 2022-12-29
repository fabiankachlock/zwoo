using ZwooGameLogic.ZRP;
using log4net;

namespace ZwooGameLogic.Lobby;

public class ZRPPlayerManager
{
    private INotificationAdapter _webSocketManager;
    private ZwooRoom _game;
    private ILog _logger;

    public ZRPPlayerManager(INotificationAdapter webSocketManager, ZwooRoom game)
    {
        _webSocketManager = webSocketManager;
        _game = game;
        _logger = LogManager.GetLogger("PlayerManager");
    }

    public async Task ConnectPlayer(long playerId)
    {
        var player = _game.Lobby.GetPlayer(playerId);
        if (player == null) return;

        _logger.Info($"{playerId} connected");
        _game.Lobby.PlayerConnected(playerId);
        if (player.Role == ZRPRole.Spectator)
        {
            // TODO: change player model to include wins
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorJoined, new SpectatorJoinedDTO(player.Username, 0));
        }
        else
        {
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerReconnected, new PlayerReconnectedDTO(player.Username));
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerJoined, new PlayerJoinedDTO(player.Username, 0));
        }
    }

    public async Task DisconnectPlayer(long playerId)
    {
        var player = _game.Lobby.GetPlayer(playerId);
        if (player == null) return;


        LobbyResult playerRemoveResult = LobbyResult.Success;

        if (_game.Game.IsRunning)
        {
            _logger.Info($"{playerId} disconnected from running game");
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerDisconnected, new PlayerDisconnectedDTO(player.Username));
            _game.Lobby.PlayerDisconnected(playerId);

            if (player.Role == ZRPRole.Host)
            {
                var newHost = _game.Lobby.SelectNewHost();
                if (newHost != null)
                {
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(newHost.Username, ZRPRole.Host, 0));
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.HostChanged, new HostChangedDTO(newHost.Username));
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
            playerRemoveResult = _game.Lobby.RemovePlayer(playerId);

            // only send leave message when the player leaves (NOT disconnects)
            if (player.Role == ZRPRole.Spectator)
            {
                // TODO: change player model to include wins
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorLeft, new SpectatorLeftDTO(player.Username));
            }
            else
            {
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerLeft, new PlayerLeftDTO(player.Username));
            }
        }

        if (_game.Lobby.ActivePlayerCount() == 0 || _game.Game.IsRunning && _game.Lobby.PlayerCount() < 2 || playerRemoveResult == LobbyResult.Error)
        {
            _logger.Info($"force closing game {_game.Id} due to a lack of players");
            _game.Close();
        }
    }

    public async Task FinishGame()
    {
        // transform all disconnected players into a spectator
        var disconnectedPlayers = _game.Lobby.Players()
            .Select(p => _game.Lobby.GetPlayer(p)!)
            .Where(p => p != null && p.State == PlayerState.Disconnected);

        foreach (var player in disconnectedPlayers)
        {
            // make them a spectator
            var result = _game.Lobby.ChangeRole(player.Username, ZRPRole.Spectator);
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(player.Username, ZRPRole.Spectator, 0));
        }

        // TODO: what was the intention on this??? this makes it impossible for player to rejoin after the game finished
        // TODO: rethink disconnected state - spectators should be excluded
        // TODO: this whole state & role model should be documented
        // game.Lobby.ResetDisconnectedStates();
    }
}
