﻿using ZwooBackend.ZRP;
using ZwooGameLogic.Game.Settings;

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
        _preparedPlayers.Add(new PlayerEntry(hostId, host, ZRPRole.Host));
        _password = password;
        _usePassword = usePassword;
        return LobbyResult.Success;
    }

    public LobbyResult AddPlayer(long id, string name, ZRPRole role, string password)
    {
        if (_usePassword && password != _password) return LobbyResult.ErrorWrongPassword;
        if (HasPlayer(name) || role == ZRPRole.Host) return LobbyResult.ErrorAlredyInGame;
        if (PlayerCount() >= _settings.MaxAmountOfPlayers && role != ZRPRole.Spectator) return LobbyResult.ErrorLobbyFull;

        _preparedPlayers.Add(new PlayerEntry(id, name, role));
        return LobbyResult.Success;
    }

    public LobbyResult PlayerConnected(long id)
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
            return LobbyResult.Error;
        }
    }

    public LobbyResult RemovePlayer(string name)
    {
        PlayerEntry? player = GetPlayer(name);
        if (player == null) return LobbyResult.ErrorInvalidPlayer;
        if (player.Role == ZRPRole.Host && PlayersNames().Count > 1)
        {
            PlayerEntry newHost = _players.First(p => p.Role == ZRPRole.Player);
            newHost.Role = ZRPRole.Host;
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

    public List<PlayerEntry> ListAll() => _players.ToList();
}
