#ifndef _EMAIL_H_
#define _EMAIL_H_

#include <string>

struct Email
{
    std::string email, username, code;
    uint64_t puid;
};


#endif