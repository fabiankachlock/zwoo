using ZwooGameLogic.Lobby;

namespace ZwooGameLogic.ZRP.Handlers;

public class LobbyHandler : IMessageHandler
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
            _webSocketManager.SendPlayer(context.Id, ZRPCode.AckKeepAlive, new AckKeepAliveDTO());
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
    private void SpectatorToPlayer(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            LobbyResult result = context.Lobby.ChangeRole(context.UserName, ZRPRole.Player);
            if (result == LobbyResult.Success)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(context.UserName, ZRPRole.Player, 0));
            }
            else if (result == LobbyResult.ErrorLobbyFull)
            {
                _webSocketManager.SendPlayer(context.Id, ZRPCode.LobbyFullError, new ErrorDTO((int)ZRPCode.LobbyFullError, "max amount of players reached"));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void PlayerToSpectator(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            PlayerWantsToSpectateDTO payload = message.DecodePayload<PlayerWantsToSpectateDTO>();
            context.Lobby.ChangeRole(payload.Username, ZRPRole.Spectator);
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(payload.Username, ZRPRole.Spectator, 0));
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void PlayerToHost(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            PromotePlayerToHostDTO payload = message.DecodePayload<PromotePlayerToHostDTO>();
            context.Lobby.ChangeRole(payload.Username, ZRPRole.Host);
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(payload.Username, ZRPRole.Host, 0));
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(context.UserName, ZRPRole.Player, 0));
            _webSocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new HostChangedDTO(payload.Username));
            _webSocketManager.SendPlayer(context.Lobby.ResolvePlayer(payload.Username), ZRPCode.PromotedToHost, new PromotedToHostDTO());
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void KickPlayer(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            KickPlayerDTO payload = message.DecodePayload<KickPlayerDTO>();
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
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftDTO(player.Username));
            }
            else if (player != null)
            {
                _webSocketManager.DisconnectPlayer(player.Id);
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftDTO(player.Username));
            }
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()));
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
                if (context.Game.PlayerCount == 1)
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
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleDTO(newHost, ZRPRole.Host, 0));
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new HostChangedDTO(newHost));
                _webSocketManager.SendPlayer(context.Lobby.ResolvePlayer(newHost), ZRPCode.PromotedToHost, new PromotedToHostDTO());
            }

            if (result != LobbyResult.Success) return;
            if (context.Role == ZRPRole.Spectator)
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftDTO(context.UserName));
            }
            else
            {
                _webSocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftDTO(context.UserName));
            }

        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void GetPlayers(UserContext context, IIncomingZRPMessage message)
    {
        try
        {
            ListPlayersDTO payload = new ListPlayersDTO(context.Lobby.ListAll().Select(p => new ListPlayers_PlayerDTO(p.Username, p.Role, 0)).ToArray());
            _webSocketManager.SendPlayer(context.Id, ZRPCode.ListAllPlayers, payload);
        }
        catch (Exception e)
        {
            _webSocketManager.SendPlayer(context.Id, ZRPCode.GeneralError, new ErrorDTO((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
