#include <iostream>

#include "served/multiplexer.hpp"
#include "HttpServer.h"

int main()
{
    served::multiplexer multi;
    auto server = Backend::HttpServer(multi);

    server.InitEndpoints();
    server.StartServer();

    return 0;
}