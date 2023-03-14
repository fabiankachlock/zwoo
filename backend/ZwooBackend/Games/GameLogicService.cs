﻿using ZwooBackend.Websockets;
using ZwooDatabaseClasses;
using ZwooGameLogic.ZRP;
using ZwooGameLogic;
using log4net;

namespace ZwooBackend.Games;

public interface IGameLogicService
{
    public bool HasGame(long gameId);

    public ZwooRoom CreateGame(string name, bool isPublic);

    public bool RemoveGame(long gameId);

    public ZwooRoom? GetGame(long gameId);

    public IEnumerable<ZwooRoom> ListAll();

    public IEnumerable<ZwooRoom> FindGames(string searchTerm);

    public void DistributeEvent(long gameId, IIncomingZRPMessage msg);

}

public class GameLogicService : IGameLogicService
{
    // globally used GameLogic instance
    private GameManager _gameManager;
    private IWebSocketManager _wsManager;
    private ILog _logger = LogManager.GetLogger("GamesService");


    public GameLogicService(IWebSocketManager wsManager)
    {
        _wsManager = wsManager;
        _gameManager = new GameManager(wsManager, new Log4NetFactory());
    }

    public bool HasGame(long gameId)
    {
        return _gameManager.GetGame(gameId) != null;
    }

    public ZwooRoom CreateGame(string name, bool isPublic)
    {
        ZwooRoom room = _gameManager.CreateGame(name, isPublic);
        room.Game.OnFinished += (data, gameMeta) =>
        {
            List<PlayerScore> scores = new();
            foreach (KeyValuePair<long, int> score in data.Scores)
            {
                var player = room.GetPlayer(score.Key);
                if (player != null)
                {
                    scores.Add(new PlayerScore(player.Username, score.Value, player.Role == ZRPRole.Bot));
                }
            }

            Globals.ZwooDatabase.SaveGame(scores, gameMeta);

            if (scores.Where(score => !score.IsBot).Count() > 1)
            {
                if (room.GetPlayer(data.Winner)?.Role != ZRPRole.Bot)
                {
                    _logger.Info($"[{gameMeta.Id}] incrementing win of winner {data.Winner}");
                    uint winnerWins = Globals.ZwooDatabase.IncrementWin((ulong)data.Winner);
                }
            }
            else
            {
                _logger.Info($"[{gameMeta.Id}] dont incrementing winner wins because there are no real opponents");
            }

        };
        return room;
    }

    public bool RemoveGame(long gameId)
    {
        return _gameManager.RemoveGame(gameId);
    }


    public ZwooRoom? GetGame(long gameId)
    {
        return _gameManager.GetGame(gameId);
    }

    public IEnumerable<ZwooRoom> ListAll()
    {
        return _gameManager.GetAllGames();
    }

    public IEnumerable<ZwooRoom> FindGames(string searchTerm)
    {
        return _gameManager.FindGames(searchTerm);
    }

    public void DistributeEvent(long gameId, IIncomingZRPMessage msg)
    {
        _gameManager.GetGame(gameId)?.DistributeEvent(msg);
    }

}
