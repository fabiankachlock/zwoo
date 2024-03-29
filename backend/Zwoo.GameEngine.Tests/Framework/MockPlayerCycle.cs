using Zwoo.GameEngine.Game;
using Zwoo.GameEngine.Game.State;

namespace Zwoo.GameEngine.Tests.Framework;

internal class MockPlayerCycle : IPlayerCycle
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

    public MockPlayerCycle()
    {
        _currentIndex = 0;
        _players = [0];
    }

    public MockPlayerCycle(List<long> players)
    {
        _currentIndex = 0;
        _players = players;
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

    public void RemovePlayer(long id, GameDirection direction)
    {
        if (id == ActivePlayer)
        {
            Next(direction);
        }
        long activePlayer = ActivePlayer;
        _players.Remove(id);
        _currentIndex = _players.IndexOf(activePlayer);
    }

}
