#include "SMTPClient.h"

namespace Backend::Authentication
{
   enum SMTPCodes {
        OK = 0,
        StartUp = 100,
        Ver,
        Send,
        Recv,
        Connect,
        GetHost,
        InvalidSocket,
        Select,
        BadIPv4,
        // Undefined
        MsgHeader = 200,
        MailSender,
        Subject,
        Recipient,
        Login,
        Password,
        Mail,
        // Commands
        MailFrom = 300,
        EHLO,
        AuthLogin,
        Data,
        Quit,
        RcptTo,
        BodyError,
        ConnectionClosed = 400, // From Server
        ServerNotReady,         // Remote Server
        ServerNotResponding,
        Timeout,
        FileDoesNotExist,
        MsgTooBig,
        BadLogin,
        UndefXYZResponse,
        OutOfMemory,
        TimeError,
        RecBufferEmpty,
        SendBufferEmpty,
        OutOfMessageRange
   };


   SMTPClient::SMTPClient()
   {

   }

   SMTPClient::~SMTPClient()
   {

   }

    void SMTPClient::SetSMTPHost(const char* server, const unsigned short port = 0)
    {
        m_serverPort = port;
        m_serverName.erase();
        m_serverName.insert(0, server);
    }

    bool SMTPClient::SendEmail(Email* email)
    {
        unsigned int i, rcpt_count, res, file_id;
        char* buffer, *file_name;
        FILE* file;
        unsigned long int file_size, total_size, message_size;
        bool accepted;

        // if (m_socket = connect(m_))

        
        return true;
    }

    int SMTPClient::ConnectToServer()
    {
        


        return 0;
    }

}