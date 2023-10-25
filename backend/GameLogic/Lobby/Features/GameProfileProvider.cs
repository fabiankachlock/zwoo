using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Lobby.Features;

public class ExternalGameProfile
{
    public string Id { get; set; }

    public string Name { get; set; }

    public GameProfile Settings { get; set; }

    public ExternalGameProfile(string id, string name, GameProfile settings)
    {
        Id = id;
        Name = name;
        Settings = settings;
    }
}

public class GameProfileProvider
{
    private readonly IExternalGameProfileProvider _provider;

    public GameProfileProvider(IExternalGameProfileProvider provider)
    {
        _provider = provider;
    }

    private string _createScopedId(IPlayer player, string profileId) => $"{player.LobbyId}::${profileId}";

    private string _extractRawId(string internalId)
    {
        var parts = internalId.Split("::");
        return parts.Count() > 1 ? parts[1] : "";
    }

    public void SaveConfig(IPlayer player, string name, GameProfile config)
    {
        _provider.SaveConfig(player.RealId, name, config);
    }

    public void UpdateConfig(IPlayer player, string id, GameProfile config)
    {
        var rawId = _extractRawId(id);
        _provider.UpdateConfig(player.RealId, rawId, config);
    }

    public IEnumerable<ExternalGameProfile> GetConfigsOfPlayer(IPlayer player)
    {
        return _provider.GetConfigsOfPlayer(player.RealId)
            .Select(config => new ExternalGameProfile(_createScopedId(player, config.Id), config.Name, config.Settings));
    }
}
