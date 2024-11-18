using Zwoo.Api.ZRP;
using Zwoo.GameEngine.Game.Settings;

namespace Zwoo.GameEngine.Lobby;

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

    /// <summary>
    /// list all players of the lobby
    /// </summary>
    /// <returns></returns>
    public List<IPlayer> ListAll() => _players.ToList<IPlayer>();

    /// <summary>
    /// get the current host
    /// </summary>
    public IPlayer? GetHost() => _players.FirstOrDefault(p => p.Role == ZRPRole.Host);

    /// <summary>
    /// get all players
    /// </summary>
    public List<IPlayer> GetPlayers() => _players.FindAll(p => p.Role != ZRPRole.Spectator).ToList<IPlayer>();

    /// <summary>
    /// get all spectators
    /// </summary>
    public List<IPlayer> GetSpectators() => _players.FindAll(p => p.Role == ZRPRole.Spectator).ToList<IPlayer>();

    /// <summary>
    /// get the current count of players
    /// </summary>
    public int PlayerCount() => GetPlayers().Count;

    /// <summary>
    /// get the current amount of active (connected) players
    /// </summary>
    public int ActivePlayerCount() => _players.FindAll(p => p.Role != ZRPRole.Spectator && p.State == ZRPPlayerState.Connected).Count;

    /// <summary>
    /// return whether the lobby has a host
    /// </summary>
    public bool HasHost() => _players.Where(p => p.Role == ZRPRole.Host).Count() == 1;

    /// <summary>
    /// determine whether the lobby has a player with a specifies public id
    /// </summary>
    /// <param name="lobbyId">the players public id</param>
    public bool HasLobbyId(long lobbyId)
    {
        return _players.Where(p => p.LobbyId == lobbyId).Count() == 1 || _preparedPlayers.Where(p => p.LobbyId == lobbyId).Count() == 1;
    }

    /// <summary>
    /// determine whether the lobby has a player with a specifies public id
    /// </summary>
    /// <param name="id">the players real id</param>
    public bool HasPlayerId(long id)
    {
        return _players.Where(p => p.RealId == id).Count() == 1 || _preparedPlayers.Where(p => p.RealId == id).Count() == 1;
    }

    /// <summary>
    /// return a player if present
    /// </summary>
    /// <param name="lobbyId">the players public id</param>
    public LobbyEntry? GetPlayer(long lobbyId)
    {
        return _players.FirstOrDefault(p => p.LobbyId == lobbyId);
    }

    /// <summary>
    /// return a player if present
    /// </summary>
    /// <param name="id">the players real id</param>
    public LobbyEntry? GetPlayerByUserId(long id)
    {
        return _players.FirstOrDefault(p => p.RealId == id);
    }

    /// <summary>
    /// return a prepared player if present
    /// </summary>
    /// <param name="id">the players real id</param>
    public LobbyEntry? GetPossiblyPreparedPlayerByUserId(long id)
    {
        return _preparedPlayers.FirstOrDefault(p => p.RealId == id) ?? GetPlayerByUserId(id);
    }

    /// <summary>
    /// return a player if present
    /// </summary>
    /// <param name="username">the players real id</param>
    public LobbyEntry? GetPlayerByUserName(string username)
    {
        return _players.FirstOrDefault(p => p.Username == username);
    }

    /// <summary>
    /// create a new lobby with a given host
    /// </summary>
    /// <param name="hostId">the hosts user id</param>
    /// <param name="hostName">the hosts username</param>
    /// <param name="password">the games password</param>
    /// <param name="usePassword">whether the game is password protected</param>
    public LobbyResult Initialize(long hostId, string hostName, string password, bool usePassword)
    {
        if (HasHost()) return LobbyResult.ErrorAlreadyInitialized;
        _preparedPlayers.Add(new LobbyEntry(hostId, 1, hostName, ZRPRole.Host, ZRPPlayerState.Disconnected));
        _password = password;
        _usePassword = usePassword;
        return LobbyResult.Success;
    }

    /// <summary>
    /// add a player to the lobby
    /// </summary>
    /// <param name="userId">the players id</param>
    /// <param name="username">the players name</param>
    /// <param name="role">the role in which the player wants to join</param>
    /// <param name="password">the password entered by the user</param>
    public LobbyResult AddPlayer(long userId, long lobbyId, string username, ZRPRole role, string password)
    {
        if (_usePassword && password != _password) return LobbyResult.ErrorWrongPassword; // check whether the password is correct
        if (GetPlayerByUserId(userId)?.State == ZRPPlayerState.Disconnected) return LobbyResult.Success; // handle reconnect after disconnect
        if (HasPlayerId(userId) || role == ZRPRole.Host) return LobbyResult.ErrorAlreadyInGame; // cant join hosts or players which are already in the lobby
        if (PlayerCount() >= _settings.MaxAmountOfPlayers && role != ZRPRole.Spectator) return LobbyResult.ErrorLobbyFull; // check if theres is space in the lobby

        _preparedPlayers.Add(new LobbyEntry(userId, lobbyId, username, role, ZRPPlayerState.Disconnected));
        return LobbyResult.Success;
    }

    /// <summary>
    /// check whether a player is allowed to connect to the game
    /// </summary>
    /// <param name="id">the players id</param>
    public LobbyResult IsPlayerAllowedToConnect(long id)
    {
        // allow reconnects
        if (GetPlayerByUserId(id)?.State == ZRPPlayerState.Disconnected) return LobbyResult.Success;

        // find if the player is prepared
        int playerIndex = _preparedPlayers.FindIndex((p) => p.RealId == id);
        if (playerIndex < 0)
        {
            // if not hes not allowed
            return LobbyResult.Error;
        }
        // else he is allowed to join
        _players.Add(_preparedPlayers[playerIndex]);
        _preparedPlayers.RemoveAt(playerIndex);
        return LobbyResult.Success;
    }

    /// <summary>
    /// set the status of a player as connected
    /// </summary>
    /// <param name="id">the id of the player who connected</param>
    public void MarkPlayerConnected(long id)
    {
        LobbyEntry? player = GetPlayerByUserId(id);
        if (player != null)
        {
            player.State = ZRPPlayerState.Connected;
        }
    }

    /// <summary>
    /// set the status of a player as disconnected
    /// </summary>
    /// <param name="id">the id of the player who disconnected</param>
    public void MarkPlayerDisconnected(long id)
    {
        LobbyEntry? player = GetPlayerByUserId(id);
        if (player != null)
        {
            player.State = ZRPPlayerState.Disconnected;
        }
    }

    /// <summary>
    /// remove a player from the lobby
    /// </summary>
    /// <param name="publicId">the player public id</param>
    public LobbyResult RemovePlayer(long lobbyId)
    {
        LobbyEntry? player = GetPlayer(lobbyId);
        if (player == null) return LobbyResult.ErrorInvalidPlayer; // check if the player is present

        if (player.Role == ZRPRole.Host)
        {
            // if the host gets removed, a new host must be chosen
            LobbyEntry? newHost = SelectNewHost();
            if (newHost == null)
            {
                _players.RemoveAll(p => p.LobbyId == lobbyId); // remove the player
                return LobbyResult.Error;
            }
        }
        _players.RemoveAll(p => p.LobbyId == lobbyId); // remove the player
        return LobbyResult.Success;
    }

    /// <summary>
    /// change the role of a active player
    /// </summary>
    /// <param name="lobbyId">the players public id</param>
    /// <param name="newRole">the role the players wants to assume</param>
    public LobbyResult ChangeRole(long lobbyId, ZRPRole newRole)
    {
        if (newRole == ZRPRole.Host)
        {
            // a new host was elected
            LobbyEntry? currentHost = GetPlayer(GetHost()?.LobbyId ?? 0);
            LobbyEntry? newHost = GetPlayer(lobbyId);
            if (newHost == null || currentHost == null) return LobbyResult.ErrorInvalidPlayer; // check both players exist
            // swap roles
            newHost.Role = ZRPRole.Host;
            currentHost.Role = ZRPRole.Player;
        }
        else
        {
            LobbyEntry? player = GetPlayer(lobbyId);
            if (player == null) return LobbyResult.ErrorInvalidPlayer;
            if (newRole == ZRPRole.Player && PlayerCount() >= _settings.MaxAmountOfPlayers) return LobbyResult.ErrorLobbyFull; // check if the lobby has space for another player
            player.Role = newRole;
        }
        return LobbyResult.Success;
    }

    /// <summary>
    /// select a new host from all active players
    /// </summary>
    /// <returns></returns>
    public LobbyEntry? SelectNewHost()
    {
        LobbyEntry? host = GetPlayer(GetHost()?.LobbyId ?? 0);
        if (host != null && GetPlayers().Count > 1)
        {
            LobbyEntry? newHost = _players.FirstOrDefault(p => p.Role == ZRPRole.Player && p.State == ZRPPlayerState.Connected);
            if (newHost == null) return null;
            newHost.Role = ZRPRole.Host;
            host.Role = ZRPRole.Player;
            return newHost;
        }
        return null;
    }
}
