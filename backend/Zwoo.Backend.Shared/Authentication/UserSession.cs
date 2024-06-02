using System.Security.Claims;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Shared.Authentication;

/// <summary>
/// Represents the data stored in a user session
/// </summary>
public class UserSessionData
{
    public static string ClaimsTypeUserId = "uid";
    public static string ClaimsTypeSessionId = "sid";

    public ulong UserId { get; set; }
    public string SessionId { get; set; }

    public UserSessionData(ulong userId, string sessionId)
    {
        UserId = userId;
        SessionId = sessionId;
    }

    /// <summary>
    /// Create a list of Claims with the current session data 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Claim> ToClaims() => new List<Claim> {
        new Claim(ClaimsTypeUserId, UserId.ToString()),
        new Claim(ClaimsTypeSessionId, SessionId),
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
    public static UserSessionData? FromClaims(IEnumerable<Claim> claims)
    {
        var session = new UserSessionData(0, "");
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
            else if (claim.Type == ClaimsTypeSessionId)
            {
                session.SessionId = claim.Value;
            }
        }

        if (session.UserId == 0 || session.SessionId == "") return null;
        return session;
    }
}

/// <summary>
/// Represents the data of an active (verified) session
/// </summary>
public class ActiveUserSession
{
    public UserDao User { get; set; }

    public string ActiveSession { get; set; }

    public ActiveUserSession(UserDao user, string activeSession)
    {
        User = user;
        ActiveSession = activeSession;
    }

    public UserSessionData GetSessionData() => new UserSessionData(User.Id, ActiveSession);

    public string Username => User.Username;
    public string Email => User.Email;
    public uint Wins => User.Wins;
}