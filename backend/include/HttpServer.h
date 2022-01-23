#ifndef _HTTP_SERVER_HPP_
#define _HTTP_SERVER_HPP_

#include "oatpp/network/Server.hpp"

#include "Server/ServerComponent.hpp"

class HttpServer {
public:
    HttpServer();

    void RunServer();

private:
    // void CreateLogger();


};

#endif // _HTTP_SERVER_HPP_
