﻿using ZwooGameLogic.ZRP;
using log4net;
using ZwooGameLogic.Notifications;


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

        _game.Game.OnFinished += async (data, meta) => await this.FinishGame();
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
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorJoined, new SpectatorJoinedNotification(player.Username));
        }
        else
        {
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerReconnected, new PlayerReconnectedNotification(player.Username));
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerJoined, new PlayerJoinedNotification(player.Username));
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
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerDisconnected, new PlayerDisconnectedNotification(player.Username));
            _game.Lobby.PlayerDisconnected(playerId);

            if (player.Role == ZRPRole.Host)
            {
                var newHost = _game.Lobby.SelectNewHost();
                if (newHost != null)
                {
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(newHost.Username, ZRPRole.Host));
                    await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.HostChanged, new NewHostNotification(newHost.Username));
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
            playerRemoveResult = _game.Lobby.RemovePlayer(playerId);

            // only send leave message when the player leaves (NOT disconnects)
            if (player.Role == ZRPRole.Spectator)
            {
                // TODO: change player model to include wins
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.Username));
            }
            else
            {
                await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.Username));
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
            .Where(p => p != null && p.State == ZRPPlayerState.Disconnected);

        foreach (var player in disconnectedPlayers)
        {
            // make them a spectator
            var result = _game.Lobby.ChangeRole(player.Username, ZRPRole.Spectator);
            await _webSocketManager.BroadcastGame(_game.Id, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(player.Username, ZRPRole.Spectator));
        }

        // TODO: what was the intention on this??? this makes it impossible for player to rejoin after the game finished
        // TODO: rethink disconnected state - spectators should be excluded
        // TODO: this whole state & role model should be documented
        // game.Lobby.ResetDisconnectedStates();
    }
}
