using Zwoo.GameEngine.Lobby;
using Zwoo.GameEngine.Lobby.Features;
using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Events.Handler;

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
            {ZRPCode.GetAllGameProfiles, GetGameProfiles },
            {ZRPCode.SaveToGameProfile, SafeGameProfile },
            {ZRPCode.UpdateGameProfile, UpdateGameProfile },
            {ZRPCode.ApplyGameProfile, ApplyGameProfile },
            {ZRPCode.DeleteGameProfile, DeleteGameProfile },
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

    private void PlayerToSpectator(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        PlayerToSpectatorEvent payload = message.DecodePayload<PlayerToSpectatorEvent>();
        context.Lobby.ChangeRole(payload.Id, ZRPRole.Spectator);
        websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerChangedRole, new PlayerChangedRoleNotification(payload.Id, ZRPRole.Spectator, 0));
    }

    private void PlayerToHost(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
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

    private void KickPlayer(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        KickPlayerEvent payload = message.DecodePayload<KickPlayerEvent>();
        var player = context.Lobby.GetPlayer(payload.Id);
        if (player == null) return;

        // remove player from  active game
        if (context.Game.IsRunning)
        {
            context.Game.RemovePlayer(player.LobbyId);
        }


        websocketManager.DisconnectPlayer(player.LobbyId);
        if (player != null && player.Role == ZRPRole.Spectator)
        {
            websocketManager.BroadcastGame(context.GameId, ZRPCode.SpectatorLeft, new SpectatorLeftNotification(player.LobbyId));
        }
        else if (player != null)
        {
            websocketManager.BroadcastGame(context.GameId, ZRPCode.PlayerLeft, new PlayerLeftNotification(player.LobbyId));
        }

        // remove player from lobby
        LobbyResult result = context.Lobby.RemovePlayer(payload.Id);
        if (context.Room.ShouldClose() || result == LobbyResult.Error)
        {
            // stop game if lobby is empty
            context.Room.Close();
            return;
        }
    }

    private void LeavePlayer(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        // remove player from lobby
        LobbyResult result = context.Lobby.RemovePlayer(context.LobbyId);
        if (context.Room.ShouldClose() || result == LobbyResult.Error)
        {
            // stop game when it has no active players
            context.Room.Close();
            return;
        }

        // remove player from  active game
        if (context.Game.IsRunning)
        {
            context.Game.RemovePlayer(context.LobbyId);
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

    private void GetPlayers(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        var players = context.Lobby.ListAll().Select(p => new GetLobby_PlayerDTO(p.LobbyId, p.Username, p.Role, p.State, 0));
        var bots = context.BotManager.ListBots().Select(b => new GetLobby_PlayerDTO(b.AsPlayer().LobbyId, b.Username, ZRPRole.Bot, ZRPPlayerState.Connected, 0));
        GetLobbyNotification payload = new GetLobbyNotification(players.Concat(bots).ToArray());
        websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendLobby, payload);
    }

    private void GetGameProfiles(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role != ZRPRole.Host) return;
        var profiles = context.Room.GameProfileProvider.GetConfigsOfPlayer(context);
        websocketManager.SendPlayer(context.LobbyId, ZRPCode.SendAllGameProfiles, new AllGameProfilesNotification(
            profiles.Select(p => new AllGameProfiles_ProfileDTO(p.Id, p.Name, p.Group)).ToArray()
        ));
    }

    private void SafeGameProfile(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role != ZRPRole.Host) return;
        SafeToGameProfileEvent payload = message.DecodePayload<SafeToGameProfileEvent>();
        var profile = context.Game.Settings.SaveCurrent();
        context.Room.GameProfileProvider.SaveConfig(context, payload.Name, profile);
    }

    private void UpdateGameProfile(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role != ZRPRole.Host) return;
        UpdateGameProfileEvent payload = message.DecodePayload<UpdateGameProfileEvent>();
        var profile = context.Game.Settings.SaveCurrent();
        context.Room.GameProfileProvider.UpdateConfig(context, payload.Id, profile);
    }

    private void ApplyGameProfile(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role != ZRPRole.Host) return;
        ApplyGameProfileEvent payload = message.DecodePayload<ApplyGameProfileEvent>();
        var profile = context.Room.GameProfileProvider.GetConfigsOfPlayer(context).FirstOrDefault(p => p.Id == payload.Id);
        if (profile != null)
        {
            context.Game.Settings.ApplyProfile(profile.Settings);
            AllSettingsNotification newSettings = new AllSettingsNotification(context.Game.Settings.GetSettings().Select(s => new AllSettings_SettingDTO(s.Key, s.Value, s.Title, s.Description, s.Type, false, s.Min, s.Max)).ToArray());
            websocketManager.BroadcastGame(context.GameId, ZRPCode.SendAllSettings, newSettings);
        }
    }

    private void DeleteGameProfile(UserContext context, IIncomingEvent message, INotificationAdapter websocketManager)
    {
        if (context.Role != ZRPRole.Host) return;
        DeleteGameProfileEvent payload = message.DecodePayload<DeleteGameProfileEvent>();
        context.Room.GameProfileProvider.DeleteConfig(context, payload.Id);
    }
}
