using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game;

internal sealed class PlayerManager
{
    private HashSet<long> PreparedPlayers;

    public List<long> Players { get => PreparedPlayers.ToList(); }
    public int PlayerCount { get => PlayerCount; }

    public PlayerManager()
    {
        PreparedPlayers = new HashSet<long>();
    }

    public bool AddPlayer(long id)
    {
        return PreparedPlayers.Add(id);
    }

    public bool RemovePlayer(long id)
    {
        return PreparedPlayers.Remove(id);
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
