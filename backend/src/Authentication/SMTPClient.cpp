#include "Authentication/SMTPClient.h"

namespace Backend::Authentication
{
    static std::vector<std::string> m_payload;

    struct s_UploadStatus
    {
        int m_lines_read;
    };

    SMTPClient::SMTPClient()
    {
    }

    SMTPClient::~SMTPClient()
    {
    }

    std::string GetFormattedTime()
    {
        std::stringstream string_stream;
        auto timer = std::time(NULL);
        auto tm = *std::localtime(&timer);
        string_stream << std::put_time(&tm, "%a, %d %b %Y %H:%M:%S");

        return string_stream.str();
    }

    int SMTPClient::SendEmail(Email email)
    {
        CURL *curl;
        CURLcode res = CURLE_OK;
        struct curl_slist *recipients = NULL;
        struct s_UploadStatus upload_ctx;

        upload_ctx.m_lines_read = 0;

        curl = curl_easy_init();

        if (curl)
        {
            m_payload = {
                "Date: " + GetFormattedTime() + "\r\n",
                "From: " + email.from + "\r\n",
                "To: " + email.to + "\r\n",
                "Subject: " + email.subject + "\r\n",
                "\r\n",
                email.header,
                "\r\n"};

            for (auto line : email.message)
                m_payload.push_back(line + "\r\n");

            m_payload.push_back("NULL");

            // Set SMTP Server and Login data
            curl_easy_setopt(curl, CURLOPT_USERNAME, m_username.c_str());
            curl_easy_setopt(curl, CURLOPT_PASSWORD, m_password.c_str());
            curl_easy_setopt(curl, CURLOPT_URL, "smtp://smtp.gmail.com:587");
            // gmail smtp need ssl so we use it
            curl_easy_setopt(curl, CURLOPT_USE_SSL, CURLUSESSL_ALL);

            // Set Sender Email
            curl_easy_setopt(curl, CURLOPT_MAIL_FROM, email.from.c_str());
            // Set Recipients
            recipients = curl_slist_append(recipients, email.to.c_str());
            curl_easy_setopt(curl, CURLOPT_MAIL_RCPT, recipients);

            // Settig Callback function for sending data
            curl_easy_setopt(curl, CURLOPT_READFUNCTION, PayloadSource);
            curl_easy_setopt(curl, CURLOPT_READDATA, &upload_ctx);
            curl_easy_setopt(curl, CURLOPT_UPLOAD, 1L);
            curl_easy_setopt(curl, CURLOPT_VERBOSE, 1L);

            res = curl_easy_perform(curl);

            if (res != CURLE_OK)
                fprintf(stderr, "curl_easy_perform() failed: %s\n",
                        curl_easy_strerror(res));

            /* Free the list of recipients */
            curl_slist_free_all(recipients);
            curl_easy_cleanup(curl);
        }
        return true;
    }

    size_t SMTPClient::PayloadSource(void *ptr, size_t size, size_t nmemb, void *userp)
    {
        struct s_UploadStatus *upload_ctx = (struct s_UploadStatus *)userp;
        const char *data;

        if ((size == 0) || (nmemb == 0) || ((size * nmemb) < 1))
        {
            return 0;
        }

        if (m_payload[upload_ctx->m_lines_read] != "NULL")
            data = m_payload[upload_ctx->m_lines_read].c_str();
        else
            data = NULL;

        if (data)
        {
            size_t len = strlen(data);
            memcpy(ptr, data, len);
            upload_ctx->m_lines_read++;

            return len;
        }

        return 0;
    }
}