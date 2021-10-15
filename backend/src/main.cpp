#include <iostream>
#include "served/multiplexer.hpp"

#include "HttpServer.h"

int main()
{
    served::multiplexer multiplexer;
    Backend::HttpServer server(multiplexer);

    server.InitEndpoints();
    server.StartServer();

    return 0;
}