using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Game;

internal class PlayerCycle
{
    private int _currentIndex;
    private List<long> _players;

    public long ActivePlayer
    {
        get => _players[_currentIndex];
    }

    public List<long> Order
    {
        get => _players;
    }

    public PlayerCycle(List<long> players)
    {
        _currentIndex = 0;
        _players = players;
        Random random = new Random();

        int n = players.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            long value = players[k];
            players[k] = players[n];
            players[n] = value;
        }
    }

    public long Next()
    {
        _currentIndex = (_currentIndex + 1) % _players.Count;
        return _players[_currentIndex];
    }

    public long Next(GameDirection direction)
    {
        if (direction == GameDirection.Left)
        {
            return Next();
        }
        return Previous();
    }

    public long Previous()
    {
        _currentIndex = (_players.Count + _currentIndex - 1) % _players.Count;
        return _players[_currentIndex];
    }

    public int GetOrder(long playerId)
    {
        return _players.FindIndex(id => playerId == id);
    }

    public void RemovePlayer(long id)
    {
        int playerIndex = _players.IndexOf(id);
        if (_players.Remove(id) && (_currentIndex > playerIndex || _currentIndex >= _players.Count()))
        {
            _currentIndex--;
        }
    }

}
