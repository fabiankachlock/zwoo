namespace ZwooInfoDashBoard.Data;

public interface IAuthService
{
    public string Username { get; }
}

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Username
    {
        get => _httpContextAccessor.HttpContext?.User.FindFirst("preferred_username")?.Value ?? "unknown";
    }
}