#ifndef _HTTP_SERVER_H_
#define _HTTP_SERVER_H_

namespace Backend
{
    // Authentication Callbacks
    constexpr char* kCreateAccount = "authentication/create";
    constexpr char* kLoginAccount = "authentication/login";

    class HttpServer
    {
    private:
        /* data */
    public:
        HttpServer(/* args */);
        ~HttpServer();
    };
} // namespace Backend

#endif