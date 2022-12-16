﻿using ZwooBackend.ZRP;
using ZwooGameLogic.ZRP;
using ZwooGameLogic;

namespace ZwooBackend.Games;

public interface IGameLogicService
{
    public bool HasGame(long gameId);

    public long CreateGame(string name, bool isPublic);

    public bool RemoveGame(long gameId);

    public ZwooRoom? GetGame(long gameId);

    public IEnumerable<ZwooRoom> ListAll();

    public IEnumerable<ZwooRoom> FindGames(string searchTerm);

    public void DistributeEvent(long gameId, IZRPMessage msg);

}

public class GameLogicService : IGameLogicService
{
    // globally used GameLogic instance
    private GameManager _gameManager;

    public GameLogicService()
    {
        // TODO: update Notification Manager Interface
        // TODO: forward IWebSocketManager
        _gameManager = new GameManager();
    }

    public bool HasGame(long gameId)
    {
        return _gameManager.GetGame(gameId) != null;
    }

    public long CreateGame(string name, bool isPublic)
    {
        ZwooRoom game = _gameManager.CreateGame(name, isPublic);
        return game.Game.Id;
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

    public void DistributeEvent(long gameId, IZRPMessage msg)
    {
        // TODO: needs GameLogic implementation
        throw new NotImplementedException();
    }

}
