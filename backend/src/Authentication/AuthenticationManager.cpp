#include "Authentication/AuthenticationManager.h"

namespace Backend
{
    AuthenticationManager::AuthenticationManager(/* args */)
    {
    }

    AuthenticationManager::~AuthenticationManager()
    {
    }

    void AuthenticationManager::SendVerificationEmail(const char *email_address, const char *link)
    {
        Authentication::SMTPClient client = Authentication::SMTPClient();
        client.m_password = ;
        client.m_username = ;

        Authentication::Email email = Authentication::Email();

        email.from = "TODO: From Login_data.h";
        email.to = email_address;

        email.subject = "Authenticate your zwoo Account";
        email.header = "Your verification code!";

        email.AddLine("Klick the link below to verify your zwoo account");
        email.AddLine(link);

        client.SendEmail(email);
    }
} // namespace Backend
