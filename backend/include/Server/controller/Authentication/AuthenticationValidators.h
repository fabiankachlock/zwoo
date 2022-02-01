#ifndef _AUTHENTICATION_VALIDATORS_H_
#define _AUTHENTICATION_VALIDATORS_H_

#include <string>

bool isValidEmail(std::string email);
bool isValidPassword(std::string password);
bool isValidUsername(std::string username);

#endif // _AUTHENTICATION_VALIDATORS_H_
