#ifndef _EMAIL_H_
#define _EMAIL_H_

#include <string>

#include "Server/logger/logger.h"

struct Email
{
    std::string email, username, code;
    uint64_t puid;
};

void send_verification_mail( Email email, std::shared_ptr<Logger> logger );

#endif