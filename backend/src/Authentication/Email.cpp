#include "Authentication/Email.h"

namespace Backend::Authentication
{

    Email::Email() {}
    Email::~Email() {}

    void Email::SetSender(std::string _sender)
    {
        m_senderEmail = _sender;
    }

    void Email::SetRecipient(std::string _recipient)
    {
        m_recipientEmail = _recipient;
    }

    void Email::SetSubject(std::string subject)
    {
        m_subject = subject;
    }

    void Email::AddLine(std::string new_line)
    {
        m_mailContent.push_back(new_line + "\r\n");
    }

    std::vector<std::string> Email::GetPayload()
    {
        std::vector<std::string> payload = std::vector<std::string>();
        payload[0] = "Date: " + GetFormatedTime() + "\r\n";
        payload[1] = "To: " + m_recipientEmail + "\r\n";
        payload[2] = "From: " + m_senderEmail + "\r\n";
        payload[3] = "Cc: \r\n";
        payload[4] = "Subject: " + m_subject + "\r\n";
        payload[5] = "\r\n";

        int i = 5;
        for (auto line : m_mailContent)
        {
            i += 1;
            payload[i] = line;
        }

        payload[i + 1] = (char)NULL;
    }

    std::string Email::GetFormatedTime()
    {
        std::stringstream string_stream;
        auto timer = std::time(NULL);
        auto tm = *std::localtime(&timer);
        string_stream << std::put_time(&tm, "%a, %d %b %Y %H:%M:%S");

        return string_stream.str();
    }

}