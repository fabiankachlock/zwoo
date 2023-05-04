using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for executing account service related database operations
/// </summary>
public interface IAccountEventService
{
    public void CreateAttempt(UserDao user, bool success);

    public void VerifyAttempt(UserDao user, bool success);

    public void LoginAttempt(UserDao user, bool success);

    public void LogoutAttempt(UserDao user, bool success);

    public void DeleteAttempt(UserDao user, bool success);
}