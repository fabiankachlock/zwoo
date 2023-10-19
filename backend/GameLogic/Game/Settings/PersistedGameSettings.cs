namespace ZwooGameLogic.Game.State;

/// <summary>
/// A game config is a partial subset of game settings, that 
/// can be persisted somewhere outside the gamelogic for players
/// to reuse existing game configurations 
/// </summary>
public class PersistedGameSettings
{
    /// <summary>
    /// an id assigned by the external provider
    /// </summary>
    public readonly string ExternalId;

    /// <summary>
    /// an id assigned by the zwoo room
    /// </summary>
    public readonly long Id;

    /// <summary>
    /// a subset of game settings to be applied
    /// </summary>
    public readonly Dictionary<string, int> Settings;

    public PersistedGameSettings(string externalId, long id, Dictionary<string, int> settings)
    {
        ExternalId = externalId;
        Id = id;
        Settings = settings;
    }
}