#ifndef _AUTHENTICATION_H_
#define _AUTHENTICATION_H_

#include "zwoo.h"
#include "Authentication/SMTPClient.h"

namespace Backend
{
    class AuthenticationManager
    {
    private:
    public:
        AuthenticationManager();
        ~AuthenticationManager();

        static void SendVerificationEmail(const char *email, const char *link);
    };
}
#endif