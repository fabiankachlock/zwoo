using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Database;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.LocalServer.Services;

public class Guest
{
    public ulong Id { get; set; }
    public required string Name { get; set; }
}

public class GuestLoginResult
{
    public Guest? User { get; set; }

    public ApiError? Error { get; set; }

    public GuestLoginResult(Guest? user, ApiError? error)
    {
        User = user;
        Error = error;
    }
}

public interface ILocalUserManager
{
    /// <summary>
    /// return a guest user by id
    /// </summary>
    /// <param name="id">the id of the guest</param>
    /// <returns>the guest</returns>
    public Guest? GetGuestById(ulong id);

    /// <summary>
    /// check if a guest session is valid
    /// </summary>
    /// <param name="id">the id of the guest</param>
    /// <returns>true if the guest is logged in, false otherwise</returns>
    public GuestLoginResult IsGuestLoggedIn(ulong id);

    /// <summary>
    /// create a new guest user
    /// </summary>
    /// <param name="name">the name of the guest</param>
    /// <returns>the freshly created guest</returns>
    public GuestLoginResult CreateGuest(string name);

    /// <summary>
    /// delete a guest user
    /// </summary>
    /// <param name="id">the id of the guest</param>
    /// <returns>true if the guest was deleted, false otherwise</returns>
    public bool DeleteGuest(ulong id);
}

public class LocalUserManager : ILocalUserManager, IDisposable
{
    private Dictionary<ulong, Guest> _guests = new();
    private ulong _nextGuestId = 1;
    private readonly ILogger<LocalUserManager> _logger;

    public LocalUserManager(ILogger<LocalUserManager> logger)
    {
        _logger = logger;
    }

    public Guest? GetGuestById(ulong id)
    {
        if (_guests.ContainsKey(id))
        {
            return _guests[id];
        }

        return null;
    }

    public GuestLoginResult IsGuestLoggedIn(ulong id)
    {
        if (_guests.ContainsKey(id))
        {
            return new GuestLoginResult(_guests[id], null);
        }

        return new GuestLoginResult(null, ApiError.UserNotFound);
    }

    public GuestLoginResult CreateGuest(string name)
    {
        _logger.LogInformation("creating guest {name}", name);
        var guest = new Guest()
        {
            Id = ++_nextGuestId,
            Name = name
        };

        // check if a guest with this name already exists
        foreach (var existingGuest in _guests.Values)
        {
            if (existingGuest.Name == name)
            {
                return new GuestLoginResult(null, ApiError.UsernameTaken);
            }
        }

        _guests.Add(guest.Id, guest);
        return new GuestLoginResult(guest, null);
    }

    public bool DeleteGuest(ulong id)
    {
        if (_guests.ContainsKey(id))
        {
            _guests.Remove(id);
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        _guests.Clear();
    }
}