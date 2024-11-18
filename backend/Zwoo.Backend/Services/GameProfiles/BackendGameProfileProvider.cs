using Zwoo.Database;
using Zwoo.Database.Dao;
using Zwoo.GameEngine.Game.State;

namespace Zwoo.Backend.Services.GameProfiles;

public interface IGameProfileProvider
{
    IEnumerable<GameProfile> GetConfigsOfPlayer(UserDao user);
    void SaveConfig(UserDao user, string name, Dictionary<string, int> config);
    void UpdateConfig(UserDao user, string id, string name, Dictionary<string, int> config);
    void DeleteConfig(UserDao user, string id);
}

public class GameProfileProvider : IGameProfileProvider
{
    private readonly IUserService _userService;

    public GameProfileProvider(IUserService userService)
    {
        _userService = userService;
    }

    public IEnumerable<GameProfile> GetConfigsOfPlayer(UserDao user)
    {
        return user.GameProfiles.Select(p => new GameProfile(p.Id.ToString(), p.Name, GameProfileGroup.User,
            new Dictionary<string, int>(p.Settings)));
    }

    public void SaveConfig(UserDao user, string name, Dictionary<string, int> config)
    {
        user.GameProfiles.Add(new UserGameProfileDao()
        {
            Name = name,
            Settings = config
        });
        _userService.UpdateUser(user, AuditOptions.WithMessage("saved new game profile"));
    }

    public void UpdateConfig(UserDao user, string id, string name, Dictionary<string, int> config)
    {
        var profile = user.GameProfiles.FirstOrDefault(p => p.Id.ToString() == id);
        if (profile == null) return;
        profile.Name = name;
        profile.Settings = config;
        _userService.UpdateUser(user, AuditOptions.WithMessage("updated game profile"));
    }

    public void DeleteConfig(UserDao user, string id)
    {
        user.GameProfiles = user.GameProfiles.Where(p => p.Id.ToString() != id).ToList();
        _userService.UpdateUser(user, AuditOptions.WithMessage("updated game profile"));
    }
}