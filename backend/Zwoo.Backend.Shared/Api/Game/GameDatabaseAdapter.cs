using Zwoo.Database;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Shared.Api.Game;

public interface IGameDatabaseAdapter
{
    /// <summary>
    /// increment the win counter for a user
    /// </summary>
    /// <param name="userId">the users id</param>
    public void IncrementUserWin(long userId);

    /// <summary>
    /// save a game to the database
    /// </summary>
    /// <param name="gameInfo">the game data</param>
    public void SaveGame(GameInfoDao gameInfo);
}

public class GameDatabaseAdapter : IGameDatabaseAdapter
{
    private readonly IUserService _userService;
    private readonly IGameInfoService _gameInfoService;

    public GameDatabaseAdapter(IUserService userService, IGameInfoService gameInfoService)
    {
        _userService = userService;
        _gameInfoService = gameInfoService;
    }

    public void IncrementUserWin(long userId)
    {
        _userService.IncrementWin((ulong)userId);
    }

    public void SaveGame(GameInfoDao gameInfo)
    {
        _gameInfoService.SaveGame(gameInfo);
    }
}