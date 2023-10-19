using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Lobby.Features;

internal class IExternalGameSettingsImpl : IExternalGameSettings
{
    public string Id { get; set; }

    public Dictionary<string, int> Settings { get; set; }

    public IExternalGameSettingsImpl(string id, Dictionary<string, int> settings)
    {
        Id = id;
        Settings = settings;
    }
}

public class PersistedGameSettingsProvider
{
    private readonly IExternalGameSettingsProvider _provider;
    private long _internalId = 0;

    public PersistedGameSettingsProvider(IExternalGameSettingsProvider provider)
    {
        _provider = provider;
    }

    public void SaveConfigForPlayer(IPlayer player, PersistedGameSettings config)
    {
        _provider.SaveConfigForPlayer(player.RealId, new IExternalGameSettingsImpl(config.ExternalId, config.Settings));
    }

    public IEnumerable<PersistedGameSettings> GetConfigsOfPlayer(IPlayer player)
    {
        return _provider.GetConfigsOfPlayer(player.RealId)
            .Select(config => new PersistedGameSettings(config.Id, ++_internalId, config.Settings));
    }
}
