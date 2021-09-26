#include <iostream>

#include "Authentication/SMTPClient.h"

int main()
{
    auto client = Backend::Authentication::SMTPClient();

    client.m_password = "";
    client.m_username = "zwoo.auth@gmail.com";

    Backend::Authentication::Email mail = Backend::Authentication::Email();

    mail.from = "<zwoo.auth@gmail.com>";
    mail.to = "<contact@fabiankachlock.dev>";
    mail.subject = "Test Mail";
    mail.header = "Header";

    mail.AddLine("");
    mail.AddLine("This is a Test Mail!");
    mail.AddLine("Hopefully it works...");
    mail.AddLine("Please Send Respons via Discord.");

    client.SendEmail(mail);

    return 0;
}