#include "Authentication/SMTPClient.h"

namespace Backend::Authentication
{
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

    /*void SMTPClient::SetSMTPHost(std::string server, const unsigned short port = 0)
    {
        // curl_easy_setopt(curl, CURLOPT_URL, server + ":" + std::to_string(port));
    }*/

    bool SMTPClient::SendEmail(Email *email)
    {
        CURLcode res = CURLE_OK;

        struct curl_slist *recipients = NULL;
        struct s_UploadStatus *upload_ctx;

        email->SetSender(m_senderEmail);

        // Set SMTP Server
        curl_easy_setopt(curl, CURLOPT_URL, "smtp://smtp.gmail.com:587");
        // Set Sender Email
        curl_easy_setopt(curl, CURLOPT_MAIL_FROM, m_senderEmail);
        // Set Recipients
        recipients = curl_slist_append(recipients, email->GetRecipient().c_str());
        curl_easy_setopt(curl, CURLOPT_MAIL_RCPT, recipients);

        // Login data for smtp server
        curl_easy_setopt(curl, CURLOPT_USERNAME, "");
        curl_easy_setopt(curl, CURLOPT_PASSWORD, "");
        // gmail smtp need ssl so we use it
        curl_easy_setopt(curl, CURLOPT_USE_SSL, CURLUSESSL_ALL);
        // gets the Emails payload/content
        m_messagePayload = email->GetPayload();
        // Settig Callback function for sending data
        curl_easy_setopt(curl, CURLOPT_READFUNCTION, PayloadSource);
        curl_easy_setopt(curl, CURLOPT_READDATA, &upload_ctx);
        curl_easy_setopt(curl, CURLOPT_UPLOAD, 1L);

        res = curl_easy_perform(curl);

        if (res != CURLE_OK)
            fprintf(stderr, "curl_easy_perform() failed: %s\n",
                    curl_easy_strerror(res));

        /* Free the list of recipients */
        curl_slist_free_all(recipients);
        curl_easy_cleanup(curl);

        return true;
    }

    int SMTPClient::ConnectToServer()
    {

        return 0;
    }

    size_t SMTPClient::PayloadSource(void *ptr, size_t size, size_t nmemb, void *userp)
    {
        struct s_UploadStatus *upload_ctx = (struct s_UploadStatus *)userp;
        const char *data;

        if ((size == 0) || (nmemb == 0) || ((size * nmemb) < 1))
        {
            return 0;
        }

        data = m_messagePayload[upload_ctx->m_lines_read].c_str();

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