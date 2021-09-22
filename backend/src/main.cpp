#include <iostream>

#include "SMTPClient.h"

int main()
{
    
    auto smtpClient = Backend::Authentication::SMTPClient();

    smtpClient.SetSMTPHost(SMTP_HOST, SMTP_PORT);

    std::cout << "Hello World!" << std::endl;

    return 0;
}