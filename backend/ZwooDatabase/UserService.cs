using System.Text;
using System.Security.Cryptography;
using MongoDB.Driver;
using ZwooDatabase.Dao;
using BackendHelper;

namespace ZwooDatabase;

/// <summary>
/// a service for executing user related database operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// create a new user
    /// </summary>
    /// <param name="username">the users name</param>
    /// <param name="email">the users email</param>
    /// <param name="password">the users password</param>
    /// <param name="betaCode">the users beta code</param>
    /// <returns></returns>
    public UserDao CreateUser(string username, string email, string password, string? betaCode, AuditOptions? auditOptions = null);

    /// <summary>
    /// update a users data
    /// </summary>
    /// <param name="data">the users data</param>
    public UserDao? UpdateUser(UserDao data, AuditOptions? auditOptions = null);

    /// <summary>
    /// delete an user
    /// </summary>
    /// <param name="user">the user to delete</param>
    /// <param name="password">teh users password</param>
    public bool DeleteUser(UserDao user, string password, AuditOptions? auditOptions = null);


    /// <summary>
    /// query a user by its id
    /// </summary>
    /// <param name="id">the users id</param>
    public UserDao? GetUserById(ulong id);

    /// <summary>
    /// query a user by email
    /// </summary>
    /// <param name="email">the users email</param>
    public UserDao? GetUserByEmail(string email);


    /// <summary>
    /// verify a new user
    /// </summary>
    /// <param name="id">the id of the user to verify</param>
    /// <param name="code">the provided verification code</param>
    /// <param name="isBeta">whether the beta mode is activated</param>
    public UserDao? VerifyUser(ulong id, string code, bool isBeta, AuditOptions? auditOptions = null);

    /// <summary>
    /// login a user
    /// </summary>
    /// <param name="email">the user email</param>
    /// <param name="password">teh users password</param>
    public UserLoginResult LoginUser(string email, string password, AuditOptions? auditOptions = null);

    /// <summary>
    /// logout a user
    /// </summary>
    /// <param name="user">the users email</param>
    /// <param name="sessionId">teh users current session</param>
    public bool LogoutUser(UserDao user, string sessionId, AuditOptions? auditOptions = null);

    /// <summary>
    /// check whether a user with a given username exists
    /// </summary>
    /// <param name="username">the username to check</param>
    public bool ExistsUsername(string username);

    /// <summary>
    /// check whether a user with a given email exists
    /// </summary>
    /// <param name="email">the email to check</param>
    public bool ExistsEmail(string email);


    /// <summary>
    /// increment the wins of an user by one
    /// </summary>
    /// <param name="playerId">the users id</param>
    /// <returns></returns>
    public uint IncrementWin(ulong playerId, AuditOptions? auditOptions = null);

    /// <summary>
    /// change the password of an user
    /// </summary>
    /// <param name="user">the user to change the password of</param>
    /// <param name="oldPassword">the users old password</param>
    /// <param name="newPassword">the users new password</param>
    /// <param name="sid">the current session od the user</param>
    public bool ChangePassword(UserDao user, string oldPassword, string newPassword, string sid, AuditOptions? auditOptions = null);

    /// <summary>
    /// request a password change of an user
    /// </summary>
    /// <param name="email">the users email address</param>
    public UserDao? RequestChangePassword(string email, AuditOptions? auditOptions = null);

    /// <summary>
    /// reset the password of a user
    /// </summary>
    /// <param name="code">the reset code</param>
    /// <param name="password">the new password</param>
    public UserDao? ResetPassword(string code, string password, AuditOptions? auditOptions = null);
}

public class UserService : IUserService
{

    private IDatabase _db;
    private IAuditTrailService _audits;
    private IAccountEventService _accountEvents;
    private IBetaCodesService _betaCodes;

    public UserService(IDatabase db, IAuditTrailService audits, IAccountEventService accountEvents, IBetaCodesService betaCodes)
    {
        _db = db;
        _audits = audits;
        _accountEvents = accountEvents;
        _betaCodes = betaCodes;
    }

    public UserDao CreateUser(string username, string email, string password, string? betaCode, AuditOptions? auditOptions = null)
    {
        var code = StringHelper.GenerateNDigitString(6);
        ulong id = _db.Users.AsQueryable().Any() ? _db.Users.AsQueryable().Max(x => x.Id) + 1 : 1;
        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(password).Concat(salt).ToArray());

