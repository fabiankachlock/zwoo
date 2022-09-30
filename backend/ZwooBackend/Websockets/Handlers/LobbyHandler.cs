using System.Net.WebSockets;
using ZwooBackend.Websockets.Interfaces;
using ZwooBackend.Games;
using ZwooBackend.ZRP;
using ZwooGameLogic.Game;

namespace ZwooBackend.Websockets.Handlers;

public class LobbyHandler : MessageHandler
{
    private SendableWebSocketManager _webSocketManager;

    public LobbyHandler(SendableWebSocketManager websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, ZRPMessage message)
    {
        if (message.Code == ZRPCode.KeepAlive)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.AckKeepAlive, new AckKeepAliveDTO()));
            return true;
        }
        else if (message.Code == ZRPCode.PlayerLeftGame)
        {
            LeavePlayer(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.SpectatorWantsToPlay)
        {
            SpectatorToPlayer(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.PlayerWantsToSpectate)
        {
            PlayerToSpectator(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.PromotePlayerToHost)
        {
            PlayerToHost(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.KickPlayer)
        {
            KickPlayer(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.GetAllPlayers)
        {
            GetPlayers(context, message);
            return true;
        }
        return false;
    }

    // TODO wins
    private void SpectatorToPlayer(UserContext context, ZRPMessage message)
    {
        try
        {
            LobbyResult result = context.GameRecord.Lobby.ChangeRole(context.UserName, ZRPRole.Player);
            if (result == LobbyResult.Success)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(context.UserName, ZRPRole.Player, 0)));
            }
            else if (result == LobbyResult.ErrorLobbyFull)
            {
                _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.LobbyFullError, new ErrorDTO((int)ZRPCode.LobbyFullError, "max amount of players reached")));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int) ZRPCode.GeneralError, e.ToString())));
        }
    }

    private void PlayerToSpectator(UserContext context, ZRPMessage message)
    {
        try
        {
            PlayerWantsToSpectateDTO payload = message.DecodePyload<PlayerWantsToSpectateDTO>();
            context.GameRecord.Lobby.ChangeRole(payload.Username, ZRPRole.Spectator);
            _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(payload.Username, ZRPRole.Spectator, 0)));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString())));
        }
    }

    private void PlayerToHost(UserContext context, ZRPMessage message)
    {
        try
        {
            PromotePlayerToHostDTO payload = message.DecodePyload<PromotePlayerToHostDTO>();
            context.GameRecord.Lobby.ChangeRole(payload.Username, ZRPRole.Host);
            _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(payload.Username, ZRPRole.Host, 0)));
            _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(context.UserName, ZRPRole.Player, 0)));
            _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.HostChanged, new HostChangedDTO(payload.Username)));
            _webSocketManager.SendPlayer(context.GameRecord.Lobby.ResolvePlayer(payload.Username), ZRPEncoder.EncodeToBytes(ZRPCode.PromotedToHost, new PromotedToHostDTO()));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString())));
        }
    }

    private void KickPlayer(UserContext context, ZRPMessage message)
    {
        try
        {
            KickPlayerDTO payload = message.DecodePyload<KickPlayerDTO>();
            var player = context.GameRecord.Lobby.GetPlayer(payload.Username);

            context.GameRecord.Lobby.RemovePlayer(payload.Username);
            if (player != null && player.Role == ZRPRole.Spectator)
            {
                _webSocketManager.Disconnect(player.Id);
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.SpectatorLeft, new SpectatorLeftDTO(player.Username)));
            }
            else if (player != null)
            {
                _webSocketManager.Disconnect(player.Id);
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerLeft, new PlayerLeftDTO(player.Username)));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString())));
        }
    }

    private void LeavePlayer(UserContext context, ZRPMessage message)
    {
        try
        {
            if (context.GameRecord.Game.IsRunning)
            {
                context.GameRecord.Game.RemovePlayer(context.Id);
                if (context.GameRecord.Game.PlayerCount == 1)
                {
                    // close game
                    _webSocketManager.QuitGame(context.GameId);
                    GameManager.Global.RemoveGame(context.GameId);
                    return;
                }
            }

            LobbyResult result = context.GameRecord.Lobby.RemovePlayer(context.UserName);
            if (context.GameRecord.Lobby.PlayerCount() == 0)
            {
                // close game
                _webSocketManager.QuitGame(context.GameId);
                GameManager.Global.RemoveGame(context.GameId);
                return;
            }

            if (context.Role == ZRPRole.Host)
            {
                string newHost = context.GameRecord.Lobby.Host();
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(newHost, ZRPRole.Host, 0)));
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.HostChanged, new HostChangedDTO(newHost)));
                _webSocketManager.SendPlayer(context.GameRecord.Lobby.ResolvePlayer(newHost), ZRPEncoder.EncodeToBytes(ZRPCode.PromotedToHost, new PromotedToHostDTO()));
            }

            if (result != LobbyResult.Success) return;
            if (context.Role == ZRPRole.Spectator)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.SpectatorLeft, new SpectatorLeftDTO(context.UserName)));
            }
            else
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPEncoder.EncodeToBytes(ZRPCode.PlayerLeft, new PlayerLeftDTO(context.UserName)));
            }

        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString())));
        }
    }

    private void GetPlayers(UserContext context, ZRPMessage message)
    {
        try
        {
            ListPlayersDTO payload = new ListPlayersDTO(context.GameRecord.Lobby.ListAll().Select(p => new ListPlayers_PlayerDTO(p.Username, p.Role, 0)).ToArray());
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.ListAllPlayers, payload));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPEncoder.EncodeToBytes(ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString())));
        }
    }
}
