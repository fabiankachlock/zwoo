using ZwooDatabase.Dao;

namespace ZwooDatabase;


public class UserLoginResult
{
    public UserDao? User { get; set; }

    public string? SessionId { get; set; }

    public ErrorCode? Error { get; set; }

    public UserLoginResult(UserDao? user, string? sessionId, ErrorCode? error)
    {
        User = user;
        SessionId = sessionId;
        Error = error;
    }
}

