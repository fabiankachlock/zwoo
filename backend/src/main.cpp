#include "HttpServer.h"
#include "Database/DatabaseHandler.h"

int main()
{
    auto server = Backend::HttpServer();
    server.RunServer();

    return 0;
}