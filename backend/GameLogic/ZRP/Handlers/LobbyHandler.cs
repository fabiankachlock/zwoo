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

    /// <summary>
    /// handle ZRP 110
    /// </summary>
    private void SpectatorToPlayer(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            LobbyResult result = context.Lobby.ChangeRole(context.LobbyId, ZRPRole.Player);
            if (result == LobbyResult.Success)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(context.LobbyId, ZRPRole.Player, 0));
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
            context.Lobby.ChangeRole(payload.Id, ZRPRole.Spectator);
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(payload.Id, ZRPRole.Spectator, 0));
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
            var result = context.Lobby.ChangeRole(payload.Id, ZRPRole.Host);
            if (result != LobbyResult.Success)
            {
                _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, result.ToString()));
                return;
            }

            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(payload.Id, ZRPRole.Host, 0));
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(context.LobbyId, ZRPRole.Player, 0));
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new NewHostNotification(payload.Id));
            var newHost = context.Lobby.GetPlayerByUserId(payload.Id);
            if (newHost != null)
            {
                _webSocketManager.SendPlayer(newHost.Id, ZRPCode.PromotedToHost, new YouAreHostNotification());
            }
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
            var player = context.Lobby.GetPlayerByUserId(payload.Id);

            // remove player from  active game
            if (context.Game.IsRunning && player != null)
            {
                context.Game.RemovePlayer(player.Id);
                if (context.Game.PlayerCount == 1)
                {
                    // stop game when it has no active players
                    context.Room.Close();
                    return;
                }
            }

            // remove player from lobby
            LobbyResult result = context.Lobby.RemovePlayer(payload.Id);
            if (context.Lobby.ActivePlayerCount() == 0)
            {
                // stop game if lobby is empty
                context.Room.Close();
                return;
            }

            if (player != null && player.Role == ZRPRole.Spectator)
            {
                _webSocketManager.DisconnectPlayer(player.Id);
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.LobbyId));
            }
            else if (player != null)
            {
                _webSocketManager.DisconnectPlayer(player.Id);
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.LobbyId));
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
            // remove player from  active game
            if (context.Game.IsRunning)
            {
                context.Game.RemovePlayer(context.Id);
                if (context.Game.PlayerCount <= 1)
                {
                    // stop game when it has no active players
                    context.Room.Close();
                    return;
                }
            }

            // remove player from lobby
            LobbyResult result = context.Lobby.RemovePlayer(context.LobbyId);
            if (context.Lobby.ActivePlayerCount() == 0)
            {
                // stop game when it has no active players
                context.Room.Close();
                return;
            }

            if (context.Role == ZRPRole.Host)
            {
                IPlayer? newHost = context.Lobby.GetHost();
                if (newHost != null)
                {
                    _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(newHost.LobbyId, ZRPRole.Host, 0));
                    _webSocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new NewHostNotification(newHost.LobbyId));
                    _webSocketManager.SendPlayer(newHost.Id, ZRPCode.PromotedToHost, new YouAreHostNotification());
                }
                else
                {
                    // a game without host cant exist
                    context.Room.Close();
                    return;
                }
            }

            if (result != LobbyResult.Success) return;
            if (context.Role == ZRPRole.Spectator)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(context.LobbyId));
            }
            else
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftNotification(context.LobbyId));
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
            var players = context.Lobby.ListAll().Select(p => new GetLobby_PlayerDTO(p.LobbyId, p.Username, p.Role, p.State, 0));
            var bots = context.BotManager.ListBots().Select(b => new GetLobby_PlayerDTO(b.AsPlayer().LobbyId, b.Username, ZRPRole.Bot, ZRPPlayerState.Connected, 0));
            GetLobbyNotification payload = new GetLobbyNotification(players.Concat(bots).ToArray());
            _webSocketManager.SendPlayer(context.Id, ZRPCode.SendLobby, payload);
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
