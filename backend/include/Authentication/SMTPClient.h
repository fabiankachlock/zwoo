#pragma once

namespace Backend::Authentication
{
    #define SMTP_HOST "smtp.gmail.com"
    #define SMTP_PORT 465
    
    class SMTPClient {

    public:
        SMTPClient();
        ~SMTPClient();
    };

} // namespace Backend::Authentication