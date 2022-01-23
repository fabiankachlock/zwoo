#ifndef _HTTP_SERVER_HPP_
#define _HTTP_SERVER_HPP_

#include "oatpp/network/Server.hpp"

#include "Server/ServerComponent.hpp"
#include "Server/logger/logger.h"

class HttpServer {
public:
    HttpServer();

    void RunServer();

private:
    std::shared_ptr<Logger> logger;


};

#endif // _HTTP_SERVER_HPP_
