using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using BackendHelper;
using ZwooBackend.Controllers;
using ZwooBackend.Database;
using ZwooDatabaseClasses;

namespace ZwooBackend;

public static class CookieHelper
{
    public struct CookieData
    {
        public string UserId { get; set; }
        public string SessionId { get; set; }
    }

    public static CookieData ParseCookie(string cookie)
    {
        var cookieData = cookie.Split(",");
        if (cookieData.Length < 2)
        {
            return new CookieData()
            {
                SessionId = "",
                UserId = cookieData[0],
            };
        }
        return new CookieData()
        {
            UserId = cookieData[0],
            SessionId = cookieData[1],
        };
    }

    public static string CreateCookieValue(ulong userId, string sessionId)
    {
        return $"{userId},{sessionId}";
    }
}