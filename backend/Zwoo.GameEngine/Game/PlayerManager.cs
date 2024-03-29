using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zwoo.GameEngine.Game;

internal sealed class PlayerManager
{
    private HashSet<long> _preparedPlayers;
    private Action<long> _onPlayerLeaveHandler;

    public List<long> Players { get => _preparedPlayers.ToList(); }
    public int PlayerCount { get => _preparedPlayers.Count(); }

    public PlayerManager()
    {
        _preparedPlayers = new HashSet<long>();
        _onPlayerLeaveHandler = (long id) => { };
    }

    public bool AddPlayer(long id)
    {
        return _preparedPlayers.Add(id);
    }

    public bool RemovePlayer(long id)
    {
        if (_preparedPlayers.Contains(id))
        {
            _onPlayerLeaveHandler(id);
        }
        return _preparedPlayers.Remove(id);
    }

    public bool HasPlayer(long id)
    {
        return _preparedPlayers.Contains(id);
    }

    public void Reset()
    {
        _preparedPlayers.Clear();
    }

    public void OnPlayerLeave(Action<long> handler)
    {
        _onPlayerLeaveHandler = handler;
    }

    public (IPlayerCycle, Dictionary<long, int>) ComputeOrder()
    {
        var cycle = new PlayerCycle(Players);
        var order = new Dictionary<long, int>();

        foreach (long player in Players)
        {
            order[player] = cycle.GetOrder(player);
        }

        return (cycle, order);
    }
}
