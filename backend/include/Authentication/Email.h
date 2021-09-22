#ifndef _EMAIL_H_
#define _EMAIL_H_

#include "zwoo.h"

namespace Backend::Authentication
{

    class Email
    {
    public:
        Email(std::string recipient, std::string subject);
        ~Email();

        void AddLine(std::string line);

        std::string m_recipient;
        std::string m_subject;

        std::string m_emailText;
    };

}

#endif