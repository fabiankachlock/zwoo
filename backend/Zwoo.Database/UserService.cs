using System.Text;
using System.Security.Cryptography;
using MongoDB.Driver;
using Zwoo.Database.Dao;
using BackendHelper;
using Microsoft.Extensions.Logging;

namespace Zwoo.Database;

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
    public UserDao CreateUser(string username, string email, string password, bool acceptedTerms, string? betaCode, AuditOptions? auditOptions = null);

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
    /// delete an user as an admin
    /// </summary>
    /// <param name="user">the user to delete</param>
    public bool DeleteUserAdmin(UserDao user, AuditOptions? auditOptions = null);


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
    /// <param name="password">the users password</param>
    public UserLoginResult LoginUser(string email, string password, AuditOptions? auditOptions = null);

    /// <summary>
    /// check whether a user has an active session
    /// </summary>
    /// <param name="id">the users id</param>
    /// <param name="sid">the current session id</param>
    public UserLoginResult IsUserLoggedIn(ulong id, string sid, AuditOptions? auditOptions = null);

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
    /// change the password of an user as an admin in ziad
    /// </summary>
    /// <param name="user">the user to change the password of</param>
    /// <param name="newPassword">the users new password</param>
    public bool ChangePasswordAdmin(UserDao user, string newPassword, AuditOptions? auditOptions = null);

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

    /// <summary>
    /// deletes all data that references a certain user (DSGVO save)
    /// </summary>
    /// <param name="id">the users id</param>
    public void ClearAllUserData(ulong id);
    public void ClearAllUserData(DeletedUserDao data);


    /// <summary>
    /// clean up unverified users and reset password reset codes
    /// </summary>
    public void CleanUpUsers();
}

public class UserService : IUserService
{

    private readonly IDatabase _db;
    private readonly IAuditTrailService _audits;
    private readonly IAccountEventService _accountEvents;
    private readonly IBetaCodesService _betaCodes;
    private readonly ILogger<UserService> _logger;

    public UserService(IDatabase db, IAuditTrailService audits, IAccountEventService accountEvents, IBetaCodesService betaCodes, ILogger<UserService> logger)
    {
        _db = db;
        _audits = audits;
        _accountEvents = accountEvents;
        _betaCodes = betaCodes;
        _logger = logger;
    }

    private long _getSessionExpiryDate() => DateTimeOffset.Now.ToUnixTimeSeconds() + 60 * 60 * 24 * 30; // now + 30 days

    public UserDao CreateUser(string username, string email, string password, bool acceptedTerms, string? betaCode, AuditOptions? auditOptions = null)
    {
        var code = StringHelper.GenerateNDigitString(6);
        ulong maxId = _db.Users.AsQueryable().Any() ? _db.Users.AsQueryable().Max(x => x.Id) : 0;
        ulong deletedId = _db.AccountEvents.AsQueryable().Where(evt => evt.UserData != null).Any() ? _db.AccountEvents.AsQueryable().Where(evt => evt.UserData != null).Max(evt => evt.UserData!.Id) : 0;
        ulong id = maxId > deletedId ? maxId + 1 : deletedId + 1;
        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(password).Concat(salt).ToArray());
        _logger.LogInformation($"creating new user with id {id}");

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
            AcceptedTerms = acceptedTerms,
            AcceptedTermsAt = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
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
        _logger.LogDebug($"deleting user {user.Id}");
        if (!StringHelper.CheckPassword(password, user.Password) || !user.Verified)
        {
            _logger.LogWarning($"deleting user {user.Id} failed - wrong password");
            _accountEvents.DeleteAttempt(user, false);
            return false;
        }

        _db.Users.DeleteOne(u => u.Id == user.Id);
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

