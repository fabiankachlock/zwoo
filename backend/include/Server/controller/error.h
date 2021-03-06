#ifndef _ERROR_H_
#define _ERROR_H_

#include <string>

enum e_Errors
{
    BACKEND_ERROR = 100,

    COOKIE_MISSING = 111,
    USER_NOT_FOUND = 112,
    PASSWORD_NOT_MATCHING = 113,
    SESSION_ID_NOT_MATCHING = 114,
    INVALID_EMAIL = 115,
    INVALID_USERNAME = 116,
    INVALID_PASSWORD = 117,

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
    ALREADY_INGAME = 146
};

enum e_ZRPError
{
    GENERAL_ERROR = 400,
    ACCESS_DENIED_ERROR = 420
};

// Return Response Maybe?
std::string constructErrorMessage( std::string message, e_Errors code );
#endif
