using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine.Lobby;

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
        var player = _game.Lobby.GetPlayerByUserId(playerId);
        if (player == null) return;

        _logger.Info($"{playerId} connected");
        _game.Lobby.MarkPlayerConnected(playerId);
        if (player.Role == ZRPRole.Spectator)
        {
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorJoined, new SpectatorJoinedNotification(player.LobbyId, player.Username));
        }
        else
        {
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerReconnected, new PlayerReconnectedNotification(player.LobbyId));
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerJoined, new PlayerJoinedNotification(player.LobbyId, player.Username, 0, false));
        }
    }

    public async Task DisconnectPlayer(long playerId)
    {
        var player = _game.Lobby.GetPlayerByUserId(playerId);
        if (player == null) return;

        LobbyResult playerRemoveResult = LobbyResult.Success;
        if (_game.Game.IsRunning && player.Role != ZRPRole.Spectator)
        {
            _logger.Info($"{playerId} disconnected from running game");
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerDisconnected, new PlayerDisconnectedNotification(player.LobbyId));
            _game.Lobby.MarkPlayerDisconnected(playerId);

            if (player.Role == ZRPRole.Host)
            {
                var newHost = _game.Lobby.SelectNewHost();
                if (newHost != null)
                {
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(newHost.LobbyId, ZRPRole.Host, 0));
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.HostChanged, new NewHostNotification(newHost.LobbyId));
                    await _webSocketManager.SendPlayer(newHost.LobbyId, ZRPCode.PromotedToHost, new YouAreHostNotification());
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
            var previousRole = player.Role;
            playerRemoveResult = _game.Lobby.RemovePlayer(player.LobbyId);

            if (previousRole == ZRPRole.Host)
            {
                IPlayer? newHost = _game.Lobby.GetHost();
                if (newHost != null)
                {
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(newHost.LobbyId, ZRPRole.Host, 0));
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.HostChanged, new NewHostNotification(newHost.LobbyId));
                    await _webSocketManager.SendPlayer(newHost.LobbyId, ZRPCode.PromotedToHost, new YouAreHostNotification());
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.LobbyId));
                }
            }
            else if (previousRole == ZRPRole.Spectator)
            {
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.LobbyId));
            }
            else
            {
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.LobbyId));
            }
        }

        if (_game.ShouldClose() || playerRemoveResult == LobbyResult.Error)
        {
            _logger.Info($"force closing game {_game.Id} due to a lack of players");
            _game.Close();
        }
    }

    public async Task FinishGame()
    {
        // kick all disconnected players
        var disconnectedPlayers = _game.Lobby.GetPlayers()
            .Where(p => p != null && p.State == ZRPPlayerState.Disconnected);

        foreach (var player in disconnectedPlayers)
        {
            _game.Lobby.RemovePlayer(player.LobbyId);
            if (player.Role == ZRPRole.Spectator)
            {
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.LobbyId));
            }
            else
            {
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.LobbyId));
            }
        }
    }
}