        var user = new UserDao
        {
            Id = id,
            Sid = new(),
            Username = username,
            Email = email,
            Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}",
            Wins = 0,
            ValidationCode = code.Trim().Normalize(),
            Verified = false,
            BetaCode = betaCode
        };

        _db.Users.InsertOne(user);
        _accountEvents.CreateAttempt(user, true);
        _audits.Protocol(_audits.GetAuditId(user), AuditOptions.CreateEvent(auditOptions, new AuditEventDao()
        {
            Actor = AuditActor.System,
            Message = "created an the user account",
            NewValue = user,
        }));
        return user;
    }

    public UserDao? UpdateUser(UserDao data, AuditOptions? auditOptions = null)
    {
        var old = _db.Users.AsQueryable().FirstOrDefault(user => user.Id == data.Id);
        if (old == null)
        {
            return null;
        }

        _db.Users.ReplaceOne(user => user.Id == data.Id, data);
        _audits.Protocol(_audits.GetAuditId(data), AuditOptions.CreateEvent(auditOptions, new AuditEventDao()
        {
            Actor = AuditActor.System,
            Message = "updated the users data",
            OldValue = old,
            NewValue = data,
        }));
        return data;
    }

    public bool DeleteUser(UserDao user, string password, AuditOptions? auditOptions = null)
    {
        if (!StringHelper.CheckPassword(password, user.Password) || !user.Verified)
        {
            _accountEvents.DeleteAttempt(user, false);
            return false;
        }

        _db.Users.DeleteOne(user => user.Id == user.Id);
        _accountEvents.DeleteAttempt(user, true);
        _audits.Protocol(_audits.GetAuditId(user), AuditOptions.CreateEvent(auditOptions, new AuditEventDao()
        {
            Actor = AuditActor.User(user.Id, ""),
            Message = "deleted the users account",
            NewValue = user,
            OldValue = user
        }));
        return true;
    }


    public UserDao? GetUserById(ulong id)
    {
        return _db.Users.AsQueryable().FirstOrDefault(user => user.Id == id);
    }

    public UserDao? GetUserByEmail(string email)
    {
        return _db.Users.AsQueryable().FirstOrDefault(user => user.Email == email);
    }


    public UserDao? VerifyUser(ulong id, string code, bool isBeta, AuditOptions? auditOptions = null)
    {
        var user = _db.Users.AsQueryable().FirstOrDefault(user => user.Id == id && user.ValidationCode == code);
        if (user == null)
        {
            return null;
        }

        if (isBeta && !_betaCodes.RemoveBetaCode(user.BetaCode ?? "#"))
        {
            _accountEvents.VerifyAttempt(user, false);
            return null;
        }

        user.Verified = true;
        user.ValidationCode = "";

        _accountEvents.VerifyAttempt(user, true);
        UpdateUser(user, AuditOptions.WithMessage("verified the user").Merge(auditOptions));
        return user;
    }

    public UserLoginResult LoginUser(string email, string password, AuditOptions? auditOptions = null)
    {
        var user = _db.Users.AsQueryable().FirstOrDefault(user => user.Email == email);
        if (user == null)
        {
            return new UserLoginResult(user, null, ErrorCode.UserNotFound);
        }

        if (StringHelper.CheckPassword(password, user.Password) && user.Verified)
        {
            string sid = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            user.Sid.Add(sid);
            UpdateUser(user, AuditOptions.WithMessage("logged user in").Merge(auditOptions));
            _accountEvents.LoginAttempt(user, true);
            return new UserLoginResult(user, sid, null);
        }

        _accountEvents.LoginAttempt(user, false);
        return new UserLoginResult(user, null, user.Verified ? ErrorCode.WrongPassword : ErrorCode.NotVerified);
    }

    public bool LogoutUser(UserDao user, string sessionId, AuditOptions? auditOptions = null)
    {
        user.Sid.Remove(sessionId);
        var res = UpdateUser(user, new AuditOptions(AuditActor.User(user.Id, sessionId), "logged out user").Merge(auditOptions));
        _accountEvents.LogoutAttempt(user, res != null);
        return res != null;
    }


    public bool ExistsUsername(string username)
    {
        return _db.Users.AsQueryable().FirstOrDefault(user => user.Username == username) != null;
    }

    public bool ExistsEmail(string email)
    {
        return _db.Users.AsQueryable().FirstOrDefault(user => user.Email == email) != null;
    }

    public uint IncrementWin(ulong playerId, AuditOptions? auditOptions = null)
    {
        // TODO: would be great to have acces to the current users session id for audit logging
        // if (_db.Users.UpdateOne(x => x.Id == puid, Builders<User>.Update.Inc(u => u.Wins, (uint)1)).ModifiedCount != 0)
        //     return _userCollection.AsQueryable().First(u => u.Id == puid).Wins;
        // return 0;
        return 0;
    }

    public bool ChangePassword(UserDao user, string oldPassword, string newPassword, string sid, AuditOptions? auditOptions = null)
    {
        if (StringHelper.CheckPassword(oldPassword, user.Password))
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(newPassword).Concat(salt).ToArray());
            user.Sid = new List<string> { sid };
            user.Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}";

            var res = UpdateUser(user, new AuditOptions(AuditActor.User(user.Id, sid), "changed password").Merge(auditOptions));
            return res != null;
        }
        return false;
    }

    public UserDao? RequestChangePassword(string email, AuditOptions? auditOptions = null)
    {
        var user = _db.Users.AsQueryable().First(user => user.Email == email);
        if (user == null)
        {
            return null;
        }

        user.PasswordResetCode = Guid.NewGuid().ToString();
        user = UpdateUser(user, AuditOptions.WithMessage("requested password reset").Merge(auditOptions));
        return user;
    }

    public UserDao? ResetPassword(string code, string password, AuditOptions? auditOptions = null)
    {
        var user = _db.Users.AsQueryable().FirstOrDefault(user => user.PasswordResetCode == code);
        if (user == null)
        {
            return null;
        }

        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(password).Concat(salt).ToArray());

        user.Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}";
        user.Sid = new List<string>();
        user.PasswordResetCode = "";
        return UpdateUser(user, AuditOptions.WithMessage("resetting user password").Merge(auditOptions));
    }
}