using ZwooGameLogic.Lobby;
using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Events.Handler;

public class LobbyHandler : IUserEventHandler
{
    public Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>> GetHandles()
    {
        return new Dictionary<ZRPCode, Action<UserContext, IIncomingEvent, INotificationAdapter>>() {
        {ZRPCode.KeepAlive, HandleKeepAlive },
        {ZRPCode.PlayerLeaves, LeavePlayer },
        {ZRPCode.SpectatorToPlayer, SpectatorToPlayer },
        {ZRPCode.PlayerToSpectator, PlayerToSpectator },
        {ZRPCode.PlayerToHost, PlayerToHost },
        {ZRPCode.KickPlayer, KickPlayer },
        {ZRPCode.GetLobby, GetPlayers },
        };
    }

    public void HandleKeepAlive(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        websocketManager.SendPlayer(context.LobbyId, ZRPCode.AckKeepAlive, new AckKeepAliveNotification());
    }

    /// <summary>
    /// handle ZRP 110
    /// </summary>
    private void SpectatorToPlayer(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            LobbyResult result = context.Lobby.ChangeRole(context.LobbyId, ZRPRole.Player);
            if (result == LobbyResult.Success)
            {
                websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(context.LobbyId, ZRPRole.Player, 0));
            }
            else if (result == LobbyResult.ErrorLobbyFull)
            {
                websocketManager.SendPlayer(context.LobbyId, ZRPCode.LobbyFullError, new LobbyFullError((int)ZRPCode.LobbyFullError, "max amount of players reached"));
            }
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void PlayerToSpectator(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            PlayerToSpectatorEvent payload = message.DecodePayload<PlayerToSpectatorEvent>();
            context.Lobby.ChangeRole(payload.Id, ZRPRole.Spectator);
            websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(payload.Id, ZRPRole.Spectator, 0));
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void PlayerToHost(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            PlayerToHostEvent payload = message.DecodePayload<PlayerToHostEvent>();
            var result = context.Lobby.ChangeRole(payload.Id, ZRPRole.Host);
            if (result != LobbyResult.Success)
            {
                websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, result.ToString()));
                return;
            }

            websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(payload.Id, ZRPRole.Host, 0));
            websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(context.LobbyId, ZRPRole.Player, 0));
            websocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new NewHostNotification(payload.Id));
            var newHost = context.Lobby.GetPlayerByUserId(payload.Id);
            if (newHost != null)
            {
                websocketManager.SendPlayer(newHost.LobbyId, ZRPCode.PromotedToHost, new YouAreHostNotification());
            }
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void KickPlayer(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            KickPlayerEvent payload = message.DecodePayload<KickPlayerEvent>();
            var player = context.Lobby.GetPlayerByUserId(payload.Id);

            // remove player from  active game
            if (context.Game.IsRunning && player != null)
            {
                context.Game.RemovePlayer(player.LobbyId);
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
                websocketManager.DisconnectPlayer(player.LobbyId);
                websocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.LobbyId));
            }
            else if (player != null)
            {
                websocketManager.DisconnectPlayer(player.LobbyId);
                websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.LobbyId));
            }
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void LeavePlayer(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            // remove player from  active game
            if (context.Game.IsRunning)
            {
                context.Game.RemovePlayer(context.LobbyId);
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
                    websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(newHost.LobbyId, ZRPRole.Host, 0));
                    websocketManager.BroadcastGame(context.GameId, ZRPCode.HostChanged, new NewHostNotification(newHost.LobbyId));
                    websocketManager.SendPlayer(newHost.LobbyId, ZRPCode.PromotedToHost, new YouAreHostNotification());
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
                websocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(context.LobbyId));
            }
            else
            {
                websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftNotification(context.LobbyId));
            }

        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }

    private void GetPlayers(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        try
        {
            var players = context.Lobby.ListAll().Select(p => new GetLobby_PlayerDTO(p.LobbyId, p.Username, p.Role, p.State, 0));
            var bots = context.BotManager.ListBots().Select(b => new GetLobby_PlayerDTO(b.AsPlayer().LobbyId, b.Username, ZRPRole.Bot, ZRPPlayerState.Connected, 0));
            GetLobbyNotification payload = new GetLobbyNotification(players.Concat(bots).ToArray());
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendLobby, payload);
        }
        catch (Exception e)
        {
            websocketManager.SendPlayer(context.LobbyId, ZRPCode.GeneralError, new Error((int)ZRPCode.GeneralError, e.ToString()));
        }
    }
}