    public bool DeleteUserAdmin(UserDao user, AuditOptions? auditOptions = null)
    {
        _logger.LogDebug($"deleting user {user.Id} as admin");
        _db.Users.DeleteOne(u => u.Id == user.Id);
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
        _logger.LogDebug($"verifying user {id}");
        var user = _db.Users.AsQueryable().FirstOrDefault(user => user.Id == id && user.ValidationCode == code);
        if (user == null)
        {
            _logger.LogWarning($"verifying user {id} failed - unknown user");
            return null;
        }

        if (isBeta && !_betaCodes.RemoveBetaCode(user.BetaCode ?? "#"))
        {
            _logger.LogWarning($"verifying user {id} failed - wrong beta code");
            _accountEvents.VerifyAttempt(user, false);
            return null;
        }

        user.Verified = true;
        user.VerifiedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
            _logger.LogDebug($"logging in {user.Id}");
            string sid = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            user.Sid.Add(new UserSessionDao()
            {
                Id = sid,
                Expires = _getSessionExpiryDate()
            });
            UpdateUser(user, AuditOptions.WithMessage("logged user in").Merge(auditOptions));
            _accountEvents.LoginAttempt(user, true);
            return new UserLoginResult(user, sid, null);
        }

        _logger.LogWarning($"logging in {user.Id} failed - {(user.Verified ? "wrong password" : "not verified")}");
        _accountEvents.LoginAttempt(user, false);
        return new UserLoginResult(user, null, user.Verified ? ErrorCode.WrongPassword : ErrorCode.NotVerified);
    }
    public UserLoginResult IsUserLoggedIn(ulong id, string sid, AuditOptions? auditOptions = null)
    {
        var user = GetUserById(id);
        if (user == null) return new UserLoginResult(null, null, ErrorCode.UserNotFound);

        var session = user.Sid.FirstOrDefault(session => session.Id == sid);
        if (session == null || session.Expires < DateTimeOffset.Now.ToUnixTimeSeconds())
        {
            _cleanUpUserSessions(user, auditOptions);
            return new UserLoginResult(null, null, ErrorCode.SessionExpired);
        }

        _cleanUpUserSessions(user, auditOptions);
        return new UserLoginResult(user, session.Id, null);
    }

    private void _cleanUpUserSessions(UserDao user, AuditOptions? auditOptions = null)
    {
        var cleanedSessions = user.Sid.Where(session => session.Expires > DateTimeOffset.Now.ToUnixTimeSeconds()).ToList();
        if (cleanedSessions.Count != user.Sid.Count)
        {
            user.Sid = cleanedSessions;
            UpdateUser(user, AuditOptions.WithMessage("user session clean up").Merge(auditOptions));
        }
    }

    public bool LogoutUser(UserDao user, string sessionId, AuditOptions? auditOptions = null)
    {
        _logger.LogDebug($"logging out {user.Id}");
        user.Sid.RemoveAll(s => s.Id == sessionId);
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
        var user = GetUserById(playerId);
        if (user == null)
        {
            _logger.LogWarning($"incrementing win of unknown user {playerId}");
            return 0;
        }

        _logger.LogDebug($"incrementing win of {user.Id}");
        _db.Users.UpdateOne(user => user.Id == playerId, Builders<UserDao>.Update.Inc(u => u.Wins, (uint)1));
        var newUser = _db.Users.AsQueryable().First(user => user.Id == playerId);

        _audits.Protocol(_audits.GetAuditId(user), AuditOptions.CreateEvent(auditOptions, new AuditEventDao()
        {
            Actor = AuditActor.System,
            Message = "incremented wins",
            NewValue = newUser,
            OldValue = user,
        }));
        return newUser.Wins;
    }

    public bool ChangePassword(UserDao user, string oldPassword, string newPassword, string currentSession, AuditOptions? auditOptions = null)
    {
        _logger.LogDebug($"changing password of {user.Id}");
        if (!StringHelper.CheckPassword(oldPassword, user.Password))
        {
            _logger.LogWarning($"changing password of {user.Id} failed - wrong password");
            return false;
        }
        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(newPassword).Concat(salt).ToArray());
        // reset session to only current one - logs out all other devices
        user.Sid = new List<UserSessionDao>() { new UserSessionDao() {
                Id = currentSession,
                Expires = _getSessionExpiryDate()
        }};
        user.Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}";

        var res = UpdateUser(user, new AuditOptions(AuditActor.User(user.Id, currentSession), "changed password").Merge(auditOptions));
        return res != null;
    }

    public bool ChangePasswordAdmin(UserDao user, string newPassword, AuditOptions? auditOptions = null)
    {
        _logger.LogDebug($"changing password of {user.Id} as admin");
        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(newPassword).Concat(salt).ToArray());
        user.Sid = new(); // reset all sessions
        user.Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}";

        var res = UpdateUser(user, AuditOptions.WithMessage("changed password as admin").Merge(auditOptions));
        return res != null;
    }

    public UserDao? RequestChangePassword(string email, AuditOptions? auditOptions = null)
    {
        var user = _db.Users.AsQueryable().First(user => user.Email == email);
        if (user == null)
        {
            _logger.LogWarning($"requested password reset for unknown user {email}");
            return null;
        }

        _logger.LogDebug($"requested password reset for {user.Id}");
        user.PasswordResetCode = Guid.NewGuid().ToString();
        return UpdateUser(user, AuditOptions.WithMessage("requested password reset").Merge(auditOptions));
    }

    public UserDao? ResetPassword(string code, string password, AuditOptions? auditOptions = null)
    {
        var user = _db.Users.AsQueryable().FirstOrDefault(user => user.PasswordResetCode == code);
        if (user == null)
        {
            _logger.LogWarning($"resetting password for {code} failed - unknown code");
            return null;
        }

        _logger.LogDebug($"resetting password for {user.Id}");
        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(password).Concat(salt).ToArray());

        user.Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}";
        user.Sid = new(); // reset all sessions
        user.PasswordResetCode = "";
        return UpdateUser(user, AuditOptions.WithMessage("resetting user password").Merge(auditOptions));
    }

    public void ClearAllUserData(ulong id)
    {
        var user = _db.Users.AsQueryable().FirstOrDefault(user => user.Id == id);
        if (user == null) return;

        ClearAllUserData(user.Id, user.Username);
    }

    public void ClearAllUserData(DeletedUserDao data)
    {
        ClearAllUserData(data.Id, data.Username);
    }

    private void ClearAllUserData(ulong id, string username)
    {
        _db.Users.DeleteOne(u => u.Id == id);
        _db.AccountEvents.DeleteMany(e => e.PlayerID == id);
        var p = _audits.GetProtocol(_audits.GetAuditId(id));
        if (p != null)
        {
            _db.AuditTrails.DeleteOne(t => t.Id == p.Id);
            foreach (var e in p.Events)
            {
                if (e.NewValue is GameInfoDao info)
                {
                    var score = info.Scores.Find(score => score.PlayerUsername == username);
                    if (score != null)
                    {
                        score.PlayerUsername = "-/-";
                    }
                    _db.GamesInfo.ReplaceOne(i => i.Id == info.Id, info);
                }
            }
        }
    }

    public void CleanUpUsers()
    {
        var unverifiedUsers = _db.Users.AsQueryable().Where(x => !x.Verified);
        _logger.LogInformation($"[CleanUp] deleted {unverifiedUsers.Count()} unverified user(s).");
        foreach (var user in unverifiedUsers)
        {
            ClearAllUserData(user.Id);
        }

        var usersWithPasswordReset = _db.Users.AsQueryable().Where(x => !String.IsNullOrEmpty(x.PasswordResetCode));
        _logger.LogInformation($"[CleanUp] deleted {usersWithPasswordReset.Count()} password reset codes.");
        foreach (var user in usersWithPasswordReset)
        {
            _db.Users.UpdateOne(x => x.Id == user.Id, Builders<UserDao>.Update.Set(u => u.PasswordResetCode, ""));
        }
    }
}