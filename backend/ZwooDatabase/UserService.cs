using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for executing user related database operations
/// </summary>
public interface IUserService
{
    public UserDao CreateUser(UserDao data);

    public UserDao UpdateUser(UserDao data);

    public bool DeleteUser(UserDao user, string password);


    public UserDao? GetUserById(string id);

    public UserDao? GetUserByEmail(string email);


    public UserDao? VerifyUser(UserDao user, string code);

    public ErrorAble<UserDao> LoginUser(string email, string password);

    public bool LogoutUser(UserDao user, string sessionId);


    public bool ExistsUsername(string username);

    public bool ExistsEmail(string email);


    public uint IncrementWin(ulong playerId);


    public bool ChangePassword(UserDao user, string oldPassword, string newPassword, string sid);

    public UserDao? RequestChangePassword(string email);
    public UserDao? ResetPassword(string code, string password);
}