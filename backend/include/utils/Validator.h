#ifndef _VALIDATOR_H_
#define _VALIDATOR_H_

#include "RegEx.h"

#include <regex>

bool isValidEmail(const char* email)
{
    const std::regex reg(EMAIL_REGEX);
    std::cmatch m;
    return std::regex_match (email, m, reg);
}

bool isValidPassword(const char* pw)
{
    std::cmatch m;
    const std::regex r1(NUMBERS_REGEX);
    const std::regex r2(SPECIAL_CHARACTERS_REGEX);
    const std::regex r3(NORMAL_CHARACTER_REGEX);

    return std::regex_search(pw, m, r1) && std::regex_search(pw, m, r2) && std::regex_search(pw, m, r3);
}


#endif // _VALIDATOR_H_