using ZwooBackend.Controllers.DTO;
using ZwooDatabase;

namespace ZwooBackend.Controllers;

public static class ErrorCodes
{
    public enum Errors
    {
        NONE = 0,

        BACKEND_ERROR = 100,
        CAPTCHA_INVALID = 101,

        USER_NOT_VERIFIED = 110,
        COOKIE_MISSING = 111,
        USER_NOT_FOUND = 112,
        PASSWORD_NOT_MATCHING = 113,
        SESSION_ID_NOT_MATCHING = 114,
        INVALID_EMAIL = 115,
        INVALID_USERNAME = 116,
        INVALID_PASSWORD = 117,
        INVALID_BETACODE = 118,

        EMAIL_ALREADY_TAKEN = 120,
        USERNAME_ALREADY_TAKEN = 121,
        ACCOUNT_FAILED_TO_VERIFIED = 122,

        DELETING_USER_FAILED = 135,

        GAME_NOT_FOUND = 140,
        GAME_NAME_MISSING = 141,
        JOIN_FAILED = 142,
        OPCODE_MISSING = 143,
        INVALID_OPCODE = 144,
        INVALID_GAMEID = 145,
        ALREADY_INGAME = 146,
        GAME_FULL = 147,
    };

    public static Errors FromDatabaseError(ErrorCode? error)
    {
        switch (error)
        {
            case ErrorCode.NotVerified:
                return Errors.USER_NOT_VERIFIED;
            case ErrorCode.UserNotFound:
                return Errors.USER_NOT_FOUND;
            case ErrorCode.WrongPassword:
                return Errors.PASSWORD_NOT_MATCHING;
        }
        return Errors.NONE;
    }

    public static ErrorDTO GetResponse(Errors code, string text)
    {
        return new ErrorDTO()
        {
            Code = (int)code,
            Message = text,
        };
    }
}