using ZwooBackend.ZRP;

namespace ZwooBackend.Games;


public class LobbyManager
{

    internal class PlayerEntry
    {
        public long Id;
        public string Username;
        public ZRPRole Role;

        public PlayerEntry(long id, string username, ZRPRole role)
        {
            Id = id;
            Username = username;
            Role = role;
        }
    }


    public readonly long GameId;
    private List<PlayerEntry> _players;

    public LobbyManager(long gameId)
    {
        GameId = gameId;
        _players = new List<PlayerEntry>();
    }

    public string Host()
    {
        return _players.FirstOrDefault(p => p.Role == ZRPRole.Host)?.Username ?? "";
    }

    public List<string> Players()
    {
        return _players.FindAll(p => p.Role == ZRPRole.Player).Select(p => p.Username).ToList();
    }

    public List<string> Spectators()
    {
        return _players.FindAll(p => p.Role == ZRPRole.Spectator).Select(p => p.Username).ToList();
    }

    public bool HasHost()
    {
        return _players.Where(p => p.Role == ZRPRole.Host).Count() == 1;
    }

    public bool HasPlayer(string name)
    {
        return _players.Where(p => p.Username == name).Count() == 1;
    }

    private PlayerEntry? GetPlayer(string name)
    {
        try
        {
            return _players.First(p => p.Username == name);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public string ResolvePlayer(long id)
    {
        try
        {
            return _players.First(p => p.Id == id).Username;
        }
        catch (Exception e)
        {
            return "unknown player";
        }
    }

    public long ResolvePlayer(string name)
    {
        try
        {
            return _players.First(p => p.Username == name).Id;
        }
        catch (Exception e)
        {
            return 0;
        }
    }

    public bool Initialize(long hostId, string host)
    {
        if (HasHost()) return false;
        _players.Add(new PlayerEntry(hostId, host, ZRPRole.Host));
        return true;
    }

    public bool AddPlayer(long id, string name, ZRPRole role)
    {
        if (HasPlayer(name) && role == ZRPRole.Host) return false;
        _players.Add(new PlayerEntry(id, name, role));
        return true;
    }

    public bool RemovePlayer(string name)
    {
        PlayerEntry? player = GetPlayer(name);
        if (player == null) return false;
        if (player.Role == ZRPRole.Host && Players().Count > 0)
        {
            PlayerEntry newHost = _players.First(p => p.Role == ZRPRole.Player);
            newHost.Role = ZRPRole.Host;
        }
        _players.RemoveAll(p => p.Username == name);
        return true;
    }

    public bool ChangeRole(string name, ZRPRole newRole)
    {
        if (newRole == ZRPRole.Host)
        {
            PlayerEntry? currentHost = GetPlayer(Host());
            PlayerEntry? newHost = GetPlayer(name);
            if (newHost == null || currentHost == null) return false;
            newHost.Role = ZRPRole.Host;
            currentHost.Role = ZRPRole.Player;
        }
        else
        {
            PlayerEntry? player = GetPlayer(name);
            if (player == null) return false;
            player.Role = newRole;
        }
        return true;
    }

}
