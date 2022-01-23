#include <iostream>
#include "HttpServer.h"

int main()
{
    auto server = HttpServer();
    server.RunServer();

    exit(0);
}
