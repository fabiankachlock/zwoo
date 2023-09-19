namespace ZwooGameLogic.Game.Settings;

public interface IGameSettingsStore
{
    public bool Set(string key, int value);
    public int Get(string key);
}
