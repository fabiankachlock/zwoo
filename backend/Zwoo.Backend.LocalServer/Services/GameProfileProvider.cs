using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Lobby.Features;

namespace Zwoo.Backend.LocalServer.Services;

/// <summary>
/// This is a local implementation of the game profile provider.
/// Since there is no database, this service is mostly a stub an does not return any game profiles
/// </summary>
public class EmptyGameProfileProvider : IExternalGameProfileProvider
{
    public EmptyGameProfileProvider()
    {
    }

    public IEnumerable<IExternalGameProfile> GetConfigsOfPlayer(long playerId)
    {
        return [];
    }

    public void SaveConfig(long playerId, string name, GameProfile config)
    {
    }

    public void UpdateConfig(long playerId, string id, GameProfile config)
    {
    }

    public void DeleteConfig(long playerId, string id)
    {
    }
}