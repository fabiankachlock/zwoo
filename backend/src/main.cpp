#include <iostream>

#include "../../login_data.h"
#include "Authentication/SMTPClient.h"
#include "utils/Queue.h"

int main()
{
    auto client = Backend::Authentication::SMTPClient();

    client.m_password = PASSWORD;
    client.m_username = USERNAME;

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