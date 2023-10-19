using System.Runtime;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Lobby.Features;

/// <summary>
/// an interface defining the outline of an external entity providing access to externally persisted game configs
/// </summary>
public interface IExternalGameSettingsProvider
{
    public void SaveConfigForPlayer(long playerId, IExternalGameSettings config);

    public IEnumerable<IExternalGameSettings> GetConfigsOfPlayer(long playerId);
}

public interface IExternalGameSettings
{
    public string Id { get; }

    public Dictionary<string, int> Settings { get; }
}