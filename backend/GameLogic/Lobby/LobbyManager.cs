using ZwooGameLogic.ZRP;
using ZwooGameLogic.Game.Settings;

namespace ZwooGameLogic.Lobby;

public enum PlayerState
{
    Connected,
    Disconnected
}


public class LobbyManager
{

    public class PlayerEntry
    {
        public long Id;
        public string Username;
        public ZRPRole Role;
        public PlayerState State;

        public PlayerEntry(long id, string username, ZRPRole role, PlayerState state)
        {
            Id = id;
            Username = username;
            Role = role;
            State = state;
        }
    }


    public readonly long GameId;
    private List<PlayerEntry> _players;
    private List<PlayerEntry> _preparedPlayers;
    private string _password;
    private bool _usePassword = false;
    private GameSettings _settings;

    public LobbyManager(long gameId, GameSettings settings)
    {
        GameId = gameId;
        _players = new List<PlayerEntry>();
        _preparedPlayers = new List<PlayerEntry>();
        _password = "";
        _settings = settings;
    }

    public string Host() => _players.FirstOrDefault(p => p.Role == ZRPRole.Host)?.Username ?? "";

    public List<string> PlayersNames() => _players.FindAll(p => p.Role != ZRPRole.Spectator).Select(p => p.Username).ToList();

    public List<string> SpectatorsNames() => _players.FindAll(p => p.Role == ZRPRole.Spectator).Select(p => p.Username).ToList();

    public List<long> Players() => _players.FindAll(p => p.Role != ZRPRole.Spectator).Select(p => p.Id).ToList();

    public List<long> Spectators() => _players.FindAll(p => p.Role == ZRPRole.Spectator).Select(p => p.Id).ToList();


    public int PlayerCount() => PlayersNames().Count();

    public int ActivePlayerCount() => _players.FindAll(p => p.Role != ZRPRole.Spectator && p.State == PlayerState.Connected).Select(p => p.Username).Count();

    public bool HasHost() => _players.Where(p => p.Role == ZRPRole.Host).Count() == 1;

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

    public LobbyResult Initialize(long hostId, string host, string password, bool usePassword)
    {
        if (HasHost()) return LobbyResult.ErrorAlredyInitialized;
        _preparedPlayers.Add(new PlayerEntry(hostId, host, ZRPRole.Host, PlayerState.Disconnected));
        _password = password;
        _usePassword = usePassword;
        return LobbyResult.Success;
    }

    public LobbyResult AddPlayer(long id, string name, ZRPRole role, string password)
    {
        if (_usePassword && password != _password) return LobbyResult.ErrorWrongPassword;
        if (GetPlayer(id)?.State == PlayerState.Disconnected) return LobbyResult.Success; // handle reconnect after disconnect
        if (HasPlayer(name) || role == ZRPRole.Host) return LobbyResult.ErrorAlredyInGame;
        if (PlayerCount() >= _settings.MaxAmountOfPlayers && role != ZRPRole.Spectator) return LobbyResult.ErrorLobbyFull;

        _preparedPlayers.Add(new PlayerEntry(id, name, role, PlayerState.Disconnected));
        return LobbyResult.Success;
    }

    public LobbyResult PlayerWantsToConnect(long id)
    {
        try
        {
            PlayerEntry preparedPlayer = _preparedPlayers.First(p => p.Id == id);
            _preparedPlayers.Remove(preparedPlayer);
            _players.Add(preparedPlayer);
            return LobbyResult.Success;
        }
        catch
        {
            if (GetPlayer(id)?.State == PlayerState.Disconnected)
            {
                return LobbyResult.Success;
            }
            return LobbyResult.Error;
        }
    }

    public void PlayerConnected(long id)
    {
        PlayerEntry? player = GetPlayer(id);
        if (player != null)
        {
            player.State = PlayerState.Connected;
        }
    }

    public void PlayerDisconnected(long id)
    {
        PlayerEntry? player = GetPlayer(id);
        if (player != null)
        {
            player.State = PlayerState.Disconnected;
        }
    }

    public LobbyResult RemovePlayer(string name)
    {
        PlayerEntry? player = GetPlayer(name);
        if (player == null) return LobbyResult.ErrorInvalidPlayer;
        if (player.Role == ZRPRole.Host)
        {
            PlayerEntry? newHost = SelectNewHost();
            _players.RemoveAll(p => p.Username == name);
            if (newHost == null) return LobbyResult.Error;
        }
        _players.RemoveAll(p => p.Username == name);
        return LobbyResult.Success;
    }

    public LobbyResult RemovePlayer(long id) => RemovePlayer(ResolvePlayer(id));

    public LobbyResult ChangeRole(string name, ZRPRole newRole)
    {
        if (newRole == ZRPRole.Host)
        {
            PlayerEntry? currentHost = GetPlayer(Host());
            PlayerEntry? newHost = GetPlayer(name);
            if (newHost == null || currentHost == null) return LobbyResult.ErrorInvalidPlayer;
            newHost.Role = ZRPRole.Host;
            currentHost.Role = ZRPRole.Player;
        }
        else
        {
            PlayerEntry? player = GetPlayer(name);
            if (player == null) return LobbyResult.ErrorInvalidPlayer;
            if (newRole == ZRPRole.Player && PlayerCount() >= _settings.MaxAmountOfPlayers) return LobbyResult.ErrorLobbyFull;
            player.Role = newRole;
        }
        return LobbyResult.Success;
    }

    public PlayerEntry? SelectNewHost()
    {
        PlayerEntry host = GetPlayer(Host())!;
        if (PlayersNames().Count > 1)
        {
            PlayerEntry? newHost = _players.FirstOrDefault(p => p.Role == ZRPRole.Player && p.State == PlayerState.Connected);
            if (newHost == null) return null;
            newHost.Role = ZRPRole.Host;
            host.Role = ZRPRole.Player;
            return newHost;
        }
        return null;
    }

    public List<PlayerEntry> ListAll() => _players.ToList();

    public void ResetDisconnectedStates()
    {
        foreach (PlayerEntry player in _players)
        {
            player.State = PlayerState.Connected;
        }
    }
}
