using MongoDB.Driver;
using ZwooDatabase.Dao;

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
    private IDatabase _db;

    public AccountEventService(IDatabase db)
    {
        _db = db;
    }

    public void CreateAttempt(UserDao user, bool success)
    {
        _db.AccountEvents.InsertOne(new AccountEventDao("create", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void VerifyAttempt(UserDao user, bool success)
    {
        _db.AccountEvents.InsertOne(new AccountEventDao("verify", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void LoginAttempt(UserDao user, bool success)
    {
        _db.AccountEvents.InsertOne(new AccountEventDao("login", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void LogoutAttempt(UserDao user, bool success)
    {
        _db.AccountEvents.InsertOne(new AccountEventDao("logout", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    }

    public void DeleteAttempt(UserDao user, bool success)
    {
        var u = new AccountEventDao("delete", user.Id, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds())
        {
            UserData = new DeletedUserDao(user)
        };
        _db.AccountEvents.InsertOne(u);
    }
}