using ZwooGameLogic.ZRP;
using ZwooGameLogic.Game.Settings;

namespace ZwooGameLogic.Lobby;

public class LobbyManager
{
    public readonly long GameId;
    private List<LobbyEntry> _players;
    private List<LobbyEntry> _preparedPlayers;
    private string _password;
    private bool _usePassword = false;
    private GameSettings _settings;

    public LobbyManager(long gameId, GameSettings settings)
    {
        GameId = gameId;
        _players = new List<LobbyEntry>();
        _preparedPlayers = new List<LobbyEntry>();
        _password = "";
        _settings = settings;
    }

    public string Host() => _players.FirstOrDefault(p => p.Role == ZRPRole.Host)?.Username ?? "";

    public List<string> PlayersNames() => _players.FindAll(p => p.Role != ZRPRole.Spectator).Select(p => p.Username).ToList();

    public List<string> SpectatorsNames() => _players.FindAll(p => p.Role == ZRPRole.Spectator).Select(p => p.Username).ToList();

    public List<long> Players() => _players.FindAll(p => p.Role != ZRPRole.Spectator).Select(p => p.Id).ToList();

    public List<long> Spectators() => _players.FindAll(p => p.Role == ZRPRole.Spectator).Select(p => p.Id).ToList();


    public int PlayerCount() => PlayersNames().Count();

    public int ActivePlayerCount() => _players.FindAll(p => p.Role != ZRPRole.Spectator && p.State == ZRPPlayerState.Connected).Select(p => p.Username).Count();

    public bool HasHost() => _players.Where(p => p.Role == ZRPRole.Host).Count() == 1;

    public bool HasPlayer(string name)
    {
        return _players.Where(p => p.Username == name).Count() == 1 || _preparedPlayers.Where(p => p.Username == name).Count() == 1;
    }

    public bool HasPlayer(long id)
    {
        return _players.Where(p => p.Id == id).Count() == 1 || _preparedPlayers.Where(p => p.Id == id).Count() == 1;
    }

    public LobbyEntry? GetPlayer(string name)
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

    public LobbyEntry? GetPlayer(long id)
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
        _preparedPlayers.Add(new LobbyEntry(hostId, PlayerPublicId.ForUser(host, ZRPRole.Host), host, ZRPRole.Host, ZRPPlayerState.Disconnected));
        _password = password;
        _usePassword = usePassword;
        return LobbyResult.Success;
    }

    public LobbyResult AddPlayer(long id, string name, ZRPRole role, string password)
    {
        if (_usePassword && password != _password) return LobbyResult.ErrorWrongPassword;
        if (GetPlayer(id)?.State == ZRPPlayerState.Disconnected) return LobbyResult.Success; // handle reconnect after disconnect
        if (HasPlayer(name) || role == ZRPRole.Host) return LobbyResult.ErrorAlredyInGame;
        if (PlayerCount() >= _settings.MaxAmountOfPlayers && role != ZRPRole.Spectator) return LobbyResult.ErrorLobbyFull;

        _preparedPlayers.Add(new LobbyEntry(id, PlayerPublicId.ForUser(name, role), name, role, ZRPPlayerState.Disconnected));
        return LobbyResult.Success;
    }

    public LobbyResult PlayerWantsToConnect(long id)
    {
        try
        {
            LobbyEntry preparedPlayer = _preparedPlayers.First(p => p.Id == id);
            _preparedPlayers.Remove(preparedPlayer);
            _players.Add(preparedPlayer);
            return LobbyResult.Success;
        }
        catch
        {
            if (GetPlayer(id)?.State == ZRPPlayerState.Disconnected)
            {
                return LobbyResult.Success;
            }
            return LobbyResult.Error;
        }
    }

    public void PlayerConnected(long id)
    {
        LobbyEntry? player = GetPlayer(id);
        if (player != null)
        {
            player.State = ZRPPlayerState.Connected;
        }
    }

    public void PlayerDisconnected(long id)
    {
        LobbyEntry? player = GetPlayer(id);
        if (player != null)
        {
            player.State = ZRPPlayerState.Disconnected;
        }
    }

    public LobbyResult RemovePlayer(string name)
    {
        LobbyEntry? player = GetPlayer(name);
        if (player == null) return LobbyResult.ErrorInvalidPlayer;
        if (player.Role == ZRPRole.Host)
        {
            LobbyEntry? newHost = SelectNewHost();
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
            LobbyEntry? currentHost = GetPlayer(Host());
            LobbyEntry? newHost = GetPlayer(name);
            if (newHost == null || currentHost == null) return LobbyResult.ErrorInvalidPlayer;
            newHost.Role = ZRPRole.Host;
            currentHost.Role = ZRPRole.Player;
        }
        else
        {
            LobbyEntry? player = GetPlayer(name);
            if (player == null) return LobbyResult.ErrorInvalidPlayer;
            if (newRole == ZRPRole.Player && PlayerCount() >= _settings.MaxAmountOfPlayers) return LobbyResult.ErrorLobbyFull;
            player.Role = newRole;
        }
        return LobbyResult.Success;
    }

    public LobbyEntry? SelectNewHost()
    {
        LobbyEntry host = GetPlayer(Host())!;
        if (PlayersNames().Count > 1)
        {
            LobbyEntry? newHost = _players.FirstOrDefault(p => p.Role == ZRPRole.Player && p.State == ZRPPlayerState.Connected);
            if (newHost == null) return null;
            newHost.Role = ZRPRole.Host;
            host.Role = ZRPRole.Player;
            return newHost;
        }
        return null;
    }

    public List<LobbyEntry> ListAll() => _players.ToList();

    public void ResetDisconnectedStates()
    {
        foreach (LobbyEntry player in _players)
        {
            player.State = ZRPPlayerState.Connected;
        }
    }
}
