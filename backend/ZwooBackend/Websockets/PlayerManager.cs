using Microsoft.AspNetCore;
using ZwooBackend.Games;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.ZRP;
using ZwooGameLogic.Game;
using static ZwooBackend.Globals;

namespace ZwooBackend.Websockets;

public class PlayerManager
{
    private Dictionary<long, HashSet<long>> _games = new Dictionary<long, HashSet<long>>();
    private SendableWebSocketManager _webSocketManager;

    public PlayerManager(SendableWebSocketManager webSocketManager)
    {
        _webSocketManager = webSocketManager;
    }

    public async Task<(LobbyManager.PlayerEntry?, GameRecord?)> ConnectPlayer(long playerId, long gameId)
    {
        GameRecord? game = GameManager.Global.GetGame(gameId);
        var player = game?.Lobby.GetPlayer(playerId);
        if (game == null || player == null)
        {
            return (null, null);
        }

        WebSocketLogger.Info($"[PlayerManager] {playerId} connected");
        game?.Lobby.PlayerConnected(playerId);
        if (player.Role == ZRPRole.Spectator)
        {
            // TODO: change player model to include wins
            await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.SpectatorJoined, new SpectatorJoinedDTO(player.Username, 0)));
        }
        else
        {
            await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerReconnected, new PlayerReconnectedDTO(player.Username)));
            await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerJoined, new PlayerJoinedDTO(player.Username, 0)));
        }
        return (player, game);
    }

    public async Task DisconnectPlayer(long playerId, long gameId)
    {
        GameRecord? game = GameManager.Global.GetGame(gameId);
        var player = game?.Lobby.GetPlayer(playerId);
        if (player == null)
        {
            return;
        }
   
        if (game != null)
        {
            LobbyResult playerRemoveResult = LobbyResult.Success;

            if (game.Game.IsRunning) {
                WebSocketLogger.Info($"[PlayerManager] {playerId} disconnected from running game");
                await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerDisconnected, new PlayerDisconnectedDTO(player.Username)));
                game?.Lobby.PlayerDisconnected(playerId);

                if (player.Role == ZRPRole.Host)
                {
                    var newHost = game!.Lobby.SelectNewHost();
                    if (newHost != null)
                    {
                        await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(newHost.Username, ZRPRole.Host, 0)));
                        await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.HostChanged, new HostChangedDTO(newHost.Username)));
                        await _webSocketManager.SendPlayer(newHost.Id, ZRPEncoder.EncodeToBytes(ZRPCode.PromotedToHost, new PromotedToHostDTO()));
                    } else
                    {
                        playerRemoveResult = LobbyResult.Error;
                    }
                }

            }
            else {
                WebSocketLogger.Info($"[PlayerManager] {playerId} removed from lobby");
                playerRemoveResult = game.Lobby.RemovePlayer(playerId);

                // only send leave message when the player leaves (NOT disconnects)
                if (player.Role == ZRPRole.Spectator)
                {
                    // TODO: change player model to include wins
                    await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.SpectatorLeft, new SpectatorLeftDTO(player.Username)));
                }
                else
                {
                    await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerLeft, new PlayerLeftDTO(player.Username)));
                }
            }


            WebSocketLogger.Info(game.Lobby.ListAll());
            if (game?.Lobby.ActivePlayerCount() == 0 || (game.Game.IsRunning && game?.Lobby.PlayerCount() < 2) || playerRemoveResult == LobbyResult.Error)
            {
                WebSocketLogger.Info($"force closing game {gameId} due to a lack of players");
                await _webSocketManager.QuitGame(gameId);
                GameManager.Global.RemoveGame(gameId);
            }
        }
    }

    public async Task FinishGame(long gameId)
    {
        GameRecord? game = GameManager.Global.GetGame(gameId);
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
            await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(player.Username, ZRPRole.Spectator, 0)));
        }

        game.Lobby.ResetDisconnectedStates();
    }
}
