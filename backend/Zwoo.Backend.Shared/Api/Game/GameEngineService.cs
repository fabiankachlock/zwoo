using Microsoft.Extensions.Logging;
using Zwoo.Backend.Games;
using Zwoo.Database;
using Zwoo.Database.Dao;
using Zwoo.GameEngine;
using Zwoo.GameEngine.Game;
using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Lobby.Features;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.Backend.Shared.Api.Game;

public interface IGameEngineService
{
    /// <summary>
    /// whether a game with this id exists or not
    /// </summary>
    /// <param name="gameId">the games id</param>
    public bool HasGame(long gameId);

    /// <summary>
    /// return a game with the specified id
    /// </summary>
    /// <param name="id">the id of teh game</param>
    /// <returns>a ZwooRoom instance (null if not found)</returns>
    public ZwooRoom? GetGame(long id);

    /// <summary>
    /// create a new ZwooRoom with given properties
    /// </summary>
    /// <param name="name">teh rooms name</param>
    /// <param name="isPublic">whether the room is public or not</param>
    /// <returns>the created room</returns>
    public ZwooRoom CreateGame(string name, bool isPublic);

    /// <summary>
    /// remove a game with the specifies id
    /// </summary>
    /// <param name="gameId">the id of teh game to remove</param>
    /// <returns>whether an entry was deleted or not</returns>
    public bool RemoveGame(long gameId);

    /// <summary>
    /// send an incoming event to a room
    /// </summary>
    /// <param name="gameId">the id of the game to send to event to</param>
    /// <param name="msg">the event itself</param>
    public void DistributeEvent(long gameId, IIncomingZRPMessage msg);
}

public class GameEngineService : IGameEngineService
{
    private readonly GameManager _gameManager;
    private readonly IUserService _userService;
    private readonly IGameInfoService _gameInfo;
    private ILogger<GameEngineService> _logger;


    public GameEngineService(GameManager gameManager, IGameInfoService gameInfo, IUserService userService, ILoggerFactory loggerFactory)
    {
        _gameManager = gameManager;
        _gameInfo = gameInfo;
        _userService = userService;
        _logger = loggerFactory.CreateLogger<GameEngineService>();
    }

    public bool HasGame(long gameId) => _gameManager.GetGame(gameId) != null;

    public ZwooRoom CreateGame(string name, bool isPublic)
    {
        ZwooRoom room = _gameManager.CreateGame(name, isPublic);
        _logger.LogInformation($"created game {room.Id} ({(isPublic ? "public" : "private")})");

        room.Game.OnFinished += (data, meta) => _handleGameFinished(room, data, meta);
        return room;
    }

    private void _handleGameFinished(ZwooRoom room, GameEvent.PlayerWonEvent data, GameMeta gameMeta)
    {
        _logger.LogDebug($"[Game-{gameMeta.Id}] game finished");
        List<PlayerScoreDao> scores = new();
        foreach (KeyValuePair<long, int> score in data.Scores)
        {
            var player = room.GetPlayer(score.Key);
            if (player != null)
            {
                scores.Add(new PlayerScoreDao(player.Username, score.Value, player.Role == ZRPRole.Bot));
            }
        }

        _gameInfo.SaveGame(new GameInfoDao()
        {
            GameId = gameMeta.Id,
            GameName = gameMeta.Name,
            IsPublic = gameMeta.IsPublic,
            Scores = scores,
        });

        if (scores.Where(score => !score.IsBot).Count() > 1)
        {
            var winner = room.GetPlayer(data.Winner);
            if (winner != null && winner.Role != ZRPRole.Bot)
            {
                _logger.LogDebug($"[Game-{gameMeta.Id}] incrementing win of winner {winner.RealId}");
                _userService.IncrementWin((ulong)winner.RealId);
            }
        }
        else
        {
            _logger.LogDebug($"[Game-{gameMeta.Id}] not incrementing winner wins in lack of real opponents");
        }
    }

    public bool RemoveGame(long gameId) => _gameManager.RemoveGame(gameId);

    public ZwooRoom? GetGame(long gameId) => _gameManager.GetGame(gameId);

    public IEnumerable<ZwooRoom> ListAll() => _gameManager.GetAllGames();

    public IEnumerable<ZwooRoom> FindGames(string searchTerm) => _gameManager.FindGames(searchTerm);

    public void DistributeEvent(long gameId, IIncomingZRPMessage msg)
    {
        _gameManager.GetGame(gameId)?.EventDistributer.DistributeEvent(msg);
    }

}