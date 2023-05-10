namespace ZwooInfoDashBoard.Data;

public interface IAuthService
{
    public string Username { get; }
}

public class AuthService : IAuthService
{
    public string Username
    {
        get => "unknown";
    }
}