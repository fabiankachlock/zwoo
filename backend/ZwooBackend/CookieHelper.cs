using ZwooDatabase;
using ZwooDatabase.Dao;

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

    public static (UserDao?, string?) GetUser(IUserService serviceInstance, string? cookie)
    {
        var data = CookieHelper.ParseCookie(cookie ?? "");
        if (data.UserId == "")
        {
            return (null, null);
        }

        return (serviceInstance.GetUserById(Convert.ToUInt64(data.UserId)), data.SessionId);
    }
}