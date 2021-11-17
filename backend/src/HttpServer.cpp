#include "HttpServer.h"

namespace Backend
{

    HttpServer::HttpServer(served::multiplexer m) : multiplexer(m)
    {
        gamemanager = Game::GameManager();
    }

    void HttpServer::InitEndpoints()
    {
        multiplexer.handle("hello-world").get(HelloWorld());
        multiplexer.handle("gamemanager/create").get(CreateGame());
    }

}
