#include "HttpServer.h"

int main()
{
    auto server = Backend::HttpServer();
    server.RunServer();

    return 0;
}