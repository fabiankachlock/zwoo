using ZwooGameLogic.ZRP;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Lobby;

public class ZRPPlayerManager
{
    private INotificationAdapter _webSocketManager;
    private ZwooRoom _game;
    private ILogger _logger;

    public ZRPPlayerManager(INotificationAdapter webSocketManager, ZwooRoom game, ILogger logger)
    {
        _webSocketManager = webSocketManager;
        _game = game;
        _game.Game.OnFinished += async (data, meta) => await this.FinishGame();
        _logger = logger;
    }

    public async Task ConnectPlayer(long playerId)
    {
        var player = _game.Lobby.GetPlayer(playerId);
        if (player == null) return;

        _logger.Info($"{playerId} connected");
        _game.Lobby.MarkPlayerConnected(playerId);
        if (player.Role == ZRPRole.Spectator)
        {
            // TODO: change player model to include wins
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorJoined, new SpectatorJoinedNotification(player.PublicId, player.Username));
        }
        else
        {
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerReconnected, new PlayerReconnectedNotification(player.PublicId));
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerJoined, new PlayerJoinedNotification(player.PublicId, player.Username));
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
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerDisconnected, new PlayerDisconnectedNotification(player.PublicId));
            _game.Lobby.MarkPlayerDisconnected(playerId);

            if (player.Role == ZRPRole.Host)
            {
                var newHost = _game.Lobby.SelectNewHost();
                if (newHost != null)
                {
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(newHost.PublicId, ZRPRole.Host));
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.HostChanged, new NewHostNotification(newHost.PublicId));
                    await _webSocketManager.SendPlayer(newHost.Id, ZRPCode.PromotedToHost, new YouAreHostNotification());
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
            playerRemoveResult = _game.Lobby.RemovePlayer(player.PublicId);

            // only send leave message when the player leaves (NOT disconnects)
            if (player.Role == ZRPRole.Spectator)
            {
                // TODO: change player model to include wins
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.PublicId));
            }
            else
            {
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.PublicId));
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
        var disconnectedPlayers = _game.Lobby.GetPlayers()
            .Where(p => p != null && p.State == ZRPPlayerState.Disconnected);

        foreach (var player in disconnectedPlayers)
        {
            // make them a spectator
            var result = _game.Lobby.ChangeRole(player.PublicId, ZRPRole.Spectator);
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(player.PublicId, ZRPRole.Spectator));
        }

        // TODO: what was the intention on this??? this makes it impossible for player to rejoin after the game finished
        // TODO: rethink disconnected state - spectators should be excluded
        // TODO: this whole state & role model should be documented
        // game.Lobby.ResetDisconnectedStates();
    }
}
