using System.Security.Claims;
using Zwoo.Backend.LocalServer.Services;

namespace Zwoo.Backend.LocalServer.Authentication;

/// <summary>
/// Represents the data stored in a user session
/// </summary>
public class GuestSessionData
{
    public static string ClaimsTypeUserId = "uid";
    public ulong UserId { get; set; }

    public GuestSessionData(ulong userId)
    {
        UserId = userId;
    }

    /// <summary>
    /// Create a list of Claims with the current session data 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Claim> ToClaims() => new List<Claim> {
        new Claim(ClaimsTypeUserId, UserId.ToString()),
    };

    /// <summary>
    /// Create a ClaimsPrincipal with the current session data 
    /// </summary>
    /// <param name="scheme">the authentication scheme of the ClaimsIdentity</param>
    /// <returns>the created principal</returns>
    public ClaimsPrincipal ToPrincipal(string? scheme)
    {
        var identity = new ClaimsIdentity(ToClaims(), scheme);
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Extract session data from a list of claims
    /// </summary>
    /// <param name="claims">the list of claims from the identity</param>
    /// <returns>the extracted session data</returns>
    public static GuestSessionData? FromClaims(IEnumerable<Claim> claims)
    {
        var session = new GuestSessionData(0);
        foreach (var claim in claims)
        {
            if (claim.Type == ClaimsTypeUserId)
            {
                try
                {
                    session.UserId = Convert.ToUInt64(claim.Value);
                }
                catch
                {
                    return null;
                }
            }
        }

        if (session.UserId == 0) return null;
        return session;
    }
}

/// <summary>
/// Represents the data of an active (verified) session
/// </summary>
public class ActiveGuestSession
{
    public Guest User { get; set; }

    public ActiveGuestSession(Guest user)
    {
        User = user;
    }

    public GuestSessionData GetSessionData() => new GuestSessionData(User.Id);

    public string Name => User.Name;
}