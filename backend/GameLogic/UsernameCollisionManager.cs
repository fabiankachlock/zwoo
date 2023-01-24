namespace ZwooGameLogic;

static public class UsernameCollisionManager
{
    static public string EscapeUsername(string username)
    {
        return "u_" + username;
    }

    static public string UnescapeUsername(string username)
    {
        if (username.StartsWith("u_")) return username.Substring(2);
        return username;
    }

    static public string EscapeBotName(string botName)
    {
        return "b_" + botName;
    }

    static public string UnescapeBotName(string botName)
    {
        if (botName.StartsWith("b_")) return botName.Substring(2);
        return botName;
    }
}