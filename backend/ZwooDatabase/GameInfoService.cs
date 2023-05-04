using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for executing game info related database operations
/// </summary>
public interface IGameInfoService
{
    public object GetLeaderBoard(string code);

    public ulong GetPosition(UserDao user);

    public void SaveGame(GameInfoDao data);
}