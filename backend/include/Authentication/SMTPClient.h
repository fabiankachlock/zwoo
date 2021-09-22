#pragma once

#include "Email.h"
#include "zwoo.h"

#ifdef Linux
    #include <sys/types.h>
	#include <sys/socket.h>
	#include <sys/ioctl.h>
	#include <netinet/in.h>
	#include <arpa/inet.h>
	#include <netdb.h>
	#include <errno.h>
#endif 

namespace Backend::Authentication
{   
    class SMTPClient {
    private:
        // Login Data
        std::string m_password;
        std::string m_username;

        std::string m_senderName;
        std::string m_senderEmail;

        std::string m_serverName;
        unsigned short m_serverPort;

        int m_socket;

    public:
        SMTPClient();
        ~SMTPClient();

        void SetSMTPHost(const char* server, const unsigned short port = 0);

        bool SendEmail(Email* email);
        int ConnectToServer();
    };

} // namespace Backend::Authentication