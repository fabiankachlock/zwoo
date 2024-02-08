using Microsoft.AspNetCore.Http;
using Zwoo.Backend.LocalServer.Authentication;
using Zwoo.Backend.LocalServer.Services;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Shared.Authentication;

public static class HttpContextExtensions
{
    public static string UserContextKey => "zwoo__guest";

    /// <summary>
    /// store the currently authenticated guest in the context
    /// </summary>
    /// <param name="context">the current http context</param>
    /// <param name="user">the currently authenticated guest</param>
    public static void StoreGuestSession(this HttpContext context, Guest user)
    {
        context.Items.Add(UserContextKey, user);
    }

    /// <summary>
    /// retrieve the currently authenticated guest from the context
    /// </summary>
    /// <param name="context">the current http context</param>
    /// <returns>an active guest session</returns>
    public static ActiveGuestSession GetActiveGuest(this HttpContext context)
    {

        var user = (Guest?)context.Items[UserContextKey];

        return new ActiveGuestSession(user ?? new() { Name = "unknown" });
    }

    /// <summary>
    /// retrieve the session data of the currently active session from the http context
    /// </summary>
    /// <param name="context">the current http context</param>
    /// <returns>an active user session</returns>
    public static GuestSessionData GetActiveGuestSession(this HttpContext context)
    {
        var user = (Guest?)context.Items[UserContextKey];

        return new GuestSessionData(user?.Id ?? 0);
    }
}