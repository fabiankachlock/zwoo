using System.Runtime;
using Zwoo.GameEngine.Game.State;

namespace Zwoo.GameEngine.Lobby.Features;

/// <summary>
/// an interface defining the outline of an external entity providing access to externally persisted game configs
/// </summary>
public interface IExternalGameProfileProvider
{
    public void SaveConfig(long playerId, string name, GameProfile config);
    public void UpdateConfig(long playerId, string id, GameProfile config);
    public void DeleteConfig(long playerId, string id);
    public IEnumerable<IExternalGameProfile> GetConfigsOfPlayer(long playerId);
}

public interface IExternalGameProfile
{
    public string Id { get; }
    public string Name { get; }
    public GameProfile Settings { get; }
}