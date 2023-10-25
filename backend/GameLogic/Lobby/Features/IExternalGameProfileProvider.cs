using System.Runtime;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Lobby.Features;

/// <summary>
/// an interface defining the outline of an external entity providing access to externally persisted game configs
/// </summary>
public interface IExternalGameProfileProvider
{
    public void SaveConfig(long playerId, string name, GameProfile config);
    public void UpdateConfig(long playerId, string id, GameProfile config);
    public IEnumerable<IExternalGameProfile> GetConfigsOfPlayer(long playerId);
}

public interface IExternalGameProfile
{
    public string Id { get; }
    public string Name { get; }
    public GameProfile Settings { get; }
}