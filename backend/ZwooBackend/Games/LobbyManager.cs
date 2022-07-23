using ZwooBackend.ZRP;

namespace ZwooBackend.Games;


public class LobbyManager
{

    public class PlayerEntry
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
    private List<PlayerEntry> _preparedPlayers;
    private string _password;

    public LobbyManager(long gameId)
    {
        GameId = gameId;
        _players = new List<PlayerEntry>();
        _preparedPlayers = new List<PlayerEntry>();
        _password = "";
    }

    public string Host()
    {
        return _players.FirstOrDefault(p => p.Role == ZRPRole.Host)?.Username ?? "";
    }

    public List<string> Players()
    {
        return _players.FindAll(p => p.Role != ZRPRole.Spectator).Select(p => p.Username).ToList();
    }

    public List<string> Spectators()
    {
        return _players.FindAll(p => p.Role == ZRPRole.Spectator).Select(p => p.Username).ToList();
    }

    public int PlayerCount()
    {
        return Players().Count();
    }

    public bool HasHost()
    {
        return _players.Where(p => p.Role == ZRPRole.Host).Count() == 1;
    }

    public bool HasPlayer(string name)
    {
        return _players.Where(p => p.Username == name).Count() == 1 || _preparedPlayers.Where(p => p.Username == name).Count() == 1;
    }

    public PlayerEntry? GetPlayer(string name)
    {
        try
        {
            return _players.First(p => p.Username == name);
        }
        catch
        {
            return null;
        }
    }

    public PlayerEntry? GetPlayer(long id)
    {
        try
        {
            return _players.First(p => p.Id == id);
        }
        catch
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
        catch
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
        catch
        {
            return 0;
        }
    }

    public bool Initialize(long hostId, string host, string password)
    {
        if (HasHost()) return false;
        _preparedPlayers.Add(new PlayerEntry(hostId, host, ZRPRole.Host));
        _password = password;
        return true;
    }

    public bool AddPlayer(long id, string name, ZRPRole role, string password)
    {
        if (HasPlayer(name) || role == ZRPRole.Host || password != _password) return false;
        _preparedPlayers.Add(new PlayerEntry(id, name, role));
        return true;
    }

    public bool PlayerConnected(long id)
    {
        try
        {
            PlayerEntry preparedPlayer = _preparedPlayers.First(p => p.Id == id);
            _preparedPlayers.Remove(preparedPlayer);
            _players.Add(preparedPlayer);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool RemovePlayer(string name)
    {
        PlayerEntry? player = GetPlayer(name);
        if (player == null) return false;
        if (player.Role == ZRPRole.Host && Players().Count > 1)
        {
            PlayerEntry newHost = _players.First(p => p.Role == ZRPRole.Player);
            newHost.Role = ZRPRole.Host;
        }
        _players.RemoveAll(p => p.Username == name);
        return true;
    }

    public bool RemovePlayer(long id)
    {
        return RemovePlayer(ResolvePlayer(id));
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

    public List<PlayerEntry> ListAll()
    {
        return _players.ToList();
    }

}
