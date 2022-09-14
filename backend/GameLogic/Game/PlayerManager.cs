using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game;

internal sealed class PlayerManager
{
    private HashSet<long> _preparedPlayers;

    public List<long> Players { get => _preparedPlayers.ToList(); }
    public int PlayerCount { get => PlayerCount; }

    public PlayerManager()
    {
        _preparedPlayers = new HashSet<long>();
    }

    public bool AddPlayer(long id)
    {
        return _preparedPlayers.Add(id);
    }

    public bool RemovePlayer(long id)
    {
        return _preparedPlayers.Remove(id);
    }

    public void Reset()
    {
        _preparedPlayers.Clear();
    }

    public (PlayerCycle, Dictionary<long, int>) ComputeOrder()
    {
        PlayerCycle cycle = new PlayerCycle(Players);
        Dictionary<long, int> order = new Dictionary<long, int>();

        foreach (long player in Players)
        {
            order[player] = cycle.GetOrder(player);
        }

        return (cycle, order);
    }
}
