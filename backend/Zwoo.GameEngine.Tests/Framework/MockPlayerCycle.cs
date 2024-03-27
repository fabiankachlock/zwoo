using Zwoo.GameEngine.Game;
using Zwoo.GameEngine.Game.State;

namespace Zwoo.GameEngine.Tests.Framework;

internal class MockPlayerCycle : PlayerCycle
{
    private int _currentIndex;
    private List<long> _players;

    public new long ActivePlayer
    {
        get => _players[_currentIndex];
    }

    public new List<long> Order
    {
        get => _players;
    }

    public MockPlayerCycle() : base([0])
    {
        _currentIndex = 0;
        _players = [0];
    }

    public MockPlayerCycle(List<long> players) : base(players)
    {
        _currentIndex = 0;
        _players = players;
    }

    public new long Next()
    {
        _currentIndex = (_currentIndex + 1) % _players.Count;
        return _players[_currentIndex];
    }

    public new long Next(GameDirection direction)
    {
        if (direction == GameDirection.Left)
        {
            return Next();
        }
        return Previous();
    }

    public new long Previous()
    {
        _currentIndex = (_players.Count + _currentIndex - 1) % _players.Count;
        return _players[_currentIndex];
    }

    public new int GetOrder(long playerId)
    {
        return _players.FindIndex(id => playerId == id);
    }

    public new void RemovePlayer(long id, GameDirection direction)
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
