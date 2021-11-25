#pragma once

#include <cstring>
#include <iomanip>

#include "zwoo.h"

#include <curl/curl.h>

namespace Backend::Authentication
{
    struct Email
    {
        std::string from;
        std::string to;

        std::string subject = "";
        std::string header = "";

        std::vector<std::string> message;

        void AddLine(std::string line) { message.push_back(line); }
    };

    class SMTPClient
    {
    public:
        // Login Data
        std::string m_password;
        std::string m_username;

    public:
        SMTPClient();
        ~SMTPClient();

        int SendEmail(Email email);

    private:
        static size_t PayloadSource(void *ptr, size_t size, size_t nmemb, void *userp);
    };

} // namespace Backend::Authentication