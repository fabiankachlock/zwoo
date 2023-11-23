using Microsoft.AspNetCore.Http;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Shared.Authentication;

public static class HttpContextExtensions
{
    public static string UserContextKey => "zwoo__user";
    public static string SessionIdContextKey => "zwoo__user_session";

    /// <summary>
    /// retrieve the currently authenticated user from the context
    /// </summary>
    /// <param name="context">the current http context</param>
    /// <returns>an active user session</returns>
    public static ActiveUserSession GetActiveUser(this HttpContext context)
    {

        var user = (UserDao?)context.Items[UserContextKey];
        var session = (string?)context.Items[SessionIdContextKey];

        return new ActiveUserSession(user ?? new(), session ?? "");
    }

    /// <summary>
    /// retrieve the session data of the currently active session from the http context
    /// </summary>
    /// <param name="context">the current http context</param>
    /// <returns>an active user session</returns>
    public static UserSessionData GetActiveSession(this HttpContext context)
    {
        var user = (UserDao?)context.Items[UserContextKey];
        var session = (string?)context.Items[SessionIdContextKey];

        return new UserSessionData(user?.Id ?? 0, session ?? "");
    }
}