#pragma once

#include <cstring>

#include "Authentication/Email.h"
#include "zwoo.h"

#include <curl/curl.h>

namespace Backend::Authentication
{   
    class SMTPClient {
    private:
        // Login Data
        std::string m_password;
        std::string m_username;

        std::string m_senderName;
        std::string m_senderEmail;

        static std::vector<std::string> m_messagePayload;

        CURL* curl;

    public:
        SMTPClient();
        ~SMTPClient();

        //void SetSMTPHost(std::string server, const unsigned short port = 0);

        bool SendEmail(Email* email);
        int ConnectToServer();

    private:

        static size_t PayloadSource(void *ptr, size_t size, size_t nmemb, void *userp);
    };

} // namespace Backend::Authentication