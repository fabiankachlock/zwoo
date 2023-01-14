using ZwooGameLogic.Lobby;
using ZwooGameLogic.Notifications;


namespace ZwooGameLogic.ZRP.Handlers;

public class LobbyHandler : IEventHandler
{
    private INotificationAdapter _webSocketManager;

    public LobbyHandler(INotificationAdapter websocketManager)
    {
        _webSocketManager = websocketManager;
    }

    public bool HandleMessage(UserContext context, IIncomingZRPMessage message)
    {
        if (message.Code == ZRPCode.KeepAlive)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.AckKeepAlive, new AckKeepAliveNotification());
            return true;
        }
        else if (message.Code == ZRPCode.PlayerLeaves)
        {
            LeavePlayer(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.SpectatorToPlayer)
        {
            SpectatorToPlayer(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.PlayerToSpectator)
        {
            PlayerToSpectator(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.PlayerToHost)
        {
            PlayerToHost(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.KickPlayer)
        {
            KickPlayer(context, message);
            return true;
        }
        else if (message.Code == ZRPCode.GetLobby)
        {
            GetPlayers(context, message);
            return true;
        }
        return false;
    }

    // TODO wins
    private void SpectatorToPlayer(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            LobbyResult result = context.Lobby.ChangeRole(context.UserName, ZRPRole.Player);
            if (result == LobbyResult.Success)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(context.UserName, ZRPRole.Player));
            }
            else if (result == LobbyResult.ErrorLobbyFull)
            {
                _webSocketManager.SendPlayer(context.Id, ZRPCode.LobbyFullError, new LobbyFullError((int)ZRPCode.LobbyFullError, "max amount of players reached"));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void PlayerToSpectator(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            PlayerToSpectatorEvent payload = message.DecodePayload<PlayerToSpectatorEvent>();
            context.Lobby.ChangeRole(payload.Username, ZRPRole.Spectator);
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(payload.Username, ZRPRole.Spectator));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void PlayerToHost(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            PlayerToHostEvent payload = message.DecodePayload<PlayerToHostEvent>();
            context.Lobby.ChangeRole(payload.Username, ZRPRole.Host);
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(payload.Username, ZRPRole.Host));
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(context.UserName, ZRPRole.Player));
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new NewHostNotification(payload.Username));
            _webSocketManager.SendPlayer(context.Lobby.ResolvePlayer(payload.Username), ZRPCode.PromotedToHost, new YouAreHostNotification());
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void KickPlayer(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            KickPlayerEvent payload = message.DecodePayload<KickPlayerEvent>();
            var player = context.Lobby.GetPlayer(payload.Username);

            // mutation of active game
            if (context.Game.IsRunning && player != null)
            {
                context.Game.RemovePlayer(player.Id);
                if (context.Game.PlayerCount == 1)
                {
                    context.Room.Close();
                    return;
                }
            }

            LobbyResult result = context.Lobby.RemovePlayer(payload.Username);
            if (context.Lobby.ActivePlayerCount() == 0)
            {
                context.Room.Close();
                return;
            }

            if (player != null && player.Role == ZRPRole.Spectator)
            {
                _webSocketManager.DisconnectPlayer(player.Id);
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.Username));
            }
            else if (player != null)
            {
                _webSocketManager.DisconnectPlayer(player.Id);
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.Username));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void LeavePlayer(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            // mutation of active game
            if (context.Game.IsRunning)
            {
                context.Game.RemovePlayer(context.Id);
                if (context.Game.PlayerCount <= 1)
                {
                    context.Room.Close();
                    return;
                }
            }

            LobbyResult result = context.Lobby.RemovePlayer(context.UserName);
            if (context.Lobby.ActivePlayerCount() == 0)
            {
                context.Room.Close();
                return;
            }

            if (context.Role == ZRPRole.Host)
            {
                string newHost = context.Lobby.Host();
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(newHost, ZRPRole.Host));
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new NewHostNotification(newHost));
                _webSocketManager.SendPlayer(context.Lobby.ResolvePlayer(newHost), ZRPCode.PromotedToHost, new YouAreHostNotification());
            }

            if (result != LobbyResult.Success) return;
            if (context.Role == ZRPRole.Spectator)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(context.UserName));
            }
            else
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftNotification(context.UserName));
            }

        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void GetPlayers(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            GetLobbyNotification payload = new GetLobbyNotification(context.Lobby.ListAll().Select(p => new GetLobby_PlayerDTO(p.Username, p.Role, p.State)).ToArray());
            _webSocketManager.SendPlayer(context.Id, ZRPCode.SendLobby, payload);
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
