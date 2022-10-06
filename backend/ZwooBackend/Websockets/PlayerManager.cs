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
            // player still in lobby --> disconnected
            WebSocketLogger.Info($"{playerId} disconnected");
            await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerDisconnected, new PlayerReconnectedDTO(player.Username)));
            game?.Lobby.PlayerDisconnected(playerId);

            // only send leave message when the player disconnects
            if (player.Role == ZRPRole.Spectator)
            {
                // TODO: change player model to include wins
                await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.SpectatorLeft, new SpectatorLeftDTO(player.Username)));
            }
            else
            {
                await _webSocketManager.BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerLeft, new PlayerLeftDTO(player.Username)));
            }

            if (game?.Lobby.PlayerCount() == 0)
            {
                WebSocketLogger.Info($"force closing game {gameId} due to a lack of players");
                GameManager.Global.RemoveGame(game.Game.Id);
            }
        }
    }
}
