using Microsoft.AspNetCore.Http;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Shared.Authentication;

public static class HttpContextExtensions
{
    public static string UserContextKey => "zwoo__user";
    public static string SessionIdContextKey => "zwoo__user_session";

    public static ActiveUserSession GetActiveUser(this HttpContext context)
    {
        var user = (UserDao)context.Items[UserContextKey]!;
        var session = (string)context.Items[SessionIdContextKey]!;

        return new ActiveUserSession(user, session);
    }

    public static UserSessionData GetActiveSession(this HttpContext context)
    {
        var user = (UserDao)context.Items[UserContextKey]!;
        var session = (string)context.Items[SessionIdContextKey]!;

        return new UserSessionData(user.Id, session);
    }
}