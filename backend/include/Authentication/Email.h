#ifndef _EMAIL_H_
#define _EMAIL_H_

#include <iomanip>

#include "zwoo.h"

#include <curl/curl.h>
#include <sstream>

namespace Backend::Authentication
{
    class Email
    {
    private:
        std::string m_senderEmail;
        std::string m_recipientEmail;
        std::string m_subject;

        std::vector<std::string> m_mailContent;

    public:
        Email(/* args */);
        ~Email();

        void SetSender(std::string _sender);
        void SetRecipient(std::string _recipient);
        void SetSubject(std::string subject);

        std::string GetRecipient() { return m_recipientEmail; }

        void AddLine(std::string new_line);

        std::vector<std::string> GetPayload();
    private:

        std::string GetFormatedTime();
    };
    

} // namespace Backend::Authentication



#endif