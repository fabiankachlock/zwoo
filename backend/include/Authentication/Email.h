#ifndef _EMAIL_H_
#define _EMAIL_H_

#include "zwoo.h"

#include <libwebsockets.h>

namespace Backend::Authentication
{

    class Email
    {
    public:
        Email(std::string recipient, std::string subject);
        ~Email();

        void AddLine(std::string line);

    private:
        // m_email;
    };

}

#endif