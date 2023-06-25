namespace ZwooDatabase;

public static class AuditActor
{
    public static string System = "@system";

    public static string Staff(string username) => "@staff/" + username;

    public static string User(ulong id, string sessionId) => "@user/" + id.ToString() + "[" + sessionId + "]";
}