#include "HttpServer.h"

int main(int argc, const char * argv[]) 
{
    auto server = Backend::HttpServer();
    server.RunServer(oatpp::base::CommandLineArguments(argc, argv));


    return 0;
}