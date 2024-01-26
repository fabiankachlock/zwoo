using Zwoo.Database;
using Zwoo.Database.Dao;

namespace Zwoo.Backend;

public static class CookieHelper
{
    public struct CookieData
    {
        public ulong UserId { get; set; }
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
                UserId = Convert.ToUInt64(cookieData[0]),
            };
        }
        return new CookieData()
        {
            UserId = Convert.ToUInt64(cookieData[0]),
            SessionId = cookieData[1],
        };
    }

    public static string CreateCookieValue(ulong userId, string sessionId)
    {
        return $"{userId},{sessionId}";
    }
}