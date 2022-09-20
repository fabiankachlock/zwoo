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
    public static bool CheckUserCookie( string? cookie, out User user )
    {
        user = new User();
        if (cookie == null)
            return false;
        return Globals.ZwooDatabase.GetUser(cookie, out user);
    }
}