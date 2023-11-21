namespace Zwoo.GameEngine.Game.State;

/// <summary>
/// A game profile is a partial subset of game settings, that 
/// can be persisted somewhere outside the gamelogic for players
/// to reuse existing game configurations 
/// </summary>
public class GameProfile
{
    /// <summary>
    /// a subset of game settings to be applied
    /// </summary>
    public readonly Dictionary<string, int> Settings;

    public GameProfile(Dictionary<string, int> settings)
    {
        Settings = settings;
    }
}