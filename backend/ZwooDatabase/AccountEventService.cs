using ZwooDatabase.Dao;
using log4net;

namespace ZwooDatabase;

/// <summary>
/// a service for executing account service related database operations
/// </summary>
public interface IAccountEventService
{
    /// <summary>
    /// create an account event for creating a account
    /// </summary>
    /// <param name="user">the users account</param>
    /// <param name="success">whether it was successful</param>
    public void CreateAttempt(UserDao user, bool success);

    /// <summary>
    /// create an account event for verifying a account
    /// </summary>
    /// <param name="user">the users account</param>
    /// <param name="success">whether it was successful</param>
    public void VerifyAttempt(UserDao user, bool success);

    /// <summary>
    /// create an account event for logging into a account
    /// </summary>
    /// <param name="user">the users account</param>
    /// <param name="success">whether it was successful</param>
    public void LoginAttempt(UserDao user, bool success);

    /// <summary>
    /// create an account event for logging out a account
    /// </summary>
    /// <param name="user">the users account</param>
    /// <param name="success">whether it was successful</param>
    public void LogoutAttempt(UserDao user, bool success);

    /// <summary>
    /// create an account event for deleting a account
    /// </summary>
    /// <param name="user">the users account</param>
    /// <param name="success">whether it was successful</param>
    public void DeleteAttempt(UserDao user, bool success);
}

public class AccountEventService : IAccountEventService
{
    private readonly IDatabase _db;
    private readonly ILog _logger;

    public AccountEventService(IDatabase db, ILog? logger = null)
    {
        _db = db;
        _logger = logger ?? LogManager.GetLogger("AccountEventService");
    }

    public void CreateAttempt(UserDao user, bool success)
    {
        _logger.Debug($"creating create user attempt for {user.Id}");
        _db.AccountEvents.InsertOne(new AccountEventDao("create", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void VerifyAttempt(UserDao user, bool success)
    {
        _logger.Debug($"creating verify attempt for {user.Id}");
        _db.AccountEvents.InsertOne(new AccountEventDao("verify", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void LoginAttempt(UserDao user, bool success)
    {
        _logger.Debug($"creating login attempt for {user.Id}");
        _db.AccountEvents.InsertOne(new AccountEventDao("login", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void LogoutAttempt(UserDao user, bool success)
    {
        _logger.Debug($"creating logout attempt for {user.Id}");
        _db.AccountEvents.InsertOne(new AccountEventDao("logout", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void DeleteAttempt(UserDao user, bool success)
    {
        _logger.Debug($"creating delete account attempt for {user.Id}");
        var u = new AccountEventDao("delete", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds())
        {
            UserData = new DeletedUserDao(user)
        };
        _db.AccountEvents.InsertOne(u);
    }
}