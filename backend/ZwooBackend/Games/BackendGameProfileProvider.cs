using ZwooDatabase;
using ZwooDatabase.Dao;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Lobby.Features;

namespace ZwooBackend.Games;

public class BackendGameProfile : IExternalGameProfile
{
    public string Id { get; set; }

    public string Name { get; set; }

    public GameProfile Settings { get; set; }

    public BackendGameProfile(string id, string name, GameProfile settings)
    {
        Id = id;
        Name = name;
        Settings = settings;
    }
}

public class BackendGameProfileProvider : IExternalGameProfileProvider
{
    private readonly IUserService _userService;

    public BackendGameProfileProvider(IUserService userService)
    {
        _userService = userService;
    }

    public IEnumerable<IExternalGameProfile> GetConfigsOfPlayer(long playerId)
    {
        var user = _userService.GetUserById((ulong)playerId);
        if (user == null) return new List<BackendGameProfile>();
        return user.GameProfiles.Select(p => new BackendGameProfile(p.Id.ToString(), p.Name, new GameProfile(new Dictionary<string, int>(p.Settings))));
    }

    public void SaveConfig(long playerId, string name, GameProfile config)
    {
        var user = _userService.GetUserById((ulong)playerId);
        if (user == null) return;
        user.GameProfiles.Add(new UserGameProfileDao()
        {
            Name = name,
            Settings = config.Settings
        });
        _userService.UpdateUser(user, AuditOptions.WithMessage("saved new game profile"));
    }

    public void UpdateConfig(long playerId, string id, GameProfile config)
    {
        var user = _userService.GetUserById((ulong)playerId);
        if (user == null) return;
        var profile = user.GameProfiles.FirstOrDefault(p => p.Id.ToString() == id);
        if (profile == null) return;
        profile.Settings = config.Settings;
        _userService.UpdateUser(user, AuditOptions.WithMessage("updated game profile"));
    }
}