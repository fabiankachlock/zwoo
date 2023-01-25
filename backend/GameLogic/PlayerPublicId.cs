using ZwooGameLogic.ZRP;

namespace ZwooGameLogic;

public class PlayerPublicId
{

    private static string _prefixForRole(ZRPRole role)
    {
        switch (role)
        {
            case ZRPRole.Host:
            case ZRPRole.Spectator:
            case ZRPRole.Player: return "p";
            case ZRPRole.Bot: return "b";
        }
        return "x";
    }

    static public string ForUser(string username, ZRPRole role)
    {
        return $"{_prefixForRole(role)}_{username}";
    }
}